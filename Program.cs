using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Roundabound {
    class Program {
        static void Main(string[] args) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("roundabound.cfg", optional : false, reloadOnChange : false)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var rotationSets = configuration.GetSection("sets").Get<IEnumerable<RotationSet>>();
            var logRotate = new LogRotate(rotationSets);
            Console.Out.WriteLine($"What up {rotationSets.Count()}");
            logRotate.Rotate();
        }
    }
}