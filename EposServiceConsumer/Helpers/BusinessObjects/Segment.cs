using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Segment
    {
        [DataMember]
        public int SegmentId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IEnumerable<TimeScope> TimeScopes { get; set; }

        [DataMember]
        public IEnumerable<Area> Areas { get; set; }

        [DataMember]
        public IEnumerable<Service> Services { get; set; }

        [DataMember]
        public IEnumerable<Table> Tables { get; set; }
    }
}