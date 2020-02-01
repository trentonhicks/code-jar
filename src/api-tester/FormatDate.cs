using System;

namespace api_tester
{
    public static class FormatDate
    {
        public static string YearMonthDay(DateTime date)
        {
            var dateString = $"{date.Year}-";

            if(date.Month < 10)
            {
                dateString += $"0{date.Month}";
            }

            else
            {
                dateString += $"{date.Month}";
            }

            if(date.Day < 10)
            {
                dateString += $"-0{date.Day}";
            }

            else
            {
                dateString += $"-{date.Day}";
            }

            return dateString;
        }
    }
}