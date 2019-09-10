using System;

namespace RD.EposServiceConsumer.UnitTests
{
    public static class Settings
    {
        public static string ServiceBaseUri { get; private set; }
        public static string OathUri { get; private set; }
        public static string ConsumerKey { get; private set; }
        public static string ConsumerSecret { get; private set; }
        public static bool IgnoreSslErrors { get; private set; }

        public static string ApiSandboxSecondSecret { get; private set; }
        public static int ApiSandboxRestaurantId { get; private set; }
        public static int ApiSandboxSegmentId { get; private set; }
        public static string ApiSandboxSegmentName { get; private set; }
        public static int ApiSandboxRestaurantAreaId { get; private set; }
        public static int ApiSandboxTable1Id { get; private set; }
        public static int ApiSandboxTable6Id { get; private set; }
        public static int ApiSandboxTable7Id { get; private set; }
        public static int ApiSandboxApiTestChannelId { get; private set; }
        public static int ApiSandboxForeverPromotionId { get; private set; }

        public static string ApiSandboxWithBumpingSecondSecret { get; private set; }
        public static int ApiSandboxWithBumpingRestaurantId { get; private set; }
        public static int ApiSandboxWithBumpingSegmentId { get; private set; }
        public static string ApiSandboxWithBumpingSegmentName { get; private set; }
        public static int ApiSandboxWithBumpingRestaurantAreaId { get; private set; }
        public static int ApiSandboxWithBumpingTable1Id { get; private set; }
        public static int ApiSandboxWithBumpingTable6Id { get; private set; }
        public static int ApiSandboxWithBumpingTable7Id { get; private set; }
        public static int ApiSandboxWithBumpingApiTestChannelId { get; private set; }
        public static int ApiSandboxWithBumpingForeverPromotionId { get; private set; }

        public static string RestaurantName {  get; private set; }
        public static int TableJoinCovers { get; private set; }


        public static int ApiSandboxPaymentsRestaurantId { get; private set; }


        static Settings()
        {
            ServiceBaseUri = GetSetting("EPOS_SERVICE_BASE_URI");
            OathUri = GetSetting("EPOS_SERVICE_OAUTH_URI");
            ConsumerKey = GetSetting("EPOS_CONSUMER_KEY");
            ConsumerSecret = GetSetting("EPOS_CONSUMER_SECRET");
            IgnoreSslErrors = bool.Parse(GetSetting("EPOS_IGNORE_SSL_ERRORS"));

            ApiSandboxSecondSecret = GetSetting("EPOS_API_SANDBOX_SECOND_SECRET");
            ApiSandboxRestaurantId = int.Parse(GetSetting("EPOS_API_SANDBOX_RESTAURANT_ID"));
            ApiSandboxSegmentId = int.Parse(GetSetting("EPOS_API_SANDBOX_SEGMENT_ID"));
            ApiSandboxSegmentName = GetSetting("EPOS_API_SANDBOX_SEGMENT_NAME");
            ApiSandboxRestaurantAreaId = int.Parse(GetSetting("EPOS_API_SANDBOX_RESTAURANT_AREA_ID"));
            ApiSandboxTable1Id = int.Parse(GetSetting("EPOS_API_SANDBOX_TABLE_1_ID"));
            ApiSandboxTable6Id = int.Parse(GetSetting("EPOS_API_SANDBOX_TABLE_6_ID"));
            ApiSandboxTable7Id = int.Parse(GetSetting("EPOS_API_SANDBOX_TABLE_7_ID"));
            ApiSandboxApiTestChannelId = int.Parse(GetSetting("EPOS_API_SANDBOX_API_TEST_CHANNEL_ID"));
            ApiSandboxForeverPromotionId = int.Parse(GetSetting("EPOS_API_SANDBOX_FOREVER_PROMOTION_ID"));

            ApiSandboxWithBumpingSecondSecret = GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_CONSUMER_SECOND_SECRET");
            ApiSandboxWithBumpingRestaurantId = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_RESTAURANT_ID"));
            ApiSandboxWithBumpingSegmentId = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_SEGMENT_ID"));
            ApiSandboxWithBumpingSegmentName = GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_SEGMENT_NAME");
            ApiSandboxWithBumpingRestaurantAreaId = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_RESTAURANT_AREA_ID"));
            ApiSandboxWithBumpingTable1Id = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_TABLE_1_ID"));
            ApiSandboxWithBumpingTable6Id = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_TABLE_6_ID"));
            ApiSandboxWithBumpingTable7Id = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_TABLE_7_ID"));
            ApiSandboxWithBumpingApiTestChannelId = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_API_TEST_CHANNEL_ID"));
            ApiSandboxWithBumpingForeverPromotionId = int.Parse(GetSetting("EPOS_API_SANDBOX_WITH_BUMPING_FOREVER_PROMOTION_ID"));
            RestaurantName = GetSetting("Restaurant_Name");
            TableJoinCovers = int.Parse(GetSetting("Table_Join_Covers"));


            ApiSandboxPaymentsRestaurantId = int.Parse(GetSetting("EPOS_API_SANDBOX_PAYMENTS_RESTAURANT_ID"));

        }

        private static string GetSetting(string key)
        {
            var environmentValue = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(environmentValue))
            {
                return environmentValue;
            }

            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}