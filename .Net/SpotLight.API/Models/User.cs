using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpotLight.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre_completo")]
        public string NombreCompleto { get; set; } = null!;

        [BsonElement("correo_institucional")]
        public string CorreoInstitucional { get; set; } = null!;

        [BsonElement("password")]
        public string Password { get; set; } = null!;

        [BsonElement("rol")]
        public string Rol { get; set; } = "evaluador";

        [BsonElement("area_especialidad")]
        public string AreaEspecialidad { get; set; } = null!;

        [BsonElement("status_verificacion")]
        public bool StatusVerificacion { get; set; } = true;
    }

    // Clase auxiliar para recibir solo usuario y contraseña en el Login
    public class LoginRequest
    {
        public string correo_institucional { get; set; } = null!;
        public string password { get; set; } = null!;
    }
}
