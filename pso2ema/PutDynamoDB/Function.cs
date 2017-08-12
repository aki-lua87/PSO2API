using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.DynamoDBv2.DataModel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace PutDynamoDB
{
    public class Function
    {
        private static readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);

        public LambdaResponse FunctionHandler(List<TableValue> input, ILambdaContext context)
        {
            // Table table = Table.LoadTable(Client, tableName);

            var dbContext = new DynamoDBContext(Client);

            foreach (var v in input)
            {
                var insertTask = dbContext.SaveAsync(v);
                insertTask.Wait();
            }

            return new LambdaResponse
            {
                StatusCode = HttpStatusCode.OK,
                Test = input[0].Event
            };
        }
    }

    [DynamoDBTable("PSO2ema")]
    public class TableValue
    {
        [DynamoDBHashKey]
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [DynamoDBRangeKey]
        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        [DynamoDBProperty("Month")]
        [JsonProperty(PropertyName = "month")]
        public int Month { get; set; }

        [DynamoDBProperty("Date")]
        [JsonProperty(PropertyName = "date")]
        public int Date { get; set; }

        [DynamoDBProperty("Hour")]
        [JsonProperty(PropertyName = "hour")]
        public int Hour { get; set; }
    }

    public class LambdaResponse
    {
        [JsonProperty(PropertyName = "statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty(PropertyName = "test")]
        public string Test { get; set; }
    }
}
