using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class ReallocatedBooking
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public Booking Booking { get; set; }
    }
}
