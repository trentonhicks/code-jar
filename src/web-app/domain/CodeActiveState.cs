using System;
using System.Collections.Generic;
using System.Text;

namespace CodeJar.Domain
{
    public class CodeActiveState : CodeState
    {
        public override CodeExpiredState Expire() => new CodeExpiredState();

        public override CodeRedeemedState Redeem() => new CodeRedeemedState();

        public override CodeInactiveState Deactivate() => new CodeInactiveState();

        public override string ToString() => "Active";
    }
}
 