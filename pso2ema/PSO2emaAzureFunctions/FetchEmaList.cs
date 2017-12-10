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
        const string pso2Url = "http://pso2.jp/players/boost/";
        const int NowYear = 2017;

        // 水曜16：30(JST)に実行
        [FunctionName("FetchEmaList")]
        public static void Run([TimerTrigger("0 30 7 * * 3")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info("Exec Start");

            // LambdaのURL
            string postUrl = Environment.GetEnvironmentVariable("DBEndpointURL");
            // LambdaのAPIキー
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");

            log.Info($"url - {postUrl}");
            log.Info($"API Ver.2.1");

            // 全HTMLを読み込み
            string html = (new HttpClient()).GetStringAsync(pso2Url).Result;

            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;

            //サイト全体の読み込み
            doc.LoadHtml(html);

            PSO2EmagScraping emagScraping = new PSO2EmagScraping(log,doc);

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
                _log.Info("Excec Nest2");
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
                _log.Info($"Excec Nest3 {eventName}");
                foreach (var enent in eventsNodes)
                {
                    _log.Info("Debug 0");
                    var emagValue = new EmagTableValue
                    {
                        Hour = hour,
                        Minute = minute,
                        EventType = eventName
                    };

                    _log.Info("Nest3 Debug1");

                    var emaStr = new HtmlDocument();
                    emaStr.LoadHtml(enent.InnerHtml);

                    var time = emaStr.DocumentNode.SelectNodes("//strong[@class='start']");
                    foreach (var t in time)
                    {
                        var monthAndDate = t.InnerHtml.Split('/');
                        emagValue.Month = int.Parse(monthAndDate[0]);
                        emagValue.Date = int.Parse(monthAndDate[1]);
                    }

                    _log.Info("Nest3 Debug2");

                    var name = emaStr.DocumentNode.SelectNodes("//span");
                    foreach (var n in name)
                    {
                        emagValue.EventName = n.InnerHtml;
                    }
                    _log.Info("Nest3 Debug3");
                    emagValue.Key = $"{NowYear}{emagValue.Month:00}{emagValue.Date:00}"; // 2017をどうにかする
                    emagValue.Rkey = $"{emagValue.Hour:00}{emagValue.Minute:00}{eventName}";
                    _table.Add(emagValue);
                }
                _log.Info("Excec Nest3 End");
            }

            // LambdaのAPIに向けてPost
            public string PostEmagList(string postUrl, string apiKey)
            {
                // リクエスト作成
                var json = JsonConvert.SerializeObject(_table);
                _log.Info("Excec Nest4(POST)");
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

        // リクエストJson用クラス
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