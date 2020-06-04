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
            if (State == CodeStates.Generated && DateActive >= today.Date && DateActive < DateExpires)
                State = CodeStates.Active;

            else
                throw new InvalidOperationException("can't change state to active when today's date is not the active date.");
        }

        public void ExpireActive(DateTime today)
        {
            if (State == CodeStates.Active && DateExpires >= today.Date)
                State = CodeStates.Expired;
            
            else
                throw new InvalidOperationException("can't set expired when state is active.");
        }

        public void ExpireGenerated(DateTime today)
        {
            if(State == CodeStates.Generated && today.Date >= DateExpires)
                State = CodeStates.Expired;
                
            else
                throw new InvalidOperationException("can't set expired when state is generated.");
        }
    }    
}
