using AutoMapper;
using DevOps.Api.Models;
using DevOps.AppLogic;
using DevOps.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;

    public TeamsController(ITeamService teamService, ITeamRepository teamRepository, IMapper mapper)
    {
        _teamService = teamService;
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _teamRepository.GetAllAsync();
       
        var teamDtos = teams.Select(team => _mapper.Map<TeamDetailModel>(team)).ToList();

        return Ok(teamDtos);
    }

    [HttpPost("{id}/assemble")]
    public async Task<IActionResult> AssembleTeam(Guid id, TeamAssembleInputModel model)
    {
        try
        {
            var team = await _teamRepository.GetByIdAsync(id);

            if (team == null)
            {
                return new NotFoundResult();
            }

            await _teamService.AssembleDevelopersAsyncFor(team, model.RequiredNumberOfDevelopers);

            return new OkResult();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}
