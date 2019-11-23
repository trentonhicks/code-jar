using System;
using System.IO;

namespace _6_Digit_Code_Generator
{
    class Program
    {

        static void Main(string[] args)
        {
            CodeGenerator codeGenerator = new CodeGenerator();
            Console.WriteLine(Convert.ToString(codeGenerator.DigitalCode(40, 10)));
            Console.ReadLine();
        }

    }
}
