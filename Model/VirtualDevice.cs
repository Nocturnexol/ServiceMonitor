using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BS.Microservice.Web.Model
{
    public class VirtualDevice
    {
        public ObjectId _id { get; set; }
        public int Rid { get; set; }
        public string ModelNum { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? Date { get; set; }
        public string PublicIP { get; set; }
        public string IntranetIP { get; set; }
        public string Remark { get; set; }
        public int DeviceType { get; set; }
        public int? HostDevice { get; set; }
        //public string HostDeviceName { get; set; }
        public string Cpu { get; set; }
        public string Memory { get; set; }
        public string Storage { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? StartDate { get; set; }
        public string MachineName { get; set; }
        public string DomainIP { get; set; }
    }
}