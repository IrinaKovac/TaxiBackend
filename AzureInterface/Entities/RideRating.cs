using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AzureInterface.Entities
{
    public class RideRating : ITableEntity
    {
        public RideRating() {}
        public RideRating(string partitionKey, string rowKey) 
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string ClientEmail { get; set; }
        public long RideTimestamp { get; set; }
        public string DriverEmail { get; set; }
        public int Value { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
