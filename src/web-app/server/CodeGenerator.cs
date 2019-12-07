using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace CodeJar.WebApp
{
    class CodeGenerator
    {
        public CodeGenerator(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public string alphabet { get; } = "2BCD3FGH4JKLMN5PQRST6VWXYZ";

        public void CreateDigitalCode(int amount)
        {
            var sql = new SQL("Data Source=.; Initial Catalog=Random-Code; Integrated Security=SSPI;");

            long offset = sql.GetOffset();

            if (offset % 4 != 0)
            {
                throw new ArgumentException("Offset must be divisible by 4");
            }


            string filePath = @"C:\Users\Trenton Hicks\Documents\CodeFlip\Projects\code-jar\code-jar\src\web-app\Binary.bin";

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                reader.BaseStream.Position = offset;
                for (var i = 0; i < amount; i++)
                {
                    var seedvalue = reader.ReadInt32();
                    offset = reader.BaseStream.Position;
                    sql.StoreRequestedCodes(seedvalue, offset);
                }
            }
        }

        
    }
}
