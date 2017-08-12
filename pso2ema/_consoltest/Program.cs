using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                                value.Event = n.InnerHtml;
                            }
                            value.Key = $"{value.Month:00}{value.Date:00}{value.Hour:00}";
                            table.Add(value);
                        }
                    }
                }
            }
            var json = JsonConvert.SerializeObject(table);
            Console.WriteLine($"{json}");
        }
    }
    public class PostParam
    {
        public List<TableValue> table { get; set; }
    }
    public class TableValue
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        [JsonProperty(PropertyName = "month")]
        public int Month { get; set; }

        [JsonProperty(PropertyName = "date")]
        public int Date { get; set; }

        [JsonProperty(PropertyName = "hour")]
        public int Hour { get; set; }
    }
}
