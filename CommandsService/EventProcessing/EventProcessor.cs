﻿using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using System.Text.Json;

namespace CommandsService.EventProcessing
{
	public class EventProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IMapper _mapper;

		public EventProcessor(
			IServiceScopeFactory scopeFactory,
			IMapper mapper)
        {
			_scopeFactory = scopeFactory;
			_mapper = mapper;
        }
        public void ProcessEvent(string message)
		{
			var eventType = DetermineEvent(message);

			switch(eventType)
			{
				case EventType.PlatformPublished:
					AddPlatform(message);
					break;
				default: 
					break;
			}
		}

		private EventType DetermineEvent(string notificationMessage)
		{
            Console.WriteLine("--> Determining Event");

			var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

			switch (eventType.Event)
			{
				case "Platform_Published":
                    Console.WriteLine("Platform Published Event.Detected");
					return EventType.PlatformPublished;
				default:
                    Console.WriteLine("--> Could not determine the event type");
					return EventType.Undetermine;
            }
        }

		private void AddPlatform(string platformPublishedMessage)
		{
			using(var scope = _scopeFactory.CreateScope())
			{
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

				var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

				try
				{
					var plat = _mapper.Map<Platform>(platformPublishedDto);
					if (!repo.ExternalPlatformExists(plat.ExternalID))
					{
						repo.CreatePlatform(plat);
						repo.SaveChanges();
						Console.WriteLine("--> Platform was added...");
					}
					else
					{
                        Console.WriteLine("--> Platform already exsists...");
                    }
				}
				catch (Exception ex)
				{
                    Console.WriteLine($"--> Couldn't add Platfrom to DB {ex.Message}");
                    throw;
				}
			}
		}
	}

	enum EventType
	{
		PlatformPublished,
		Undetermine
	}
}
