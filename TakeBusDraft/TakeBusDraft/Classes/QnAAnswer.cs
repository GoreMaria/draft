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
        [JsonProperty(PropertyName = "questions")]
        public List<string> Questions { get; set; }
        // The top answer found in the QnA Service.
        [JsonProperty(PropertyName = "answer")]
        public string Answer{ get; set; }

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