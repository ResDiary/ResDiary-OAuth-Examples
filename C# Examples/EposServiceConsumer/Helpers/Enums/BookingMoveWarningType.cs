using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RD.EposServiceConsumer.Helpers.Enums
{
    public enum BookingMoveWarningType
    {
        /// <summary>
        /// The move was successful.
        /// </summary>
        Ok,

        /// <summary>
        /// The booking you're trying to move is locked.
        /// </summary>
        CannotMoveLockedBookings,

        /// <summary>
        /// The table you're trying to move the booking onto contains a locked booking.
        /// </summary>
        CannotSwapLockedBookings,

        /// <summary>
        /// The table you're trying to move the booking to isn't available.
        /// </summary>
        UnsuitableDestinationTable,

        /// <summary>
        /// Moving the booking causes rules to be broken.
        /// </summary>
        MovingBookingBreaksRules,

        /// <summary>
        /// Moving the booking will unallocate other bookings, but the user doesn't have permission.
        /// </summary>
        UserDoesNotHavePermissionToUnallocate,

        /// <summary>
        /// One or more bookings will be unallocated by moving this booking.
        /// </summary>
        BookingsWillBeUnallocated,

        /// <summary>
        /// The booking has been moved successfully, but one or more affected bookings
        /// has been left unallocated by the operation.
        /// </summary>
        OneOrMoreBookingsAreUnallocated,

        /// <summary>
        /// You tried to move a booking with a party size of 0.
        /// </summary>
        MoveBookingZeroCovers,

        /// <summary>
        /// We couldn't find your user information.
        /// </summary>
        CannotFindUserInformation,

        /// <summary>
        /// 
        /// </summary>
        NoBookingsToSwap,

        /// <summary>
        /// The booking you're trying to swap with has a party size of 0.
        /// </summary>
        CannotSwapBookingsWithZeroCovers,

        /// <summary>
        /// You are trying to move your booking onto a table that can seat fewer covers than the booking party size.
        /// </summary>
        TableCapacityExceeded
    }
}
