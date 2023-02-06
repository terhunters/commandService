using System;
using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(
            IServiceScopeFactory scopeFactory,
            IMapper mapper
        )
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    Console.WriteLine("Start Platform Published Action");
                    AddPlatform(message);
                    break;
                case EventType.Undetermined:
                    Console.WriteLine("Start Undetermined Action");
                    break;
            }
        }

        private void AddPlatform(string platformPublishMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);

                    if (!repository.ExternalPlatformExist(plat.ExternalId))
                    {
                        repository.CreatePlatform(plat);
                        repository.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("--> ExternalId is exist");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--> Could not add platform after receive message from message bus");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Get notification");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine($"--> Determine: {eventType.Event}");
                    return EventType.PlatformPublished;
                default: return EventType.Undetermined;
            }
        }
    }

    internal enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}