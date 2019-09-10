using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class Customer
    {
        [DataMember]
        public string AccessCode { get; set; }

        [DataMember]
        public IEnumerable<int> Interests { get; set; }

        [DataMember]
        public bool IsRoyaltyCardHolder { get; set; }

        [DataMember]
        public bool IsVip { get; set; }

        [DataMember]
        public int CustomerTypeId { get; set; }

        [DataMember]
        public int CustomerId { get; set; }

        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public DateTime? Birthday { get; set; }

        [DataMember]
        public DateTime? Anniversary { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string MobileNumber { get; set; }
    }
}