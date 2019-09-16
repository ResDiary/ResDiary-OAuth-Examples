using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RD.EposServiceConsumer.Helpers.BusinessObjects
{
    [DataContract]
    public class ApiErrorDto
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public IEnumerable<ValidationErrorDto> ValidationErrors { get; set; }
    }
}