using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class ValidationErrorDto
    {
        [DataMember]
        public string PropertyName { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
