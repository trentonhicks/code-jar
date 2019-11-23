using System;
using System.IO;

class PascalTriangle
{
    static string symbols = "2BCD3FGH4JKLMN5PQRST6VWXYZ";

    static void Main(string[] args)
    {
            Console.WriteLine(Convert.ToString(DigitalCode(41, 10)));
            Console.ReadLine();
    }

    private static string EncodeToBaseString(int number)
    {
        var encBase = symbols.Length;

        var digits = "";
        var num = number;

        if (num == 0)
            return symbols[0].ToString();

        while (num > 0)
        {
            digits = symbols[num % encBase] + digits;
            num = num / encBase;
        }

        return digits;
    }

    private static long DigitalCode(long offset, int amount)
    {
        if(offset % 4 != 0)
        {
            throw new ArgumentException("Offset must be divisible by 4");
        }
       

        string filePath = @"C:\git\6 digit redeem project\6 Digit Code Generator\BitFile.bin";

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            reader.BaseStream.Position = offset;
            for (var i = 0; i < amount; i++)
            {
                var number = reader.ReadInt32();
                CreateDigitalCode(number);
            }

            offset = reader.BaseStream.Position;

            return offset;
        }
    }

    private static void CreateDigitalCode(int number)
    {
        var result = EncodeToBaseString(number);

        result = result.PadLeft(6, symbols[0]);

        Console.WriteLine(result);
    }
}