using System;
using System.IO;


namespace _6_Digit_Code_Generator
{
    class Program
    {

        static void Main(string[] args)
        {
            CodeGenerator codeGenerator = new CodeGenerator();
            codeGenerator.CreateDigitalCode(50);
        }

    }
}
