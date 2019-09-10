using System;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Payment
    {
        [DataMember]
        public float Amount { get; set; }

        [DataMember]
        public DateTime ProcessedOn { get; set; }

        [DataMember]
        public string PaymentMethod { get; set; }

        [DataMember]
        public bool IsProcessed { get; set; }

        [DataMember]
        public int ProcessedBy { get; set; }
    }
}