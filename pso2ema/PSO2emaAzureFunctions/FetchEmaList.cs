using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http;

namespace PSO2emaAzureFunctions
{
    public static class FetchEmaList
    {
        [FunctionName("FetchEmaList")]
        public static void Run([TimerTrigger("0 30 7 * * 3")]TimerInfo myTimer, TraceWriter log)
        {
            const string pso2Url = "http://pso2.jp/players/boost/";
            string postUrl = Environment.GetEnvironmentVariable("DBEndpointURL");
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");

            log.Info($"url - {postUrl}");
            log.Info($"API Ver.2.0");

            var table = new List<TableValue>();

            
            string html = (new HttpClient()).GetStringAsync(pso2Url).Result;

            var doc = new HtmlAgilityPack.HtmlDocument
            {
                OptionAutoCloseOnEnd = false,
                OptionCheckSyntax = false,
                OptionFixNestedTags = true
            };

            doc.LoadHtml(html);

            HtmlAgilityPack.HtmlNodeCollection events = doc.DocumentNode.SelectNodes($"//div[@class='eventTable--event']");
            foreach (HtmlAgilityPack.HtmlNode eventNode in events)
            {
                var eveStr = new HtmlAgilityPack.HtmlDocument();
                eveStr.LoadHtml(eventNode.InnerHtml);
                for (var i = 0; i < 24; i++)
                {
                    var timeTagStr = $"{i:00}";
                    var nodes = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m00']"); // t02m00
                    var nodes30 = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m30']"); // t02m30
                    // 00の処理
                    foreach (var node in nodes)
                    {
                        var timeNode = new HtmlAgilityPack.HtmlDocument();
                        timeNode.LoadHtml(node.InnerHtml);
                        var emaList = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']");
                        var liveList = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-live']");

                        // 緊急の処理
                        if (emaList == null)
                        {
                            break;
                        }

                        foreach (var ema in emaList)
                        {
                            var value = new TableValue();
                            value.Hour = i;
                            value.Min = 0;

                            HtmlAgilityPack.HtmlDocument emaStr = new HtmlAgilityPack.HtmlDocument();
                            emaStr.LoadHtml(ema.InnerHtml);

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
                            value.Key = $"2017{value.Month:00}{value.Date:00}"; // 2017をどうにかする
                            value.Rkey = $"{value.Hour:00}{value.Min:00}";　// mmはいる？
                            table.Add(value);
                        }

                        // ライブの処理
                        if (liveList == null)
                        {
                            break;
                        }

                        foreach (var live in liveList)
                        {
                            var value = new TableValue();
                            value.Hour = i;
                            value.Min = 0;

                            HtmlAgilityPack.HtmlDocument emaStr = new HtmlAgilityPack.HtmlDocument();
                            emaStr.LoadHtml(live.InnerHtml);

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
                            value.Key = $"2017{value.Month:00}{value.Date:00}"; // 2017をどうにかする
                            value.Rkey = $"{value.Hour:00}{value.Min:00}";　// mmはいる？
                            table.Add(value);
                        }

                    }

                    // 30の処理
                    foreach (var node30 in nodes30)
                    {
                        var timeNode = new HtmlAgilityPack.HtmlDocument();
                        timeNode.LoadHtml(node30.InnerHtml);
                        var emaList = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']");
                        var liveList = timeNode.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-live']");

                        // 緊急の処理
                        if (emaList == null)
                        {
                            break;
                        }

                        foreach (var ema in emaList)
                        {
                            var value = new TableValue();
                            value.Hour = i;
                            value.Min = 30;

                            HtmlAgilityPack.HtmlDocument emaStr = new HtmlAgilityPack.HtmlDocument();
                            emaStr.LoadHtml(ema.InnerHtml);

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
                            value.Key = $"2017{value.Month:00}{value.Date:00}"; // 2017をどうにかする
                            value.Rkey = $"{value.Hour:00}{value.Min:00}";　// mmはいる？
                            table.Add(value);
                        }

                        // ライブの処理
                        if (liveList == null)
                        {
                            break;
                        }

                        foreach (var live in liveList)
                        {
                            var value = new TableValue();
                            value.Hour = i;
                            value.Min = 30;

                            HtmlAgilityPack.HtmlDocument emaStr = new HtmlAgilityPack.HtmlDocument();
                            emaStr.LoadHtml(live.InnerHtml);

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
                            value.Key = $"2017{value.Month:00}{value.Date:00}"; // 2017をどうにかする
                            value.Rkey = $"{value.Hour:00}{value.Min:00}";
                            table.Add(value);
                        }

                    }
                }

                

            }
            var json = JsonConvert.SerializeObject(table);
            Console.WriteLine($"{json}");

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
                Console.WriteLine($"{result}");
            }
        }
        public class TableValue
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