namespace RD.EposServiceConsumer.Helpers.Enums
{
    /// <summary>
    /// Represents the status of a booking.
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// The booking is unconfirmed.
        /// </summary>
        Unconfirmed,

        /// <summary>
        /// The booking has been made.
        /// </summary>
        Booked,

        /// <summary>
        /// The booking is confirmed
        /// </summary>
        Confirmed,

        /// <summary>
        /// The booking is closed.
        /// </summary>
        Closed,

        /// <summary>
        /// The booking has been cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The booking has expired.
        /// </summary>
        Expired,

        /// <summary>
        /// The booking is pending.
        /// </summary>
        Pending,

        /// <summary>
        /// The booking is declined.
        /// </summary>
        Declined,

        /// <summary>
        /// The booking is locked.
        /// </summary>
        Locked,
        /// <summary>
        /// The booking is waitlisted.
        /// </summary>
        Wait,
        /// <summary>
        /// The booking is unallocated and it's currently not assigned to any table.
        /// </summary>
        Bumped
    }
}
