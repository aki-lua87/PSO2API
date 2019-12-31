using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Amazon.XRay.Recorder.Handlers.System.Net;

namespace PSO2emagPut
{
    public class Function
    {
        const string pso2Url = "https://pso2.jp/players/boost/";
        const int CurrentYear = 2020;
        const int NextYear = CurrentYear + 1;

        public void FunctionHandler()
        {
            AWSSDKHandler.RegisterXRayForAllServices();

            LambdaLogger.Log("Function Start\n");

            string DebugFlag = Environment.GetEnvironmentVariable("DebugFlag");
            LambdaLogger.Log($"Debug {DebugFlag}\n");

            var discordURL = Environment.GetEnvironmentVariable("DiscordURL");
            var tableName = Environment.GetEnvironmentVariable("TABLE_NAME");

            LambdaLogger.Log($"Function Ver.3.0\n");

            LambdaLogger.Log($"Request for {pso2Url}\n");
            string html = (new HttpClient(new HttpClientXRayTracingHandler(new HttpClientHandler()))).GetStringAsync(pso2Url).Result;
            LambdaLogger.Log($"Get html\n");

            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;

            doc.LoadHtml(html);

            PSO2EmagScraping emagScraping = new PSO2EmagScraping(doc,tableName);

            if (DebugFlag == "TRUE")
            {
                LambdaLogger.Log(emagScraping.DebugShowList2() + "\n");
                return;
            }

            emagScraping.PutEmagList();

            if(false)
            {
                using (var client = new HttpClient())
                {
                    LambdaLogger.Log("Discord Post Start \n");
                    var discordContent = JsonConvert.SerializeObject(new DiscordMessage(emagScraping.DebugShowList2()));
                    var stringContent = new StringContent(discordContent, Encoding.UTF8, "application/json");
                    var _ = client.PostAsync(discordURL, stringContent).Result;
                    LambdaLogger.Log("Discord Post Finish \n");
                }
            }
            LambdaLogger.Log("Exec Finish\n");
        }

        public class PSO2EmagScraping
        {
            private List<EmagTableValue> _table = new List<EmagTableValue>();
            private List<string> emaStrList = new List<string>();
            private string _tableName;

            public PSO2EmagScraping(HtmlDocument htmlDoc,string tableName)
            {
                LambdaLogger.Log("Excec PSO2EmagScraping Class");
                this._tableName = tableName;

                HtmlNodeCollection events = htmlDoc.DocumentNode.SelectNodes($"//div[@class='eventTable--event']");
                foreach (HtmlNode eventNode in events)
                {
                    var eveStr = new HtmlDocument();
                    eveStr.LoadHtml(eventNode.InnerHtml);

                    for (var i = 0; i < 24; i++)
                    {
                        var timeTagStr = $"{i:00}";

                        var hourNodes = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m00']");
                        ScrapingHour(hourNodes, i, 0);

                        var harfHourNode = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m30']");
                        ScrapingHour(harfHourNode, i, 30);
                    }
                }
            }

            public struct TagAndEvent
            {
                private readonly HtmlNodeCollection _eventsNode;
                private readonly string _eventsType;

                public TagAndEvent(HtmlNodeCollection eventsNode, string eventsType)
                {
                    _eventsNode = eventsNode;
                    _eventsType = eventsType;
                }

                public HtmlNodeCollection GetEventsNode() => _eventsNode;
                public string GetEventType() => _eventsType;
            }

            public void ScrapingHour(HtmlNodeCollection hourNodes, int hour, int minute)
            {
                foreach (var hourNode in hourNodes)
                {
                    var timeNode = new HtmlDocument();
                    timeNode.LoadHtml(hourNode.InnerHtml);

                    List<TagAndEvent> tagAndEventsList = new List<TagAndEvent>();

                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']"), "緊急"));
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W02 event-emergency']"), "緊急"));
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-live']"), "ライブ"));
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H06 cell-W01 event-casino']"), "カジノイベント"));// casino
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-league']"), "アークスリーグ"));// league
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W02 event-league']"), "アークスリーグ"));// league

                    foreach (var tagAndEvent in tagAndEventsList)
                    {
                        if (tagAndEvent.GetEventsNode() != null)
                        {
                            ScrapingEvents(tagAndEvent.GetEventsNode(), hour, minute, tagAndEvent.GetEventType());
                        }
                    }
                }
            }

            private void ScrapingEvents(HtmlNodeCollection eventsNodes, int hour, int minute, string eventName)
            {
                foreach (var enent in eventsNodes)
                {
                    var emagValue = new EmagTableValue
                    {
                        Hour = hour,
                        Minute = minute,
                        EventType = eventName
                    };

                    var emaStr = new HtmlDocument();
                    emaStr.LoadHtml(enent.InnerHtml);

                    var time = emaStr.DocumentNode.SelectNodes("//strong[@class='start']");
                    foreach (var t in time)
                    {
                        var monthAndDate = t.InnerHtml.Split('/');
                        emagValue.Month = int.Parse(monthAndDate[0]);
                        emagValue.Date = int.Parse(monthAndDate[1]);
                    }


                    var name = emaStr.DocumentNode.SelectNodes("//dd");
                    foreach (var n in name)
                    {
                        emagValue.EventName = n.InnerHtml.Replace('"', ' ');
                    }

                    emagValue.yyyymmdd = $"{CurrentYear}{emagValue.Month:00}{emagValue.Date:00}"; 

                    emagValue.hhname = $"{emagValue.Hour:00}{emagValue.Minute:00}{eventName}";
                    _table.Add(emagValue);

                    emaStrList.Add(emagValue.yyyymmdd+ " : " + emagValue.EventName);
                }
            }

            public void PutEmagList()
            {
                var insertParams = _table;
                var Client = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);
                // var dbContext = new DynamoDBContext(Client);
                
                foreach (var param in insertParams)
                {
                    var request = new PutItemRequest
                    {
                        TableName = _tableName,
                        Item = new Dictionary<string, AttributeValue>()
                        {
                            { "yyyymmdd", new AttributeValue { S = param.yyyymmdd }},
                            { "hhname", new AttributeValue { S = param.hhname }},
                            { "EventName", new AttributeValue { S = param.EventName }},
                            { "EventType", new AttributeValue { S = param.EventType }},
                            { "Month", new AttributeValue { N = param.Month.ToString() }},
                            { "Date", new AttributeValue { N = param.Date.ToString() }},
                            { "Hour", new AttributeValue { N = param.Hour.ToString() }},
                            { "Minute", new AttributeValue { N = param.Minute.ToString() }},
                        }
                    };
                    var insertTask = Client.PutItemAsync(request);
                    insertTask.Wait();
                }
            }

            public string DebugShowList()
            {
                return JsonConvert.SerializeObject(_table);
            }

            public string DebugShowList2()
            {
                string s = "緊急クエスト取得バッチ結果 \n \n";
                foreach (var v in emaStrList)
                {
                    s = s + v + " \n";
                }

                return s;
            }
        }
    }

    [DynamoDBTable("pso2_emergency_default")]
    public class EmagTableValue
    {
        [JsonProperty(PropertyName = "yyyymmdd")] // yyyymmddhhmm
        public string yyyymmdd { get; set; }

        [JsonProperty(PropertyName = "hhname")] // hhmmEvent
        public string hhname { get; set; }

        [JsonProperty(PropertyName = "EventName")]
        public string EventName { get; set; }

        [JsonProperty(PropertyName = "EventType")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "Month")]
        public int Month { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public int Date { get; set; }

        [JsonProperty(PropertyName = "Hour")]
        public int Hour { get; set; }

        [JsonProperty(PropertyName = "Minute")]
        public int Minute { get; set; }
    }

    public class DiscordMessage
    {
        public DiscordMessage(string value)
        {
            this.content = value;
        }
        public string content { get; set; }
    }
}
