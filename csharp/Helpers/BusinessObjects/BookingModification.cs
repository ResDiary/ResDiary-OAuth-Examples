using System;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    public class BookingModification
    {
        public int BookingId { get; set; }
        public DateTime ChangeDateTime { get; set; }
        public BookingModificationChangeType ChangeType { get; set; }
        public BookingStatus? Status { get; set; }
        public int? Covers { get; set; }
        public DateTime? VisitDateTime { get; set; }
        public int[] Promotions { get; set; }
        public double? Cost { get; set; }
        public bool? IsReviewed { get; set; }
        public int? OldProviderId { get; set; }
        public int? NewProviderId { get; set; }
        public ArrivalStatus ArrivalStatus { get; set; }
    }
}
