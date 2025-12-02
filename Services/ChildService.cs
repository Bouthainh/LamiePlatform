using BadeePlatform.Data;
using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BadeePlatform.Services
{
    public class ChildService : IChildService
    {
        private readonly BadeedbContext _db;

        public ChildService(BadeedbContext db)
        {
            _db = db;
        }

       
        public async Task<bool> DeleteChildProfileAsync(string parentId, string childId)
        {
            var parentChildRecord = await _db.ParentChildren
                .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);

            if (parentChildRecord == null)
            {
                return false;
            }

            _db.ParentChildren.Remove(parentChildRecord);
            await _db.SaveChangesAsync();

            return true;
        }

    
        public async Task<string> GenerateUniqueLoginCodeAsync()
        {
            const int codeLength = 8;
            string loginCode;
            bool isUnique;
            do
            {
                loginCode = System.Security.Cryptography.RandomNumberGenerator
                    .GetInt32(10000000, 99999999).ToString();
                isUnique = !await _db.Children.AnyAsync(c => c.LoginCode == loginCode);
            } while (!isUnique);

            return loginCode;
        }

    
        public async Task<List<string>> GetAllCitiesAsync()
        {
            return await _db.Schools
                .Where(s => !string.IsNullOrEmpty(s.City))
                .Select(s => s.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<List<School>> GetSchoolsByCityAsync(string city)
        {
            return await _db.Schools
                .Where(s => s.City == city)
                .OrderBy(s => s.SchoolName)
                .ToListAsync();
        }

        public async Task<List<Grade>> GetGradesBySchoolIdAsync(Guid schoolId)
        {
            return await _db.Grades
                .Where(g => g.SchoolId == schoolId)
                .OrderBy(g => g.GradeName)
                .ToListAsync();
        }

        public async Task<List<Class>> GetClassesByGradeIdAsync(Guid gradeId)
        {
            return await _db.Classes
                .Where(c => c.GradeId == gradeId)
                .Include(c => c.Educator)
                .OrderBy(c => c.ClassName)
                .ToListAsync();
        }

    
        public async Task<ServiceResult> AddChildAsync(string parentId, AddChildDTO dto, string relationshipType)
        {
            try
            {
                // Validate Parent
                var parent = await _db.Parents.FindAsync(parentId);
                if (parent == null)
                {
                    return new ServiceResult(false, "معلومات ولي الأمر غير موجودة.");
                }

                // Validate School Data
                var validationResult = await ValidateSchoolDataAsync(dto);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Check if relationship already exists
                var relationshipExists = await _db.ParentChildren
                    .AnyAsync(pc => pc.ParentId == parentId && pc.ChildId == dto.ChildId);

                if (relationshipExists)
                {
                    return new ServiceResult(false, "هذا الطفل مضاف مسبقاً في قائمتك.");
                }

                // Check if child exists
                var existingChild = await _db.Children
                    .FirstOrDefaultAsync(c => c.ChildId == dto.ChildId);

                if (existingChild != null)
                {
                    // If Child exists, just add relationship
                    return await HandleExistingChildAsync(existingChild, parentId, dto, relationshipType);
                }
                else
                {
                    // Child doesn't exist, create new
                    return await CreateNewChildAsync(parentId, dto, relationshipType);
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DB Error in AddChild: {ex.Message}");
                return new ServiceResult(false, "حدث خطأ أثناء حفظ البيانات في قاعدة البيانات.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddChild: {ex.Message}");
                return new ServiceResult(false, "حدث خطأ غير متوقع أثناء إضافة الطفل.");
            }
        }

    
        private async Task<ServiceResult> ValidateSchoolDataAsync(AddChildDTO dto)
        {
            var school = await _db.Schools
                .FirstOrDefaultAsync(s => s.SchoolId == dto.SchoolId && s.City == dto.City);

            if (school == null)
            {
                return new ServiceResult(false, "المدرسة المختارة غير صحيحة.");
            }

            var grade = await _db.Grades
                .FirstOrDefaultAsync(g => g.GradeId == dto.GradeId && g.SchoolId == dto.SchoolId);

            if (grade == null)
            {
                return new ServiceResult(false, "المرحلة الدراسية غير صحيحة.");
            }

            var classInfo = await _db.Classes
                .FirstOrDefaultAsync(c => c.ClassId == dto.ClassId && c.GradeId == dto.GradeId);

            if (classInfo == null)
            {
                return new ServiceResult(false, "الفصل غير صحيح.");
            }

            return new ServiceResult(true, "School data validated successfully");
        }

    
        private async Task<ServiceResult> HandleExistingChildAsync(Child existingChild, string parentId, AddChildDTO dto,
            string relationshipType)
        {
            // Update child info
            existingChild.SchoolId = dto.SchoolId;
            existingChild.GradeId = dto.GradeId;
            existingChild.ClassId = dto.ClassId;
            existingChild.Age = dto.Age;

            // Add relationship
            var parentChild = new ParentChild
            {
                ParentId = parentId,
                ChildId = dto.ChildId,
                RelationshipType = relationshipType
            };

            _db.ParentChildren.Add(parentChild);
            await _db.SaveChangesAsync();

            // Grant educator access if requested
            if (dto.AllowEducatorAccess)
            {
                await CreateEducatorPermissionAsync(parentId, dto.ChildId, dto.ClassId);
            }

            return new ServiceResult(true, "تم إضافة الطفل بنجاح إلى قائمتك.");
        }

   
        private async Task<ServiceResult> CreateNewChildAsync(string parentId, AddChildDTO dto, string relationshipType)
        { 
            var loginCode = await GenerateUniqueLoginCodeAsync();

            var newChild = new Child
            {
                ChildId = dto.ChildId,
                ChildName = dto.ChildName,
                Gender = dto.Gender,
                Age = dto.Age,
                LoginCode = loginCode,
                SchoolId = dto.SchoolId,
                GradeId = dto.GradeId,
                ClassId = dto.ClassId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Children.Add(newChild);

            var newParentChild = new ParentChild
            {
                ParentId = parentId,
                ChildId = dto.ChildId,
                RelationshipType = relationshipType
            };

            _db.ParentChildren.Add(newParentChild);
            await _db.SaveChangesAsync();

            if (dto.AllowEducatorAccess)
            {
                await CreateEducatorPermissionAsync(parentId, dto.ChildId, dto.ClassId);
            }

            return new ServiceResult(true, "تم إضافة الطفل بنجاح.", parentId: null, data: loginCode);
        }

        private async Task CreateEducatorPermissionAsync(string parentId, string childId, Guid classId)
        {
            try
            {
                var classInfo = await _db.Classes
                    .FirstOrDefaultAsync(c => c.ClassId == classId);

                if (classInfo?.EducatorId == null)
                    return;

                var existingPermission = await _db.EducatorPermissions
                    .AnyAsync(r => r.EducatorId == classInfo.EducatorId
                                && r.ChildId == childId
                                && r.ParentId == parentId);

                if (existingPermission)
                    return;

                var permission = new EducatorPermission
                {
                    EducatorId = classInfo.EducatorId,
                    ChildId = childId,
                    ParentId = parentId,
                };

                _db.EducatorPermissions.Add(permission);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Permission Error: {ex.Message}");
            }
        }

        public async Task<ChildProfileViewModel> GetChildProfileByIdAsync(string parentId, string childId)
        {
            try
            {
                var parentChild = await _db.ParentChildren
                    .Include(pc => pc.Child)
                        .ThenInclude(c => c.School)
                    .Include(pc => pc.Child)
                        .ThenInclude(c => c.Grade)
                    .Include(pc => pc.Child)
                        .ThenInclude(c => c.Class)
                            .ThenInclude(cl => cl.Educator)
                    .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);

                if (parentChild == null)
                {
                    return null;
                }

                var child = parentChild.Child;

                var hasPermission = await _db.EducatorPermissions
                    .AnyAsync(ep => ep.ParentId == parentId && ep.ChildId == childId);

                var pendingRequest = await _db.Requests
                    .Where(r => r.ParentId == parentId && r.ChildId == childId && r.RequestStatus == "Pending")
                    .OrderByDescending(r => r.SentAt)
                    .FirstOrDefaultAsync();

                var viewModel = new ChildProfileViewModel
                {
                    ChildId = child.ChildId,
                    ChildName = child.ChildName,
                    Age = child.Age ?? 0,
                    Gender = child.Gender,
                    LoginCode = child.LoginCode,
                    IconImgPath = child.IconImgPath,

                    SchoolName = child.School?.SchoolName,
                    City = child.School?.City,
                    Grade = child.Grade?.GradeName,
                    Class = child.Class?.ClassName,
                    EducatorName = child.Class?.Educator?.EducatorName,

                    RelationshipType = parentChild.RelationshipType,
                    HasPermission = hasPermission,
                    RequestStatus = pendingRequest?.RequestStatus
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChildProfile: {ex.Message}");
                return null;
            }
        }

        public async Task<ServiceResult> GrantEducatorAccessAsync(string parentId, string childId)
        {
            try
            {
                // Check if permission already exists
                var existingPermission = await _db.EducatorPermissions
                    .FirstOrDefaultAsync(ep => ep.ParentId == parentId && ep.ChildId == childId);

                if (existingPermission != null)
                {
                    return new ServiceResult(true, "الصلاحية موجودة مسبقًا.");
                }

                // Get child and educator info
                var child = await _db.Children
                    .Include(c => c.Class)
                    .FirstOrDefaultAsync(c => c.ChildId == childId);

                if (child == null || child.Class?.EducatorId == null)
                {
                    return new ServiceResult(false, "لا يوجد معلم مسؤول عن هذا الفصل.");
                }

                // Add permission
                var permission = new EducatorPermission
                {
                    EducatorId = child.Class.EducatorId,
                    ChildId = childId,
                    ParentId = parentId,
                    CreatedAt = DateTime.UtcNow
                };

                _db.EducatorPermissions.Add(permission);
                await _db.SaveChangesAsync();

                return new ServiceResult(true, "تم السماح للمعلم بالوصول لملف الطفل.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GrantAccess: {ex.Message}");
                return new ServiceResult(false, "حدث خطأ أثناء منح الصلاحية.");
            }
        }

  
        public async Task<ServiceResult> RevokeEducatorAccessAsync(string parentId, string childId)
        {
            try
            {
                var permission = await _db.EducatorPermissions
                    .FirstOrDefaultAsync(ep => ep.ParentId == parentId && ep.ChildId == childId);

                if (permission == null)
                {
                    return new ServiceResult(true, "الصلاحية غير موجودة.");
                }

                _db.EducatorPermissions.Remove(permission);
                await _db.SaveChangesAsync();

                return new ServiceResult(true, "تم منع المعلم من الوصول لملف الطفل.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RevokeAccess: {ex.Message}");
                return new ServiceResult(false, "حدث خطأ أثناء إلغاء الصلاحية.");
            }
        }

        public async Task<ServiceResult> EditChildProfileAsync(string parentId, string childId, EditChildDTO dto)
        {
            try
            {
                // Verify parent-child relationship
                var parentChild = await _db.ParentChildren
                    .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);

                if (parentChild == null)
                {
                    return new ServiceResult(false, "الطفل غير موجود في قائمتك");
                }

                // Get child data
                var child = await _db.Children.FindAsync(childId);

                if (child == null)
                {
                    return new ServiceResult(false, "الطفل غير موجود");
                }

                // Validate school data
                var validationResult = await ValidateSchoolDataForEditAsync(dto);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Update child data
                child.ChildName = dto.ChildName;
                child.Gender = dto.Gender;
                child.Age = dto.Age;
                child.SchoolId = dto.SchoolId;
                child.GradeId = dto.GradeId;
                child.ClassId = dto.ClassId;

                await _db.SaveChangesAsync();

                return new ServiceResult(true, "تم تعديل بيانات الطفل بنجاح");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DB Error in EditChild: {ex.Message}");
                return new ServiceResult(false, "حدث خطأ أثناء حفظ البيانات");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditChild: {ex.Message}");
                return new ServiceResult(false, "حدث خطأ غير متوقع");
            }
        }

        private async Task<ServiceResult> ValidateSchoolDataForEditAsync(EditChildDTO dto)
        {
            var school = await _db.Schools
                .FirstOrDefaultAsync(s => s.SchoolId == dto.SchoolId && s.City == dto.City);

            if (school == null)
            {
                return new ServiceResult(false, "المدرسة المختارة غير صحيحة");
            }

            var grade = await _db.Grades
                .FirstOrDefaultAsync(g => g.GradeId == dto.GradeId && g.SchoolId == dto.SchoolId);

            if (grade == null)
            {
                return new ServiceResult(false, "المرحلة الدراسية غير صحيحة");
            }

            var classInfo = await _db.Classes
                .FirstOrDefaultAsync(c => c.ClassId == dto.ClassId && c.GradeId == dto.GradeId);

            if (classInfo == null)
            {
                return new ServiceResult(false, "الفصل غير صحيح");
            }

            return new ServiceResult(true, "Validation passed");
        }

 
        public async Task<EditChildDTO?> GetChildForEditAsync(string parentId, string childId)
        {
            try
            {
                var parentChild = await _db.ParentChildren
                    .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);

                if (parentChild == null)
                {
                    return null;
                }

                var child = await _db.Children
                    .Include(c => c.School)
                    .Include(c => c.Grade)
                    .Include(c => c.Class)
                    .FirstOrDefaultAsync(c => c.ChildId == childId);

                if (child == null)
                {
                    return null;
                }

                var dto = new EditChildDTO
                {
                    ChildName = child.ChildName,
                    Gender = child.Gender,
                    Age = child.Age ?? 0,
                    City = child.School?.City,
                    SchoolId = child.SchoolId ?? Guid.Empty,
                    GradeId = child.GradeId ?? Guid.Empty,
                    ClassId = child.ClassId ?? Guid.Empty
                };

                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChildForEdit: {ex.Message}");
                return null;
            }
        }

        //Retrieve children by parent to view & manage them 
        public async Task<List<ChildrenViewModel>> GetAllChildrenByParentIdAsync(string parentId)
        {
            return await _db.ParentChildren
                .Where(pc => pc.ParentId == parentId)
                .Select(pc => new ChildrenViewModel
                {
                    ChildId = pc.ChildId,
                    ChildName = pc.Child.ChildName,
                    Age = pc.Child.Age,
                    Gender = pc.Child.Gender,
                    IconImgPath = pc.Child.Gender == "ذكر"
                                                    ? "/images/ChildBoy.png"
                                                     : "/images/ChildGirl.png"

                })
                .ToListAsync();
        }
    }
}