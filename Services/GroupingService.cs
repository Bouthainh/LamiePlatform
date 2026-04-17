using LamiePlatform.Data;
using LamiePlatform.Models;
using LamiePlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace LamiePlatform.Services;

public class GroupingService : IGroupingService
{
    private readonly LamieDbContext _context;

    public GroupingService(LamieDbContext context)
    {
        _context = context;
    }

    // Check if class already has groups
    public async Task<bool> ClassAlreadyGroupedAsync(Guid classId)
    {
        return await _context.ChildGroups
            .AnyAsync(g => g.ClassId == classId);
    }

    // Main grouping method 
    public async Task<List<ChildGroup>> CreateGroupsForClassAsync(Guid classId)
    {
        // 1. Load all children in this class with their intelligences
        var children = await _context.Children
            .Where(c => c.ClassId == classId)
            .Include(c => c.ChildIntelligences)
                .ThenInclude(ci => ci.Intelligence)
            .ToListAsync();

        if (children.Count < 2)
            throw new InvalidOperationException("Not enough children in this class to form groups.");

        // 2. For each child, find their Top 1 intelligence
        var childProfiles = children
            .Select(c => new
            {
                Child = c,
                Top1 = c.ChildIntelligences
                    .OrderByDescending(ci => ci.ProficiencyScore)
                    .FirstOrDefault()
            })
            .Where(x => x.Top1 != null)
            .ToList();

        // 3. Group children by their Top 1 intelligence type — each type = one pool
        var pools = childProfiles
            .GroupBy(x => x.Top1!.IntelligenceId)
            .ToDictionary(
                g => g.Key,
                g => new Queue<Child>(g.Select(x => x.Child))
            );

        // 4. Build diverse groups of 4
        //    Pick one child from each pool in rotation until all assigned
        var allGroups = new List<List<Child>>();
        var currentGroup = new List<Child>();

        while (pools.Any(p => p.Value.Count > 0))
        {
            foreach (var pool in pools.Values)
            {
                if (pool.Count == 0) continue;

                currentGroup.Add(pool.Dequeue());

                if (currentGroup.Count == 4)
                {
                    allGroups.Add(new List<Child>(currentGroup));
                    currentGroup = new List<Child>();
                }
            }
        }

        // Leftover children (less than 4) become a final smaller group
        if (currentGroup.Count > 0)
            allGroups.Add(currentGroup);

        // 5. Save each group to DB
        var savedGroups = new List<ChildGroup>();
        int groupNumber = 1;

        foreach (var groupChildren in allGroups)
        {
            // Group name = most common Top 1 intelligence among members
            var dominantIntelligence = groupChildren
                .Select(c => c.ChildIntelligences
                    .OrderByDescending(ci => ci.ProficiencyScore)
                    .FirstOrDefault()?.Intelligence?.IntelligenceName ?? "Mixed")
                .GroupBy(name => name)
                .OrderByDescending(g => g.Count())
                .First().Key;

            var childGroup = new ChildGroup
            {
                ChildGroupId = Guid.NewGuid(),
                ClassId = classId,
                GroupName = $"Group {groupNumber} - {dominantIntelligence}",
                MatchScore = null
            };

            _context.ChildGroups.Add(childGroup);
            await _context.SaveChangesAsync();

            // Link each child to this group
            foreach (var child in groupChildren)
            {
                child.ChildGroupId = childGroup.ChildGroupId;
                _context.Children.Update(child);
            }

            await _context.SaveChangesAsync();

            savedGroups.Add(childGroup);
            groupNumber++;
        }

        return savedGroups;
    }

    //Get existing groups for a class 
    public async Task<List<ChildGroup>> GetGroupsByClassAsync(Guid classId)
    {
        return await _context.ChildGroups
            .Where(g => g.ClassId == classId)
            .Include(g => g.Children)
                .ThenInclude(c => c.ChildIntelligences)
                    .ThenInclude(ci => ci.Intelligence)
            .ToListAsync();
    }
}