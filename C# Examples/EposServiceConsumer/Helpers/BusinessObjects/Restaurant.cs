using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Restaurant
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TimeZone { get; set; }

        [DataMember]
        public bool IsTrafficLightReasonRequired { get; set; }

        [DataMember]
        public IEnumerable<Segment> Segments { get; set; }

        [DataMember]
        public IEnumerable<Channel> Channels { get; set; }

        [DataMember]
        public IEnumerable<Interest> Interests { get; set; }

        [DataMember]
        public IEnumerable<CustomerType> CustomerTypes { get; set; }

        [DataMember]
        public IEnumerable<SpecialRequest> SpecialRequests { get; set; }

        [DataMember]
        public string MicrositeName { get; set; }

        [DataMember]
        public int DeploymentId { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int GroupId { get; set; }

        [DataMember]
        public string GroupName { get; set; }
    }
}