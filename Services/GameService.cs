using LamiePlatform.Data;
using LamiePlatform.Models;
using LamiePlatform.DTOs.GameDTOs;
using Microsoft.EntityFrameworkCore;

namespace LamiePlatform.Services
{
    public class GameService
    {
        private readonly LamieDbContext _db;

        public GameService(LamieDbContext db)
        {
            _db = db;
        }

        public class ServiceResult<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public int StatusCode { get; set; }
            public T Data { get; set; }
        }

        public async Task<ServiceResult<object>> CheckLoginCode(string code)
        {
            var child = await _db.Children
                .FirstOrDefaultAsync(c => c.LoginCode == code);

            if (child == null)
            {
                return new ServiceResult<object>
                {
                    Success = false,
                    Message = "Wrong code",
                    StatusCode = 200, 
                    Data = new { ChildId = "", Gender = "" }
                };
            }

            return new ServiceResult<object>
            {
                Success = true,
                Message = "Login successful",
                StatusCode = 200,
                Data = new
                {
                    ChildId = child.ChildId,
                    Gender = child.Gender
                }
            };
        }

     
        public async Task<ServiceResult<List<object>>> GetGameLevels()
        {
            var levels = await _db.GameLevels
                .Where(l => l.IntelligenceId != null)
                .Select(l => new
                {
                    levelId = l.LevelId,
                    levelName = l.LevelName
                })
                .ToListAsync();

            return new ServiceResult<List<object>>
            {
                Success = true,
                Message = "Success",
                StatusCode = 200,
                Data = levels.Cast<object>().ToList()
            };
        }


        public async Task<ServiceResult<object>> SaveGameResult(SaveGameResultRequest request)
        {
            if (request == null || request.AspectResult == null)
            {
                return new ServiceResult<object>
                {
                    Success = false,
                    Message = "البيانات ناقصة",
                    StatusCode = 400
                };
            }

            var child = await _db.Children.FindAsync(request.ChildId);
            if (child == null)
            {
                return new ServiceResult<object>
                {
                    Success = false,
                    Message = "الطفل غير موجود",
                    StatusCode = 404
                };
            }
            var level = await _db.GameLevels.FindAsync(Guid.Parse(request.LevelId));


            if (level == null)
            {
                return new ServiceResult<object>
                {
                    Success = false,
                    Message = "اللعبة غير موجودة",
                    StatusCode = 404
                };
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1. GameSession
                var session = new GameSession
                {
                    GameSessionId = Guid.NewGuid(),
                    ChildId = request.ChildId,
                    LevelId = Guid.Parse(request.LevelId),
                    TotalTime = request.TotalTime,
                    PlayedAt = DateTime.UtcNow
                };

                _db.GameSessions.Add(session);
                await _db.SaveChangesAsync();

                // 2. Aspect
                var aspect = new AspectResult
                {
                    GameSessionId = session.GameSessionId,
                    IntelligenceId = level.IntelligenceId!.Value,
                    AspectName = request.AspectResult.AspectName,
                    AspectScore = request.AspectResult.AspectScore,
                    AspectRating = request.AspectResult.AspectRating,
                    RecordedAt = DateTime.UtcNow
                };

                _db.AspectResults.Add(aspect);
                await _db.SaveChangesAsync();

                // 3. Indicators + Items
                foreach (var indDto in request.AspectResult.Indicators)
                {
                    var indicator = new IndicatorResult
                    {
                        AspectResultId = aspect.AspectResultId,
                        IndicatorName = indDto.IndicatorName,
                        IndicatorScore = indDto.IndicatorScore,
                        IndicatorRating = indDto.IndicatorRating,
                        PsychometricPts = indDto.PsychometricPts,
                        RecordedAt = DateTime.UtcNow
                    };

                    _db.IndicatorResults.Add(indicator);
                    await _db.SaveChangesAsync();

                    foreach (var itemDto in indDto.Items)
                    {
                        _db.AssessmentItems.Add(new AssessmentItem
                        {
                            IndicatorResultId = indicator.IndicatorResultId,
                            ItemIndex = itemDto.ItemIndex,
                            ItemName = itemDto.ItemName,
                            FinalScore = itemDto.FinalScore,
                            Rating = itemDto.Rating,
                            PsychometricPts = itemDto.PsychometricPts,
                            RecordedAt = DateTime.UtcNow
                        });
                    }

                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return new ServiceResult<object>
                {
                    Success = true,
                    Message = "تم الحفظ بنجاح",
                    StatusCode = 200,
                    Data = new { sessionId = session.GameSessionId }
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ServiceResult<ChildProfileDto>> GetChildProfileAsync(string childId)
        {
            var child = await _db.Children
                .Where(c => c.ChildId == childId)
                .Select(c => new ChildProfileDto
                {
                    FullName = c.ChildName,
                    Age = c.Age,
                    Gender = c.Gender
                })
                .FirstOrDefaultAsync();

            if (child == null)
            {
                return new ServiceResult<ChildProfileDto>
                {
                    Success = false,
                    Message = "الطفل غير موجود",
                    StatusCode = 404,
                    Data = null
                };
            }

            return new ServiceResult<ChildProfileDto>
            {
                Success = true,
                Message = "تم جلب البيانات بنجاح",
                StatusCode = 200,
                Data = child
            };
        }
    }

}