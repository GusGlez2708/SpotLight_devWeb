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
        private readonly EvaluationsService _evaluationsService;

        public ProjectsController(ProjectsService projectsService, EvaluationsService evaluationsService)
        {
            _projectsService = projectsService;
            _evaluationsService = evaluationsService;
        }

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

        [HttpPost("recalculate-stats")]
        public async Task<IActionResult> RecalculateStats()
        {
            var projects = await _projectsService.GetAsync();
            int updatedCount = 0;
            var detailedLogs = new List<string>();

            foreach (var p in projects)
            {
                if (p.Id == null) continue;
                
                // Debug log
                Console.WriteLine($"[Sync] Procesando proyecto: {p.Id} - {p.Title}");
                detailedLogs.Add($"Procesando proyecto: {p.Id} - {p.Title}");

                var evaluations = await _evaluationsService.GetByProjectIdAsync(p.Id);
                
                Console.WriteLine($"[Sync] Evaluaciones encontradas: {evaluations.Count}");
                detailedLogs.Add($"Evaluaciones encontradas: {evaluations.Count}");

                if (evaluations.Any())
                {
                    double avg = evaluations.Average(e => e.AiAnalysis.PuntuacionFactibilidad);
                    Console.WriteLine($"[Sync] Nuevo promedio: {avg}");
                    detailedLogs.Add($"Nuevo promedio: {avg}");
                    
                    p.Stats.PuntuacionFactibilidad = Math.Round(avg, 1);
                    p.Stats.TotalEvaluaciones = evaluations.Count;
                    
                    await _projectsService.UpdateAsync(p.Id, p);
                    updatedCount++;
                }
            }
            return Ok(new { message = $"Recalculadas las estadísticas de {updatedCount} proyectos", logs = detailedLogs });
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