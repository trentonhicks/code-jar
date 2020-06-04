using System;
using System.Collections.Generic;
using System.Text;
using CodeJar.Domain;

namespace CodeJar.Infrastructure
{

    public static class CodeStateSerializer
    {
        public static byte Generated = 0;
        public static byte Active = 1;
        public static byte Expired = 2;
        public static byte Redeemed = 3;
        public static byte Inactive = 4;

        public static CodeState DeserializeState(byte state)
        {
            switch(state)
            {
                case 0:
                    return new CodeGeneratedState();
                case 1:
                    return new CodeActiveState();
                case 2:
                    return new CodeExpiredState();
                case 3:
                    return new CodeRedeemedState();
                case 4:
                    return new CodeInactiveState();
                default:
                    throw new ArgumentException("invalid state value.");
            }
        }

        public static byte SerializeState(CodeState state)
        {
            if (state is CodeGeneratedState)
                return 0;

            else if (state is CodeActiveState)
                return 1;
            
            else if (state is CodeExpiredState)
                return 2;
            
            else if (state is CodeRedeemedState)
                return 3;

            else if (state is CodeInactiveState)
                return 4;
            
            else
                throw new ArgumentException("invalid state value.");
        }
    }
    
}
 