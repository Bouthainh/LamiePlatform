using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.Models.ViewModels;

namespace BadeePlatform.Services
{
    public interface IChildService
    {
        public Task<bool> DeleteChildProfileAsync(string parentId, string childId);
        public Task<string> GenerateUniqueLoginCodeAsync();
        public Task<ServiceResult> AddChildAsync(string parentId, AddChildDTO dto, string relationshipType);
        public Task<List<string>> GetAllCitiesAsync();
        public Task<List<School>> GetSchoolsByCityAsync(string city);
        public Task<List<Grade>> GetGradesBySchoolIdAsync(Guid schoolId);
        public Task<List<Class>> GetClassesByGradeIdAsync(Guid gradeId);

        public Task<ChildProfileViewModel> GetChildProfileByIdAsync(string parentId, string childId);

        public Task<ServiceResult> GrantEducatorAccessAsync(string parentId, string childId);
        public Task<ServiceResult> RevokeEducatorAccessAsync(string parentId, string childId);

        public Task<EditChildDTO?> GetChildForEditAsync(string parentId, string childId);
        public Task<ServiceResult> EditChildProfileAsync(string parentId, string childId, EditChildDTO dto);

        public Task<List<ChildrenViewModel>> GetAllChildrenByParentIdAsync(string parentId);



    }
}
