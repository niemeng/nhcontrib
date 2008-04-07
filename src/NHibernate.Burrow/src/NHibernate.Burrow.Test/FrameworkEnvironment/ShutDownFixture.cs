using NHibernate.Burrow.Exceptions;
using NUnit.Framework;

namespace NHibernate.Burrow.Test.FrameworkEnvironment {
    [TestFixture]
    public class ShutDownFixture {
        Facade f = new Facade();

        [TearDown]
        public void TearDown() {
            if(!f.BurrowEnvironment.IsRunning)
                f.BurrowEnvironment.Start();
        }

        [Test]
        public void CannotInitializeDomainTest() {
            f.BurrowEnvironment.ShutDown();
            try {
                f.InitializeDomain();
                Assert.Fail("Failed to throw FrameworkAlreadyShutDownException");
            }catch(FrameworkAlreadyShutDownException) {
                
            }

        }
    }
}