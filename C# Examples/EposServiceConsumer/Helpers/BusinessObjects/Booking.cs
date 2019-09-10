using System;
using System.Runtime.Serialization;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract(Namespace = "http://app.restaurantdiary.com/WebServices/Epos/v1")]
    public class Booking
    {
        [DataMember]
        public int BookingId { get; set; }

        [DataMember]
        public string BookingReference { get; set; }

        [DataMember]
        public BookingStatus Status { get; set; }

        [DataMember]
        public DateTime? VisitDateTime { get; set; }

        [DataMember]
        public DateTime? BookingDateTime { get; set; }

        [DataMember]
        public int Covers { get; set; }

        [DataMember]
        public int AreaId { get; set; }

        [DataMember]
        public int ServiceId { get; set; }

        [DataMember]
        public ArrivalStatus ArrivalStatus { get; set; }

        [DataMember]
        public MealStatus MealStatus { get; set; }

        [DataMember]
        public string ChannelName { get; set; }

        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public string MenuName { get; set; }

        [DataMember]
        public int MenuId { get; set; }

        [DataMember]
        public BookingType Type { get; set; }

        [DataMember]
        public int ChannelId { get; set; }

        [DataMember]
        public Customer Customer { get; set; }

        [DataMember]
        public Promotion[] Promotions { get; set; }

        [DataMember]
        public Payment[] Payments { get; set; }

        [DataMember]
        public int[] Tables { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public int TurnTime { get; set; }

        [DataMember]
        public int RestaurantId { get; set; }

        [DataMember]
        public bool ConfirmedByPhone { get; set; }

        [DataMember]
        public bool IsLeaveTimeConfirmed { get; set; }

        [DataMember]
        public string ReceiptXml { get; set; }

        [DataMember]
        public double CustomerSpend { get; set; }

        [DataMember]
        public bool IsGuestIntendingToPayByApp { get; set; }

        [DataMember]
        public BookingMoveResultCode MoveStatus { get; set; }

        [DataMember]
        public string MoveMessage { get; set; }

        [DataMember]
        public BookingMoveWarningType MoveWarningType { get; set; }

        [DataMember]
        public BookingReason[] BookingReasons { get; set; }
    }
}