using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.Storage.Blob;
using System.Threading;
using System.Threading.Tasks;
using CodeJar.Domain;
using System.Data.SqlClient;
using CodeJar.Infrastructure;

namespace CodeFlip.CodeJar.Api
{
    public class CloudReader : ISeedValueReader
    {
        private readonly SqlConnection _connection;

        public CloudReader(string filePath, SqlConnection connection)
        {
            FilePath = new Uri(filePath);
            _connection = connection;
        }

        public Uri FilePath { get; private set; }

        public IEnumerable<int> ReadSeedValues(int count)
        {
            var offset = new Offset(_connection);
            var startAndEnd = offset.UpdateOffset(count);
            var start = startAndEnd.Item1;
            var end = startAndEnd.Item2;
            
            var file = new CloudBlockBlob(FilePath);

            for(var i = start; i < end; i += 4)
            {
                var bytes = new byte[4];
                file.DownloadRangeToByteArray(bytes, index: 0, blobOffset: i, length: 4);
                var seedValue = BitConverter.ToInt32(bytes, 0);
                yield return seedValue;
            }
        }
    }
}
