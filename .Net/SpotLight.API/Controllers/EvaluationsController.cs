using SpotLight.API.Models;
using SpotLight.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace SpotLight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly EvaluationsService _evaluationsService;
        private readonly ProjectsService _projectsService;

        public EvaluationsController(
            EvaluationsService evaluationsService,
            ProjectsService projectsService)
        {
            _evaluationsService = evaluationsService;
            _projectsService = projectsService;
        }

        [HttpGet]
        public async Task<List<Evaluation>> Get() =>
            await _evaluationsService.GetAsync();

        // GET por projectId — obtener evaluaciones de un proyecto
        [HttpGet("project/{projectId:length(24)}")]
        public async Task<List<Evaluation>> GetByProject(string projectId) =>
            await _evaluationsService.GetByProjectIdAsync(projectId);

        [HttpPost]
        public async Task<IActionResult> Post(Evaluation newEvaluation)
        {
            // 1. Calcular el finalScore como suma de scores
            var s = newEvaluation.Scores;
            newEvaluation.FinalScore = s.Innovacion + s.Funcionalidad + s.DisenoUx + s.Impacto;
            
            // 2. Guardar la evaluación
            await _evaluationsService.CreateAsync(newEvaluation);

            // 3. Recalcular la factibilidad promedio del proyecto
            await RecalcularFactibilidadProyecto(newEvaluation.ProjectId);

            return CreatedAtAction(nameof(Get), new { id = newEvaluation.Id }, newEvaluation);
        }

        /// <summary>
        /// Obtiene todas las evaluaciones del proyecto, calcula el promedio
        /// de puntuacion_factibilidad y actualiza el proyecto.
        /// </summary>
        private async Task RecalcularFactibilidadProyecto(string projectId)
        {
            // Obtener el proyecto
            var proyecto = await _projectsService.GetAsync(projectId);
            if (proyecto == null) return;

            // Obtener todas las evaluaciones del proyecto
            var evaluaciones = await _evaluationsService.GetByProjectIdAsync(projectId);

            if (evaluaciones.Count == 0) return;

            // Calcular promedio de puntuacion_factibilidad
            double sumaFactibilidad = evaluaciones.Sum(e => e.AiAnalysis.PuntuacionFactibilidad);
            double promedio = sumaFactibilidad / evaluaciones.Count;

            // Actualizar el proyecto con el promedio
            proyecto.Stats.PuntuacionFactibilidad = Math.Round(promedio, 1);
            proyecto.Stats.TotalEvaluaciones = evaluaciones.Count;

            await _projectsService.UpdateAsync(projectId, proyecto);
        }
    }
}