using _6_Digit_Code_Generator;
using System;
using System.IO;

class Program
{

    static void Main(string[] args)
    {
            CodeGenerator codeGenerator = new CodeGenerator();
            Console.WriteLine(Convert.ToString(codeGenerator.DigitalCode(40,10)));
            Console.ReadLine();
    }
  
}