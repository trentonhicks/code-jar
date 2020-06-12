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
        public string State {get; set;}
        public DateTime DateActive {get; set;}
        public DateTime DateExpires {get; set;}

        public IEnumerable<Code> GenerateCodes(ISeedValueReader reader, string alphabet)
        {
            foreach(var seedValue in reader.ReadSeedValues(BatchSize))
            {
                var code = new Code(new CodeGeneratedState())
                {
                    BatchId = Id,
                    SeedValue = seedValue,
                    StringValue = CodeConverter.ConvertToCode(seedValue, alphabet)
                };

                yield return code;
            }
        }
    }
}
