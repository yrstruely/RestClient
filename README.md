# REST API Client Library - Built on Top of RestSharp

This Rest API Client Library serves as a base class to clients in your project. Simplifying the building 
use of REST API Clients in your project.

### Features

* Extends RestSharp Functionality with:
* Caching
* Error Logging
* Timeout checking and Error Logging
* Automatic XML and JSON deserialization
* Supports custom serialization and deserialization via ISerializer and IDeserializer
* Fuzzy element name matching ('product_id' in XML/JSON will match C# property named 'ProductId')
* Automatic detection of type of content returned
* GET, POST, PUT, PATCH, HEAD, OPTIONS, DELETE, COPY supported
* Other non-standard HTTP methods also supported
* OAuth 1, OAuth 2, Basic, NTLM and Parameter-based Authenticators included
* Supports custom authentication schemes via IAuthenticator
* Multi-part form/file uploads

```csharp
	class ExampleClient : BaseClient
    {
        public ExampleClient(ICacheService cache, IDeserializer deserializer, IErrorLogger errorLogger)
        : base(cache, deserializer, errorLogger, "https://your_base_url.com") { }

        public Product GetProductById(int id)
        {
            RestRequest request = new RestRequest("products/{id}", Method.GET);
            request.AddUrlSegment("id", id.ToString());

            return Get<Product>(request);
        }

        public User GetUserById(int id)
        {
            RestRequest request = new RestRequest("users/{id}", Method.GET);
            request.AddUrlSegment("id", id.ToString());

            return GetFromCache<User>(request, "User" + id.ToString());
        }
    }
```