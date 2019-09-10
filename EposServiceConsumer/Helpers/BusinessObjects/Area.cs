using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Area
    {
        [DataMember]
        public int AreaId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int SortOrder { get; set; }
    }
}