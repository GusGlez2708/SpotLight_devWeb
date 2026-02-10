namespace SpotLight.API.Models
{
    public class SpotLightDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProjectsCollectionName { get; set; } = null!;
        public string EvaluationsCollectionName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
        public string RankingCollectionName { get; set; } = null!;
    }
}