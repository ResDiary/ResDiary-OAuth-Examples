using System;
using NUnit.Framework;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.UnitTests
{
    [TestFixture]
    public class CloseBookingTests : EposTestsBase
    {
        [Test]
        [TestCaseSource(nameof(TestCaseData))]
        public void UpdateBooking_DoesNotAlterStatus_AfterBookingClosed(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            var booking = CreateNewBooking(secondSecret, GetBookingDto(useBumpEnabledRestaurant));

            booking = Client.UpdateMealStatus(targetRestaurantId, booking.BookingId, (int) MealStatus.Closed);

            booking.CustomerSpend = 100;
            booking = Client.UpdateBooking(booking.RestaurantId, booking.BookingId, booking, true);

            var updatedBooking = Client.GetBooking(booking.RestaurantId, booking.BookingId);

            Assert.AreEqual(MealStatus.Closed, (MealStatus)updatedBooking.MealStatus);
            Assert.AreEqual(BookingStatus.Closed, (BookingStatus)updatedBooking.Status);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseData))]
        public void CloseBooking_ReducesDuration_IfLeaveTimeIsAfterCurrentTime(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // This is the situation where the leave time of the booking we're closing is after the current time,
            // so we reduce the duration of the booking to the current time

            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var currentTime = DateTime.Now;
            bookingDto.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);
            bookingDto.Duration = ((int)(DateTime.Now - bookingDto.VisitDateTime.Value).TotalMinutes) + 30;

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);

            // Close the booking
            booking = Client.UpdateMealStatus(booking.RestaurantId, booking.BookingId, (int)MealStatus.Closed);

            // Because the booking ended before the current time, its duration should not have been altered
            Assert.Less(booking.Duration, bookingDto.Duration);
        }

        #region closing bookings doesn't cause clashes with other bookings
        [Test]
        [TestCaseSource(nameof(TestCaseData))]
        public void CloseBooking_DoesNotClashWithUpComingBookingIfLeaveTimeInThePast(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // This is the situation where the current time is after the leave time of the booking we're closing,
            // and we don't want to extend the booking to the current time in case it causes it to overlap
            // with another booking. Note that the restaurant without bumping enabled has a turn time of 15 minutes.

            var currentTime = DateTime.Now;

            var bookingToClose = GetBookingDto(useBumpEnabledRestaurant);
            var secondBooking = GetBookingDto(useBumpEnabledRestaurant);

            bookingToClose.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0).AddDays(-1);
            bookingToClose.Duration = 45; //in the restaurant with a 15 minute turn time, this will basically mean its full duration is 60

            secondBooking.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 0, 0).AddDays(-1);
            secondBooking.Duration = 45;

            // Create the bookings
            var booking1 = CreateNewBooking(secondSecret, bookingToClose);
            CreateNewBooking(secondSecret, secondBooking);

            // Close the booking
            booking1 = Client.UpdateMealStatus(booking1.RestaurantId, booking1.BookingId, (int)MealStatus.Closed);
            
            // Because the booking ended before the current time, its duration should not have been altered
            Assert.AreEqual(booking1.Duration, 45);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseData))]
        public void CloseBooking_DoesNotClashWithUpComingBookingIfLeaveTimeInFuture(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // This is the situation where the current time is after the leave time of the booking we're closing,
            // and we don't want to extend the booking to the current time in case it causes it to overlap
            // with another booking. Note that the restaurant without bumping enabled has a turn time of 15 minutes.

            var currentTime = DateTime.Now;

            var bookingToClose = GetBookingDto(useBumpEnabledRestaurant);
            var secondBooking = GetBookingDto(useBumpEnabledRestaurant);

            bookingToClose.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0).AddDays(1);
            bookingToClose.Duration = 45; //in the restaurant with a 15 minute turn time, this will basically mean its full duration is 60

            secondBooking.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 0, 0).AddDays(1);
            secondBooking.Duration = 45;

            // Create the bookings
            var booking1 = CreateNewBooking(secondSecret, bookingToClose);
            CreateNewBooking(secondSecret, secondBooking);

            // Close the booking
            booking1 = Client.UpdateMealStatus(booking1.RestaurantId, booking1.BookingId, (int)MealStatus.Closed);

            // Because the booking ended before the current time, its duration should not have been altered
            Assert.AreEqual(booking1.Duration, 45);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseData))]
        public void CloseBooking_DoesNotClashWithUpComingBookingIfLeaveTimeIsPastMidnight(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // This is the situation where the current time is after the leave time of the booking we're closing,
            // and we don't want to extend the booking to the current time in case it causes it to overlap
            // with another booking. Note that the restaurant without bumping enabled has a turn time of 15 minutes.

            var currentTime = DateTime.Now;

            var bookingToClose = GetBookingDto(useBumpEnabledRestaurant);
            var secondBooking = GetBookingDto(useBumpEnabledRestaurant);

            bookingToClose.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 30, 0).AddDays(-1);
            bookingToClose.Duration = 45; //in the restaurant with a 15 minute turn time, this will basically mean its full duration is 60

            secondBooking.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 30, 0).AddDays(-1);
            secondBooking.Duration = 45;

            // Create the bookings
            var booking1 = CreateNewBooking(secondSecret, bookingToClose);
            CreateNewBooking(secondSecret, secondBooking);

            // Close the booking
            booking1 = Client.UpdateMealStatus(booking1.RestaurantId, booking1.BookingId, (int)MealStatus.Closed);

            // Because the booking ended before the current time, its duration should not have been altered
            Assert.AreEqual(booking1.Duration, 45);
        }
        #endregion
    }
}
