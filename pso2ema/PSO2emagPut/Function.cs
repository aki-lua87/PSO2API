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
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using HtmlAgilityPack;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PSO2emagPut
{
    public class Function
    {
        private static readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);
        const string pso2Url = "https://pso2.jp/players/boost/";
        const int OldYear = 2018;
        const int NowYear = 2019;

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(ILambdaContext context)
        {
            LambdaLogger.Log("Function Start\n");

            string DebugFlag = Environment.GetEnvironmentVariable("DebugFlag");
            LambdaLogger.Log($"Debug {DebugFlag}\n");

            var discordURL = Environment.GetEnvironmentVariable("DiscordURL");

            LambdaLogger.Log($"Function Ver.3.0\n");

            // 全HTMLを読み込み
            LambdaLogger.Log($"Request for {pso2Url}\n");
            string html = (new HttpClient()).GetStringAsync(pso2Url).Result;
            LambdaLogger.Log($"Get html\n");

            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;

            //サイト全体の読み込み
            doc.LoadHtml(html);

            PSO2EmagScraping emagScraping = new PSO2EmagScraping(doc);

            if (DebugFlag == "TRUE")
            {
                LambdaLogger.Log("Debug Test↓" + "\n");
                LambdaLogger.Log(emagScraping.DebugShowList2() + "\n");
                return;
            }

            emagScraping.PutEmagList();

            using (var client = new HttpClient())
            {
                LambdaLogger.Log("Discord Post Start \n");
                var discordContent = JsonConvert.SerializeObject(new DiscordMessage(emagScraping.DebugShowList2()));
                var stringContent = new StringContent(discordContent, Encoding.UTF8, "application/json");
                var _ = client.PostAsync(discordURL, stringContent).Result;
                LambdaLogger.Log("Discord Post Finish \n");
            }

            LambdaLogger.Log("Exec Finish\n");
        }

        [DynamoDBTable("PSO2ema")]
        public class TableValue
        {
            [DynamoDBHashKey]
            [JsonProperty(PropertyName = "Key")] // yyyymmdd
            public string Key { get; set; }

            [DynamoDBRangeKey]
            [JsonProperty(PropertyName = "RKey")] //hhmm
            public string Rkey { get; set; }

            [DynamoDBProperty("EvantName")]
            [JsonProperty(PropertyName = "EventName")]
            public string EventName { get; set; }

            [DynamoDBProperty("EvantType")]
            [JsonProperty(PropertyName = "EventType")]
            public string EventType { get; set; }

            [DynamoDBProperty("Month")]
            [JsonProperty(PropertyName = "Month")]
            public int Month { get; set; }

            [DynamoDBProperty("Date")]
            [JsonProperty(PropertyName = "Date")]
            public int Date { get; set; }

            [DynamoDBProperty("Hour")]
            [JsonProperty(PropertyName = "Hour")]
            public int Hour { get; set; }

            [DynamoDBProperty("Minute")]
            [JsonProperty(PropertyName = "Minute")]
            public int Min { get; set; }
        }

        public class PSO2EmagScraping
        {
            private List<EmagTableValue> _table = new List<EmagTableValue>();
            private List<string> emaStrList = new List<string>();

            public PSO2EmagScraping(HtmlDocument htmlDoc)
            {
                LambdaLogger.Log("Excec PSO2EmagScraping Class");

                // イベントスケジュール部の抽出
                HtmlNodeCollection events = htmlDoc.DocumentNode.SelectNodes($"//div[@class='eventTable--event']");
                foreach (HtmlNode eventNode in events)
                {
                    var eveStr = new HtmlDocument();
                    eveStr.LoadHtml(eventNode.InnerHtml);

                    // 24時間分のタグを抽出
                    for (var i = 0; i < 24; i++)
                    {
                        var timeTagStr = $"{i:00}";

                        // t0Xm00なノードリスト
                        var hourNodes = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m00']");
                        ScrapingHour(hourNodes, i, 0);

                        // t0Xm30なノードリスト
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

            // 時間タグ(tHHmMM)ごとに情報を分割
            public void ScrapingHour(HtmlNodeCollection hourNodes, int hour, int minute)
            {
                foreach (var hourNode in hourNodes)
                {
                    var timeNode = new HtmlDocument();
                    timeNode.LoadHtml(hourNode.InnerHtml);

                    List<TagAndEvent> tagAndEventsList = new List<TagAndEvent>();

                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']"), "緊急"));
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W02 event-emergency']"), "緊急"));
                    // tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='event-emergency']"), "緊急")); これができないのがあれ
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-live']"), "ライブ"));// ライブ
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

            // 各イベントを取得してリストへ
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

                    // 年度変更線対応
                    emagValue.Key = $"{OldYear}{emagValue.Month:00}{emagValue.Date:00}"; 

                    emagValue.Rkey = $"{emagValue.Hour:00}{emagValue.Minute:00}{eventName}";
                    _table.Add(emagValue);

                    emaStrList.Add(emagValue.Key+ " : " + emagValue.EventName);
                }
            }

            // DynamoDB へ ポスト
            public void PutEmagList()
            {
                var input = _table;
                var dbContext = new DynamoDBContext(Client);

                foreach (var v in input)
                {
                    var insertTask = dbContext.SaveAsync(v);
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

    // リクエストJson用クラス
    [DynamoDBTable("PSO2ema")]
    public class EmagTableValue
    {
        [JsonProperty(PropertyName = "Key")] // yyyymmddhhmm
        public string Key { get; set; }

        [JsonProperty(PropertyName = "RKey")] // hhmmEvent
        public string Rkey { get; set; }

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
