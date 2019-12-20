using System;
using System.Collections.Generic;
using System.Text;

namespace CodeJar.WebApp
{

    public static class States
    {
        public static byte Generated = 0;
        public static byte Active = 1;
        public static byte Expired = 2;
        public static byte Redeemed = 3;
        public static byte Inactive = 4;

        public static string ConvertToString(byte state)
        {
            var stateString = "";

            switch(state)
            {
                case 0:
                    stateString = "Generated";
                    break;
                case 1:
                    stateString = "Active";
                    break;
                case 2:
                    stateString = "Expired";
                    break;
                case 3:
                    stateString = "Redeemed";
                    break;
                case 4:
                    stateString = "Inactive";
                    break;
            }

            return stateString;
        }
    }
    
}
 