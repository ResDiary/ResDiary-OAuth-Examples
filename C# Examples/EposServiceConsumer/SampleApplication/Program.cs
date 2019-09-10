using System;
using System.Configuration;
using System.IO;
using System.Linq;
using RD.EposServiceConsumer.Helpers;
using RD.EposServiceConsumer.Helpers.BusinessObjects;
using log4net;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.SampleApplication
{
    class Program
    {
        private static EposServiceClient _client;
        private static ILog _log;
        private static int _restaurantId;

        static void Main(string[] args)
        {
            _log = LogManager.GetLogger(System.Reflection.Assembly.GetCallingAssembly(), "RD.EposServiceConsumer.SampleApplication");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

            _log.Info("Starting Up");

            string serviceBase = ConfigurationManager.AppSettings["ServiceBaseUri"];
            string consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            string consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            string secondSecret = ConfigurationManager.AppSettings["SecondSecret"];
            string oAuthEndpoint = ConfigurationManager.AppSettings["OAuthEndpoint"];
            _restaurantId = Convert.ToInt32(ConfigurationManager.AppSettings["DevelopmentRestaurantId"]);

            _log.Info("Creating API client " + serviceBase + " with key=" + consumerKey + " and secret=" + consumerSecret);
            _log.Info("Accessing restaurant " + _restaurantId + " with secondSecret=" + secondSecret);

            _client = new EposServiceClient(serviceBase, consumerKey, consumerSecret, secondSecret, oAuthEndpoint);

            var message = new TableMessage
                              {
                                  Table = "50",
                                  CloseDateTime = DateTime.Now,
                                  Spend = 123.45,
                                  ReceiptXml = "<receipt/>"
                              };

            try
            {
                UpdateTable(message);
            }
            catch (Exception e)
            {
                _log.Fatal("Unable to process message: " + message, e);
            }
        }

        static void UpdateTable(TableMessage message)
        {
            //Get the restaurant structure
            Restaurant restaurant = _client.GetRestaurant(_restaurantId);

            if (restaurant == null)
            {
                _log.Error("Unable to get restaurant from the API.");
                return;
            }

            //Get all of the bookings for the date that we're processing
            DiaryBooking[] diaryBookings = _client.GetDiaryData(_restaurantId, DateTime.Now.Date);

            if (diaryBookings.Count() == 0)
            {
                _log.Warn(string.Format("No bookings found for {0}.", DateTime.Now.Date));
                return;
            }

            Segment currentSegment = null;

            //find the current segment/table plan for the message date
            foreach (var segment in restaurant.Segments)
            {
                var timeScopes = from t in segment.TimeScopes
                                 where t.FromDate <= DateTime.Now.Date && t.ToDate > DateTime.Now.Date
                                 select t;

                if (timeScopes.Any())
                    currentSegment = segment;
            }

            if (currentSegment == null)
            {
                _log.Warn("Unable to find the current segment.");
                return;
            }

            //identify what the restaurantdiary TableId is for the EPOS table number.
            var tableId = (from t in currentSegment.Tables
                           where t.Number == message.Table
                           select t.TableId).First();

            if (0 == tableId)
            {
                _log.Warn(string.Format("Unable to match table \"{0}\" to the restaurant.", message.Table));
                return;
            }

            //find all of the bookings on that particular table that have not already been closed.
            var bookings = from b in diaryBookings
                           where b.Tables.Contains(tableId) && (int)b.MealStatus != 8
                           select b;

            //filter out the bookings that are arriving after the close message, doesn't really make sense to close a table before it is opened.
            bookings = from b in bookings
                       where b.VisitDateTime < message.CloseDateTime
                       select b;

            //order the bookings so we're looking at the most suitable booking.
            bookings = from b in bookings
                       orderby b.VisitDateTime
                       select b;

            var targetDiaryBooking = bookings.FirstOrDefault();

            if (targetDiaryBooking == null)
            {
                _log.Warn("No open bookings found for table \"" + message.Table + "\".");
                return;
            }

            var booking = _client.GetBooking(_restaurantId, targetDiaryBooking.BookingId);

            booking.ArrivalStatus = ArrivalStatus.FullySeated; //Arrived
            booking.MealStatus = MealStatus.Paid; //Paid / treated as left
            booking.CustomerSpend = message.Spend;
            booking.Duration = Convert.ToInt32((message.CloseDateTime - booking.VisitDateTime)?.TotalMinutes);
            booking.ReceiptXml = message.ReceiptXml;

            var updatedBooking = _client.UpdateBooking(_restaurantId, booking.BookingId, booking);

            _log.Info(string.Format("Message Processed for booking + {0}:{1}", updatedBooking.BookingId, message));
            return;
        }
    }
}
