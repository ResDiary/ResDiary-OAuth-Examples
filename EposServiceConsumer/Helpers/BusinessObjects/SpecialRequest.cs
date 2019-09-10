using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class SpecialRequest
    {
        [DataMember]
        public int SpecialRequestId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Symbol { get; set; }
    }
}