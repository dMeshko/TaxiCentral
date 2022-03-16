using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TaxiCentral.FunctionalTests
{
    public class BaseScenario
    {
        private readonly HttpClient _httpClient;

        public BaseScenario()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        ~BaseScenario()
        {
            _httpClient.Dispose();
        }

        public TestServer CreateServer()
        {
            return null;


        }
    }
}
