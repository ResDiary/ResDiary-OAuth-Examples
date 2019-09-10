using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Interest
    {
        [DataMember]
        public int InterestId { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}