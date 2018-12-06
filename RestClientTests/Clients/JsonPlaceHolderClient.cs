using RestClientTests.DataModels;
using RestSharp;
using RestSharp.Deserializers;

namespace RestClientTests.Clients
{
    class JsonPlaceHolderClient : BaseClient
    {
        public JsonPlaceHolderClient(ICacheService cache, IDeserializer deserializer, IErrorLogger errorLogger)
        : base(cache, deserializer, errorLogger, "https://jsonplaceholder.typicode.com") { }

        public Post GetPostById(int id)
        {
            RestRequest request = new RestRequest("posts/{id}", Method.GET);
            request.AddUrlSegment("id", id.ToString());

            return Get<Post>(request);
        }

        public User GetUserById(int id)
        {
            RestRequest request = new RestRequest("users/{id}", Method.GET);
            request.AddUrlSegment("id", id.ToString());

            return GetFromCache<User>(request, "User" + id.ToString());
        }
    }
}
