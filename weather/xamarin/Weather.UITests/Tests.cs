using System;
using System.Threading;
using NUnit.Framework;
using Weather.UITests.Pages;
using Xamarin.UITest;

namespace Weather.UITests
{
    public class Tests : BaseTestFixture
    {
        public Tests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void LaunchApp()
        {
            //Test app launch
        }

        [Test]
        public void SetLocationRedmond()
        {
            //Set location before testing the page
            app.Device.SetLocation(latitude: 47.6739, longitude: -122.1215);
            Thread.Sleep(TimeSpan.FromMinutes(1)); //Wait for location to be set on the phone
            new MainPage().SetLocationToRedmond();
        }

        [Test]
        public void Repl()
        {
            if (TestEnvironment.IsTestCloud)
            {
                Assert.Ignore("Local only");
            }

            app.Repl();
        }
    }
}
