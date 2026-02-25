using SpotLight.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SpotLight.API.Services
{
    public class ProjectsService
    {
        private readonly IMongoCollection<Project> _projectsCollection;

        public ProjectsService(IOptions<SpotLightDatabaseSettings> spotLightDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                spotLightDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                spotLightDatabaseSettings.Value.DatabaseName);

            _projectsCollection = mongoDatabase.GetCollection<Project>(
                spotLightDatabaseSettings.Value.ProjectsCollectionName);
        }

        public async Task<List<Project>> GetAsync() =>
                        await _projectsCollection.Find(_ => true).ToListAsync();


        public async Task<Project?> GetAsync(string id) =>
            await _projectsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Project newProject) =>
            await _projectsCollection.InsertOneAsync(newProject);

        public async Task UpdateAsync(string id, Project updatedProject) =>
            await _projectsCollection.ReplaceOneAsync(x => x.Id == id, updatedProject);

        public async Task RemoveAsync(string id) =>
            await _projectsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<Project?> GetByTeamNumberAsync(int teamNumber) =>
            await _projectsCollection.Find(paragraph => paragraph.EquipoNumero == teamNumber).FirstOrDefaultAsync();

        public async Task<Project?> GetByTitleAsync(string title) =>
            await _projectsCollection.Find(paragraph => paragraph.Title.ToLower() == title.ToLower()).FirstOrDefaultAsync();
    }
}