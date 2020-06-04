using System;
using System.Collections.Generic;
using System.Text;

namespace CodeJar.Domain
{
    public abstract class CodeState
    {
        public virtual CodeExpiredState Expire()
        {
            throw new InvalidOperationException("cannot expire code.");
        }
        public virtual CodeActiveState Activate()
        {
            throw new InvalidOperationException("cannot activate code.");
        }
        public virtual CodeRedeemedState Redeem()
        {
            throw new InvalidOperationException("cannot redeem code.");
        }
        public virtual CodeInactiveState Deactivate()
        {
            throw new InvalidOperationException("cannot deactivate code.");
        }
    }
}
 