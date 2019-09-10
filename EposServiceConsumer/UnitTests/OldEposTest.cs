using NUnit.Framework;
using RD.EposServiceConsumer.Helpers;
using RD.EposServiceConsumer.Helpers.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace RD.EposServiceConsumer.UnitTests
{
    /// <summary>
    /// Summary description for EposTest
    /// </summary>
    [TestFixture]
    public class OldEposTest
    {
        private EposServiceClient _client;
        private int _targetRestaurantId;

        [SetUp]
        public void Setup()
        {
            string serviceBase = RemoveTrailingSlashFromUrl(ConfigurationManager.AppSettings["ServiceBaseUri"]);
            string consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            string consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            string secondSecret = ConfigurationManager.AppSettings["SecondSecret"];
            string oAuthEndpoint = RemoveTrailingSlashFromUrl(ConfigurationManager.AppSettings["OAuthEndpoint"]);
            _targetRestaurantId = Convert.ToInt32(ConfigurationManager.AppSettings["DevelopmentRestaurantId"]);
            const string applicationName = "Unit Tests";
            const string applicationVersion = "1.0.0.0";
            
            _client = new EposServiceClient(serviceBase, consumerKey, consumerSecret, secondSecret, oAuthEndpoint, applicationName, applicationVersion);
        }

        [Test]
        public void GetRestaurant()
        {
            Restaurant restaurant = _client.GetRestaurant(_targetRestaurantId);
            Assert.IsNotNull(restaurant);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void GetDiaryData()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Today);
            Assert.IsNotNull(diaryBookings);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void GetBooking()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date.AddDays(0));
            Booking booking = _client.GetBooking(_targetRestaurantId, diaryBookings[0].BookingId);
            Assert.IsNotNull(booking);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void AddBooking()
        {
            DateTime targetDateTime = DateTime.Now.Date.AddHours(12).AddMinutes(30);
            
            //Restaurant restaurant = _client.GetRestaurant(_targetRestaurantId);

            //Segment currentSegment = Utilities.FindSegmentByDate(restaurant.Segments,targetDateTime);

            //if(currentSegment == null)
            //    throw  new Exception("Segment not found");

            //Table table = currentSegment.Tables.FirstOrDefault();

            //if (table == null)
            // throw  new Exception("Table cannot be null");

            var testBooking = new Booking
                                  {
                                      AreaId = 3780,
                                      ArrivalStatus = 0,
                                      BookingId = 0,
                                      BookingReference = null,
                                      ConfirmedByPhone = false,
                                      Customer = new Customer { FirstName = "Tam", Surname = "Bam", AccessCode = "12345"},
                                      Comments = null,
                                      Covers = 2,
                                      Duration = 120,
                                      IsLeaveTimeConfirmed = false,
                                      MealStatus = 4,
                                      MenuId = 0,
                                      ServiceId = 5534,
                                      RestaurantId = _targetRestaurantId,
                                      VisitDateTime = targetDateTime,
                                      ChannelId = 2,
                                      ChannelName = "Online",
                                      Tables = new[] { 60330 }
            };

            Booking actualBooking = _client.AddBooking(_targetRestaurantId, testBooking, true);

            Assert.IsNotNull(actualBooking);
        }

        [Test]
        [Ignore("This test has no assert!")]
        public void UpdateBooking()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);

            Booking booking = _client.GetBooking(_targetRestaurantId, diaryBookings[0].BookingId);

            booking.Duration = 70;
            booking.ArrivalStatus = 1; //Arrived
            booking.MealStatus = 8; //Paid
            booking.CustomerSpend = 123.99;
            booking.Covers = 10;

            var updatedBooking = _client.UpdateBooking(_targetRestaurantId, diaryBookings[0].BookingId, booking, true);
        }

        [Test]
        [Ignore("This test has no assert!")]
        public void CancelBooking()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date.AddDays(2));

           _client.CancelBooking(_targetRestaurantId, diaryBookings[0].BookingId);
        }

        [Test]
        [Ignore("This test has no assert!")]
        public void AddReciptToBooking()
        {
            const string xmlReceipt = "<receipt/>";

            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);

            int bookingId = diaryBookings[0].BookingId;         

            _client.AddBookingReceipt(_targetRestaurantId, bookingId, xmlReceipt);
        }

        [Test]
        [Ignore("This test has no assert!")]
        public void UpdateMealStatus()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);

            int bookingId = diaryBookings[0].BookingId;

            _client.UpdateMealStatus(_targetRestaurantId, bookingId, 8);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void UpdateMealStatus_TestUpdateMealStatusIsSameAsSent()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            int bookingId = diaryBookings[0].BookingId;
            Booking booking = _client.UpdateMealStatus(_targetRestaurantId, bookingId, 4);
            Assert.AreEqual(4, booking.MealStatus);
        }

        [Test]
        [Ignore("This test has no assert!")]
        public void UpdateArrivalStatus()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);

            int bookingId = diaryBookings[0].BookingId;

            var booking = _client.UpdateArrivalStatus(_targetRestaurantId, bookingId, 1);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void GetCustomer()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            if (diaryBookings.Any())
            {
                Customer customer = _client.GetCustomer(diaryBookings[0].CustomerId);
                Assert.IsNotNull(customer);     
            }
           
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void GetCustomersBookingHistoryTest()
        {
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);

            BookingHistory[] history = _client.GetBookingHistory(diaryBookings[0].CustomerId);

            Assert.IsNotNull(history);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void GetNextFreeTablesSuccessful()
        {
            NextFreeTable[] nextFreeTables = _client.GetNextFreeTables(_targetRestaurantId);

            Assert.IsNotEmpty(nextFreeTables);
        }

        [Test]
        [Ignore("This test depends on there being table joins in the diary. This may not always be the case.")]
        public void GetTableJoinsSuccessful()
        {
            var joins = _client.GetTableJoins(_targetRestaurantId, 5, DateTime.UtcNow);
            Assert.IsNotEmpty(joins);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void SwapBookings()
        {
            var diaryBookingsBefore = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date.AddDays(2));
            var booking1Tables = diaryBookingsBefore[0].Tables;
            var booking2Tables = diaryBookingsBefore[1].Tables;
            _client.SwapBookings(_targetRestaurantId, diaryBookingsBefore[0].BookingId, diaryBookingsBefore[1].BookingId);
            var diaryBookingsAfter = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);
            Assert.AreEqual(booking2Tables, diaryBookingsAfter[0].Tables);
            Assert.AreEqual(booking1Tables, diaryBookingsAfter[1].Tables);
        }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case.")]
        public void TableSwap()
        {
            //

            DiaryBooking[] diaryBookings = _client.GetDiaryData(_targetRestaurantId, DateTime.Now.Date);

            int bookingId = diaryBookings[0].BookingId;


            //const int bookingId = 18672103;

            var newTableIds = new List<int> { 60330 };
            Booking booking;
            try
            {
                _client.UpdateBookingTables(_targetRestaurantId, bookingId, newTableIds);
                booking = _client.GetBooking(_targetRestaurantId, bookingId);
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
            Assert.AreEqual(booking.Tables, newTableIds);
       }

        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case")]
        public void CreateBooking_NoCustomer_ReturnsNoCustomer()
        {
            DateTime targetDateTime = DateTime.Now.Date.AddHours(12).AddMinutes(30);

            Restaurant restaurant = _client.GetRestaurant(_targetRestaurantId);

            Segment currentSegment = Utilities.FindSegmentByDate(restaurant.Segments, targetDateTime);

            
            if (currentSegment == null)
                throw new Exception("Segment not found");

            Table table = currentSegment.Tables.FirstOrDefault();

            if (table == null)
                throw new Exception("Table cannot be null");
            
            var testBooking = new Booking
            {
                AreaId = currentSegment.Areas.FirstOrDefault().AreaId,
                ArrivalStatus = 0,
                BookingId = 0,
                BookingReference = null,
                ConfirmedByPhone = false,
                Customer = null,
                Comments = "hi",
                Covers = 2,
                Duration = 120,
                IsLeaveTimeConfirmed = false,
                MealStatus = 4,
                MenuId = 0,
                ServiceId = currentSegment.Services.FirstOrDefault().ServiceId,
                RestaurantId = _targetRestaurantId,
                VisitDateTime = targetDateTime,
                ChannelId = 2,
                ChannelName = "Online",
                Tables = new[] { table.TableId }
            };

            Booking actualBooking = _client.AddBooking(_targetRestaurantId, testBooking, true);

            Assert.IsNull(actualBooking.Customer);
        }


        [Test]
        [Ignore("This test depends on there being bookings in the diary. This may not always be the case")]
        public void CreateBooking_Customer_ReturnsCustomer()
        {
            DateTime targetDateTime = DateTime.Now.Date.AddDays(2);

            Restaurant restaurant = _client.GetRestaurant(_targetRestaurantId);

            Segment currentSegment = Utilities.FindSegmentByDate(restaurant.Segments, targetDateTime);


            if (currentSegment == null)
                throw new Exception("Segment not found");

            Table table = currentSegment.Tables.FirstOrDefault();

            if (table == null)
                throw new Exception("Table cannot be null");

            var customer = _client.GetCustomer(13424395);

            var testBooking = new Booking
            {
                AreaId = currentSegment.Areas.FirstOrDefault().AreaId,
                ArrivalStatus = 0,
                BookingId = 0,
                BookingReference = null,
                ConfirmedByPhone = false,
                Customer = customer,
                Comments = "hi",
                Covers = 2,
                Duration = 120,
                IsLeaveTimeConfirmed = false,
                MealStatus = 4,
                MenuId = 0,
                ServiceId = currentSegment.Services.FirstOrDefault().ServiceId,
                RestaurantId = _targetRestaurantId,
                VisitDateTime = targetDateTime,
                ChannelId = 2,
                ChannelName = "Online",
                Tables = new[] { table.TableId }
            };

            Booking actualBooking = _client.AddBooking(_targetRestaurantId, testBooking, true);

            Assert.AreEqual(customer, actualBooking.Customer.CustomerId);
        }

        [Test]
        [Ignore]
        public void GetBookingModifications()
        {
            var bookingModifications = _client.GetBookingChanges(4075, DateTime.Today);
            Assert.Greater(bookingModifications.Count(),0);
        }

        [Test]
        [Ignore]
        public void GetBookingModificationForServiceSince()
        {
            var bookingModifications = _client.GetBookingChangesForServiceSince(_targetRestaurantId, 5534,
                DateTime.UtcNow.Date, new TimeSpan(12, 30, 00));
            Assert.Greater(bookingModifications.Count(), 0);
        }

        private static string RemoveTrailingSlashFromUrl(string url)
        {
            return url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
        }
    }
}
