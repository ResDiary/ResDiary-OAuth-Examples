using System.Collections.Generic;
using NUnit.Framework;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.UnitTests
{
    [TestFixture]
    public class BumpBookingsTests : EposTestsBase
    {
        [Test]
        public void CreateBooking_ReseatsBumpedBookings()
        {
            var booking1 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, true);

            // Make sure booking is not marked as arrived
            booking1 = Client.UpdateArrivalStatus(booking1.RestaurantId, booking1.BookingId,
                (int) ArrivalStatus.NotArrived);

            var originalTables = booking1.Tables;

            var booking2 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, true);

            booking1 = Client.GetBooking(booking1.RestaurantId, booking1.BookingId);

            // Neither booking should be bumped
            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking1.Status);
            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking2.Status);

            // The second booking should now be on the original table, and the first booking
            // should have been moved to a new table.
            Assert.AreEqual(originalTables, booking2.Tables);
            Assert.AreNotEqual(originalTables, booking1.Tables);
        }

        [Test]
        public void UpdateBooking_ReseatsBumpedBookings()
        {
            var booking1 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, true);

            // Make sure booking is not marked as arrived
            booking1 = Client.UpdateArrivalStatus(booking1.RestaurantId, booking1.BookingId,
                (int)ArrivalStatus.NotArrived);

            var originalTables = booking1.Tables;

            var booking2Request = GetBookingDto(true);
            booking2Request.Tables = new[] {Settings.ApiSandboxWithBumpingTable6Id};
            var booking2 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, booking2Request, true);

            booking2.Tables = booking1.Tables;
            Client.UpdateBooking(booking2.RestaurantId, booking2.BookingId, booking2);

            booking1 = Client.GetBooking(booking1.RestaurantId, booking1.BookingId);

            // Neither booking should be bumped
            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking1.Status);
            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking2.Status);

            // The second booking should now be on the original table, and the first booking
            // should have been moved to a new table.
            Assert.AreEqual(originalTables, booking2.Tables);
            Assert.AreNotEqual(originalTables, booking1.Tables);
        }

        [Test]
        public void UpdateBookingTables_ReseatsBumpedBookings()
        {
            var booking1 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, true);

            // Make sure booking is not marked as arrived
            booking1 = Client.UpdateArrivalStatus(booking1.RestaurantId, booking1.BookingId,
                (int)ArrivalStatus.NotArrived);

            var originalTables = booking1.Tables;

            var booking2Request = GetBookingDto(true);
            booking2Request.Tables = new[] { Settings.ApiSandboxWithBumpingTable6Id };
            var booking2 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, booking2Request, true);

            booking2.Tables = booking1.Tables;
            Client.UpdateBookingTables(booking2.RestaurantId, booking2.BookingId, new List<int>(booking2.Tables));

            booking1 = Client.GetBooking(booking1.RestaurantId, booking1.BookingId);

            // Neither booking should be bumped
            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking1.Status);
            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking2.Status);

            // The second booking should now be on the original table, and the first booking
            // should have been moved to a new table.
            Assert.AreEqual(originalTables, booking2.Tables);
            Assert.AreNotEqual(originalTables, booking1.Tables);
        }
    }
}
