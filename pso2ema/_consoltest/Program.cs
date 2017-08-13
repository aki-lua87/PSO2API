using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _consoltest
{
    class Program
    {
        static void Main(string[] args)
        {
            var table = new List<TableValue>();

            var url = "http://pso2.jp/players/boost/";
            string html = (new HttpClient()).GetStringAsync(url).Result;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;  //最後に自動で閉じる（？）
            doc.OptionCheckSyntax = false;     //文法チェック。
            doc.OptionFixNestedTags = true;    //閉じタグが欠如している場合の処理
            doc.LoadHtml(html);

            HtmlAgilityPack.HtmlNodeCollection events = doc.DocumentNode.SelectNodes($"//div[@class='eventTable--event']");
            foreach (HtmlAgilityPack.HtmlNode event_node in events)
            {
                var eveStr = new HtmlAgilityPack.HtmlDocument();
                eveStr.LoadHtml(event_node.InnerHtml);
                for (var i = 0; i < 24; i++)
                {
                    var timeTagStr = String.Format("{0:00}", i);
                    var nodes = eveStr.DocumentNode.SelectNodes($"//tr[@class='t{timeTagStr}m00']"); // t02m00
                    foreach (var node in nodes)
                    {
                        var time_node = new HtmlAgilityPack.HtmlDocument();
                        time_node.LoadHtml(node.InnerHtml);
                        var emaList = time_node.DocumentNode.SelectNodes("//div[@class='cell-H01 cell-W01 event-emergency']");
                        if (emaList == null)
                        {
                            break;
                        }

                        foreach (var ema in emaList)
                        {
                            var value = new TableValue();
                            value.Hour = i;

                            HtmlAgilityPack.HtmlDocument emaStr = new HtmlAgilityPack.HtmlDocument();
                            emaStr.LoadHtml(ema.InnerHtml);

                            var time = emaStr.DocumentNode.SelectNodes("//strong[@class='start']");
                            foreach (var t in time)
                            {
                                // Console.WriteLine($"{t.InnerHtml} {timeTagStr}:00");
                                var monthAndDate = t.InnerHtml.Split('/');
                                value.Month = int.Parse(monthAndDate[0]);
                                value.Date = int.Parse(monthAndDate[1]);
                            }

                            var name = emaStr.DocumentNode.SelectNodes("//span");
                            foreach (var n in name)
                            {
                                // Console.WriteLine($"{n.InnerHtml}");
                                value.EventName = n.InnerHtml;
                            }
                            value.Key = $"2017{value.Month:00}{value.Date:00}"; // 2017をどうにかする
                            value.Rkey = $"{value.Hour}";　// mmはいる？
                            table.Add(value);
                        }
                    }
                }
            }
            var json = JsonConvert.SerializeObject(table);
            Console.WriteLine($"{json}");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://uucn9ca6he.execute-api.ap-northeast-1.amazonaws.com/PSO2emaPut");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("x-api-key", "6h1speyDY86CPxFt3t0ZT6kKvHSIn9lV51ydfmDd");

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

            [JsonProperty(PropertyName = "rkey")] //hhmm
            public string Rkey { get; set; }

            [JsonProperty(PropertyName = "evant")]
            public string EventName { get; set; }

            [JsonProperty(PropertyName = "month")]
            public int Month { get; set; }

            [JsonProperty(PropertyName = "date")]
            public int Date { get; set; }

            [JsonProperty(PropertyName = "hour")]
            public int Hour { get; set; }
        }
    }
}