using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;
using Service.Controllers;
using Service.Infrastructure;

namespace Service.Tests.Controllers
{
    public class ValuesControllerTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            storage = fixture.Freeze<IStorage>();
            sut = fixture.Build<ValuesController>().OmitAutoProperties().Create();
        }

        private ValuesController sut;
        private IStorage storage;

        [Test]
        public void Delete_KnownId_ShouldNotThrow()
        {
            var id = fixture.Create<string>();
            storage.Stub(s => s.Delete(id)).Return(true);

            Assert.DoesNotThrow(() => sut.Delete(id));
        }

        [Test]
        public void Delete_UnknownId_ShouldThrow()
        {
            var id = fixture.Create<string>();
            storage.Stub(s => s.Delete(id)).Return(false);

            Assert.Throws<HttpResponseException>(() => sut.Delete(id));
        }

        [Test]
        public void GetAll_Always_ShouldReturnValuesFromRepository()
        {
            IEnumerable<ValueWithId> result = fixture.CreateMany<ValueWithId>();
            storage.Stub(s => s.GetAll()).Return(result);

            ValueWithId[] actual = sut.Get().ToArray();

            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void Get_KnownId_ShouldReturnValueFromRepository()
        {
            var id = fixture.Create<string>();
            var result = fixture.Create<Value>();
            storage.Stub(s => s.Get(id)).Return(result);

            Value actual = sut.Get(id);

            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void Get_UnknownId_ShouldThrow()
        {
            var id = fixture.Create<string>();
            storage.Stub(s => s.Get(id)).Return(null);

            Assert.Throws<HttpResponseException>(() => sut.Get(id));
        }

        [Test]
        public void Put_Always_ShouldUpdate()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();

            sut.Put(id, value);

            storage.AssertWasCalled(s => s.Set(id, value));
            Assert.Throws<HttpResponseException>(() => sut.Delete(id));
        }
    }
}