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
            try
            {
                // Validation: Unique Team Number
                var existingTeam = await _projectsService.GetByTeamNumberAsync(newProject.EquipoNumero);
                if (existingTeam != null)
                {
                    return Conflict(new { message = $"El número de equipo {newProject.EquipoNumero} ya está registrado." });
                }

                // Validation: Unique Title
                var existingTitle = await _projectsService.GetByTitleAsync(newProject.Title);
                if (existingTitle != null)
                {
                    return Conflict(new { message = $"El nombre del proyecto '{newProject.Title}' ya existe." });
                }

                newProject.Status = "activo";
                // Ensure CreatedAt is a string for MongoDB validation schema
                if (string.IsNullOrEmpty(newProject.CreatedAt))
                {
                    newProject.CreatedAt = DateTime.UtcNow.ToString("o"); 
                }

                await _projectsService.CreateAsync(newProject);

                return CreatedAtAction(nameof(Get), new { id = newProject.Id }, newProject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Post: {ex}");
                return StatusCode(500, $"Error interno al crear proyecto: {ex.Message} -> {ex.InnerException?.Message}");
            }
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
            try
            {
                var project = await _projectsService.GetAsync(id);

                if (project is null)
                {
                    return NotFound();
                }

                // Validation: Unique Team Number (excluding current project)
                var existingTeam = await _projectsService.GetByTeamNumberAsync(updatedProject.EquipoNumero);
                if (existingTeam != null && existingTeam.Id != id)
                {
                    return Conflict(new { message = $"El número de equipo {updatedProject.EquipoNumero} ya está registrado en otro proyecto." });
                }

                // Validation: Unique Title (excluding current project)
                var existingTitle = await _projectsService.GetByTitleAsync(updatedProject.Title);
                if (existingTitle != null && existingTitle.Id != id)
                {
                    return Conflict(new { message = $"El nombre del proyecto '{updatedProject.Title}' ya existe." });
                }

                updatedProject.Id = project.Id; // Ensure ID consistency
                
                // Keep existing stats/members/createdAt if not provided
                if (updatedProject.Stats == null) updatedProject.Stats = project.Stats;
                if (updatedProject.Members == null) updatedProject.Members = project.Members;
                if (string.IsNullOrEmpty(updatedProject.CreatedAt)) updatedProject.CreatedAt = project.CreatedAt;

                await _projectsService.UpdateAsync(id, updatedProject);

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Update: {ex}");
                return StatusCode(500, $"Error interno al actualizar proyecto: {ex.Message} -> {ex.InnerException?.Message}");
            }
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