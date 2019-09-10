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
    public class RD_13580_Epos_Validation : EposTestsBase
    {
        private const string JsonContentType = "application/json; charset=utf-8";

        [Test]
        public void Test1_CreateBooking_SetCoversToFraction_FailsValidation()
        {
            CreateClient(Settings.ApiSandboxSecondSecret);
            var booking = GetBookingDto(false);

            var bookingString = JsonConvert.SerializeObject(booking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"Covers\":.+?,", "\"Covers\":0.5,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{booking.RestaurantId}/Booking?overrideCovers=false",
                HttpDeliveryMethods.PostRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test1_UpdateBooking_SetCoversToFraction_FailsValidation()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            var bookingString = JsonConvert.SerializeObject(createdBooking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"Covers\":.+?,", "\"Covers\":0.5,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{createdBooking.RestaurantId}/Booking/{createdBooking.BookingId}?overrideCovers=false",
                HttpDeliveryMethods.PutRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test2_CreateBooking_SetCoversToMinus1_FailsValidation()
        {
            var booking = GetBookingDto(false);
            booking.Covers = -1;

            var exception = Assert.Throws<RestfulException>(() => CreateNewBooking(Settings.ApiSandboxSecondSecret, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Covers", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test2_UpdateBooking_SetCoversToMinus1_FailsValidation()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);
            createdBooking.Covers = -1;

            var exception =
                Assert.Throws<RestfulException>(
                    () =>
                        Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking,
                            true));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test3_CreateBooking_SetCoversTo0_FailsValidation()
        {
            var booking = GetBookingDto(false);
            booking.Covers = 0;

            var exception = Assert.Throws<RestfulException>(() => CreateNewBooking(Settings.ApiSandboxSecondSecret, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Covers", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test3_UpdateBooking_SetCoversToZero_FailsValidation()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);
            createdBooking.Covers = 0;

            var exception =
                Assert.Throws<RestfulException>(
                    () =>
                        Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking,
                            true));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test4_CreateBooking_SetCoversTo1_Succeeds()
        {
            var booking = GetBookingDto(false);
            booking.Covers = 1;

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            Assert.AreEqual(1, createdBooking.Covers);
        }

        [Test]
        public void Test4_UpdateBooking_SetCoversTo1_Succeeds()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);
            createdBooking.Covers = 1;

            var updatedBooking = Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking, true);

            Assert.AreEqual(1, updatedBooking.Covers);
        }

        [Test]
        public void Test5_CreateBooking_SetCoversToFraction_FailsValidation()
        {
            CreateClient(Settings.ApiSandboxSecondSecret);
            var booking = GetBookingDto(false);

            var bookingString = JsonConvert.SerializeObject(booking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"Covers\":.+?,", "\"Covers\":1.5,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{booking.RestaurantId}/Booking?overrideCovers=false",
                HttpDeliveryMethods.PostRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test5_UpdateBooking_SetCoversToFraction_FailsValidation()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            var bookingString = JsonConvert.SerializeObject(createdBooking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"Covers\":.+?,", "\"Covers\":1.5,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{createdBooking.RestaurantId}/Booking/{createdBooking.BookingId}?overrideCovers=false",
                HttpDeliveryMethods.PutRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test6_CreateBooking_UseTableThatDoesNotExist_FailsValidation()
        {
            var booking = GetBookingDto(false);
            booking.Tables = new[] {12345};

            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(Settings.ApiSandboxSecondSecret, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Tables", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test6_UpdateBooking_UseTableThatDoesNotExist_FailsValidation()
        {
            var booking = GetBookingDto(false);
            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);
            createdBooking.Tables = new[] {12345};

            var exception =
                Assert.Throws<RestfulException>(
                    () =>
                        Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking,
                            true));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Tables", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test7_CreateBooking_UseTableThatExistsInAnotherRestaurant_FailsValidation()
        {
            var booking = GetBookingDto(false);
            booking.Tables = new[] { 437094 };

            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(Settings.ApiSandboxSecondSecret, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Tables", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test7_UpdateBooking_UseTableExistsInAnotherRestaurant_FailsValidation()
        {
            var booking = GetBookingDto(false);
            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);
            createdBooking.Tables = new[] { 437094 };

            var exception =
                Assert.Throws<RestfulException>(
                    () =>
                        Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking,
                            true));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Tables", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test8_CreateBooking_UseTableThatExistsInAnotherRestaurantInSameGroup_FailsValidation()
        {
            var booking = GetBookingDto(false);
            booking.Tables = new[] { 454128 };

            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(Settings.ApiSandboxSecondSecret, booking));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Tables", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test8_UpdateBooking_UseTableExistsInAnotherRestaurantInSameGroup_FailsValidation()
        {
            var booking = GetBookingDto(false);
            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);
            createdBooking.Tables = new[] { 454128 };

            var exception =
                Assert.Throws<RestfulException>(
                    () =>
                        Client.UpdateBooking(createdBooking.RestaurantId, createdBooking.BookingId, createdBooking,
                            true));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("Tables", exception.ValidationResults.Single().PropertyName);
        }

        [Test]
        public void Test12_CreateBooking_VisitDateTimeNotSpecified_FailsValidation()
        {
            CreateClient(Settings.ApiSandboxSecondSecret);
            var booking = GetBookingDto(false);

            var bookingString = JsonConvert.SerializeObject(booking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"VisitDateTime\":.+?,", "\"VisitDateTime\":null,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{booking.RestaurantId}/Booking?overrideCovers=false",
                HttpDeliveryMethods.PostRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test]
        public void Test12_UpdateBooking_VisitDateTimeNotSpecified_FailsValidation()
        {
            var booking = GetBookingDto(false);

            var createdBooking = CreateNewBooking(Settings.ApiSandboxSecondSecret, booking);

            var bookingString = JsonConvert.SerializeObject(createdBooking,
                new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            bookingString = Regex.Replace(bookingString, "\"VisitDateTime\":.+?,", "\"VisitDateTime\":null,");

            var exception = Assert.Throws<RestfulException>(() => Client.InvokeForContentType<Booking>(
                $"/WebServices/Epos/V1/Restaurant/{createdBooking.RestaurantId}/Booking/{createdBooking.BookingId}?overrideCovers=false",
                HttpDeliveryMethods.PutRequest, bookingString, JsonContentType));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
            Assert.AreEqual("VisitDateTime", exception.ValidationResults.Single().PropertyName);
        }
    }
}
