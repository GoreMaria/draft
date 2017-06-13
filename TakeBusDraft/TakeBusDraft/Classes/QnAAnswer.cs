using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TakeBusDraft.Classes
{
    [Serializable]
    public class QnAAnswer
    {
        // The top answer found in the QnA Service.
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }

        // The score in range [0, 100] corresponding to
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }

    [Serializable]
    public class QnAAnswers
    {
        [JsonProperty(PropertyName = "answers")]
        public List<QnAAnswer> Answers;
    }
}