using System.Linq;
using NUnit.Framework;

namespace RD.EposServiceConsumer.UnitTests.ReleaseTests._10._6
{
    [TestFixture]
    [Category("ReleaseTests")]
    public class RD_13793_Return_Unallocated_Bookings : EposTestsBase
    {
        [Test]
        public void Test1_UnallocateBookingForToday_UnallocatedBookingNotReturned()
        {
            // Create first booking
            var booking1 = CreateNewBooking(Settings.ApiSandboxSecondSecret, false, false);

            // Create second booking
            var booking2Dto = GetBookingDto(false);
            booking2Dto.Tables = new[] {Settings.ApiSandboxTable6Id};
            var booking2 = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking2Dto, false);

            // Unallocate first booking
            Client.UnallocateBooking(booking1.RestaurantId, booking1.BookingId);

            // Grab the diary data without unallocated bookings
            var bookings = Client.GetDiaryData(booking1.RestaurantId, booking1.VisitDateTime.Value.Date, false);

            Assert.AreEqual(1, bookings.Length);
            Assert.AreEqual(booking2.BookingId, bookings.Single().BookingId);
        }

        [Test]
        public void Test2_UnallocateBookingForToday_UnallocatedBookingReturned()
        {
            // Create first booking
            var booking1 = CreateNewBooking(Settings.ApiSandboxSecondSecret, false, false);

            // Create second booking
            var booking2Dto = GetBookingDto(false);
            booking2Dto.Tables = new[] { Settings.ApiSandboxTable6Id };
            var booking2 = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking2Dto, false);

            // Unallocate first booking
            Client.UnallocateBooking(booking1.RestaurantId, booking1.BookingId);

            // Get all diary bookings, including unallocated bookings
            var bookings = Client.GetDiaryData(booking1.RestaurantId, booking1.VisitDateTime.Value.Date, true);

            Assert.AreEqual(2, bookings.Length);
            Assert.AreEqual(booking1.BookingId, bookings.ElementAt(0).BookingId);
            Assert.AreEqual(booking2.BookingId, bookings.ElementAt(1).BookingId);
        }

        [Test]
        public void Test3_UnallocateBookingForTomorrow_UnallocatedBookingNotReturned()
        {
            // Create booking 1
            var booking1Dto = GetBookingDto(false);
            booking1Dto.VisitDateTime = booking1Dto.VisitDateTime.Value.AddDays(1);
            var booking1 = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking1Dto, false);

            // Create booking 2
            var booking2Dto = GetBookingDto(false);
            booking2Dto.VisitDateTime = booking2Dto.VisitDateTime.Value.AddDays(1);
            booking2Dto.Tables = new[] { Settings.ApiSandboxTable6Id };
            var booking2 = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking2Dto, false);

            // Unallocate booking 1
            Client.UnallocateBooking(booking1.RestaurantId, booking1.BookingId);

            // Get diary bookings without unallocated bookings
            var bookings = Client.GetDiaryData(booking1.RestaurantId, booking1.VisitDateTime.Value.Date, false);

            Assert.AreEqual(1, bookings.Length);
            Assert.AreEqual(booking2.BookingId, bookings.Single().BookingId);
        }

        [Test]
        public void Test4_UnallocateBookingForTomorrow_UnallocatedBookingReturned()
        {
            // Create booking 1
            var booking1Dto = GetBookingDto(false);
            booking1Dto.VisitDateTime = booking1Dto.VisitDateTime.Value.AddDays(1);
            var booking1 = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking1Dto, false);

            // Create booking 2
            var booking2Dto = GetBookingDto(false);
            booking2Dto.VisitDateTime = booking2Dto.VisitDateTime.Value.AddDays(1);
            booking2Dto.Tables = new[] { Settings.ApiSandboxTable6Id };
            var booking2 = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking2Dto, false);

            // Unallocate booking 1
            Client.UnallocateBooking(booking1.RestaurantId, booking1.BookingId);

            // Get all diary bookings, including unallocated bookings
            var bookings = Client.GetDiaryData(booking1.RestaurantId, booking1.VisitDateTime.Value.Date, true);

            Assert.AreEqual(2, bookings.Length);
            Assert.AreEqual(booking1.BookingId, bookings.ElementAt(0).BookingId);
            Assert.AreEqual(booking2.BookingId, bookings.ElementAt(1).BookingId);
        }
    }
}
