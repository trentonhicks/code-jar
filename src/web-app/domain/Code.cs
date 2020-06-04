using System;
using System.Collections.Generic;
using System.Text;


namespace CodeJar.Domain
{
    public class Code
    {
        public Code(CodeState state)
        {
            State = state;
        }

        public int Id {get; set;}
        public int BatchId {get; set;}
        public string StringValue { get; set; }
        public int SeedValue {get; set;}
        public CodeState State { get; private set; }
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}
        public void Activate() => State = State.Activate();
        public void Expire() => State = State.Expire();
        public void Redeem() => State = State.Redeem();
        public void Deactivate() => State = State.Deactivate();
    }    
}
