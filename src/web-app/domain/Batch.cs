using System;

namespace CodeJar.Domain
{
    public class Batch
    {
        public int ID {get; set;}
        public string BatchName {get; set;}
        public int CodeIDStart {get; set;}
        public int CodeIDEnd {get; set;}
        public int BatchSize {get; set;}
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}
    }
}
