using System;
using System.Collections.Generic;
using System.Text;


namespace CodeJar.Domain
{
    public class Code
    {
        public int Id {get; set;}
        public int BatchId {get; set;}
        public string StringValue { get; set; }
        public int SeedValue {get; set;}
        public byte State { get; set; }
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}

        public void Activate(DateTime today)
        {
            if(DateActive == today.Date)
                State = CodeStates.Active;

            else
                throw new InvalidOperationException("can't change state to active when today's date is not the active date.");
        }
    }
}
