using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class BookingHistory
    {
        [DataMember]
        public int BookingId { get; set; }

        [DataMember]
        public string BookingReference { get; set; }

        [DataMember]
        public DateTime VisitDateTime { get; set; }

        [DataMember]
        public int Covers { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int[] Tables { get; set; }

        [DataMember]
        public int RestaurantId { get; set; }

        [DataMember]
        public string RestaurantName { get; set; }

        [DataMember]
        public string BookingComments { get; set; }

        [DataMember]
        public IEnumerable<SpecialRequest> SpecialRequests { get; set; }

        [DataMember]
        public double Spend { get; set; }

        [DataMember]
        public decimal? AverageReviewScore { get; set; }

        [DataMember]
        public string ReviewComments { get; set; }
    }
}