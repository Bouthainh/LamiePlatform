using BadeePlatform.Data;
using BadeePlatform.DTOs;
using BadeePlatform.Models;
using BadeePlatform.Models.ViewModels;
using BadeePlatform.Services;
using FluentAssertions;
using LamiePlatform.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LamiePlatform.Tests.Services;

public class ParentServiceTests
{
    // Helper method to create ParentService with PasswordHasher
    private ParentService GetParentService(BadeedbContext context)
    {
        var passwordHasher = new PasswordHasher<Parent>();
        return new ParentService(context, passwordHasher);
    }

    // Login Parent - TC1: Login with Username - Success
    [Fact]
    public async Task LoginParentAsync_TC1()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);
        var passwordHasher = new PasswordHasher<Parent>();

        // Create parent with username
        var parent = new Parent
        {
            ParentId = "1119435366",
            ParentName = "Omar Ali",
            Username = "omar112233",
            Email = "omar@email.com",
            PhoneNumber = "0501234567",
            IsVerified = true,
            CreatedAt = DateTime.Now,
            Role = "Father"
        };

        // Hash the password
        parent.Password = passwordHasher.HashPassword(parent, "Omar112233");

        context.Parents.Add(parent);
        await context.SaveChangesAsync();

        var loginDto = new LoginParentDTO
        {
            UsernameOrEmail = "omar112233",  // Matches Username
            Password = "Omar112233"
        };

        // Act
        var result = await service.LoginParentAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم تسجيل الدخول بنجاح.");
        result.ParentId.Should().Be("1119435366");
        result.Data.Should().Be("Father");
    }

    // Login Parent - TC2: Login with Email - Success
    [Fact]
    public async Task LoginParentAsync_TC2()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);
        var passwordHasher = new PasswordHasher<Parent>();

        // Create parent with email
        var parent = new Parent
        {
            ParentId = "1119435366",
            ParentName = "Omar Ali",
            Username = "omar112233",
            Email = "omar@email.com",  // This will be matched
            PhoneNumber = "0551234567",
            IsVerified = true,
            CreatedAt = DateTime.Now,
            Role = "Father"
        };

        // Hash the password
        parent.Password = passwordHasher.HashPassword(parent, "Omar112233");

        context.Parents.Add(parent);
        await context.SaveChangesAsync();

        var loginDto = new LoginParentDTO
        {
            UsernameOrEmail = "omar@email.com",  // Matches Email
            Password = "Omar112233"
        };

        // Act
        var result = await service.LoginParentAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("تم تسجيل الدخول بنجاح.");
        result.ParentId.Should().Be("1119435366");
        result.Data.Should().Be("Father");
    }

    // Login Parent - TC3: User Not Found
    [Fact]
    public async Task LoginParentAsync_TC3()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);

        // No parent added to database - empty database

        var loginDto = new LoginParentDTO
        {
            UsernameOrEmail = "non-existent",
            Password = "AnyPassword"
        };

        // Act
        var result = await service.LoginParentAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("اسم المستخدم أو كلمة المرور غير صحيحة.");
        result.ParentId.Should().BeNull();
    }

    // Login Parent - TC4: Parent Exists but Password Field is Null
    [Fact]
    public async Task LoginParentAsync_TC4()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);

        // Create parent with NULL password
        var parent = new Parent
        {
            ParentId = "1119435366",
            ParentName = "Omar Ali",
            Username = "omar112233",
            Email = "omar@email.com",
            PhoneNumber = "0561234567",
            Password = null,  // Password is NULL (data corruption scenario)
            IsVerified = true,
            CreatedAt = DateTime.Now,
            Role = "Father"
        };

        context.Parents.Add(parent);
        await context.SaveChangesAsync();

        var loginDto = new LoginParentDTO
        {
            UsernameOrEmail = "omar112233",
            Password = "AnyPassword"
        };

        // Act
        var result = await service.LoginParentAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("اسم المستخدم أو كلمة المرور غير صحيحة.");
        result.ParentId.Should().BeNull();
    }

    // Login Parent - TC5: Wrong Password
    [Fact]
    public async Task LoginParentAsync_TC5()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);
        var passwordHasher = new PasswordHasher<Parent>();

        // Create parent with correct password
        var parent = new Parent
        {
            ParentId = "1119435366",
            ParentName = "Omar Ali",
            Username = "omar112233",
            Email = "omar@email.com",
            PhoneNumber = "0571234567",
            IsVerified = true,
            CreatedAt = DateTime.Now,
            Role = "Father"
        };

        // Hash the CORRECT password
        parent.Password = passwordHasher.HashPassword(parent, "Omar112233");

        context.Parents.Add(parent);
        await context.SaveChangesAsync();

        var loginDto = new LoginParentDTO
        {
            UsernameOrEmail = "omar112233",
            Password = "WrongPassword123"  // Wrong password
        };

        // Act
        var result = await service.LoginParentAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("اسم المستخدم أو كلمة المرور غير صحيحة.");
        result.ParentId.Should().BeNull();
    }
 
    // Update Parent Profile - TC1: Parent exists - Success
    [Fact]
    public async Task UpdateParentProfileAsync_TC1()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);

        // Create a parent in the database
        var parent = new Parent
        {
            ParentId = "1119435366",
            ParentName = "Omar Ali",
            Username = "omar112233",
            Email = "omar@email.com",
            PhoneNumber = "0501234567",
            IsVerified = true,
            CreatedAt = DateTime.Now,
            Role = "Father",
            Password = "hashedpassword"
        };

        context.Parents.Add(parent);
        await context.SaveChangesAsync();

        // Create update model with new data
        var updateModel = new ParentProfileViewModel
        {
            ParentId = "1119435366",
            ParentName = "Omar Ali Updated",
            Username = "omar_updated",
            Email = "omar_new@email.com",
            PhoneNumber = "0559876543"
        };

        // Act
        var result = await service.UpdateParentProfileAsync(updateModel);

        // Assert
        result.Should().BeTrue();

        // Verify the data was actually updated in the database
        var updatedParent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == "1119435366");
        updatedParent.Should().NotBeNull();
        updatedParent.ParentName.Should().Be("Omar Ali Updated");
        updatedParent.Username.Should().Be("omar_updated");
        updatedParent.Email.Should().Be("omar_new@email.com");
        updatedParent.PhoneNumber.Should().Be("0559876543");
    }

    // Update Parent Profile - TC2: Parent doesn't exist - Failure
    [Fact]
    public async Task UpdateParentProfileAsync_TC2()
    {
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = GetParentService(context);

        // No parent added to database - empty database

        // Try to update non-existent parent
        var updateModel = new ParentProfileViewModel
        {
            ParentId = "9999999999",
            ParentName = "Non Existent",
            Username = "nonexistent",
            Email = "nonexistent@email.com",
            PhoneNumber = "0501111111"
        };

        // Act
        var result = await service.UpdateParentProfileAsync(updateModel);

        // Assert
        result.Should().BeFalse();

        // Verify no data was added to the database
        var count = await context.Parents.CountAsync();
        count.Should().Be(0);
    }
}
