using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BadeePlatform.Data;
using System.Text.Json.Serialization;

namespace BadeePlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnityController : ControllerBase
    {
        private readonly BadeedbContext _db;

        public UnityController(BadeedbContext db)
        {
            _db = db;
        }

        public class LoginRequest
        {
            public string Code { get; set; }
        }

        public class LoginResponse
        {
            [JsonPropertyName("Success")]
            public bool Success { get; set; }

            [JsonPropertyName("ChildId")]
            public string ChildId { get; set; }

            [JsonPropertyName("Gender")]
            public string Gender { get; set; }

            [JsonPropertyName("Message")]
            public string Message { get; set; }
        }

        [HttpPost("CheckLoginCode")]
        public async Task<IActionResult> CheckLoginCode([FromBody] LoginRequest request)
        {
            var child = await _db.Children
                .FirstOrDefaultAsync(c => c.LoginCode == request.Code);

            if (child == null)
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    ChildId = "",
                    Gender = "",
                    Message = "Wrong code"
                });
            }

            return Ok(new LoginResponse
            {
                Success = true,
                ChildId = child.ChildId,
                Gender = child.Gender,
                Message = "Login successful"
            });
        }
    }
}