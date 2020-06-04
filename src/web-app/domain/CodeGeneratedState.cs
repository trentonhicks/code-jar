using System;
using System.Collections.Generic;
using System.Text;

namespace CodeJar.Domain
{
    public class CodeGeneratedState : CodeState
    {
        public override CodeExpiredState Expire() => new CodeExpiredState();

        public override CodeActiveState Activate() => new CodeActiveState();

        public override string ToString() => "Generated";
    }
}
 