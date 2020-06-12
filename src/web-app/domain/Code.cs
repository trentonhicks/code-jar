using System;
using System.Collections.Generic;
using System.Text;


namespace CodeJar.Domain
{
    public class Code
    {
        public Code () { }

        public Code(Guid batchId, int value)
        {
            BatchId = batchId;
            SeedValue = value;
            State = CodeStates.Generated;
        }

        public int Id {get; set;}
        public Guid BatchId {get; set;}
        public int SeedValue {get; set;}
        public string State { get; set; }
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}
    }

    public class DeactivateCode : Code
    {
        public void Deactivate()
        {
            base.State = CodeStates.Inactive;
        }
    }

    public class RedeemCode : Code
    {
        public void Redeem()
        {
            base.State = CodeStates.Redeemed;
        }
    }

    public class ExpireCode : Code
    {
        public void Expire()
        {
            base.State = CodeStates.Expired;
        }
    }

    public class ActivateCode : Code
    {
        public void Activate()
        {
            base.State = CodeStates.Active;
        }
    }
}
