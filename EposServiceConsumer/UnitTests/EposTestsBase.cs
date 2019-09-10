using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using RD.EposServiceConsumer.Helpers;
using RD.EposServiceConsumer.Helpers.BusinessObjects;

namespace RD.EposServiceConsumer.UnitTests
{
    public class EposTestsBase
    {
        private static readonly Dictionary<string, EposServiceClient> EposClients = new Dictionary<string, EposServiceClient>();

        private Dictionary<string, List<CancelBookingInfo>> _bookingsToCancel;
        private string _testName;

        protected EposServiceClient Client { get; private set; }

        public IEnumerable<TestCaseData> TestCaseData
        {
            get
            {
                yield return new TestCaseData(Settings.ApiSandboxRestaurantId, Settings.ApiSandboxSecondSecret, false).SetName("TestWithoutBumpingEnabled");
                yield return new TestCaseData(Settings.ApiSandboxWithBumpingRestaurantId, Settings.ApiSandboxWithBumpingSecondSecret, true).SetName("TestWithBumpingEnabled");
            }
        }

        [SetUp]
        public void Setup()
        {
            if (Settings.IgnoreSslErrors)
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            }

            _bookingsToCancel = new Dictionary<string, List<CancelBookingInfo>>();

            _testName = NUnit.Framework.TestContext.CurrentContext.Test.FullName;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var pair in _bookingsToCancel)
            {
                CreateClient(pair.Key);

                foreach (var booking in pair.Value)
                {
                    Client.CancelBooking(booking.RestaurantId, booking.BookingId);
                }
            }
        }

        protected void CreateClient(string secondSecret)
        {
            EposServiceClient client;
            if (!EposClients.TryGetValue(secondSecret, out client))
            {
                string serviceBase = Settings.ServiceBaseUri;
                string consumerKey = Settings.ConsumerKey;
                string consumerSecret = Settings.ConsumerSecret;
                string oAuthEndpoint = Settings.OathUri;
                const string applicationName = "Unit Tests";
                const string applicationVersion = "1.0.0.0";

                client = new EposServiceClient(serviceBase, consumerKey, consumerSecret, secondSecret, oAuthEndpoint,
                    applicationName, applicationVersion, _testName);
            }

            Client = client;
        }

        protected Booking CreateNewBooking(string secondSecret, bool useBumpEnabledRestaurant, bool overrideCapacity = false)
        {
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);

            return CreateNewBooking(secondSecret, bookingDto, overrideCapacity);
        }

        protected Booking CreateNewBooking(string secondSecret, Booking bookingDto, bool overrideCapacity = false)
        {
            CreateClient(secondSecret);

            var newBooking = Client.AddBooking(bookingDto.RestaurantId, bookingDto, overrideCapacity);

            List<CancelBookingInfo> bookingsToCancel;
            if (!_bookingsToCancel.TryGetValue(secondSecret, out bookingsToCancel))
            {
                bookingsToCancel = new List<CancelBookingInfo>();
                _bookingsToCancel[secondSecret] = bookingsToCancel;
            }

            bookingsToCancel.Add(new CancelBookingInfo(newBooking.BookingId, newBooking.RestaurantId, secondSecret));

            return newBooking;
        }

        protected Booking GetBookingDto(bool useBumpEnabledRestaurant)
        {
            var newBooking = new Booking
            {
                VisitDateTime = DateTime.SpecifyKind(DateTime.Today + new TimeSpan(20, 0, 0), DateTimeKind.Utc),
                Covers = 2,
                Comments = "Epos API Test",
                Customer =
                    new Customer
                    {
                        FirstName = "Epos",
                        Surname = "ApiTest",
                        MobileNumber = "1234",
                        Email = "dev+apitest@resdiary.com",
                        Comments = "Comments"
                    },
                Duration = 80
            };
            if (useBumpEnabledRestaurant)
            {
                newBooking.Tables = new[] {Settings.ApiSandboxWithBumpingTable1Id};
                newBooking.RestaurantId = Settings.ApiSandboxWithBumpingRestaurantId;
                newBooking.AreaId = Settings.ApiSandboxWithBumpingRestaurantAreaId;
                newBooking.ChannelId = Settings.ApiSandboxWithBumpingApiTestChannelId;
            }
            else
            {
                newBooking.Tables = new[] { Settings.ApiSandboxTable1Id };
                newBooking.RestaurantId = Settings.ApiSandboxRestaurantId;
                newBooking.AreaId = Settings.ApiSandboxRestaurantAreaId;
                newBooking.ChannelId = Settings.ApiSandboxApiTestChannelId;
            }
            return newBooking;
        }

        private class CancelBookingInfo
        {
            public CancelBookingInfo(int bookingId, int restaurantId, string secondSecret)
            {
                BookingId = bookingId;
                RestaurantId = restaurantId;
                SecondSecret = secondSecret;
            }

            public int BookingId { get; }
            public int RestaurantId { get; }
            public string SecondSecret { get; }
        }
    }
}