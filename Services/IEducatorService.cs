using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.Models.ViewModels;

namespace BadeePlatform.Services
{
    public interface IEducatorService
    {
        public Task<ServiceResult> RegisterEducatorAsync(RegisterEducatorDTO dto);
        Task<List<School>> GetAllSchoolsAsync();
        public Task<ServiceResult> LoginEducatorAsync(LoginEducatorDTO dto);
        public Task<EducatorProfileViewModel?> GetEducatorProfileAsync(string EducatorId);
        public Task<bool> UpdateEducatorProfileAsync(EducatorProfileViewModel model);

        public  Task<bool> DeleteEducatorAccountAsync(string educatorId);

    }
}
