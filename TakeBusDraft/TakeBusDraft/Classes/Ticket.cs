using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TakeBusDraft.Classes
{
    [Serializable]
    public class Ticket
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public int CalculatePrice()
        {
            //stub
            return 10000;
        }


    }
}