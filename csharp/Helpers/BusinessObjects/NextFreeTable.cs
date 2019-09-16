using System;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [Serializable]
    [DataContract]
    public class NextFreeTable
    {
        [DataMember]
        public int Covers { get; set; }

        [DataMember]
        public string TableNumber { get; set; }

        [DataMember]
        public int[] TableIds { get; set; }

        [DataMember]
        public DateTime AvailableTime { get; set; }
    }
}