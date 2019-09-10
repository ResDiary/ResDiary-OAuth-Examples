using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json;
using NUnit.Framework;
using RD.EposServiceConsumer.Helpers;
using RD.EposServiceConsumer.Helpers.BusinessObjects;

namespace RD.EposServiceConsumer.UnitTests.ReleaseTests._10._3_EPOS
{
    [TestFixture]
    [Category("ReleaseTests")]
    public class RD_13647_Negative_Duration : EposTestsBase
    {
        private const string JsonContentType = "application/json; charset=utf-8";

        [Test]
        public void Test1_CreateBookingWithNegativeDuration()
        {
            var booking = GetBookingDto(false);
            CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            booking.Duration = -15;

            var exception = Assert.Throws<RestfulException>(() => Client.AddBooking(Settings.ApiSandboxRestaurantId, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Duration", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test2_UpdateBookingToHaveNegativeDuration()
        {
            var booking = GetBookingDto(false);
            CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            booking.Duration = -1;
            var exception = Assert.Throws<RestfulException>(() => Client.UpdateBooking(booking.RestaurantId, booking.BookingId, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Duration", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test3_UpdateBookingToHaveDurationOf0_DefaultsToStandardDuration()
        {
            var booking = GetBookingDto(false);
            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            createdBooking.Duration = 0;
            var updatedBooking = Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking);

            // Check that the duration is now greater than 0 (because it's been reset to the standard duration).
            Assert.Greater(updatedBooking.Duration, 0);
        }

        [Test]
        public void Test4_UpdateBookingToHaveANegativeFractionalDuration()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            var bookingString = JsonConvert.SerializeObject(createdBooking,
                new JsonSerializerSettings {DateFormatHandling = DateFormatHandling.MicrosoftDateFormat});
            bookingString = Regex.Replace(bookingString, "\"Duration\":.+?,", "\"Duration\":-0.25,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{createdBooking.RestaurantId}/Booking/{createdBooking.BookingId}?overrideCovers=false",
                HttpDeliveryMethods.PutRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test5_UpdateBookingToHaveDurationOfMinus24()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            createdBooking.Duration = -24 * 1440;

            var exception =
                Assert.Throws<RestfulException>(
                    () => Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Duration", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test6_UpdateBookingToHaveADurationWithASpaceInFront()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            var bookingString = JsonConvert.SerializeObject(createdBooking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"Duration\":.+?,", "\"Duration\":\" -1\",");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{createdBooking.RestaurantId}/Booking/{createdBooking.BookingId}?overrideCovers=false",
                HttpDeliveryMethods.PutRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Duration", exception.ValidationResults.Single().PropertyName);
        }
    }
}
