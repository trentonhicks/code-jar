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

        public static byte ConvertToByte(string state)
        {
            byte byteState = 0;

            switch(state)
            {
                case "Generated":
                    byteState = 0;
                    break;
                case "Active":
                    byteState = 1;
                    break;
                case "Expired":
                    byteState = 2;
                    break;
                case "Redeemed":
                    byteState = 3;
                    break;
                case "Inactive":
                    byteState = 4;
                    break;
            }

            return byteState;
        }
    }
    
}
 