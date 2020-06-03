using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public class FileSystemSeedValueReader : ISeedValueReader
    {
        private readonly string path;
        private readonly SqlConnection _connection;

        public FileSystemSeedValueReader(string path, SqlConnection connection)
        {
            this.path = path;
            _connection = connection;
        }

        private (long, long) UpdateOffset(int count)
        {
            long start;
            long end;
            _connection.Open();
            using(var command = _connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Offset
                                   SET OffsetValue = OffsetValue + @offsetIncrement
                                   OUTPUT INSERTED.OffsetValue
                                   WHERE ID = 1";

                command.Parameters.AddWithValue("@offsetIncrement", count * 4);
                
                end = (long)command.ExecuteScalar();

                start = end - count * 4;

            }
            _connection.Close();
            return (start, end);
        }

        public IEnumerable<int> ReadSeedValues(int count)
        {
            var startAndEnd = UpdateOffset(count);
            var start = startAndEnd.Item1;
            var end = startAndEnd.Item2;

            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Get the next offset position
                if (start % 4 != 0)
                    throw new ArgumentException("Offset must be divisible by 4");

                // Loop to the last offset position
                for (var i = start; i < end; i += 4)
                {
                    // Set reader to offset position
                    reader.BaseStream.Position = i;
                    var seedValue = reader.ReadInt32();

                    yield return seedValue;
                }
            }
        }
    }
}