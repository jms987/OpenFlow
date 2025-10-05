using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace OpenFlowWebServer.Tests.E2E
{
    // This will be integreted in future 
    /*
    internal class TestAppHost : IDisposable
    {
        private WebApplication _app;
        private Task _runTask;
        public string Url { get; } = "https://localhost:7196";

        public async Task StartAsync()
        {
            Program.Main(new[] { $"--urls={Url}" });
            //_runTask = _app.RunAsync();

            await WaitForAppToStartAsync();
        }

        private async Task WaitForAppToStartAsync()
        {
            using var client = new HttpClient();
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var response = await client.GetAsync(Url);
                    if (response.IsSuccessStatusCode)
                        return;
                }
                catch { }

                await Task.Delay(500);
            }

            throw new Exception("App did not start in time.");
        }

        public void Dispose()
        {
            _app?.StopAsync().GetAwaiter().GetResult();
            _runTask?.GetAwaiter().GetResult();
        }
    }

    [TestFixture]
    public class SeleniumTests
    {
        private TestAppHost _host;
        private IWebDriver _driver;
        private Task _hostTask;
        private CancellationTokenSource _cts;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            // 1) tell ASP.NET Core we’re in “Test” environment:
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            // 2) create your host & a cancellation token you can use in teardown:
            _host = new TestAppHost();
            _cts = new CancellationTokenSource();

            // 3) fire off StartAsync in the background:
            _hostTask = Task.Run(() => _host.StartAsync(), _cts.Token);

            // 4) give it a moment to bind to ports, etc.
            //    (you can poll for readiness instead of a fixed delay if you like)
            await Task.Delay(500);
        }

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            _driver = new ChromeDriver(options);
        }

        [Test]
        public void HomePage_Should_Load()
        {
            _driver.Navigate().GoToUrl("https://localhost:7196");
            var a = _driver.Title;
            Assert.IsTrue(_driver.Title.Length > 0);
        }

        [Test]
        public void ProjectPage_Should_Load()
        {
            _driver.Navigate().GoToUrl("https://localhost:7196/projects");
            string bodyText = _driver.FindElement(By.TagName("body")).Text;
            var a = _driver.Title;
            Assert.IsTrue(_driver.Title.Length > 0);
        }


        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            if (_cts != null)
            {
                // signal the host to shut down
                _cts.Cancel();

                // wait for the StartAsync() task to end
                try
                {
                    await _hostTask;
                }
                catch (OperationCanceledException) {  expected  }

            }
            _hostTask.Dispose();
            _host?.Dispose();
        }
    }
            
        */
}
