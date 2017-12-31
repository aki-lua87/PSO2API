using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using HtmlAgilityPack;

namespace PSO2emaAzureFunctions
{
    public static class FetchEmaList
    {
        const string pso2Url = "https://pso2.jp/players/boost/";
        const int OldYear = 2017;
        const int NowYear = 2018;

        // ���j16�F30(JST)�Ɏ��s
        [FunctionName("FetchEmaList")]
        public static void Run([TimerTrigger("0 30 7 * * 3")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info("Function Start");
            
            // Lambda��URL
            string postUrl = Environment.GetEnvironmentVariable("DBEndpointURL");
            // Lambda��API�L�[
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");

            string DebugFlag = Environment.GetEnvironmentVariable("DebugFlag");
            log.Info($"Debug {DebugFlag}");

            log.Info($"url - {postUrl}");
            log.Info($"API Ver.2.1");

            // �SHTML��ǂݍ���
            log.Info($"Request for {pso2Url}");
            string html = (new HttpClient()).GetStringAsync(pso2Url).Result;
            log.Info($"Get html");

            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;

            //�T�C�g�S�̂̓ǂݍ���
            doc.LoadHtml(html);

            PSO2EmagScraping emagScraping = new PSO2EmagScraping(log,doc);

            if (DebugFlag == "TRUE")
            {
                log.Info(emagScraping.DebugShowList());
                return;
            }

            var res = emagScraping.PostEmagList(postUrl,apiKey);
            log.Info(res);

            log.Info("Exec Finish");
        }

        public class PSO2EmagScraping
        {
            private TraceWriter _log;
            private List<EmagTableValue> _table = new List<EmagTableValue>();

            public PSO2EmagScraping(TraceWriter log, HtmlDocument htmlDoc)
            {
                log.Info("Excec PSO2EmagScraping Class");
                _log = log;

                // �C�x���g�X�P�W���[�����̒��o
                HtmlNodeCollection events = htmlDoc.DocumentNode.SelectNodes($"//div[@class='eventTable--event']");
                foreach (HtmlNode eventNode in events)
                {
                    var eveStr = new HtmlDocument();
                    eveStr.LoadHtml(eventNode.InnerHtml);

                    // 24���ԕ��̃^�O�𒊏o
                    for (var i = 0; i < 24; i++)
                    {
                        var timeTagStr = $"{i:00}";

                        // t0Xm00�ȃm�[�h���X�g
                        var hourNodes = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m00']");
                        ScrapingHour(hourNodes, i, 0);

                        // t0Xm30�ȃm�[�h���X�g
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

            // ���ԃ^�O(tHHmMM)���Ƃɏ��𕪊�
            public void ScrapingHour(HtmlNodeCollection hourNodes, int hour, int minute)
            {
                foreach (var hourNode in hourNodes)
                {
                    var timeNode = new HtmlDocument();
                    timeNode.LoadHtml(hourNode.InnerHtml);

                    List<TagAndEvent> tagAndEventsList = new List<TagAndEvent>();

                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']"), "�ً}"));
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W02 event-emergency']"), "�ً}"));
                    // tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='event-emergency']"), "�ً}")); ���ꂪ�ł��Ȃ��̂�����
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-live']"), "���C�u"));// ���C�u
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H06 cell-W01 event-casino']"), "�J�W�m�C�x���g"));// casino
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-league']"), "�A�[�N�X���[�O"));// league
                    tagAndEventsList.Add(new TagAndEvent(timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W02 event-league']"), "�A�[�N�X���[�O"));// league

                    foreach (var tagAndEvent in tagAndEventsList)
                    {
                        if (tagAndEvent.GetEventsNode() != null)
                        {
                            ScrapingEvents(tagAndEvent.GetEventsNode(), hour, minute, tagAndEvent.GetEventType());
                        }
                    }
                }
            }

            // �e�C�x���g���擾���ă��X�g��
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
                        emagValue.EventName = n.InnerHtml;
                    }

                    // �N�x�ύX���Ή�
                    if (emagValue.Month == 12)
                    {
                        emagValue.Key = $"{OldYear}{emagValue.Month:00}{emagValue.Date:00}"; // 2017
                    }
                    else
                    {
                        emagValue.Key = $"{NowYear}{emagValue.Month:00}{emagValue.Date:00}"; // 2018
                    }
                    
                    emagValue.Rkey = $"{emagValue.Hour:00}{emagValue.Minute:00}{eventName}";
                    _table.Add(emagValue);
                }
            }

            // Lambda��API�Ɍ�����Post
            public string PostEmagList(string postUrl, string apiKey)
            {
                // ���N�G�X�g�쐬
                var json = JsonConvert.SerializeObject(_table);
                _log.Info(json);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(postUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("x-api-key", apiKey);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
                
            }
            public string DebugShowList()
            {
                return JsonConvert.SerializeObject(_table);
            }
        }

        // ���N�G�X�gJson�p�N���X
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
    }
}