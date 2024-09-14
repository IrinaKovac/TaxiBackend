using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureInterface.Entities
{
    public class Ride : ITableEntity
    {
        public Ride() { }
        public Ride(string partitionKey, string rowKey) 
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    
        // Row Key
        public long CreatedAtTimestamp { get; set; }
        public string StartAddress {  get; set; }
        public string EndAddress {  get; set; }
        // Partition Key
        public string ClientEmail { get; set; }
        public string? DriverEmail { get; set; }
        public int Status { get; set; }
        public float Price { get; set; }
        public DateTime EstimatedDriverArrival { get; set; }
        public DateTime? EstimatedRideEnd { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
