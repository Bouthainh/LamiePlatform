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
    
    // Delete Child Profile - TC1
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

    // Delete Child Profile - TC2
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

}
