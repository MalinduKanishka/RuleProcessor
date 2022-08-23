using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessorTest.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class TestHelper
    {
        public static IConfiguration GetIConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
