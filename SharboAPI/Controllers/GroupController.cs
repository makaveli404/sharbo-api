using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController(IGroupService groupService) : ControllerBase
{
	[HttpGet("{id:guid}")]
	public async Task<ActionResult> GetGroup(Guid id, CancellationToken cancellationToken)
	{
		var group = await groupService.GetById(id, cancellationToken);

		if (group is null)
		{
			return NotFound();
		}

		return Ok(group);
	}

	[HttpPost("create")]
	public async Task<ActionResult> CreateGroup([FromBody] GroupDto group, CancellationToken cancellationToken)
	{
		await groupService.AddAsync(group, cancellationToken);
		return Created();
	}
}
