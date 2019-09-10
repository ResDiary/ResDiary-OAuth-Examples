using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Table
    {
        [DataMember]
        public int TableId { get; set; }

        [DataMember]
        public int AreaId { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        [DataMember]
        public int Covers { get; set; }
    }
}