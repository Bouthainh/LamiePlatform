using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using BadeePlatform.Services;
using BadeePlatform.Models;
using BadeePlatform.Data;
using LamiePlatform.Tests.Helpers;
namespace LamiePlatform.Tests.Services;

public class ChildServiceTests
{

    // Delete Child Profile - TC1: 
    [Fact]
    public async Task DeleteChildProfileAsync_TC1()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1023874342";
        var childId = "1129874321";

        var result = await service.DeleteChildProfileAsync(parentId, childId);

        result.Should().BeFalse();

        var count = await context.ParentChildren.CountAsync();
        count.Should().Be(0);
    }

    // Delete Child Profile - TC2: 
    [Fact]
    public async Task DeleteChildProfileAsync_TC2()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1013432123";
        var childId = "1120976534";

        context.ParentChildren.Add(new ParentChild
        {
            ParentId = parentId,
            ChildId = childId,
            RelationshipType = "Father"
        });
        await context.SaveChangesAsync();

        var result = await service.DeleteChildProfileAsync(parentId, childId);

        result.Should().BeTrue();

        var deletedRecord = await context.ParentChildren
            .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);
        deletedRecord.Should().BeNull();
    }

    // Revoke Educator Access - TC1: Permission does not exist
    [Fact]
    public async Task RevokeEducatorAccessAsync_TC1()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1012345678";
        var childId = "1019876543";

        var result = await service.RevokeEducatorAccessAsync(parentId, childId);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("الصلاحية غير موجودة.");

        var count = await context.EducatorPermissions.CountAsync();
        count.Should().Be(0);
    }

    // Revoke Educator Access - TC2: Permission exists and should be deleted
    [Fact]
    public async Task RevokeEducatorAccessAsync_TC2()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1015566778";
        var childId = "1017788990";
        var educatorId = "9876543210";

        context.EducatorPermissions.Add(new EducatorPermission
        {
            EducatorId = educatorId,
            ChildId = childId,
            ParentId = parentId,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await service.RevokeEducatorAccessAsync(parentId, childId);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم منع المعلم من الوصول لملف الطفل.");

        var deletedPermission = await context.EducatorPermissions
            .FirstOrDefaultAsync(ep => ep.ParentId == parentId && ep.ChildId == childId);
        deletedPermission.Should().BeNull();
    }
}


