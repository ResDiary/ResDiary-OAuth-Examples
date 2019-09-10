using System;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [Serializable]
    [DataContract]
    public class Channel
    {
        [DataMember]
        public int ChannelId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Colour { get; set; }
    }
}
