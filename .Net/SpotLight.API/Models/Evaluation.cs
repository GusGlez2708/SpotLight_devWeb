using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpotLight.API.Models
{
    public class Evaluation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("projectId")]
        public string ProjectId { get; set; } = null!;

        [BsonElement("scores")]
        public ScoreBreakdown Scores { get; set; } = new ScoreBreakdown();

        [BsonElement("feedback")]
        public string Feedback { get; set; } = string.Empty;

        [BsonElement("finalScore")]
        public double FinalScore { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ScoreBreakdown
    {
        public int Innovation { get; set; }
        public int Functionality { get; set; }
        public int Ui { get; set; }
        public int Impact { get; set; }
    }
}