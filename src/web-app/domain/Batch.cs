using System;
using System.Collections.Generic;
using System.IO;

namespace CodeJar.Domain
{

    public class Batch
    {
        public Guid Id {get; set;}
        public string BatchName {get; set;}
        public int BatchSize {get; set;}
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}

        public IEnumerable<GeneratedCode> GenerateCodes(ISeedValueReader reader)
        {
            foreach(var seedValue in reader.ReadSeedValues(BatchSize))
                yield return new GeneratedCode(Id, seedValue);
        }
    }
}