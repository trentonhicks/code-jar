using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace api_tester
{
    public class TableData
    {
       
        public List<Code> Codes {get; set;} = new List<Code>();

        public int Pages {get; set;}
    }
}