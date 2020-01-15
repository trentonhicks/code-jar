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
        public Boolean CreateDigitalCode(DateTime dateActive, DateTime dateExpires, SqlCommand command)
        {
            var sql = new SQL(ConnectionString);
            long offset = sql.GetOffset(command);
            if (offset % 4 != 0)
            {
                throw new ArgumentException("Offset must be divisible by 4");
            }

            // Date active must be less than date expires and greater than or equal to the current date time in order to generate codes
            if(dateActive < dateExpires && dateActive.Day >= DateTime.Now.Day)
            {

                using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
                {
                    reader.BaseStream.Position = offset;
                    var seedvalue = reader.ReadInt32();
                    offset = reader.BaseStream.Position;
                    sql.StoreRequestedCodes(seedvalue, offset, dateActive, command);
                    
                }
                return true;
            }

                return false;
        }


        
    }
}