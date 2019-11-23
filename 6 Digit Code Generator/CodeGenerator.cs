using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _6_Digit_Code_Generator
{
    class CodeGenerator
    {
        public string alphabet { get; } = "2BCD3FGH4JKLMN5PQRST6VWXYZ";

        public long DigitalCode(long offset, int amount)
        {
            if (offset % 4 != 0)
            {
                throw new ArgumentException("Offset must be divisible by 4");
            }


            string filePath = @"C:\git\code-jar\BitFile.bin";

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                reader.BaseStream.Position = offset;
                for (var i = 0; i < amount; i++)
                {
                    var number = reader.ReadInt32();
                    CreateDigitalCode(number, alphabet);
                }

                offset = reader.BaseStream.Position;

                return offset;
            }
        }

        private static void CreateDigitalCode(int number, string alphabet)
        {
            var result = EncodeToBaseString(number, alphabet);

            result = result.PadLeft(6, alphabet[0]);

            Console.WriteLine(result);
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
