using System.Linq;
using Domain;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Tests.Infrastructure
{
    [TestFixture]
    public class StorageTests
    {
        private Fixture fixture;
        private Storage sut;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoRhinoMockCustomization());
            sut = fixture.Create<Storage>();
        }

        [Test]
        public void Delete_KnownId_ShouldRemoveIt()
        {
            var id = fixture.Create<string>();
            sut.Set(id, fixture.Create<Value>());

            var actual = sut.Delete(id);

            Assert.That(actual, Is.True);
            var value = sut.Get(id);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void Delete_UnknownId_ShouldReturnFalse()
        {
            var actual = sut.Delete(fixture.Create<string>());
            Assert.That(actual, Is.False);
        }

        [Test]
        public void GetAll_EmptyStorage_ShouldReturnEmptyList()
        {
            var actual = sut.GetAll();
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetAll_NonEmptyStorage_ShouldReturnAllValues()
        {
            var id1 = fixture.Create<string>();
            var value1 = fixture.Create<Value>();
            sut.Set(id1, value1);
            var id2 = fixture.Create<string>();
            var value2 = fixture.Create<Value>();
            sut.Set(id2, value2);

            var actual = sut.GetAll().ToArray();

            Assert.That(actual, Has.Some.Matches<ValueWithId>(m => m.Id == id1 && m.Value == value1));
            Assert.That(actual, Has.Some.Matches<ValueWithId>(m => m.Id == id2 && m.Value == value2));
        }

        [Test]
        public void Get_KnownId_ShouldReturnValue()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();
            sut.Set(id, value);

            var actual = sut.Get(id);

            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void Get_UnknownId_ShouldReturnNull()
        {
            var actual = sut.Get(fixture.Create<string>());
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Update_Always_ShouldOverwrite()
        {
            var id = fixture.Create<string>();
            var oldValue = fixture.Create<Value>();
            sut.Set(id, oldValue);
            var newValue = fixture.Create<Value>();

            sut.Set(id, newValue);

            var actual = sut.Get(id);
            Assert.That(actual, Is.EqualTo(newValue));
        }
    }
}