using System;
using System.Collections.Generic;
using System.Text;


namespace CodeJar.WebApp
{
    public class Code
    {
        public string StringValue { get; set; }
        public string State { get; set; }
        public DateTime DateActive { get; set; }
        public DateTime DateExpires { get; set; }
        public Boolean IsChecked { get; set; } = false;
    }
}
