using System;
using System.Collections.Generic;
using System.Text;

namespace CodeJar.Infrastructure
{

    public static class BatchStates
    {
        public static string Pending = nameof(Pending);
        public static string Generated = nameof(Generated);
        public static string ConvertToString(byte state)
        {
            var stateString = "";

            switch(state)
            {
                case 0:
                    stateString = nameof(Pending);
                    break;
                case 1:
                    stateString = nameof(Generated);
                    break;
            }

            return stateString;
        }

        public static byte ConvertToByte(string state)
        {
            byte byteState = 0;

            switch(state)
            {
                case nameof(Pending):
                    byteState = 0;
                    break;
                case nameof(Generated):
                    byteState = 1;
                    break;
            }

            return byteState;
        }
    }
    
}
 