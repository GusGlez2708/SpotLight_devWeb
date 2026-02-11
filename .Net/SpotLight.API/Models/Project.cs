using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpotLight.API.Models
{
    [BsonIgnoreExtraElements]
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("equipo_numero")]
        public int EquipoNumero { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("category")]
        public string Category { get; set; } = null!;

        [BsonElement("videoUrl")]
        public string VideoUrl { get; set; } = null!;

        [BsonElement("members")]
        public List<string> Members { get; set; } = new List<string>();

        [BsonElement("stats")]
        public ProjectStats Stats { get; set; } = new ProjectStats();

        [BsonElement("status")]
        public string Status { get; set; } = "activo";
        
        [BsonElement("createdAt")]
        public string? CreatedAt { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ProjectStats
    {
        [BsonElement("puntuacion_factibilidad")]
        public double PuntuacionFactibilidad { get; set; } = 0;

        [BsonElement("totalEvaluaciones")]
        public int TotalEvaluaciones { get; set; } = 0;
    }
}