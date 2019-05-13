using System;
using System.Collections.Generic;
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

namespace PSO2CoatOfArms
{
    public class Function
    {
        const string projectName = "PSO2CoatOfArms";
        const string pso2Url = "http://pso2.jp/players/news/i_yget/";

        private static readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);

        public bool FunctionHandler()
        {
            try
            {
                var discordURL = Environment.GetEnvironmentVariable("DiscordURL");

                var dbContext = new DynamoDBContext(Client);

                var dbContents = new TableValue();
                dbContents.ProjectName = projectName;

                var html = (new HttpClient()).GetStringAsync(pso2Url).Result;

                var doc = new HtmlDocument();
                doc.OptionAutoCloseOnEnd = false;
                doc.OptionCheckSyntax = false;
                doc.OptionFixNestedTags = true;

                doc.LoadHtml(html);

                var events = doc.DocumentNode.SelectNodes($"//th[@class='sub']");

                dbContents.StringList = new List<string>();
                foreach (var eventNode in events)
                {
                    context.Logger.Log(eventNode.InnerHtml);
                    dbContents.StringList.Add(eventNode.InnerHtml);
                }
                dbContents.UpdateTime = DateTime.UtcNow.ToString();

                var insertTask = dbContext.SaveAsync(dbContents);
                insertTask.Wait();

                var postText = "�E�҂̖�͎擾�o�b�`���� \n";
                foreach (var targetName in dbContents.StringList)
                {
                    postText += $"{targetName} \n";
                }
                postText += $"\n ���s����(UTC) : {dbContents.UpdateTime}";

                using (var client = new HttpClient())
                {
                    context.Logger.Log(postText);
                    var discordContent = JsonConvert.SerializeObject(new DiscordMessage(postText));
                    var stringContent = new StringContent(discordContent, Encoding.UTF8, "application/json");
                     var _ = client.PostAsync(discordURL, stringContent).Result;
                }
            }
            catch (Exception e)
            {
                context.Logger.Log(e.ToString());
                return false;
            }
            return true;

        }
    }

    public class DiscordMessage
    {
        public DiscordMessage(string value)
        {
            this.content = value;
        }
        public string content { get; set; }
    }

    [DynamoDBTable()]
    public class TableValue
    {
        [DynamoDBHashKey]
        public string ProjectName { get; set; }

        [DynamoDBProperty("StringList")]
        public List<string> StringList { get; set; }


        [DynamoDBProperty("UpdateTime")]
        public string UpdateTime { get; set; }
    }
}
