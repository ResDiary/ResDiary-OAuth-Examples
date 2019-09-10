using System;
using System.Collections.Generic;
using RD.EposServiceConsumer.Helpers.BusinessObjects;

namespace RD.EposServiceConsumer.Helpers
{
    interface IEposServiceClient
    {
        Restaurant GetRestaurant(int restaurantId);
        DiaryBooking[] GetDiaryData(int restaurantId, DateTime date, bool includeUnallocatedBookings);
        Booking GetBooking(int restaurantId, int bookingId);
        Booking AddBooking(int restaurantId, object booking, bool overrideCovers);
        Booking UpdateBooking(int restaurantId, int bookingId, Booking booking, bool overrideCovers);
        void CancelBooking(int restaurantId, int bookingId);
        Booking AddBookingReceiptJson(int restaurantId, int bookingId, string xmlReceipt);
        Booking AddBookingReceiptXml(int restaurantId, int bookingId, string xmlReceipt);
        Booking AddBookingReceiptNoContentType(int restaurantId, int bookingId, string xmlReceipt);
        Booking UpdateArrivalStatus(int restaurantId, int bookingId, int arrivalStatus);
        Booking UpdateMealStatus(int restaurantId, int bookingId, int mealStatus);
        Booking UpdateBookingTables(int restaurantId, int bookingId, List<int> tableIds);
        Customer GetCustomer(int customerId);
        BookingHistory[] GetBookingHistory(int customerId);
        NextFreeTable[] GetNextFreeTables(int restaurantId);
        IEnumerable<IEnumerable<int>> GetTableJoins(int restaurantId, int coversCount, DateTime diaryDate);
        void SwapBookings(int restaurantId, int bookingId1, int bookingId2);
        IEnumerable<BookingModification> GetBookingChanges(int restaurantId, DateTime changeDateTime);
        Booking UnallocateBooking(int restaurantId, int bookingId);
        ReallocatedBooking ReallocateBooking(int restaurantId, int bookingId);
    }
}
