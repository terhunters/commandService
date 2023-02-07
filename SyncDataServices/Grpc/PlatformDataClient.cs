using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using WebApplication3;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Connect to grpc server {_configuration["PlatformGrpc"]}");
            var channel = GrpcChannel.ForAddress(_configuration["PlatformGrpc"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var result = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(result.Platform);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Some issue {e.Message}");
                return null;
            }
        }
    }
}