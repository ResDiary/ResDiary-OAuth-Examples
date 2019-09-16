using System;
using System.Runtime.Serialization;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class DiaryBooking
    {
        [DataMember]
        public int BookingId { get; set; }

        [DataMember]
        public string BookingReference { get; set; }

        [DataMember]
        public DateTime VisitDateTime { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public int TurnTime { get; set; }

        [DataMember]
        public int Covers { get; set; }

        [DataMember]
        public int[] Tables { get; set; }

        [DataMember]
        public int[] SpecialRequests { get; set; }

        [DataMember]
        public int CustomerId { get; set; }

        [DataMember]
        public string CustomerFullName { get; set; }

        [DataMember]
        public bool CustomerIsVip { get; set; }

        [DataMember]
        public string CustomerComments { get; set; }

        [DataMember]
        public bool IsTableLocked { get; set; }

        [DataMember]
        public bool HasPromotions { get; set; }

        [DataMember]
        public bool HasPayments { get; set; }

        [DataMember]
        public bool IsLeaveTimeConfirmed { get; set; }

        [DataMember]
        public bool IsWalkIn { get; set; }

        [DataMember]
        public int NumberOfBookings { get; set; }

        [DataMember]
        public int AreaId { get; set; }

        [DataMember]
        public int ServiceId { get; set; }

        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public ArrivalStatus ArrivalStatus { get; set; }

        [DataMember]
        public MealStatus MealStatus { get; set; }

        [DataMember]
        public bool IsGuestIntendingToPayByApp { get; set; }

        [DataMember]
        public BookingStatus Status { get; set; }

        [DataMember]
        public BookingReason[] BookingReasons { get; set; }
    }
}