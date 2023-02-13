using System;
using System.Collections.Generic;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepDatabase(IApplicationBuilder builder)
        {
            Console.WriteLine("prepare Database");
            using (var applicationScope = builder.ApplicationServices.CreateScope())
            {
                try
                {
                    var grpcClient = applicationScope.ServiceProvider.GetService<IPlatformDataClient>();
                    SeedData(applicationScope.ServiceProvider.GetService<ICommandRepo>(),
                        grpcClient.ReturnAllPlatforms());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cannot prepare database: {ex.Message}");
                }
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> returnAllPlatforms)
        {
            foreach (var platform in returnAllPlatforms)
            {
                Console.WriteLine($"Try add platform {platform.Name}");
                if (!repo.ExternalPlatformExist(platform.Id))
                {
                    repo.CreatePlatform(platform);
                }
            }
        }
    }
}