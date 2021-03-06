﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using TakeBusDraft.Classes;

namespace TakeBusDraft.Services
{
    public class QnAMakerSearcher
    {
     
        
        public List<QnAAnswer> Search(string text)
        {
           List<QnAAnswer> QnAresponse;
           string response;

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{ServiceKeys.knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{text}\",\"top\": \""+3+"\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", ServiceKeys.qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                response = client.UploadString(builder.Uri, postBody);
            }

            try
            {
                var res = JsonConvert.DeserializeObject<QnAAnswers>(response);
                QnAresponse = res.Answers;
                

            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }


            return QnAresponse;
        }
    }
}
