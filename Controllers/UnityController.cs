using LamiePlatform.DTOs.GameDTOs;
using LamiePlatform.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace LamiePlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnityController : ControllerBase
    {
        private readonly GameService _gameService;

        public UnityController(GameService gameService)
        {
            _gameService = gameService;
        }

        public class LoginRequest
        {
            public string Code { get; set; }
        }

        public class LoginResponse
        {
            [JsonPropertyName("Success")] public bool Success { get; set; }
            [JsonPropertyName("ChildId")] public string ChildId { get; set; }
            [JsonPropertyName("Gender")] public string Gender { get; set; }
            [JsonPropertyName("Message")] public string Message { get; set; }
        }
        [HttpPost("CheckLoginCode")]
        public async Task<IActionResult> CheckLoginCode([FromBody] LoginRequest request)
        {
            var result = await _gameService.CheckLoginCode(request.Code);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new LoginResponse
                {
                    Success = false,
                    ChildId = "",
                    Gender = "",
                    Message = result.Message
                });
            }

            var data = result.Data as dynamic;

            return Ok(new LoginResponse
            {
                Success = true,
                ChildId = data.ChildId,
                Gender = data.Gender,
                Message = result.Message
            });
        }

        [HttpGet("GetGameLevels")]
        public async Task<IActionResult> GetGameLevels()
        {
            var levels = await _gameService.GetGameLevels();
            return Ok(levels.Data);
        }

        [HttpPost("SaveGameResult")]
        public async Task<IActionResult> SaveGameResult([FromBody] SaveGameResultRequest request)
        {
            try
            {
                var result = await _gameService.SaveGameResult(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetChildProfile/{childId}")]
        public async Task<IActionResult> GetChildProfile(string childId)
        {
            try
            {
                var result = await _gameService.GetChildProfileAsync(childId);

                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, new { Message = result.Message });
                }

                return Ok(result.Data);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Message = "حدث خطأ غير متوقع في السيرفر، يرجى المحاولة مرة أخرى"
                });
            }
        }
    }

}