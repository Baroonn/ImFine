using ImFine.Server.Contracts;
using ImFine.Server.Models;
using ImFine.Server.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImFine.Server.Controllers
{
    [Route("api/groups")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly ICosmosService _cosmosService;
        private readonly string claim = "nickname";
        public GroupController(ICosmosService service)
        {
            _cosmosService = service;
        }
        // GET: api/<GroupController>
        [HttpGet]
        public async Task<IEnumerable<GroupReadDto>> GetGroups()
        {
            return await _cosmosService.GetGroupsByUsernameAndMembers(User.Claims.FirstOrDefault(c => c.Type == claim)?.Value);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<GroupSearchDto>> SearchGroups([FromQuery] string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            return await _cosmosService.GetGroupsByGroupName(searchTerm, User.Claims.FirstOrDefault(c => c.Type == claim)?.Value);
        }

        // GET api/<GroupController>/5
        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            name = name.ToLower();
            var group = await _cosmosService.GetGroupAsync(name);
            if (group == null)
            {
                throw new GroupNotFoundException(name);
            }
            return Ok(group);
        }

        // POST api/<GroupController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Group group)
        {
            if (group is null)
            {
                throw new GroupNullException();
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            group.name = group.name.ToLower();
            var exists = (await _cosmosService.GetGroupAsync(group.name)) != null;
            if (exists)
            {
                throw new GroupExistsException();
            }

            group.owner = User.Claims.FirstOrDefault(c => c.Type == claim)?.Value;
            group.createdAt = DateTime.UtcNow;
            group.updatedAt = DateTime.UtcNow;

            await _cosmosService.CreateGroupAsync(group);
            return CreatedAtAction(nameof(Post), group);
        }

        // PUT api/<GroupController>/5
        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody] Group group)
        {
            name = name.ToLower();
            var eGroup = await _cosmosService.GetGroupAsync(name);
            if (group is null)
            {
                throw new GroupNullException();
            }
            if (eGroup == null)
            {
                throw new GroupNotFoundException(name);
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            if (eGroup.owner != User.Claims.FirstOrDefault(c => c.Type == claim)?.Value)
            {
                throw new GroupUnauthorizedException();
            }

            eGroup.intervalInMinutes = group.intervalInMinutes != 0 ? group.intervalInMinutes : 1;
            eGroup.updatedAt = DateTime.UtcNow;
            await _cosmosService.UpdateGroupAsync(eGroup);
            return NoContent();
        }

        // DELETE api/<GroupController>/5
        [HttpDelete("{name}")]
        public async Task<ActionResult> Delete(string name)
        {
            name = name.ToLower();
            var group = await _cosmosService.GetGroupAsync(name);
            if (group == null)
            {
                throw new GroupNotFoundException(name);
            }
            if (group.owner != User.Claims.FirstOrDefault(c => c.Type == claim)?.Value)
            {
                throw new GroupUnauthorizedException();
            }
            else
            {
                await _cosmosService.DeleteGroupAsync(group);
                return NoContent();
            }
        }

        [HttpGet("{name}/join")]
        public async Task<ActionResult> JoinGroup(string name)
        {
            name = name.ToLower();
            var group = await _cosmosService.GetGroupAsync(name);
            if (group == null)
            {
                throw new GroupNotFoundException(name);
            }
            if (group.owner == User.Claims.FirstOrDefault(c => c.Type == claim)?.Value)
            {
                throw new GroupUnauthorizedException();
            }
            if (group.members.Contains($"{User.Claims.FirstOrDefault(c => c.Type == claim)?.Value}|"))
            {
                return BadRequest("You are already in the group.");
            }
            group.members += $"{User.Claims.FirstOrDefault(c => c.Type == claim)?.Value}|";
            group.updatedAt = DateTime.UtcNow;
            await _cosmosService.UpdateGroupAsync(group);
            return NoContent();
        }

        [HttpGet("{name}/leave")]
        public async Task<ActionResult> LeaveGroup(string name)
        {
            name = name.ToLower();
            var group = await _cosmosService.GetGroupAsync(name);
            if (group == null)
            {
                throw new GroupNotFoundException(name);
            }
            if (group.owner == User.Claims.FirstOrDefault(c => c.Type == claim)?.Value)
            {
                throw new GroupUnauthorizedException();
            }
            if (!group.members.Contains($"{User.Claims.FirstOrDefault(c => c.Type == claim)?.Value}|"))
            {
                return BadRequest("You already left the group");
            }
            group.members = group.members.Replace($"{User.Claims.FirstOrDefault(c => c.Type == claim)?.Value}|", "");
            group.updatedAt = DateTime.UtcNow;
            await _cosmosService.UpdateGroupAsync(group);
            return NoContent();
        }
    }
}
