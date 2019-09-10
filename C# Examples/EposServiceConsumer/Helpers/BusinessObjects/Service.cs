using System;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Service
    {
        [DataMember]
        public int ServiceId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public DateTime LastBookingTime { get; set; }

        [DataMember]
        public int TimeSlotInterval { get; set; }
    }
}