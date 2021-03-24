using System;
using Microsoft.Extensions.Configuration;

namespace Basket.Library
{
    public static class ConfigurationHelper
    {
        #region Private Fields

        private static IConfigurationRoot _configuration;

        #endregion Private Fields

        #region Public Properties

        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {

                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    string fileName = environment == "Development" ? "appsettings.Development.json" : "appsettings.json";
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile(fileName, true, true);
                    _configuration = builder.Build();
                }

                return _configuration;
            }
        }

        #endregion Public Properties
    }
}