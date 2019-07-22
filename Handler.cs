using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AwsDotnetCsharp
{
    public class Handler
    {
       public Response Hello()
       {
          LambdaLogger.Log("Function Start\n");
          return new Response("Not Found");
       }
    }
    public class Response
    {
      public int statusCode {get; set;}
      public string body {get; set;}
      public Dictionary<string, string> headers {get; set;}

      public Response(string message){
        body = message;
        statusCode = 404;
        headers = new Dictionary<string, string> {
                        { "Content-Type", "text/html" },
                    };
      }
    }
}
