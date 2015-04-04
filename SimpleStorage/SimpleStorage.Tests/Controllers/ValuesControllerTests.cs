using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SimpleStorage.Controllers;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class ValuesControllerTests
    {
        private StateRepository stateRepository;
        private IStorage storage;
        private ValuesController sut;

        [SetUp]
        public void SetUp()
        {
            storage = new Storage(new OperationLog(), new ValueComparer());
            stateRepository = new StateRepository();
            sut = new ValuesController(storage, stateRepository, new Configuration(new Topology(Enumerable.Empty<int>())));
        }

        [Test]
        public void GetAll_Stopped_ShouldThrow()
        {
            storage.Set("id", new Value());
            stateRepository.SetState(State.Stopped);

            Assert.Throws(CheckHttpException(HttpStatusCode.InternalServerError), () => sut.Get());
        }

        [Test]
        public void GetAll_Started_ShouldReturnValuesFromRepository()
        {
            storage.Set("id", new Value());

            var actual = sut.Get().ToArray();

            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0].Id, Is.EqualTo("id"));
        }

        [Test]
        public void Get_Stopped_ShouldThrow()
        {
            const string id = "id";
            storage.Set("id", new Value());
            stateRepository.SetState(State.Stopped);

            Assert.Throws(CheckHttpException(HttpStatusCode.InternalServerError), () => sut.Get(id));
        }

        private static EqualConstraint CheckHttpException(HttpStatusCode code)
        {
            return Is.TypeOf<HttpResponseException>().And.Property("Response").Property("StatusCode").EqualTo(code);
        }

        [Test]
        public void Get_StartedAndKnownId_ShouldReturnValueFromRepository()
        {
            const string id = "id";
            var value = new Value {Content = "content"};
            storage.Set("id", value);

            var actual = sut.Get(id);

            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void Get_StartedAndUnknownId_ShouldThrow()
        {
            Assert.Throws(CheckHttpException(HttpStatusCode.NotFound), () => sut.Get("unknownId"));
        }

        [Test]
        public void Put_Stopped_ShouldThrow()
        {
            stateRepository.SetState(State.Stopped);

            Assert.Throws(CheckHttpException(HttpStatusCode.InternalServerError), () => sut.Put("id", new Value()));
        }

        [Test]
        public void Put_Started_ShouldSaveToStorage()
        {
            const string id = "id";
            var value = new Value {Content = "content"};

            sut.Put(id, value);

            var actual = storage.Get(id);
            Assert.That(actual, Is.EqualTo(value));
        }
    }
}