using System;
using System.Collections.Generic;
using System.Text;

namespace _6_Digit_Code_Generator
{
    class Code
    {
        public int ID { get; set; }
        public long SeedValue { get; set; }
        public string StringValue { get; set; }
        public string State { get; set; }
        public DateTime DateActive { get; set; }
        public DateTime DateExpires { get; set; }
    }
}
