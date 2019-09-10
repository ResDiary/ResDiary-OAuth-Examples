using NUnit.Framework;
using RD.EposServiceConsumer.Helpers;
using RD.EposServiceConsumer.Helpers.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using RD.EposServiceConsumer.Helpers.Enums;

namespace RD.EposServiceConsumer.UnitTests
{
    /// <summary>
    /// Summary description for EposTest
    /// </summary>
    [TestFixture]
    public class EposTest : EposTestsBase
    {
        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetRestaurant_ReturnsRestaurantInfo(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestauarant)
        {
            // Arrange
            CreateClient(secondSecret);

            // Act
            var restaurant = Client.GetRestaurant(targetRestaurantId);

            // Assert
            if (useBumpEnabledRestauarant)
            {
                Assert.AreEqual(Settings.RestaurantName, restaurant.Name);
                Assert.AreEqual("GMT Standard Time", restaurant.TimeZone);
                Assert.AreEqual(1, restaurant.Segments.Count());
                Assert.AreEqual(Settings.ApiSandboxWithBumpingSegmentName, restaurant.Segments.FirstOrDefault().Name);
                Assert.AreEqual(Settings.ApiSandboxWithBumpingSegmentId, restaurant.Segments.FirstOrDefault().SegmentId);
                Assert.IsNotEmpty(restaurant.Channels);
                Assert.IsNotEmpty(restaurant.Interests);
                Assert.IsNotEmpty(restaurant.CustomerTypes);
            }
            else
            {
           
                Assert.AreEqual(Settings.RestaurantName, restaurant.Name);
                Assert.AreEqual("GMT Standard Time", restaurant.TimeZone);
                Assert.AreEqual(1, restaurant.Segments.Count());
                Assert.AreEqual(Settings.ApiSandboxSegmentName, restaurant.Segments.FirstOrDefault().Name);
                Assert.AreEqual(Settings.ApiSandboxSegmentId, restaurant.Segments.FirstOrDefault().SegmentId);
                Assert.IsNotEmpty(restaurant.Channels);
                Assert.IsNotEmpty(restaurant.Interests);
                Assert.IsNotEmpty(restaurant.CustomerTypes);
            }
        }

        #region GetDiaryData

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetDiaryData_ReturnsDiaryData(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestauarant)
        {
            // Arrange
            CreateNewBooking(secondSecret, useBumpEnabledRestauarant);

            // Act
            var diaryBookings = Client.GetDiaryData(targetRestaurantId, DateTime.Today);

            // Assert
            Assert.IsNotEmpty(diaryBookings);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetDiaryData_ReturnsCorrectBookingData(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestauarant)
        {
            // Arrange
            CreateClient(secondSecret);
            var bookingDto = GetBookingDto(useBumpEnabledRestauarant);
            var booking = CreateNewBooking(secondSecret, bookingDto);

            // Act
            var diaryBookings = Client.GetDiaryData(bookingDto.RestaurantId, DateTime.Today);
            var foundBooking = diaryBookings.FirstOrDefault(d => d.BookingId == booking.BookingId);

            // Assert
            Assert.IsNotNull(foundBooking);
            Assert.AreEqual(bookingDto.Tables, foundBooking.Tables);
            Assert.AreEqual(bookingDto.Covers, foundBooking.Covers);
            Assert.AreEqual(bookingDto.Customer.FirstName + " " + bookingDto.Customer.Surname,
                foundBooking.CustomerFullName);
            Assert.AreEqual(bookingDto.Comments, foundBooking.Comments);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetDiaryData_DoesNotReturnCancelledBookings(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            Client.CancelBooking(targetRestaurantId, booking.BookingId);

            // Act
            var diaryBookings = Client.GetDiaryData(targetRestaurantId, DateTime.Today);
            var foundBooking = diaryBookings.FirstOrDefault(d => d.BookingId == booking.BookingId);

            // Assert
            Assert.IsNull(foundBooking);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetDiaryData_ReturnsBumpedBookings(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            Client.UnallocateBooking(targetRestaurantId, booking.BookingId);

            // Act
            var result = Client.GetDiaryData(targetRestaurantId, DateTime.Now, true);
            var foundBooking = result.FirstOrDefault(d => d.BookingId == booking.BookingId);

            // Assert
            Assert.IsNotNull(foundBooking);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetDiaryData_DoesntReturnBumpedBookings(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            Client.UnallocateBooking(targetRestaurantId, booking.BookingId);

            // Act
            var result = Client.GetDiaryData(targetRestaurantId, DateTime.Now);
            var foundBooking = result.FirstOrDefault(d => d.BookingId == booking.BookingId);

            // Assert
            Assert.IsNull(foundBooking);
        }

        #endregion

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBooking_ReturnsBookingData(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            // Act
            var result = Client.GetBooking(targetRestaurantId, booking.BookingId);

            // Assert
            Assert.AreEqual(booking.VisitDateTime, result.VisitDateTime);
            Assert.AreEqual(booking.Covers, result.Covers);
            Assert.AreEqual(booking.AreaId, result.AreaId);
            Assert.AreEqual(booking.ChannelId, result.ChannelId);
            Assert.AreEqual(booking.Comments, result.Comments);
            Assert.AreEqual(booking.Tables, result.Tables);
            Assert.AreEqual(booking.Duration, result.Duration);
            Assert.AreEqual(booking.RestaurantId, result.RestaurantId);
        }

        #region AddBooking

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_Throws400WhenNoTableSpecified(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Tables = new int[] { };

            // Act
            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(secondSecret, bookingDto));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_Throws400WhenZeroCovers(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Covers = 0;

            // Act
            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(secondSecret, bookingDto));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_Throws400WhenNoVisitDate(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.VisitDateTime = null;

            // Act
            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(secondSecret, bookingDto));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_AddsBooking(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);

            // Act
            var result = CreateNewBooking(secondSecret, bookingDto);

            // Assert
            Assert.AreEqual(bookingDto.VisitDateTime, result.VisitDateTime);
            Assert.AreEqual(bookingDto.Covers, result.Covers);
            Assert.AreEqual(bookingDto.AreaId, result.AreaId);
            Assert.AreEqual(bookingDto.ChannelId, result.ChannelId);
            Assert.AreEqual(bookingDto.Comments, result.Comments);
            Assert.AreEqual(bookingDto.Tables, result.Tables);
            Assert.AreEqual(bookingDto.Duration, result.Duration);
            Assert.AreEqual(bookingDto.RestaurantId, result.RestaurantId);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_FailsIfCapacityIsExceeded(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            CreateClient(secondSecret);

            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Covers = 10;

            // Act
            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(secondSecret, bookingDto, false));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_PassesIfCapacityIsExceeded_AndOverrideCapacityIsTrue(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            CreateClient(secondSecret);

            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Covers = 10;

            // Act
            var createdBooking = CreateNewBooking(secondSecret, bookingDto, true);

            // Assert
            Assert.AreEqual(bookingDto.Covers, createdBooking.Covers);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBooking_Throws400WhenNegativeDuration(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Duration = -60;

            // Act
            var exception =
                Assert.Throws<RestfulException>(() => CreateNewBooking(secondSecret, bookingDto));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBookingWithPromotion_AddsPromotion(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var promotion = useBumpEnabledRestaurant
                ? new Promotion {PromotionId = Settings.ApiSandboxWithBumpingForeverPromotionId, Quantity = 2}
                : new Promotion {PromotionId = Settings.ApiSandboxForeverPromotionId, Quantity = 2};
            bookingDto.Promotions = new[] { promotion };

            // Act
            var result = CreateNewBooking(secondSecret, bookingDto);

            // Assert
            Assert.AreEqual(promotion.PromotionId, result.Promotions[0].PromotionId);
            Assert.AreEqual(promotion.Quantity, result.Promotions[0].Quantity);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBookingWithCustomer_ReturnsCustomer(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);

            // Act
            var result = CreateNewBooking(secondSecret, bookingDto);

            // Assert
            Assert.IsNotNull(result.Customer);
            Assert.AreEqual(bookingDto.Customer.FirstName, result.Customer.FirstName);
            Assert.AreEqual(bookingDto.Customer.Surname, result.Customer.Surname);
            Assert.AreEqual(bookingDto.Customer.MobileNumber, result.Customer.MobileNumber);
            Assert.AreEqual(bookingDto.Customer.Email, result.Customer.Email);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddBookingWithNoCustomer_ReturnsNoCustomer(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Customer = null;

            // Act
            var result = CreateNewBooking(secondSecret, bookingDto);

            // Assert
            Assert.IsNull(result.Customer);
        }

        [Test]
        public void AddBooking_MarksWalkInsFullySeated()
        {
            // Arrange
            var bookingDto = GetBookingDto(false);

            // Reset customer to make it a walk-in
            bookingDto.Customer = null;

            // Act
            var result = CreateNewBooking(Settings.ApiSandboxSecondSecret, bookingDto);

            // Assert
            Assert.AreEqual(ArrivalStatus.FullySeated, result.ArrivalStatus);
        }

        #endregion

        #region UpdateBooking
        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBooking_UpdatesBooking(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var currentTime = DateTime.Now;
            bookingDto.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);
            booking.VisitDateTime = booking.VisitDateTime?.AddMinutes(30);
            booking.Tables = useBumpEnabledRestaurant ? new[] { Settings.ApiSandboxWithBumpingTable7Id } : new[] { Settings.ApiSandboxTable7Id };
            booking.Covers = 6;
            booking.Duration = 70;
            booking.MealStatus = MealStatus.Drinks;
            booking.CustomerSpend = 123.99;
            booking.ReceiptXml = "<receipt/>";
            booking.ArrivalStatus = ArrivalStatus.FullySeated;

            // Act
            var result = Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);

            // Assert
            Assert.AreEqual(booking.VisitDateTime, result.VisitDateTime);
            Assert.AreEqual(booking.Tables, result.Tables);
            Assert.AreEqual(booking.Covers, result.Covers);
            Assert.AreEqual(booking.Duration, result.Duration);
            Assert.AreEqual(booking.MealStatus, result.MealStatus);
            Assert.AreEqual(booking.CustomerSpend, result.CustomerSpend);
            Assert.AreEqual(booking.ReceiptXml, result.ReceiptXml);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBooking_DoesNotOverrideArrivalStatus(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var currentTime = DateTime.Now;
            bookingDto.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);

            Client.UpdateArrivalStatus(booking.RestaurantId, booking.BookingId, (int) ArrivalStatus.PartiallySeated);
            
            booking.Covers = 6;

            // Act
            var result = Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);

            // Assert
            Assert.AreEqual(ArrivalStatus.PartiallySeated, result.ArrivalStatus);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBooking_Throws400WhenDurationNegative(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            booking.Duration = -60;

            // Act
            var exception =
                Assert.Throws<RestfulException>(
                    () => Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBooking_DoesNotResetBookingStatus(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var currentTime = DateTime.Now;
            bookingDto.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);
            booking.Status = BookingStatus.Confirmed;

            // Act
            Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);
            var updatedBooking = Client.GetBooking(booking.RestaurantId, booking.BookingId);

            // Assert
            Assert.AreEqual(BookingStatus.Confirmed, updatedBooking.Status);
        }
        #endregion

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void CancelBooking_CancelsBooking(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            // Act
            Client.CancelBooking(targetRestaurantId, booking.BookingId);
            var result = Client.GetBooking(targetRestaurantId, booking.BookingId);

            // Assert
            Assert.AreEqual(BookingStatus.Cancelled, result.Status);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddReceiptToBooking_AddsReceipt(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            var xmlReceipt =
                XElement.Parse("<Items><Item><Description>Aged Fillet Steak</Description><Quantity>1</Quantity><Price>37.50</Price></Item><Item><Description>Bon Climat Pinot Noir</Description><Quantity>1</Quantity><Price>67.50</Price></Item></Items>");
            // Act
            var result = Client.AddBookingReceiptJson(targetRestaurantId, booking.BookingId, xmlReceipt.ToString());

            // Assert
            Assert.AreEqual(xmlReceipt.ToString(), XElement.Parse(result.ReceiptXml).ToString());
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddReceiptToBooking_AddsReceiptWithContentTypeXML(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            var xmlReceipt =
                XElement.Parse("<Items><Item><Description>Aged Fillet Steak</Description><Quantity>1</Quantity><Price>37.50</Price></Item><Item><Description>Bon Climat Pinot Noir</Description><Quantity>1</Quantity><Price>67.50</Price></Item></Items>");
            // Act
            var b = Client.AddBookingReceiptXml(targetRestaurantId, booking.BookingId, xmlReceipt.ToString());

            // Assert
            Assert.AreEqual(xmlReceipt.ToString(), XElement.Parse(b.ReceiptXml).ToString());
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddReceiptToBooking_AddsReceiptWithNoContentTypeSet(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            var xmlReceipt =
                XElement.Parse("<Items><Item><Description>Aged Fillet Steak</Description><Quantity>1</Quantity><Price>37.50</Price></Item><Item><Description>Bon Climat Pinot Noir</Description><Quantity>1</Quantity><Price>67.50</Price></Item></Items>");
            // Act
            var result = Client.AddBookingReceiptNoContentType(targetRestaurantId, booking.BookingId, xmlReceipt.ToString());

            // Assert
            Assert.AreEqual(xmlReceipt.ToString(), XElement.Parse(result.ReceiptXml).ToString());
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void AddReceiptToBooking_Gets400IfAddingNullReceipt(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            // Act
            var exception =
                Assert.Throws<RestfulException>(
                    () => Client.AddBookingReceiptJson(targetRestaurantId, booking.BookingId, null));

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateMealStatus_UpdatesStatus(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            var mealStatus = MealStatus.Dessert;

            // Act
            var result = Client.UpdateMealStatus(targetRestaurantId, booking.BookingId, (int)mealStatus);

            // Assert
            Assert.AreEqual(mealStatus, result.MealStatus);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateArrivalStatus_UpdatesStatus(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var arrivalStatus = ArrivalStatus.WaitingInBar;
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            // Act
            var result = Client.UpdateArrivalStatus(targetRestaurantId, booking.BookingId, (int)arrivalStatus);

            // Assert
            Assert.AreEqual(arrivalStatus, result.ArrivalStatus);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetCustomer_GetsCustomer(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            // Act
            var result = Client.GetCustomer(booking.Customer.CustomerId);

            // Assert
            Assert.AreEqual(booking.Customer.FirstName, result.FirstName);
            Assert.AreEqual(booking.Customer.Surname, result.Surname);
            Assert.AreEqual(booking.Customer.MobileNumber, result.MobileNumber);
            Assert.AreEqual(booking.Customer.Email, result.Email);
        }

        #region BookingHistory

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBookingHistoryV1_GetsHistory(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var randomName = Guid.NewGuid().ToString("N");

            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);

            bookingDto.Customer.Surname = randomName;
            bookingDto.Customer.Email = $"dev+{randomName}@resdiary.com";

            var booking = CreateNewBooking(secondSecret, bookingDto);

            // Act
            var customerHistory = Client.GetBookingHistory(booking.Customer.CustomerId);
            var bookingHistory = customerHistory.FirstOrDefault(h => h.BookingId == booking.BookingId);

            // Assert
            Assert.IsNotEmpty(customerHistory);
            Assert.IsNotNull(bookingHistory);
            Assert.AreEqual(bookingDto.Covers, bookingHistory.Covers);
            Assert.AreEqual(bookingDto.Comments, bookingHistory.BookingComments);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBookingHistoryV2_GetsHistory(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var randomName = Guid.NewGuid().ToString("N");

            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Customer.Surname = randomName;
            bookingDto.Customer.Email = $"dev+{randomName}@resdiary.com";

            var booking = CreateNewBooking(secondSecret, bookingDto);

            // Act
            var customerHistory = Client.GetBookingHistoryV2(booking.Customer.CustomerId);
            var bookingHistory = customerHistory.FirstOrDefault(h => h.BookingId == booking.BookingId);

            // Assert
            Assert.IsNotEmpty(customerHistory);
            Assert.IsNotNull(bookingHistory);
            Assert.AreEqual(bookingDto.Covers, bookingHistory.Covers);
            Assert.AreEqual(bookingDto.Comments, bookingHistory.BookingComments);
        }
        
        [Test]
        public void GetBookingHistoryV2_GetsHistoryForMultipleProviders()
        {
            // Arrange
            var randomName = Guid.NewGuid().ToString("N");

            var bookingDto = GetBookingDto(false);
            bookingDto.Customer.Surname = randomName;
            bookingDto.Customer.Email = $"dev+{randomName}@resdiary.com";

            var booking = CreateNewBooking(Settings.ApiSandboxSecondSecret, bookingDto);

            var bookingDto2 = GetBookingDto(true);
            bookingDto2.Customer.Surname = randomName;
            bookingDto2.Customer.Email = $"dev+{randomName}@resdiary.com";

            var booking2 = CreateNewBooking(Settings.ApiSandboxWithBumpingSecondSecret, bookingDto2);

            // Act
            var customerHistory = Client.GetBookingHistoryV2(booking.Customer.CustomerId);
            var customerHistoryForProvider1 = customerHistory.FirstOrDefault(h => h.RestaurantId == booking.RestaurantId);
            var customerHistoryForProvider2 = customerHistory.FirstOrDefault(h => h.RestaurantId == Settings.ApiSandboxWithBumpingRestaurantId);

            // Assert
            Assert.IsNotNull(customerHistoryForProvider1);
            Assert.IsNotNull(customerHistoryForProvider2);
        }

        #endregion

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetNextFreeTables_GetsTables(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Act
            CreateClient(secondSecret);
            var nextFreeTables = Client.GetNextFreeTables(targetRestaurantId);

            // Assert
            Assert.IsNotEmpty(nextFreeTables);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetTableJoins_GetsJoins(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {

            // Arrange
            CreateClient(secondSecret);
            var join = useBumpEnabledRestaurant
                ? new[] {Settings.ApiSandboxWithBumpingTable1Id, Settings.ApiSandboxWithBumpingTable7Id}
                : new[] {Settings.ApiSandboxTable1Id, Settings.ApiSandboxTable7Id};

            // Act
            var joins = Client.GetTableJoins(targetRestaurantId,Settings.TableJoinCovers, DateTime.UtcNow);

            // Assert
            Assert.IsNotEmpty(joins);
            Assert.IsTrue(joins.FirstOrDefault().SequenceEqual(join));
        }

 
    [Test, TestCaseSource(nameof(TestCaseData))]
        public void SwapBookings(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking1 = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Tables = useBumpEnabledRestaurant ? new[] { Settings.ApiSandboxWithBumpingTable7Id } : new[] { Settings.ApiSandboxTable7Id };
            var booking2 = CreateNewBooking(secondSecret, bookingDto);

            // Act
            Client.SwapBookings(targetRestaurantId, booking1.BookingId, booking2.BookingId);

            var updatedBooking1 = Client.GetBooking(targetRestaurantId, booking1.BookingId);
            var updatedBooking2 = Client.GetBooking(targetRestaurantId, booking2.BookingId);

            // Assert
            Assert.AreEqual(booking2.Tables, updatedBooking1.Tables);
            Assert.AreEqual(booking1.Tables, updatedBooking2.Tables);
        }

        #region UpdateBookingTables

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBookingTables_UpdatesTableToTable(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            var newTable = useBumpEnabledRestaurant
                ? new List<int> {Settings.ApiSandboxWithBumpingTable7Id}
                : new List<int> {Settings.ApiSandboxTable7Id};

            // Act
            var result = Client.UpdateBookingTables(targetRestaurantId, booking.BookingId, newTable);

            // Assert
            Assert.AreEqual(newTable, result.Tables);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBookingTables_UpdatesTableToJoin(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            var newTables = useBumpEnabledRestaurant
                ? new List<int> {Settings.ApiSandboxWithBumpingTable6Id, Settings.ApiSandboxWithBumpingTable7Id}
                : new List<int> {Settings.ApiSandboxTable6Id, Settings.ApiSandboxTable7Id};

            // Act
            var result = Client.UpdateBookingTables(targetRestaurantId, booking.BookingId, newTables);

            var expectedTables = newTables.ToArray();
            Array.Sort(expectedTables);

            // Assert
            Assert.AreEqual(expectedTables, result.Tables);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UpdateBookingTables_UpdatesJoinToTable(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.Tables = useBumpEnabledRestaurant
                    ? new[] {Settings.ApiSandboxWithBumpingTable6Id, Settings.ApiSandboxWithBumpingTable7Id}
                    : new[] {Settings.ApiSandboxTable6Id, Settings.ApiSandboxTable7Id}
                ;
            var booking = CreateNewBooking(secondSecret, bookingDto);
            var newTable = useBumpEnabledRestaurant
                ? new List<int> {Settings.ApiSandboxWithBumpingTable1Id}
                : new List<int> {Settings.ApiSandboxTable1Id};

            // Act
            var result = Client.UpdateBookingTables(targetRestaurantId, booking.BookingId, newTable);

            // Assert
            Assert.AreEqual(newTable.ToArray(), result.Tables);
        }
        #endregion

        #region GetBookingModification

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBookingModifications_ReturnsModifications(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var currentTime = DateTime.Now;
            bookingDto.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);
            booking.Covers = 4;
            Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);

            // Act
            var result = Client.GetBookingChanges(targetRestaurantId, DateTime.Today);

            // Assert
            Assert.IsNotEmpty(result);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBookingModifications_ReturnsBookingModification(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.VisitDateTime = DateTime.SpecifyKind(DateTime.Today + new TimeSpan(7, 0, 0), DateTimeKind.Utc);

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);
            booking.Covers = booking.Covers + 2;
            booking.VisitDateTime = booking.VisitDateTime?.AddMinutes(30);
            Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);

            // Act
            var result = Client.GetBookingChanges(targetRestaurantId, DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc));
            var foundBooking = result.FirstOrDefault(r => r.BookingId == booking.BookingId);

            // Assert
            Assert.IsNotNull(foundBooking);
            Assert.AreEqual(booking.VisitDateTime, foundBooking.VisitDateTime);
            Assert.AreEqual(booking.Covers, foundBooking.Covers);
        }

        #endregion

        #region GetBookingModificationsForServiceSince

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBookingModificationsForServiceSince_ReturnsModifications(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            var currentTime = DateTime.Now;
            bookingDto.VisitDateTime =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);

            var booking = CreateNewBooking(secondSecret, bookingDto);
            booking.Covers = 4;
            Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);

            // Act
            var result = Client.GetBookingChangesForServiceSince(targetRestaurantId, booking.ServiceId, DateTime.Today,
                new TimeSpan(1, 0, 0));

            // Assert
            Assert.IsNotEmpty(result);
        }
        
        [Test, TestCaseSource(nameof(TestCaseData))]
        public void GetBookingModificationsForServiceSince_ReturnsBookingModification(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var bookingDto = GetBookingDto(useBumpEnabledRestaurant);
            bookingDto.VisitDateTime = DateTime.SpecifyKind(DateTime.Today + new TimeSpan(7, 0, 0), DateTimeKind.Utc);

            // Create the booking
            var booking = CreateNewBooking(secondSecret, bookingDto);
            
            booking.Covers = booking.Covers + 2;
            booking.VisitDateTime = booking.VisitDateTime?.AddMinutes(30);
            Client.UpdateBooking(targetRestaurantId, booking.BookingId, booking, true);

            // Act
            var result = Client.GetBookingChangesForServiceSince(targetRestaurantId, booking.ServiceId, DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                new TimeSpan(1, 0, 0));
            var foundBooking = result.FirstOrDefault(r => r.BookingId == booking.BookingId);

            // Assert
            Assert.IsNotNull(foundBooking);
            Assert.AreEqual(booking.VisitDateTime, foundBooking.VisitDateTime);
            Assert.AreEqual(booking.Covers, foundBooking.Covers);
        }

        #endregion

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void UnallocateBooking_UnallocatesBooking(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);

            // Act
            var result = Client.UnallocateBooking(targetRestaurantId, booking.BookingId);

            // Assert
            Assert.AreEqual(BookingStatus.Bumped, result.Status);
        }

        [Test, TestCaseSource(nameof(TestCaseData))]
        public void ReallocateBooking_ReallocatesBooking(int targetRestaurantId, string secondSecret, bool useBumpEnabledRestaurant)
        {
            // Arrange
            var booking = CreateNewBooking(secondSecret, useBumpEnabledRestaurant);
            Client.UnallocateBooking(targetRestaurantId, booking.BookingId);

            // Act
            var result = Client.ReallocateBooking(targetRestaurantId, booking.BookingId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Booking);
            Assert.AreNotEqual((int)BookingStatus.Bumped, result.Booking.Status);
        }
    }
}
