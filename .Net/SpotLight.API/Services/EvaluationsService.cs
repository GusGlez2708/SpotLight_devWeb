using SpotLight.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SpotLight.API.Services
{
    public class EvaluationsService
    {
        private readonly IMongoCollection<Evaluation> _evaluationsCollection;

        public EvaluationsService(IOptions<SpotLightDatabaseSettings> spotLightDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                spotLightDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                spotLightDatabaseSettings.Value.DatabaseName);

            _evaluationsCollection = mongoDatabase.GetCollection<Evaluation>(
                spotLightDatabaseSettings.Value.EvaluationsCollectionName);
        }

        public async Task<List<Evaluation>> GetAsync() =>
            await _evaluationsCollection.Find(_ => true).ToListAsync();

        public async Task<Evaluation?> GetAsync(string id) =>
            await _evaluationsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        // Obtener todas las evaluaciones de un proyecto específico
        public async Task<List<Evaluation>> GetByProjectIdAsync(string projectId) =>
            await _evaluationsCollection.Find(x => x.ProjectId == projectId).ToListAsync();

        // Obtener la cantidad de evaluaciones hechas por un supervisor
        public async Task<long> CountBySupervisorIdAsync(string supervisorId) =>
            await _evaluationsCollection.CountDocumentsAsync(x => x.SupervisorId == supervisorId);

        public async Task CreateAsync(Evaluation newEvaluation) =>
            await _evaluationsCollection.InsertOneAsync(newEvaluation);
    }
}