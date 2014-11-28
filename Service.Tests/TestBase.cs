using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;

namespace Service.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        [SetUp]
        public virtual void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoRhinoMockCustomization());
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        protected Fixture fixture;
    }
}