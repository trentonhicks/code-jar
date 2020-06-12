using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CodeJar.Domain;

namespace CodeJar.WebApp.ViewModels
{
    public class CodesViewModel
    {
        public CodesViewModel(List<CodeViewModel> codes, int pages) 
        {
            Codes = codes;
            Pages = pages;
        }
        
        public List<CodeViewModel> Codes {get;}

        public int Pages {get;}
    }
    
}