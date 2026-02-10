using SpotLight.API.Models;
using SpotLight.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace SpotLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectsService _projectsService;

        public ProjectsController(ProjectsService projectsService) =>
            _projectsService = projectsService;

        [HttpGet]
        public async Task<List<Project>> Get() =>
            await _projectsService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Project>> Get(string id)
        {
            var project = await _projectsService.GetAsync(id);

            if (project is null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Project newProject)
        {
            newProject.Status = "activo";
            await _projectsService.CreateAsync(newProject);

            return CreatedAtAction(nameof(Get), new { id = newProject.Id }, newProject);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Project updatedProject)
        {
            var project = await _projectsService.GetAsync(id);

            if (project is null)
            {
                return NotFound();
            }

            updatedProject.Id = project.Id;
            await _projectsService.UpdateAsync(id, updatedProject);

            return NoContent();
        }

        [HttpPatch("{id:length(24)}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] StatusUpdateRequest request)
        {
            var project = await _projectsService.GetAsync(id);

            if (project is null)
            {
                return NotFound();
            }

            if (request.Status != "activo" && request.Status != "desactivado")
            {
                return BadRequest("El status debe ser 'activo' o 'desactivado'.");
            }

            project.Status = request.Status;
            await _projectsService.UpdateAsync(id, project);

            return Ok(project);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var project = await _projectsService.GetAsync(id);

            if (project is null)
            {
                return NotFound();
            }

            await _projectsService.RemoveAsync(id);

            return NoContent();
        }
    }

    public class StatusUpdateRequest
    {
        public string Status { get; set; } = null!;
    }
}