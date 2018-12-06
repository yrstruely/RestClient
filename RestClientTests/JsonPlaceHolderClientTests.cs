using NUnit.Framework;
using RestClientTests.DataModels;
using RestClientTests.Clients;

namespace Tests
{
    public class JsonPlaceHolderTests
    {
        private JsonPlaceHolderClient jsonPlaceHolderClient;
        private InMemoryCache cache;
        private JsonSerializer jsonSerializer;
        private ErrorLogger errorLogger;

        [SetUp]
        public void Setup()
        {
            cache = new InMemoryCache();
            jsonSerializer = new JsonSerializer();
            errorLogger = new ErrorLogger();

            jsonPlaceHolderClient = new JsonPlaceHolderClient(cache, jsonSerializer, errorLogger);
        }

        [Test]
        public void GetPostById_Should_ReturnValidPost()
        {
            Post post = jsonPlaceHolderClient.GetPostById(1);
            Assert.AreEqual(1, post.UserId);
            Assert.AreEqual(1, post.Id);
            Assert.True(post.Title.Contains("sunt"));
            Assert.True(post.Body.Contains("expedita "));
        }

        [Test]
        public void GetUserById_Should_ReturnValidUser()
        {
            User user = jsonPlaceHolderClient.GetUserById(1);
            Assert.AreEqual(1, user.Id);
            Assert.True(user.Name.Contains("Leanne"));
        }
    }
}