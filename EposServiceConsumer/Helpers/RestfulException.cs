using System;
using System.Collections.Generic;
using System.Net;
using RD.EposServiceConsumer.Helpers.BusinessObjects;

namespace RD.EposServiceConsumer.Helpers
{
    public class RestfulException : Exception
    {
        public RestfulException(ApiErrorDto apiErrorDto, WebException innerException)
            : base(apiErrorDto.Message, innerException)
        {
            ValidationResults = apiErrorDto.ValidationErrors;
            Response = (HttpWebResponse) innerException.Response;
        }

        public IEnumerable<ValidationErrorDto> ValidationResults { get; set; }

        public HttpWebResponse Response { get; set; }
    }
}