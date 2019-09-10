using System.Net;
using NUnit.Framework;
using RD.EposServiceConsumer.Helpers;

namespace RD.EposServiceConsumer.UnitTests.ReleaseTests._10._3_EPOS
{
    [TestFixture]
    [Category("ReleaseTests")]
    public class RD_13643_Token_Expiry : EposTestsBase
    {
        [Test]
        public void Test1_RequestFails_IfTokenExpired()
        {
            CreateClient(Settings.ApiSandboxSecondSecret);

            Client.GetRestaurant(Settings.ApiSandboxRestaurantId);

            // EXPIRE TOKEN!!!

            var exception = Assert.Throws<RestfulException>(() => Client.GetRestaurant(Settings.ApiSandboxRestaurantId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, exception.Response.StatusCode);
        }
    }
}
