using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
namespace CodeJar.WebApp
{
    public class CodeGenerator
    {
        public CodeGenerator(string connectionString, string filePath)
        {
            ConnectionString = connectionString;
            FilePath = filePath;
        }
        public string ConnectionString { get; set; }
        public string FilePath {get; set;}
        public void CreateDigitalCode(int amount)
        {
            var sql = new SQL(ConnectionString);
            long offset = sql.GetOffset();
            if (offset % 4 != 0)
            {
                throw new ArgumentException("Offset must be divisible by 4");
            }
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {
                reader.BaseStream.Position = offset;
                for (var i = 0; i < amount; i++)
                {
                    var seedvalue = reader.ReadInt32();
                    offset = reader.BaseStream.Position;
                    sql.StoreRequestedCodes(seedvalue, offset);
                }
            }
        }
        
    }
}