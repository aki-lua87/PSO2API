using System;
using System.Collections.Generic;
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

namespace PSO2CoatOfArms
{
    public class Function
    {
        const string projectName = "PSO2CoatOfArms";
        // 勇者の紋章URL
        const string pso2Url = "http://pso2.jp/players/news/i_kget/";

        public bool FunctionHandler()
        {
            try
            {
                AWSSDKHandler.RegisterXRayForAllServices();

                var discordURL = Environment.GetEnvironmentVariable("DiscordURL");
                var tableName = Environment.GetEnvironmentVariable("TABLE_NAME");

                var Client = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);
                var dbContext = new DynamoDBContext(Client);

                var dbContents = new TableValue();
                dbContents.ProjectName = projectName;

                var html = (new HttpClient(new HttpClientXRayTracingHandler(new HttpClientHandler()))).GetStringAsync(pso2Url).Result;

                var doc = new HtmlDocument();
                doc.OptionAutoCloseOnEnd = false;
                doc.OptionCheckSyntax = false;
                doc.OptionFixNestedTags = true;

                doc.LoadHtml(html);

                var events = doc.DocumentNode.SelectNodes($"//th[@class='sub']");

                dbContents.StringList = new List<string>();
                foreach (var eventNode in events)
                {
                    LambdaLogger.Log(eventNode.InnerHtml);
                    dbContents.StringList.Add(eventNode.InnerHtml);
                }
                dbContents.UpdateTime = DateTime.UtcNow.ToString();

                var request = new PutItemRequest
                {
                    TableName = tableName,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        { "keyName", new AttributeValue { S = dbContents.ProjectName }},
                        { "StringList", new AttributeValue { SS = dbContents.StringList }},
                        { "UpdateTime", new AttributeValue { S = dbContents.UpdateTime }},
                    }
                };
                var insertTask = Client.PutItemAsync(request);
                insertTask.Wait();

                var postText = "紋章取得結果 \n";
                foreach (var targetName in dbContents.StringList)
                {
                    postText += $"{targetName} \n";
                }
                postText += $"\n (UTC) : {dbContents.UpdateTime}";

                // Post Discord
                if(false)
                {
                    using (var client = new HttpClient())
                    {
                        LambdaLogger.Log(postText);
                        var discordContent = JsonConvert.SerializeObject(new DiscordMessage(postText));
                        var stringContent = new StringContent(discordContent, Encoding.UTF8, "application/json");
                        var _ = client.PostAsync(discordURL, stringContent).Result;
                    }
                }
                
            }
            catch (Exception e)
            {
                LambdaLogger.Log(e.ToString());
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

    public class TableValue
    {
        public string ProjectName { get; set; }
        public List<string> StringList { get; set; }
        public string UpdateTime { get; set; }
    }
}
