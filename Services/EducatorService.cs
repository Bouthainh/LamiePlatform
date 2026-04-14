using LamiePlatform.Data;
using LamiePlatform.Models;
using Microsoft.AspNetCore.Identity;
using LamiePlatform.Models.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LamiePlatform.DTOs.PlatformDTOs;

namespace LamiePlatform.Services
{
    public class EducatorService : IEducatorService
    {
        private readonly LamieDbContext _db;
        private readonly IPasswordHasher<Educator> _passwordHasher;

        public EducatorService(LamieDbContext db, IPasswordHasher<Educator> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult> RegisterEducatorAsync(RegisterEducatorDTO dto) {

            var errorMsg = await CheckDuplicateFields(dto);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return new ServiceResult(false, errorMsg);
            }

            var eduator = new Educator
            {
                EducatorId = dto.EducatorId,
                EducatorName = $"{dto.FirstName} {dto.LastName}",
                PhoneNumber = dto.PhoneNumber,
                SchoolId = dto.SchoolId,
                Email = dto.Email,
                Username = dto.Username
            };

            string hashedPassword = _passwordHasher.HashPassword(eduator, dto.Password);
            eduator.Password = hashedPassword;

            try
            {
                _db.Educators.Add(eduator);
                await _db.SaveChangesAsync();// save the educator first
                
                if (dto.ClassIds != null && dto.ClassIds.Any())
                {
                    var occupiedClasses = await GetOccupiedClassNamesAsync(dto.ClassIds);
                    if (occupiedClasses.Any())
                    {
                        
                        _db.Educators.Remove(eduator);
                        await _db.SaveChangesAsync();

                        var names = string.Join("، ", occupiedClasses);
                        return new ServiceResult(false, $"الفصول التالية مشغولة بمعلم آخر: {names}");
                    }

                    var selectedClasses = await _db.Classes
                        .Where(c => dto.ClassIds.Contains(c.ClassId))
                        .ToListAsync();

                    foreach (var cls in selectedClasses)
                    {
                        cls.EducatorId = eduator.EducatorId;
                    }

                    await _db.SaveChangesAsync(); //save the educatorId to the selected classes
                }
                return new ServiceResult(true, "تم التسجيل بنجاح.", userId: eduator.EducatorId);
            }
            catch (Exception)
            {
                return new ServiceResult(false, "حدث خطأ في قاعدة البيانات أثناء التسجيل.");
            }
        }

        private async Task<string> CheckDuplicateFields(RegisterEducatorDTO dto)
        {
            var existing = await _db.Educators
                 .Where(p => p.EducatorId == dto.EducatorId ||
                             p.Username == dto.Username ||
                             p.Email == dto.Email ||
                             p.PhoneNumber == dto.PhoneNumber)
                 .Select(p => new { p.EducatorId, p.Username, p.Email, p.PhoneNumber })
                 .FirstOrDefaultAsync();

            if (existing == null) return null;
            if (existing.EducatorId == dto.EducatorId) return "الهوية الوطنية مسجلة مسبقًا.";
            if (existing.Username == dto.Username) return "اسم المستخدم مسجل مسبقًا.";
            if (existing.Email == dto.Email) return "البريد الإلكتروني مسجل مسبقًا.";
            if (existing.PhoneNumber == dto.PhoneNumber) return "رقم الهاتف مسجل مسبقًا.";

            return null;
        }

        public async Task<List<School>> GetAllSchoolsAsync()
        {
            return await _db.Schools.ToListAsync();
        }
        public async Task<ServiceResult> LoginEducatorAsync(LoginEducatorDTO dto) {
            var usernameOrEmail = dto.UsernameOrEmail.ToLower();

            var educator = await _db.Educators
                .FirstOrDefaultAsync(p =>
                    p.Username.ToLower() == usernameOrEmail ||
                    p.Email.ToLower() == usernameOrEmail);

            if (educator == null || educator.Password == null)
            {
                return new ServiceResult(false, "اسم المستخدم أو كلمة المرور غير صحيحة.");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                educator,
                educator.Password,
                dto.Password
            );

            if (verificationResult == PasswordVerificationResult.Success)
            {
                return new ServiceResult(true, "تم تسجيل الدخول بنجاح.", userId: educator.EducatorId);
            }

            return new ServiceResult(false, "اسم المستخدم أو كلمة المرور غير صحيحة.");

        }


        public async Task<EducatorProfileViewModel?> GetEducatorProfileAsync(string eduactorId)
        {
            var educator = await _db.Educators
                .FirstOrDefaultAsync(p => p.EducatorId == eduactorId);

            if (educator == null)
                return null;
            var nameParts = educator.EducatorName?.Split(' ', 2) ?? new[] { "", "" };

            var classes = await _db.Classes
                .Where(c => c.EducatorId == eduactorId)
                .ToListAsync();

            var classIds = classes.Select(c => c.ClassId).ToList();
            var gradeId = classes.FirstOrDefault()?.GradeId ?? Guid.Empty;


            return new EducatorProfileViewModel
            {
                EducatorId = educator.EducatorId,
                FirstName = nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[1] : "",
                PhoneNumber = educator.PhoneNumber,
                SchoolId = educator.SchoolId,
                Email = educator.Email,
                Username = educator.Username,
                Password= educator.Password,
                GradeId = gradeId,      
                ClassIds = classIds

            };
        }

        public async Task<bool> UpdateEducatorProfileAsync(EducatorProfileViewModel model)
        {
            var educator = await _db.Educators
                .FirstOrDefaultAsync(p => p.EducatorId == model.EducatorId);

            if (educator == null)
                return false;

            educator.EducatorName = $"{model.FirstName} {model.LastName}".Trim();
            educator.PhoneNumber = model.PhoneNumber;
            educator.SchoolId = model.SchoolId;
            educator.Email = model.Email;
            educator.Username = model.Username;

            if (!string.IsNullOrEmpty(model.Password))
            {
                string hashedPassword = _passwordHasher.HashPassword(educator, model.Password);
                educator.Password = hashedPassword;
            }

            var occupiedClasses = await GetOccupiedClassNamesAsync(model.ClassIds, model.EducatorId);
            if (occupiedClasses.Any())
            {
                var names = string.Join("، ", occupiedClasses);
                throw new InvalidOperationException($"الفصول التالية مشغولة بمعلم آخر: {names}");
            }
            // clear old class associations 
            var oldClasses = await _db.Classes
                .Where(c => c.EducatorId == model.EducatorId)
                .ToListAsync();
            
            foreach (var cls in oldClasses)
            {
                cls.EducatorId = null;
            }
            // assign new educator classes
            if (model.ClassIds != null && model.ClassIds.Any())
            {
                var newClasses = await _db.Classes
                    .Where(c => model.ClassIds.Contains(c.ClassId))
                    .ToListAsync();

                foreach (var cls in newClasses)
                {
                    cls.EducatorId = educator.EducatorId;
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        private async Task<List<string>> GetOccupiedClassNamesAsync(List<Guid> classIds, string currentEducatorId = null)
        {
            return await _db.Classes
                .Where(c => classIds.Contains(c.ClassId)
                         && c.EducatorId != null
                         && c.EducatorId != currentEducatorId)
                .Select(c => c.ClassName)
                .ToListAsync();
        }

        public async Task<bool> DeleteEducatorAccountAsync(string educatorId)
        {
            var educator = await _db.Educators
                .FirstOrDefaultAsync(e => e.EducatorId == educatorId);

            if (educator == null) return false;

            _db.Educators.Remove(educator);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
