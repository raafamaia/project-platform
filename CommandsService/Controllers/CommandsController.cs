using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandRepo _repository;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"📥  --> Getting commands for platform id: {platformId}");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int platformId, int commandId)
        {
            Console.WriteLine($"📥  --> Getting command {commandId} for platform id {platformId}");
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound("Platform not found!");
            }

            var command = _repository.GetCommand(platformId, commandId);

            if (command is null)
            {
                return NotFound("Command not found!");
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"📥  --> Creating command for platform id {platformId}");
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound("Platform not found!");
            }

            var command = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandById),
                new {platformId = commandReadDto.PlatformId, commandId = commandReadDto.Id}, commandReadDto);
        }
    }
}