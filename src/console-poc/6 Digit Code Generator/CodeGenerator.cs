using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _6_Digit_Code_Generator
{
    class CodeGenerator
    {
        public string alphabet { get; } = "2BCD3FGH4JKLMN5PQRST6VWXYZ";

        public void CreateDigitalCode(int amount)
        {
            var sql = new SQL("Data Source=.; Initial Catalog=Random-Code; Integrated Security=SSPI;");

            long offset = sql.GetSeedValue();


            if (offset % 4 != 0)
            {
                throw new ArgumentException("Offset must be divisible by 4");
            }


            string filePath = @"C:\git\gatheraround\BitFile.bin";

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                reader.BaseStream.Position = offset;
                for (var i = 0; i < amount; i++)
                {
                    var number = reader.ReadInt32();
                    var code = ConvertToCode(number, alphabet);
                    var seedvalue = reader.BaseStream.Position - 4;
                    sql.StoreRequestedCodes(code, seedvalue);
                }
            }
        }

        private static string ConvertToCode(int number, string alphabet)
        {
            var result = EncodeToBaseString(number, alphabet);

            result = result.PadLeft(6, alphabet[0]);

            return result;
        }

        private static string EncodeToBaseString(int number, string alphabet)
        {
            var encBase = alphabet.Length;

            var digits = "";
            var num = number;

            if (num == 0)
                return alphabet[0].ToString();

            while (num > 0)
            {
                digits = alphabet[num % encBase] + digits;
                num = num / encBase;
            }

            return digits;
        }
    }
}
