using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class BookingReason
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string TranslatedName { get; set; }
    }
}
