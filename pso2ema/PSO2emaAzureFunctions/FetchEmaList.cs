using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http;
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

            string postUrl = Environment.GetEnvironmentVariable("DBEndpointURL");
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");

            log.Info($"url - {postUrl}");
            log.Info($"API Ver.2.1");

            PSO2EmagScraping emagScraping = new PSO2EmagScraping();

            string html = (new HttpClient()).GetStringAsync(pso2Url).Result;

            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;

            //サイト全体の読み込み
            doc.LoadHtml(html);

            emagScraping.ScrapingEventsList(doc);

            log.Info(emagScraping.DebugShowList());

            var res = emagScraping.PostEmagList(postUrl,apiKey);
            log.Info(res);

            log.Info("Exec Finish");
        }

        public class PSO2EmagScraping
        {
            private List<EmagTableValue> table = new List<EmagTableValue>();

            // LambdaのAPIに向けてPost
            public string PostEmagList(string postUrl, string apiKey)
            {
                // リクエスト作成
                var json = JsonConvert.SerializeObject(table);

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

            // 各イベントを取得してリストへ
            private void ScrapingEvents(HtmlNodeCollection eventsNodes,int hour,int minute)
            {
                foreach (var enent in eventsNodes)
                {
                    var value = new EmagTableValue();
                    value.Hour = hour;
                    value.Min = minute;

                    HtmlDocument emaStr = new HtmlDocument();
                    emaStr.LoadHtml(enent.InnerHtml);

                    var time = emaStr.DocumentNode.SelectNodes("//strong[@class='start']");
                    foreach (var t in time)
                    {
                        var monthAndDate = t.InnerHtml.Split('/');
                        value.Month = int.Parse(monthAndDate[0]);
                        value.Date = int.Parse(monthAndDate[1]);
                    }

                    var name = emaStr.DocumentNode.SelectNodes("//span");
                    foreach (var n in name)
                    {
                        value.EventName = n.InnerHtml;
                    }
                    value.Key = $"{NowYear}{value.Month:00}{value.Date:00}"; // 2017をどうにかする
                    value.Rkey = $"{value.Hour:00}{value.Min:00}";
                    table.Add(value);
                }
            }

            // 時間タグ(tHHmMM)ごとに情報を分割
            public void ScrapingHour(HtmlNodeCollection hourNodes,int hour,int minute)
            {
                foreach (var hourNode in hourNodes)
                {
                    var timeNode = new HtmlDocument();
                    timeNode.LoadHtml(hourNode.InnerHtml);
                    var emagH1W1 = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']"); // 緊急
                    var emagH1W2 = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W02 event-emergency']");// なんか半分の緊急
                    var liveH1W1 = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-live']");// ライブ

                    // 緊急の処理
                    if (emagH1W1 != null)
                    {
                        ScrapingEvents(emagH1W1,hour,minute);
                    }
                    // なんか他のと被ってる緊急
                    if (emagH1W2 != null)
                    {
                        ScrapingEvents(emagH1W2, hour, minute);
                    }
                    // ライブの処理
                    if (liveH1W1 != null)
                    {
                        ScrapingEvents(liveH1W1, hour, minute);
                    }
                }
            }

            // イベントの表を抽出
            public void ScrapingEventsList(HtmlDocument htmlDoc)
            {
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

            public string DebugShowList()
            {
                return JsonConvert.SerializeObject(table);
            }
        }

        // リクエストJson用クラス
        public class EmagTableValue
        {
            [JsonProperty(PropertyName = "key")] // yyyymmdd
            public string Key { get; set; }

            [JsonProperty(PropertyName = "rkey")] //hh
            public string Rkey { get; set; }

            [JsonProperty(PropertyName = "evant")]
            public string EventName { get; set; }

            [JsonProperty(PropertyName = "month")]
            public int Month { get; set; }

            [JsonProperty(PropertyName = "date")]
            public int Date { get; set; }

            [JsonProperty(PropertyName = "hour")]
            public int Hour { get; set; }

            [JsonProperty(PropertyName = "minute")]
            public int Min { get; set; }
        }
    }
}