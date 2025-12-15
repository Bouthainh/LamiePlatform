using BadeePlatform.Data;
using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.Services;
using FluentAssertions;
using LamiePlatform.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;
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

    // add child test 

    private (BadeedbContext context, ChildService service) CreateTestEnvironment()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new ChildService(context);
        return (context, service);
    }

    private async Task<(Guid schoolId, Guid gradeId, Guid classId)> SetupSchoolDataAsync(
        BadeedbContext context,
        string city = "Jeddah")
    {
        var schoolId = Guid.NewGuid();
        var gradeId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        context.Schools.Add(new School
        {
            SchoolId = schoolId,
            SchoolName = "Test School",
            City = city
        });

        context.Grades.Add(new Grade
        {
            GradeId = gradeId,
            GradeName = "Grade 3",
            SchoolId = schoolId
        });

        context.Classes.Add(new Class
        {
            ClassId = classId,
            ClassName = "3-A",
            GradeId = gradeId
        });

        await context.SaveChangesAsync();
        return (schoolId, gradeId, classId);
    }

    private async Task<string> SetupParentAsync(BadeedbContext context, string parentId = "1023456789")
    {
        context.Parents.Add(new Parent
        {
            ParentId = parentId,
            ParentName = "Test Parent",
            Email = "parent@test.com",
            PhoneNumber = "0501234567"
        });
        await context.SaveChangesAsync();
        return parentId;
    }

    [Fact]
    public async Task AddChildAsync_TC1_ParentNotFound_ReturnsFalse()
    {
        var (context, service) = CreateTestEnvironment();
        var dto = new AddChildDTO
        {
            ChildId = "1234567890",
            FirstName = "أحمد",
            FatherName = "محمد",
            GrandFatherName = "علي",
            LastName = "الأحمدي",
            Gender = "Male",
            Age = 7,
            City = "Jeddah",
            SchoolId = Guid.NewGuid(),
            GradeId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        var result = await service.AddChildAsync("invalid_parent", dto, "Father");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("معلومات ولي الأمر غير موجودة.");
    }

    [Fact]
    public async Task AddChildAsync_TC2_ParentExists_ContinuesToValidation()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var dto = new AddChildDTO
        {
            ChildId = "1234567890",
            FirstName = "سارة",
            FatherName = "خالد",
            GrandFatherName = "عبدالله",
            LastName = "السعيد",
            Gender = "Female",
            Age = 6,
            City = "Jeddah",
            SchoolId = Guid.NewGuid(),
            GradeId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        var result = await service.AddChildAsync(parentId, dto, "Father");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("المدرسة");
    }

    [Fact]
    public async Task AddChildAsync_TC3_InvalidSchoolData_ReturnsError()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var dto = new AddChildDTO
        {
            ChildId = "1234567890",
            FirstName = "فاطمة",
            FatherName = "أحمد",
            GrandFatherName = "حسن",
            LastName = "الزهراني",
            Gender = "Female",
            Age = 5,
            City = "Jeddah",
            SchoolId = Guid.NewGuid(),
            GradeId = Guid.NewGuid(),
            ClassId = Guid.NewGuid()
        };

        var result = await service.AddChildAsync(parentId, dto, "Father");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("المدرسة المختارة غير صحيحة.");
    }

    [Fact]
    public async Task AddChildAsync_TC4_ValidationSuccess_ContinuesToRelationshipCheck()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var (schoolId, gradeId, classId) = await SetupSchoolDataAsync(context);

        var childId = "1234567890";
        context.ParentChildren.Add(new ParentChild
        {
            ParentId = parentId,
            ChildId = childId,
            RelationshipType = "Father"
        });
        await context.SaveChangesAsync();

        var dto = new AddChildDTO
        {
            ChildId = childId,
            FirstName = "عمر",
            FatherName = "سعد",
            GrandFatherName = "محمد",
            LastName = "القحطاني",
            Gender = "Male",
            Age = 8,
            City = "Jeddah",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        };

        var result = await service.AddChildAsync(parentId, dto, "Father");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("هذا الطفل مضاف مسبقاً في قائمتك.");
    }

    [Fact]
    public async Task AddChildAsync_TC5_RelationshipExists_ReturnsError()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var (schoolId, gradeId, classId) = await SetupSchoolDataAsync(context);

        var childId = "9876543210";
        context.ParentChildren.Add(new ParentChild
        {
            ParentId = parentId,
            ChildId = childId,
            RelationshipType = "Mother"
        });
        await context.SaveChangesAsync();

        var dto = new AddChildDTO
        {
            ChildId = childId,
            FirstName = "نورة",
            FatherName = "فهد",
            GrandFatherName = "عبدالرحمن",
            LastName = "العتيبي",
            Gender = "Female",
            Age = 7,
            City = "Jeddah",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        };

        var result = await service.AddChildAsync(parentId, dto, "Mother");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("هذا الطفل مضاف مسبقاً في قائمتك.");
    }

    [Fact]
    public async Task AddChildAsync_TC6_NoRelationship_ContinuesToChildCheck()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var (schoolId, gradeId, classId) = await SetupSchoolDataAsync(context);

        var childId = "5555555555";
        context.Children.Add(new Child
        {
            ChildId = childId,
            ChildName = "خالد محمد علي السالم",
            Gender = "Male",
            Age = 6,
            LoginCode = "12345678",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        });
        await context.SaveChangesAsync();

        var dto = new AddChildDTO
        {
            ChildId = childId,
            FirstName = "خالد",
            FatherName = "محمد",
            GrandFatherName = "علي",
            LastName = "السالم",
            Gender = "Male",
            Age = 6,
            City = "Jeddah",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        };

        var result = await service.AddChildAsync(parentId, dto, "Father");

        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم إضافة الطفل بنجاح إلى قائمتك.");
    }

    [Fact]
    public async Task AddChildAsync_TC7_ChildExists_UpdatesAndAddsRelationship()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var (schoolId, gradeId, classId) = await SetupSchoolDataAsync(context);

        var childId = "7777777777";
        context.Children.Add(new Child
        {
            ChildId = childId,
            ChildName = "ريم أحمد حسن الغامدي",
            Gender = "Female",
            Age = 5,
            LoginCode = "87654321",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        });
        await context.SaveChangesAsync();

        var dto = new AddChildDTO
        {
            ChildId = childId,
            FirstName = "ريم",
            FatherName = "أحمد",
            GrandFatherName = "حسن",
            LastName = "الغامدي",
            Gender = "Female",
            Age = 6,
            City = "Jeddah",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        };

        var result = await service.AddChildAsync(parentId, dto, "Father");

        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم إضافة الطفل بنجاح إلى قائمتك.");

        var relationship = await context.ParentChildren
            .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);
        relationship.Should().NotBeNull();

        var updatedChild = await context.Children.FindAsync(childId);
        updatedChild.Age.Should().Be(6);
    }

    [Fact]
    public async Task AddChildAsync_TC8_ChildNotFound_CreatesNew()
    {
        var (context, service) = CreateTestEnvironment();
        var parentId = await SetupParentAsync(context);
        var (schoolId, gradeId, classId) = await SetupSchoolDataAsync(context);

        var childId = "3333333333";
        var dto = new AddChildDTO
        {
            ChildId = childId,
            FirstName = "يوسف",
            FatherName = "عبدالله",
            GrandFatherName = "سعيد",
            LastName = "الشهري",
            Gender = "Male",
            Age = 4,
            City = "Jeddah",
            SchoolId = schoolId,
            GradeId = gradeId,
            ClassId = classId
        };

        var result = await service.AddChildAsync(parentId, dto, "Father");

        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم إضافة الطفل بنجاح.");
        result.Data.Should().NotBeNull();
        result.Data.ToString().Should().HaveLength(8);

        var newChild = await context.Children.FindAsync(childId);
        newChild.Should().NotBeNull();
        newChild.LoginCode.Should().NotBeNullOrEmpty();

        var relationship = await context.ParentChildren
            .FirstOrDefaultAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);
        relationship.Should().NotBeNull();
    }




}

