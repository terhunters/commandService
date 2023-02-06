using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandRepo _repository;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine("--> GetCommands For Platform from Command Service");
            if (!_repository.PlatformExist(platformId)) return NotFound();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_repository.GetCommandsForPlatform(platformId)));
        }

        [HttpGet("{commandId}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int platformId, int commandId)
        {
            Console.WriteLine("--> GetCommandById For Platform from Command Service");
            if (!_repository.PlatformExist(platformId)) return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(_repository.GetCommand(platformId, commandId)));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine("--> CreateCommand For Platform from Command Service");
            if (!_repository.PlatformExist(platformId)) return NotFound();

            var command = _mapper.Map<Command>(commandDto);
            _repository.CreateCommand(platformId, command);
            if (_repository.SaveChanges())
            {
                var result = _mapper.Map<CommandReadDto>(command);
                return CreatedAtRoute(nameof(GetCommandById), new { platformId, commandId = result.Id },
                    result);
            }

            return StatusCode(500);
        }
    }
}