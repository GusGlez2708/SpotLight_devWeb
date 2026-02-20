using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpotLight.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace SpotLight.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public UsersController(IOptions<SpotLightDatabaseSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _users = mongoDatabase.GetCollection<User>(settings.Value.UsersCollectionName);
        }

        // POST: api/Users (Registro)
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            // 1. Verificar si ya existe el correo
            var existingUser = await _users
                .Find(u => u.CorreoInstitucional == newUser.CorreoInstitucional)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return Conflict(new { message = "Este correo ya está registrado." });
            }

            // 2. Encriptar contraseña (SHA256)
            newUser.Password = HashPassword(newUser.Password);

            // 3. Guardar en Mongo
            await _users.InsertOneAsync(newUser);

            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser);
        }

        // POST: api/Users/login (Inicio de Sesión)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Buscar usuario por correo
            var user = await _users
                .Find(u => u.CorreoInstitucional == request.correo_institucional)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado." });
            }

            // 2. Verificar contraseña
            var inputHash = HashPassword(request.password);

            if (user.Password != inputHash)
            {
                return Unauthorized(new { message = "Contraseña incorrecta." });
            }

            // 3. Login exitoso
            return Ok(new
            {
                message = "Login exitoso",
                userId = user.Id,
                nombre = user.NombreCompleto,
                rol = user.Rol
            });
        }

        // Método privado para encriptar contraseñas
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
