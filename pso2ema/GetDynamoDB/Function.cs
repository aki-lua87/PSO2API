using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GetDynamoDB
{
    public class Function
    {
        private static readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);

        public TableValue[] FunctionHandler(string input, ILambdaContext context)
        {
            var dbContext = new DynamoDBContext(Client);
            var emaList = dbContext.QueryAsync<TableValue>(input).GetNextSetAsync().Result;

            TableValue[] emaArray =emaList.OrderBy(a => a.Hour).ToArray();

            return emaArray;
        }
    }

    [DynamoDBTable("PSO2ema")]
    public class TableValue
    {
        // [JsonProperty(PropertyName = "key")] // yyyymmdd
        [DynamoDBHashKey]
        [JsonIgnore]
        public string Key { get; set; }

        // [JsonProperty(PropertyName = "rkey")] //hhmm
        [DynamoDBRangeKey]
        [JsonIgnore]
        public string Rkey { get; set; }

        [DynamoDBProperty("EvantName")]
        [JsonProperty(PropertyName = "evant")]
        public string EventName { get; set; }

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
        //[JsonProperty(PropertyName = "statusCode")]
        //public HttpStatusCode StatusCode { get; set; }

        [JsonProperty(PropertyName = "emajson")]
        public TableValue[] EmaList { get; set; }
    }
}
