using BadeePlatform.DTOs;
using BadeePlatform.Models.ViewModels;

namespace BadeePlatform.Services
{
    public interface IParentService
    {
        public Task<ServiceResult> RegisterParentAsync(RegisterParentDTO dto);
        public Task<ServiceResult> LoginParentAsync(LoginParentDTO dto);
        public Task<ParentProfileViewModel?> GetParentProfileAsync(string parentId);
        public Task<bool> UpdateParentProfileAsync(ParentProfileViewModel model);
        



    }
}
