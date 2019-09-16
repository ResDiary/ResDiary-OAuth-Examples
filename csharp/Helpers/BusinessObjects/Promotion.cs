using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Promotion
    {
        [DataMember]
        public int PromotionId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Quantity { get; set; }
    }
}