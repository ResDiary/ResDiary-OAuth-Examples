using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using RD.EposServiceConsumer.Helpers.BusinessObjects;

namespace RD.EposServiceConsumer.Helpers
{
    public class EposServiceClient : IEposServiceClient
    {
        private static string _key;
        private static string _secret;
        private static string _secondSecret;
        private static string _accessToken;
        private static TempMemoryTokenManager _tokenManager;
        private readonly Uri _serviceEndpoint;
        private static Uri _oAuthEndpoint;
        private static string _applicationName;
        private static string _applicationVersion;
        private static string _testName;

        public EposServiceClient(string serviceEndpoint, string key, string secret, string secondSecret, string oAuthEndpoint)
        {
            _serviceEndpoint = new Uri(serviceEndpoint);
            _key = key;
            _secret = secret;
            _secondSecret = secondSecret;
            _oAuthEndpoint = new Uri(oAuthEndpoint);
            _accessToken = string.Empty;
        }

        public EposServiceClient(string serviceEndpoint, string key, string secret, string secondSecret, string oAuthEndpoint, string applicationName, string applicationVersion, string testName = "")
        {
            _serviceEndpoint = new Uri(serviceEndpoint);
            _key = key;
            _secret = secret;
            _secondSecret = secondSecret;
            _oAuthEndpoint = new Uri(oAuthEndpoint);
            _accessToken = string.Empty;
            _applicationName = applicationName;
            _applicationVersion = applicationVersion;
            _testName = testName;
        }

        #region Implementation of IEposServiceClient

        public Restaurant GetRestaurant(int restaurantId)
        {
            return InvokeJson<Restaurant>(string.Format("/WebServices/Epos/V1/Restaurant/{0}", restaurantId));
        }

        public DiaryBooking[] GetDiaryData(int restaurantId, DateTime date, bool includeUnallocatedBookings = false)
        {
            return
                InvokeJson<DiaryBooking[]>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/DiaryData?date={1}&includeUnallocatedBookings={2}", restaurantId,
                                                     date.ToString("yyyy-MM-dd"), includeUnallocatedBookings));
        }

        public Booking GetBooking(int restaurantId, int bookingId)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}", restaurantId, bookingId));
        }

        public Booking AddBooking(int restaurantId, object booking, bool overrideCovers = false)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking?overrideCovers={1}", restaurantId, overrideCovers ? "true" : "false"), HttpDeliveryMethods.PostRequest, booking);
        }

        public Booking UpdateBooking(int restaurantId, int bookingId, Booking booking, bool overrideCovers = false)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}?overrideCovers={2}", restaurantId, bookingId, overrideCovers ? "true" : "false"), HttpDeliveryMethods.PutRequest, booking);
        }

        public void CancelBooking(int restaurantId, int bookingId)
        {
            InvokeJson<string>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}", restaurantId, bookingId),
                HttpDeliveryMethods.DeleteRequest);
        }

        public Booking UpdateBookingTables(int restaurantId, int bookingId, List<int> tableIds)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/Tables", restaurantId, bookingId), HttpDeliveryMethods.PutRequest, tableIds);
        }

        public Booking AddBookingReceiptJson(int restaurantId, int bookingId, string xmlReceipt)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/Receipt", restaurantId, bookingId), HttpDeliveryMethods.PostRequest, xmlReceipt);
        }

        public Booking AddBookingReceiptXml(int restaurantId, int bookingId, string xmlReceipt)
        {
            return InvokeXml<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/Receipt", restaurantId, bookingId), HttpDeliveryMethods.PostRequest, xmlReceipt);
        }

        public Booking AddBookingReceiptNoContentType(int restaurantId, int bookingId, string xmlReceipt)
        {
            return InvokeForContentType<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/Receipt", restaurantId, bookingId), HttpDeliveryMethods.PostRequest, xmlReceipt);
        }

        public Booking UpdateArrivalStatus(int restaurantId, int bookingId, int arrivalStatus)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/ArrivalStatus", restaurantId, bookingId), HttpDeliveryMethods.PutRequest, arrivalStatus);
        }

        public Booking UpdateMealStatus(int restaurantId, int bookingId, int mealStatus)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/MealStatus", restaurantId, bookingId), HttpDeliveryMethods.PutRequest, mealStatus);
        }

        public Customer GetCustomer(int customerId)
        {
            return InvokeJson<Customer>(string.Format("/WebServices/Epos/V1/Customer/{0}", customerId));
        }

        public BookingHistory[] GetBookingHistory(int customerId)
        {
            return InvokeJson<BookingHistory[]>(string.Format("/WebServices/Epos/V1/Customer/{0}/BookingHistory", customerId));
        }

        public BookingHistory[] GetBookingHistoryV2(int customerId)
        {
            return InvokeJson<BookingHistory[]>(string.Format("/WebServices/Epos/V1/Customer/{0}/BookingHistoryV2", customerId));
        }

        public NextFreeTable[] GetNextFreeTables(int restaurantId)
        {
            return InvokeJson<NextFreeTable[]>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/NextFreeTables", restaurantId));
        }

        public IEnumerable<IEnumerable<int>> GetTableJoins(int restaurantId, int coversCount, DateTime diaryDate)
        {
            return InvokeJson<IEnumerable<IEnumerable<int>>>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/TableJoins/{1}?date={2}", restaurantId, coversCount, diaryDate.ToString("yyyy-MM-dd")));
        }

        public void SwapBookings(int restaurantId, int bookingId1, int bookingId2)
        {
            InvokeJson<string>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/SwapBookings/{1}/{2}", restaurantId, bookingId1, bookingId2));
        }

        public IEnumerable<BookingModification> GetBookingChanges(int restaurantId, DateTime changeDateTime)
        {
            return InvokeJson<IEnumerable<BookingModification>>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/BookingChanges?date={1}", restaurantId, changeDateTime.ToString("yyyy-MM-dd")));
        }

        public IEnumerable<BookingModification> GetBookingChangesForServiceSince(int restaurantId, int serviceId, DateTime changeDateTime, TimeSpan since)
        {
            return InvokeJson<IEnumerable<BookingModification>>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/BookingChangesForServiceSince/{1}?changeDateTime={2}&time={3}", restaurantId, serviceId, changeDateTime.ToString("yyyy-MM-dd"), since));
        }

        public Booking UnallocateBooking(int restaurantId, int bookingId)
        {
            return InvokeJson<Booking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/Unallocate", restaurantId, bookingId), HttpDeliveryMethods.PostRequest);
        }

        public ReallocatedBooking ReallocateBooking(int restaurantId, int bookingId)
        {
            return InvokeJson<ReallocatedBooking>(string.Format("/WebServices/Epos/V1/Restaurant/{0}/Booking/{1}/Reallocate", restaurantId, bookingId), HttpDeliveryMethods.PostRequest);
        }

        #endregion

        public T InvokeForContentType<T>(string endpointMethod, HttpDeliveryMethods methods = HttpDeliveryMethods.GetRequest,
            string body = null, string contentType = null) where T : class
        {
            WebConsumer webConsumer = CreateWebConsumer();

            var endpoint = new MessageReceivingEndpoint(new Uri(_serviceEndpoint, endpointMethod), HttpDeliveryMethods.AuthorizationHeaderRequest | methods);

            HttpWebRequest webRequest = webConsumer.PrepareAuthorizedRequest(endpoint, GetAccessToken());

            if (contentType != null)
                webRequest.ContentType = contentType;

            if (!String.IsNullOrEmpty(_applicationName + _applicationVersion))
                webRequest.UserAgent = _applicationName + "/" + _applicationVersion;

            if (!string.IsNullOrEmpty(_testName))
                webRequest.Headers["X-RD-test-name"] = _testName;

            if (body != null)
            {
                if (methods == HttpDeliveryMethods.PutRequest || methods == HttpDeliveryMethods.PostRequest)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream))
                        {
                            writer.Write(body);
                        }

                        byte[] bytes = memoryStream.ToArray();

                        webRequest.ContentLength = bytes.Length;

                        Stream requestStream = webConsumer.Channel.WebRequestHandler.GetRequestStream(webRequest);

                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            try
            {
                return DeserializeResponse<T>(webRequest, contentType == null || contentType == "application/json; charset=utf-8");
            }
            catch (WebException we)
            {
                ApiErrorDto apiErrorDto;
                String text;

                if (TryParseResponseToRestfulException(we.Response, out apiErrorDto, out text))
                {
                    if (apiErrorDto != null)
                    {
                        if (apiErrorDto.ValidationErrors != null)
                        {
                            Console.WriteLine("ValidationSummary results:");
                            foreach (var result in apiErrorDto.ValidationErrors)
                            {
                                Console.WriteLine(result.PropertyName + " -->" + result.ErrorMessage);
                            }
                        }
                        throw new RestfulException(apiErrorDto, we);
                    }
                    else
                    {
                        throw new RestfulException(new ApiErrorDto() { Message = text }, we);
                    }
                }

                throw;
            }
            catch (ProtocolException pe)
            {
                if (pe.InnerException != null && pe.InnerException.GetType() == typeof(WebException))
                {
                    WebException we = (WebException)pe.InnerException;
                    try
                    {
                        Stream s = (we).Response.GetResponseStream();
                        StreamReader st = new StreamReader(s);
                        Console.WriteLine(st.ReadToEnd());
                    }
                    catch (Exception)
                    {
                    }
                    Console.WriteLine(we.Status + ": " + ((HttpWebResponse)we.Response).StatusDescription);
                }
                throw;
            }
        }

        public T InvokeXml<T>(string endpointMethod, HttpDeliveryMethods methods = HttpDeliveryMethods.GetRequest, string xml = "") where T : class
        {
            return InvokeForContentType<T>(endpointMethod, methods, xml, "text/xml; charset=utf-8");
        }

        public T InvokeJson<T>(string endpointMethod, HttpDeliveryMethods methods = HttpDeliveryMethods.GetRequest, object obj = null) where T : class
        {
            string jsonBody = null;
            if (obj != null)
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, obj);
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        jsonBody = reader.ReadToEnd();
                    }
                }
            }

            return InvokeForContentType<T>(endpointMethod, methods, jsonBody, "application/json; charset=utf-8");
        }

        public T DeserializeResponse<T>(HttpWebRequest webRequest, bool useJson = true) where T : class
        {
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                Stream s = webResponse.GetResponseStream();
                StreamReader st = new StreamReader(s);
                string text = st.ReadToEnd();

                if (text.Length == 0)
                {
                    return default(T);
                }

                Console.WriteLine("Response:\n" + text);
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(text);
                writer.Flush();
                stream.Position = 0;

                return useJson
                    ? new DataContractJsonSerializer(typeof(T)).ReadObject(stream) as T
                    : new DataContractSerializer(typeof(T)).ReadObject(stream) as T;
            }
        }

        private static bool TryParseResponseToRestfulException(WebResponse response, out ApiErrorDto apiErrorDto, out String text)
        {
            text = null;
            apiErrorDto = null;
            try
            {
                var errorSerializer = new DataContractJsonSerializer(typeof(ApiErrorDto));
                Stream s = response.GetResponseStream();
                StreamReader st = new StreamReader(s);
                text = st.ReadToEnd();

                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(text);
                writer.Flush();
                stream.Position = 0;

                apiErrorDto = (ApiErrorDto)errorSerializer.ReadObject(stream);

                return true;
            }
            catch (Exception e)
            {
                if (text != null)
                    return true;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public static TempMemoryTokenManager TokenManager
        {
            get { return _tokenManager ?? (_tokenManager = new TempMemoryTokenManager()); }
        }

        private static string GetAccessToken()
        {
            if (string.IsNullOrWhiteSpace(_accessToken))
            {
                WebConsumer webConsumer = CreateWebConsumer();

                var requestParameters = new Dictionary<string, string>
                                            {
                                                {
                                                    "scope",
                                                    "http://app.restaurantdiary.com/WebServices/Epos/v1"
                                                    },
                                                {
                                                    "second_secret",
                                                    _secondSecret
                                                    }
                                            };
                try
                {
                    _accessToken = webConsumer.RequestNewClientAccount(requestParameters);
                }
                catch (WebException we)
                {
                    Stream s = we.Response.GetResponseStream();
                    StreamReader st = new StreamReader(s);
                    Console.WriteLine(st.ReadToEnd());
                    throw;
                }
                catch (ProtocolException pe)
                {
                    if (pe.InnerException != null && pe.InnerException.GetType() == typeof(WebException))
                    {
                        WebException we = (WebException)pe.InnerException;
                        try
                        {
                            Stream s = (we).Response.GetResponseStream();
                            StreamReader st = new StreamReader(s);
                            Console.WriteLine(st.ReadToEnd());
                        }
                        catch (Exception)
                        {
                        }
                        Console.WriteLine(we.Status + ": " + ((HttpWebResponse)we.Response).StatusDescription);
                        throw we;
                    }
                    throw;
                }
            }

            return _accessToken;
        }

        private static WebConsumer CreateWebConsumer()
        {
            TokenManager.ConsumerKey = _key;
            TokenManager.ConsumerSecret = _secret;

            var requestEndpoint = new MessageReceivingEndpoint(_oAuthEndpoint, HttpDeliveryMethods.PostRequest);
            var authoriseEndpoint = new MessageReceivingEndpoint(_oAuthEndpoint, HttpDeliveryMethods.PostRequest);
            var accessEndpoint = new MessageReceivingEndpoint(_oAuthEndpoint, HttpDeliveryMethods.PostRequest);

            var description = new ServiceProviderDescription
            {
                RequestTokenEndpoint = requestEndpoint,
                UserAuthorizationEndpoint = authoriseEndpoint,
                AccessTokenEndpoint = accessEndpoint,
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[]
                                                                     {
                                                                         new HmacSha1SigningBindingElement()
                                                                     }
            };

            var consumer = new WebConsumer(description, TokenManager);

            return consumer;
        }

        public string Test()
        {
            return InvokeJson<string>("/Test");
        }
    }
}