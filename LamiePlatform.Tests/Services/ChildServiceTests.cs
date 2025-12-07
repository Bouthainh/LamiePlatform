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


    // Grant Educator Access - TC1: Permission already exists
    [Fact]
    public async Task GrantEducatorAccessAsync_TC1()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1012345678";
        var childId = "1019876543";
        var educatorId = "9876543210";

        var schoolId = Guid.NewGuid();
        var gradeId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        context.Schools.Add(new School
        {
            SchoolId = schoolId,
            SchoolName = "Test School",
            City = "Riyadh"
        });

        context.Grades.Add(new Grade
        {
            GradeId = gradeId,
            SchoolId = schoolId,
            GradeName = "Grade 1"
        });

        context.Educators.Add(new Educator
        {
            EducatorId = educatorId,
            EducatorName = "Ali Alghamdi",
            SchoolId = schoolId,
            Email = "teacher@test.com"
        });

        context.Classes.Add(new Class
        {
            ClassId = classId,
            GradeId = gradeId,
            ClassName = "Class A",
            EducatorId = educatorId
        });

        context.Children.Add(new Child
        {
            ChildId = childId,
            ChildName = "Test Child",
            Age = 6,
            Gender = "ذكر",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId,
            LoginCode = "12345678"
        });

        context.EducatorPermissions.Add(new EducatorPermission
        {
            EducatorId = educatorId,
            ChildId = childId,
            ParentId = parentId,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var result = await service.GrantEducatorAccessAsync(parentId, childId);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("الصلاحية موجودة مسبقًا.");

        var count = await context.EducatorPermissions
            .CountAsync(ep => ep.ParentId == parentId && ep.ChildId == childId);
        count.Should().Be(1);
    }

    // Grant Educator Access - TC2: New permission created successfully
    [Fact]
    public async Task GrantEducatorAccessAsync_TC2()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1015566778";
        var childId = "1017788990";
        var educatorId = "9876543210";

        var schoolId = Guid.NewGuid();
        var gradeId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        context.Schools.Add(new School
        {
            SchoolId = schoolId,
            SchoolName = "Test School",
            City = "Riyadh"
        });

        context.Grades.Add(new Grade
        {
            GradeId = gradeId,
            SchoolId = schoolId,
            GradeName = "Grade 1"
        });

        context.Educators.Add(new Educator
        {
            EducatorId = educatorId,
            EducatorName = "Teacher Ali",
            SchoolId = schoolId,
            Email = "teacher@test.com"
        });

        context.Classes.Add(new Class
        {
            ClassId = classId,
            GradeId = gradeId,
            ClassName = "Class A",
            EducatorId = educatorId
        });

        context.Children.Add(new Child
        {
            ChildId = childId,
            ChildName = "Test Child",
            Age = 6,
            Gender = "ذكر",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId,
            LoginCode = "12345678"
        });

        await context.SaveChangesAsync();

        var result = await service.GrantEducatorAccessAsync(parentId, childId);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم السماح للمعلم بالوصول لملف الطفل.");

        var newPermission = await context.EducatorPermissions
            .FirstOrDefaultAsync(ep => ep.ParentId == parentId
                                    && ep.ChildId == childId
                                    && ep.EducatorId == educatorId);
        newPermission.Should().NotBeNull();
    }

    // Grant Educator Access - TC3: Child does not exist
    [Fact]
    public async Task GrantEducatorAccessAsync_TC3()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1012233445";
        var childId = "1016655443";

        var result = await service.GrantEducatorAccessAsync(parentId, childId);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("لا يوجد معلم مسؤول عن هذا الفصل.");

        var count = await context.EducatorPermissions.CountAsync();
        count.Should().Be(0);
    }

    // Grant Educator Access - TC4: Educator is null
    [Fact]
    public async Task GrantEducatorAccessAsync_TC4()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);

        var parentId = "1112345678";
        var childId = "1119876543";

        var schoolId = Guid.NewGuid();
        var gradeId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        context.Schools.Add(new School
        {
            SchoolId = schoolId,
            SchoolName = "Test School",
            City = "Riyadh"
        });

        context.Grades.Add(new Grade
        {
            GradeId = gradeId,
            SchoolId = schoolId,
            GradeName = "Grade 1"
        });

        context.Classes.Add(new Class
        {
            ClassId = classId,
            GradeId = gradeId,
            ClassName = "Class A",
            EducatorId = null 
        });

        context.Children.Add(new Child
        {
            ChildId = childId,
            ChildName = "Test Child",
            Age = 6,
            Gender = "ذكر",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId,
            LoginCode = "12345678"
        });

        await context.SaveChangesAsync();

        var result = await service.GrantEducatorAccessAsync(parentId, childId);

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("لا يوجد معلم مسؤول عن هذا الفصل.");

        var count = await context.EducatorPermissions.CountAsync();
        count.Should().Be(0);
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


