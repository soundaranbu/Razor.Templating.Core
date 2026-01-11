using Microsoft.Extensions.Configuration;
using System;

namespace ExampleRazorTemplatesLibrary.Services
{
    public class ExampleConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ExampleConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConfigValue()
        {
            return _configuration["SampleConfig"];
        }
    }
}
