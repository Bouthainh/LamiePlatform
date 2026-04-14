using LamiePlatform.Models;

namespace LamiePlatform.Services;

public interface IGroupingService
{
    Task<List<ChildGroup>> CreateGroupsForClassAsync(Guid classId);
    Task<List<ChildGroup>> GetGroupsByClassAsync(Guid classId);
    Task<bool> ClassAlreadyGroupedAsync(Guid classId);
}