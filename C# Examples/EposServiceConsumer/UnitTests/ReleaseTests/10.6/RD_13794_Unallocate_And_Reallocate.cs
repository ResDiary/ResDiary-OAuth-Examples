using NUnit.Framework;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.UnitTests.ReleaseTests._10._6
{
    [TestFixture]
    [Category("ReleaseTests")]
    public class RD_13794_Unallocate_And_Reallocate : EposTestsBase
    {
        [Test]
        public void Test1_Unallocate_Booking()
        {
            // Create booking
            var booking = CreateNewBooking(Settings.ApiSandboxSecondSecret, false, false);

            // Unallocate booking
            Client.UnallocateBooking(booking.RestaurantId, booking.BookingId);

            // Grab the booking again
            booking = Client.GetBooking(booking.RestaurantId, booking.BookingId);

            Assert.AreEqual(BookingStatus.Bumped, (BookingStatus)booking.Status);
        }

        [Test]
        public void Test1_Reallocate_Booking()
        {
            // Create booking
            var booking = CreateNewBooking(Settings.ApiSandboxSecondSecret, false, false);

            // Unallocate booking
            Client.UnallocateBooking(booking.RestaurantId, booking.BookingId);
            Client.ReallocateBooking(booking.RestaurantId, booking.BookingId);

            // Grab the booking again
            booking = Client.GetBooking(booking.RestaurantId, booking.BookingId);

            Assert.AreEqual(BookingStatus.Booked, (BookingStatus)booking.Status);
        }
    }
}
