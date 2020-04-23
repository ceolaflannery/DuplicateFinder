using System;
using DuplicateFinder.FileProcessing;
using Microsoft.Extensions.DependencyInjection;

namespace DuplicateFinder
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            Console.Write(@"Enter path to the folder to be processed (To use 'C:\Temp' as default, just hit Enter): ");
            var directoryToBeProcessed = Console.ReadLine();

            RegisterServices();

            var service = _serviceProvider.GetService<IFileMultiplesService>();

            directoryToBeProcessed = string.IsNullOrWhiteSpace(directoryToBeProcessed) ? "C:\temp" : directoryToBeProcessed;
            var output = service.GroupFilesByMultiples(directoryToBeProcessed);
            Console.Write(output);

            DisposeServices();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            collection.AddScoped<IFileMultiplesService, FileMultiplesService>();
            collection.AddScoped<IFileProcessor, FileProcessor>();
            collection.AddScoped<IIOHelper, IOHelper>();
            _serviceProvider = collection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
