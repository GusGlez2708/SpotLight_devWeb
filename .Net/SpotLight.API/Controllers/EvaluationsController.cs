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

        public EvaluationsController(EvaluationsService evaluationsService) =>
            _evaluationsService = evaluationsService;

        [HttpGet]
        public async Task<List<Evaluation>> Get() =>
            await _evaluationsService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Evaluation newEvaluation)
        {
            // Calculamos el promedio final automáticamente antes de guardar
            var s = newEvaluation.Scores;
            newEvaluation.FinalScore = (s.Innovation + s.Functionality + s.Ui + s.Impact) / 4.0;
            
            await _evaluationsService.CreateAsync(newEvaluation);

            return CreatedAtAction(nameof(Get), new { id = newEvaluation.Id }, newEvaluation);
        }
    }
}