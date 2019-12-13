using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CodeJar.WebApp
{

    
    public class TableData
    {

        public TableData(List<Code> codes, int pages) 
        {
          
        }
        public List<Code> Codes {get; set;}

        public List<Code> Pages {get; set;}

       
    }
    
}