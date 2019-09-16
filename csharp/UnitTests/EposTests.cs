using NUnit.Framework;
using RD.EposServiceConsumer.Helpers;
using RD.EposServiceConsumer.Helpers.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.UnitTests
{
    /// <summary>
    /// Summary description for EposTest
    /// </summary>
    [TestFixture]
    public class EposTests
    {
        private EposServiceClient _client;
        private int _targetRestaurantId;

        [SetUp]
        public void Setup()
        {
            string serviceBase = RemoveTrailingSlashFromUrl(ConfigurationManager.AppSettings["EPOS_SERVICE_BASE_URI"]);
            string consumerKey = ConfigurationManager.AppSettings["EPOS_CONSUMER_KEY"];
            string consumerSecret = ConfigurationManager.AppSettings["EPOS_CONSUMER_SECRET"];
            string secondSecret = ConfigurationManager.AppSettings["EPOS_API_SECOND_SECRET"];
            string oAuthEndpoint = RemoveTrailingSlashFromUrl(ConfigurationManager.AppSettings["EPOS_SERVICE_OAUTH_URI"]);
            _targetRestaurantId = Convert.ToInt32(ConfigurationManager.AppSettings["EPOS_API_RESTAURANT_ID"]);
            const string applicationName = "Unit Tests";
            const string applicationVersion = "1.0.0.0";
            
            _client = new EposServiceClient(serviceBase, consumerKey, consumerSecret, secondSecret, oAuthEndpoint, applicationName, applicationVersion);
          
        }

        public Booking CreateTestBooking()
        {
            DateTime targetDateTime = DateTime.SpecifyKind(DateTime.Today + new TimeSpan(12, 0, 0), DateTimeKind.Utc);

            Restaurant restaurant = _client.GetRestaurant(_targetRestaurantId);

            Segment currentSegment = Utilities.FindSegmentByDate(restaurant.Segments, targetDateTime);

            Area availableArea = currentSegment.Areas.FirstOrDefault();

            if (currentSegment == null)
                throw new Exception("Segment not found");

            Table table = currentSegment.Tables.FirstOrDefault();

            if (table == null)
                throw new Exception("Table cannot be null");
            Service service = currentSegment.Services.FirstOrDefault();
            var testBooking = new Booking
            {
                AreaId = availableArea.AreaId,
                ArrivalStatus = 0,
                BookingId = 0,
                BookingReference = null,
                ConfirmedByPhone = false,
                Customer = new Customer { FirstName = "Customer_Name", Surname = "Customer_Surname", AccessCode = "12345" },
                Comments = null,
                Covers = 2,
                Duration = 120,
                IsLeaveTimeConfirmed = false,
                MealStatus = MealStatus.Main,
                MenuId = 0,
                ServiceId = service.ServiceId,
                RestaurantId = _targetRestaurantId,
                VisitDateTime = targetDateTime,
                ChannelId =3,
                ChannelName = "Online",
                Tables = new[] { table.TableId }
            };

            return testBooking;
        }

        [Test]
        public void VerifyRestaurantDetails()
        {
         
            var restaurant = _client.GetRestaurant(_targetRestaurantId);
            
            Assert.AreEqual(ConfigurationManager.AppSettings["RESTAURANT_NAME"], restaurant.MicrositeName);
            Assert.AreEqual("GMT Standard Time", restaurant.TimeZone);
            Assert.AreEqual(1, restaurant.Segments.Count());
            Assert.IsNotEmpty(restaurant.Channels);
            Assert.IsNotEmpty(restaurant.Interests);
            Assert.IsNotEmpty(restaurant.CustomerTypes);
        }

        [Test]
        public void GetRestaurant()
        {
            Restaurant restaurant = _client.GetRestaurant(_targetRestaurantId);
            Assert.IsNotNull(restaurant);
        }

        [Test]
       
        public void GetDiaryData()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Today);
            Assert.IsNotNull(diaryBookings);
        }

        [Test]
        
        public void GetBooking()
        {

            //This test depends on there being bookings in the diary. This may not always be the case.
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date.AddDays(0));
            if (diaryBookings.Length < 1)
            {
                var testBooking = CreateTestBooking();
                _client.AddBooking(_targetRestaurantId, testBooking, true);
                diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            }
            Booking booking = _client.GetBooking(_targetRestaurantId, diaryBookings[0].BookingId);
            Assert.IsNotNull(booking);
        }

        [Test]
        public void AddBooking()
        {
            
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            if (diaryBookings.Length>0)
            {
            
                for (int i = 0; i < diaryBookings.Length; i++)
                {
                    var existingBooking = diaryBookings[i];
                  _client.CancelBooking(_targetRestaurantId,existingBooking.BookingId);
                }
            }
            var testBooking = CreateTestBooking();
            Booking actualBooking = _client.AddBooking(_targetRestaurantId, testBooking, true);

            Assert.IsNotNull(actualBooking);
            
        }

        [Test]
        public void UpdateBooking()
        {

            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            if (diaryBookings.Length==0)
            {
                var testBooking = CreateTestBooking();
                _client.AddBooking(_targetRestaurantId, testBooking, true);
                diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            }

            Booking booking = _client.GetBooking(_targetRestaurantId, diaryBookings[0].BookingId);

            Booking updatedBooking = booking;
            updatedBooking.Duration = 70;
            updatedBooking.MealStatus = MealStatus.Paid; //Paid
            updatedBooking.CustomerSpend = 123.99;
            updatedBooking.Covers = 10;

            _client.UpdateBooking(_targetRestaurantId, diaryBookings[0].BookingId, updatedBooking, true);
            var retrievedUpdatedBooking = _client.GetBooking(_targetRestaurantId, booking.BookingId);
            Assert.AreEqual(updatedBooking.BookingId, retrievedUpdatedBooking.BookingId);
            Assert.AreEqual(updatedBooking.Duration, retrievedUpdatedBooking.Duration);
            Assert.AreEqual(updatedBooking.MealStatus, retrievedUpdatedBooking.MealStatus);
            Assert.AreEqual(updatedBooking.CustomerSpend, retrievedUpdatedBooking.CustomerSpend);
            Assert.AreEqual(updatedBooking.Covers, retrievedUpdatedBooking.Covers);
        }

        [Test]
       public void CancelBooking()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            if (diaryBookings.Length<1)
            {
                var testBooking = CreateTestBooking();
                _client.AddBooking(_targetRestaurantId, testBooking);
                diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            }
            _client.CancelBooking(_targetRestaurantId, diaryBookings[0].BookingId);
            var cancelledBooking = _client.GetBooking(_targetRestaurantId, diaryBookings[0].BookingId);
            Assert.AreEqual(cancelledBooking.Status,BookingStatus.Cancelled);
        }

        [Test]
        public void UpdateArrivalStatus()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            if (diaryBookings.Length == 0)
            {
                var testBooking = CreateTestBooking();
                _client.AddBooking(_targetRestaurantId, testBooking, true);
                diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            }

            int bookingId = diaryBookings[0].BookingId;

            var booking = _client.UpdateArrivalStatus(_targetRestaurantId, bookingId, 1);
            Assert.AreEqual(booking.ArrivalStatus, ArrivalStatus.FullySeated);
        }

        [Test]
        public void GetCustomer()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            if (diaryBookings.Length == 0)
            {
                var testBooking = CreateTestBooking();
                _client.AddBooking(_targetRestaurantId, testBooking, true);
                diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            }

            Customer customer = _client.GetCustomer(diaryBookings[0].CustomerId);
            Assert.IsNotNull(customer);     
            
           
        }

     
        [Test]
        public void GetNextFreeTablesSuccessful()
        {
            NextFreeTable[] nextFreeTables = _client.GetNextFreeTables(_targetRestaurantId);

            Assert.IsNotEmpty(nextFreeTables);
        }
        
        private static string RemoveTrailingSlashFromUrl(string url)
        {
            return url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
        }
    }
}
