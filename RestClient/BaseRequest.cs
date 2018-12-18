using System.Collections.Generic;
using System.Net;


public class BaseRequest<T>
{
    public List<(string, string)> Headers { get; set; }
    public T Body { get; set; }
    public HttpStatusCode ExpectedResponseStatusCode { get; set; }
}
