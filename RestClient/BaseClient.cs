using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Linq;
using System.Net;
using System.Security.Authentication;

public class BaseClient : RestClient
{
    protected ICacheService _cache;
    protected IErrorLogger _errorLogger;
    public BaseClient(ICacheService cache, IDeserializer deserializer, IErrorLogger errorLogger, string baseUrl)
    {
        _cache = cache;
        _errorLogger = errorLogger;
        AddHandler("application/json", deserializer);
        AddHandler("text/json", deserializer);
        AddHandler("text/x-json", deserializer);
        BaseUrl = new Uri(baseUrl);
    }

    private void TimeoutCheck(IRestRequest request, IRestResponse response)
    {
        if (response.StatusCode == 0)
        {
            LogError(BaseUrl, request, response);
        }
    }

    public override IRestResponse Execute(IRestRequest request)
    {
        var response = base.Execute(request);
        TimeoutCheck(request, response);
        return response;
    }
    public override IRestResponse<T> Execute<T>(IRestRequest request)
    {
        var response = base.Execute<T>(request);
        TimeoutCheck(request, response);
        return response;
    }

    public T Get<T>(IRestRequest request, HttpStatusCode expectedStatusCode = HttpStatusCode.OK) where T : new()
    {
        var response = Execute<T>(request);
        CheckResponseStatusCode(request, response, expectedStatusCode);
        return response.Data;
    }

    public T GetFromCache<T>(IRestRequest request, string cacheKey, HttpStatusCode expectedStatusCode = HttpStatusCode.OK) where T : class, new()
    {
        var item = _cache.Get<T>(cacheKey);
        if (item == null)
        {
            var response = Execute<T>(request);
            CheckResponseStatusCode(request, response, expectedStatusCode);

            _cache.Set(cacheKey, response.Data);
            item = response.Data;
        }
        return item;
    }

    private void CheckResponseStatusCode(IRestRequest request, IRestResponse response, HttpStatusCode expectedStatusCode)
    {
        if (response.StatusCode == expectedStatusCode) return;

            switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                LogError(BaseUrl, request, response);
                throw new WebException($"Expected {expectedStatusCode.ToString()}, but got: 200 - Success! : {response.ErrorMessage} {response.Content}");
            case HttpStatusCode.BadRequest:
                LogError(BaseUrl, request, response);
                throw new WebException($"400 - Bad Request! : {response.ErrorMessage} {response.Content}");
            case HttpStatusCode.Unauthorized:
                LogError(BaseUrl, request, response);
                throw new AuthenticationException($"401 - Not Authorized! : {response.ErrorMessage} {response.Content}");
            case (HttpStatusCode)422: // Unprocessable:
                LogError(BaseUrl, request, response);
                throw new WebException($"422 - Unprocessable! : {response.ErrorMessage} {response.Content}");
            case HttpStatusCode.InternalServerError:
                LogError(BaseUrl, request, response);
                throw new WebException($"500 - Internal Server Error! : {response.ErrorMessage} {response.Content}");

            //ToDo: add more exception cases

            default:
                LogError(BaseUrl, request, response);
                throw new Exception($"Unknown Exception! : {response.ErrorMessage} {response.Content}");
        }
    }

    private void LogError(Uri BaseUrl, IRestRequest request, IRestResponse response)
    {
        //Get the values of the parameters passed to the API
        string parameters = string.Join(", ", request.Parameters.Select(x => x.Name.ToString() + "=" + ((x.Value == null) ? "NULL" : x.Value)).ToArray());

        //Set up the information message with the URL, the status code, and the parameters.
        string info = "Request to " + BaseUrl.AbsoluteUri + request.Resource + " failed with status code " + response.StatusCode + ", parameters: "
        + parameters + ", and content: " + response.Content;

        //Acquire the actual exception
        Exception ex;
        if (response != null && response.ErrorException != null)
        {
            ex = response.ErrorException;
        }
        else
        {
            ex = new Exception(info);
            info = string.Empty;
        }

        //Log the exception and info message
        _errorLogger.LogError(ex, info);
    }
}