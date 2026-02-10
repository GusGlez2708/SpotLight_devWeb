using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpotLight.API.Models
{
    [BsonIgnoreExtraElements]
    public class Evaluation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("projectId")]
        public string ProjectId { get; set; } = null!;

        [BsonElement("evaluatorId")]
        public string EvaluatorId { get; set; } = null!;

        [BsonElement("scores")]
        public ScoreBreakdown Scores { get; set; } = new ScoreBreakdown();

        [BsonElement("finalScore")]
        public double FinalScore { get; set; }

        [BsonElement("resena_texto")]
        public string ResenaTexto { get; set; } = string.Empty;

        [BsonElement("aiAnalysis")]
        public AiAnalysis AiAnalysis { get; set; } = new AiAnalysis();

        [BsonElement("createdAt")]
        public object? CreatedAt { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ScoreBreakdown
    {
        [BsonElement("innovacion")]
        public int Innovacion { get; set; }

        [BsonElement("funcionalidad")]
        public int Funcionalidad { get; set; }

        [BsonElement("diseno_ux")]
        public int DisenoUx { get; set; }

        [BsonElement("impacto")]
        public int Impacto { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class AiAnalysis
    {
        [BsonElement("puntuacion_factibilidad")]
        public double PuntuacionFactibilidad { get; set; }

        [BsonElement("fortalezas")]
        public List<string> Fortalezas { get; set; } = new List<string>();

        [BsonElement("nivel_riesgo")]
        public string NivelRiesgo { get; set; } = string.Empty;
    }
}