using System;

namespace CodeJar.WebApp.Commands
{
    public class CreateBatchCommand
    {
        public string BatchName {get; set;}
        public int BatchSize {get; set;}
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}
    }
}