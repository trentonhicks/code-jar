using System;
using System.Collections.Generic;
using System.Text;

namespace CodeJar.Domain
{
    public class CodeDto
    {
        public int Id {get; set;}
        public int SeedValue {get; set;}
        public string State {get; set;}
    }

    public abstract class Code
    {
        public int Id {get; set;}
        public string State { get; set; }
    }

    public class GeneratedCode : Code
    {
        public GeneratedCode(Guid batchId, int seedValue)
        {
            BatchId = batchId;
            SeedValue = seedValue;
        }

        public Guid BatchId {get; set;}
        public int SeedValue {get; set;}
    }

    public class DeactivateCode : Code
    {
        public DateTime? When {get; private set;}
        public string By {get; private set;}

        public void Deactivate(string by, DateTime when)
        {
            By = by;
            When = when;
            base.State = CodeStates.Inactive;
        }
    }

    public class RedeemCode : Code
    {
        public DateTime? When {get; private set;}
        public string By {get; private set;}

        public void Redeem(string by, DateTime when)
        {
            By = by;
            When = when;
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
