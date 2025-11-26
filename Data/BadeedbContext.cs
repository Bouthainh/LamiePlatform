using System;
using System.Collections.Generic;
using BadeePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BadeePlatform.Data;

public partial class BadeedbContext : DbContext
{
    public BadeedbContext()
    {
    }

    public BadeedbContext(DbContextOptions<BadeedbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityRecommendation> ActivityRecommendations { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<ChildGroup> ChildGroups { get; set; }

    public virtual DbSet<ChildIntelligence> ChildIntelligences { get; set; }

    public virtual DbSet<Educator> Educators { get; set; }

    public virtual DbSet<EducatorPermission> EducatorPermissions { get; set; }

    public virtual DbSet<GameCharacter> GameCharacters { get; set; }

    public virtual DbSet<GameLevel> GameLevels { get; set; }

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<GameWorld> GameWorlds { get; set; }

    public virtual DbSet<IntelligenceProgress> IntelligenceProgresses { get; set; }

    public virtual DbSet<IntelligenceType> IntelligenceTypes { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<ParentChild> ParentChildren { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=badeedb;Trusted_Connection=True;TrustServerCertificate=true");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityRecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId).HasName("PK__Activity__BCABEBB78B1BE424");

            entity.ToTable("ActivityRecommendation");

            entity.Property(e => e.RecommendationId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("recommendation_ID");
            entity.Property(e => e.ActivityDescription)
                .IsUnicode(false)
                .HasColumnName("activity_description");
            entity.Property(e => e.ActivityName)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("activity_name");
            entity.Property(e => e.Category)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.GroupId).HasColumnName("group_ID");

            entity.HasOne(d => d.Child).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK__ActivityR__child__0B91BA14");

            entity.HasOne(d => d.Educator).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.EducatorId)
                .HasConstraintName("FK__ActivityR__educa__0A9D95DB");

            entity.HasOne(d => d.Group).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__ActivityR__group__0C85DE4D");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.ChildId).HasName("PK__Child__015BC0CDEE7BADAE");

            entity.ToTable("Child");

            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.CharacterId).HasColumnName("character_ID");
            entity.Property(e => e.ChildGroupId).HasColumnName("child_group_ID");
            entity.Property(e => e.ChildName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("child_name");
            entity.Property(e => e.Class)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("class");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.Grade)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IconImgPath)
                .IsUnicode(false)
                .HasColumnName("Icon_img_path");
            entity.Property(e => e.LoginCode)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("login_code");
            entity.Property(e => e.SchoolId).HasColumnName("school_ID");

            entity.HasOne(d => d.Character).WithMany(p => p.Children)
                .HasForeignKey(d => d.CharacterId)
                .HasConstraintName("FK__Child__character__4D94879B");

            entity.HasOne(d => d.ChildGroup).WithMany(p => p.Children)
                .HasForeignKey(d => d.ChildGroupId)
                .HasConstraintName("FK__Child__child_gro__4CA06362");

            entity.HasOne(d => d.School).WithMany(p => p.Children)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__Child__school_ID__4BAC3F29");
        });

        modelBuilder.Entity<ChildGroup>(entity =>
        {
            entity.HasKey(e => e.ChildGroupId).HasName("PK__ChildGro__37D56220C6EFEE80");

            entity.ToTable("ChildGroup");

            entity.Property(e => e.ChildGroupId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("child_group_ID");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("group_name");
            entity.Property(e => e.MatchScore).HasColumnName("match_score");
        });

        modelBuilder.Entity<ChildIntelligence>(entity =>
        {
            entity.HasKey(e => new { e.ChildId, e.IntelligenceId }).HasName("PK__ChildInt__886C13BF38E0B9E3");

            entity.ToTable("ChildIntelligence");

            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.IntelligenceId).HasColumnName("intelligence_ID");
            entity.Property(e => e.AssessmentDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("assessment_date");
            entity.Property(e => e.IntelligenceLevel)
                .IsUnicode(false)
                .HasColumnName("intelligence_level");
            entity.Property(e => e.ProficiencyScore)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("proficiency_score");
            entity.Property(e => e.Summary)
                .IsUnicode(false)
                .HasColumnName("summary");

            entity.HasOne(d => d.Child).WithMany(p => p.ChildIntelligences)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChildInte__child__7E37BEF6");

            entity.HasOne(d => d.Intelligence).WithMany(p => p.ChildIntelligences)
                .HasForeignKey(d => d.IntelligenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChildInte__intel__7F2BE32F");
        });

        modelBuilder.Entity<Educator>(entity =>
        {
            entity.HasKey(e => e.EducatorId).HasName("PK__Educator__5C2A4BF6AF9125E5");

            entity.ToTable("Educator");

            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.EducatorName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("educator_name");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.Password)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.SchoolId).HasColumnName("school_ID");
            entity.Property(e => e.Username)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.School).WithMany(p => p.Educators)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__Educator__school__5535A963");
        });

        modelBuilder.Entity<EducatorPermission>(entity =>
        {
            entity.HasKey(e => new { e.RequestId, e.EducatorId }).HasName("PK__Educator__6D121188FB3D2DA3");

            entity.ToTable("EducatorPermission");

            entity.Property(e => e.RequestId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("request_ID");
            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("parent_ID");

            entity.HasOne(d => d.Child).WithMany(p => p.EducatorPermissions)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK__EducatorP__child__6B24EA82");

            entity.HasOne(d => d.Educator).WithMany(p => p.EducatorPermissions)
                .HasForeignKey(d => d.EducatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EducatorP__educa__6A30C649");

            entity.HasOne(d => d.Parent).WithMany(p => p.EducatorPermissions)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__EducatorP__paren__6C190EBB");
        });

        modelBuilder.Entity<GameCharacter>(entity =>
        {
            entity.HasKey(e => e.CharacterId).HasName("PK__GameChar__11D466B63F5D2FBE");

            entity.ToTable("GameCharacter");

            entity.Property(e => e.CharacterId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("character_ID");
            entity.Property(e => e.CharacterDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("character_description");
            entity.Property(e => e.CharacterName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("character_name");
        });

        modelBuilder.Entity<GameLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__GameLeve__0345127B7EF782E5");

            entity.ToTable("GameLevel");

            entity.Property(e => e.LevelId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("level_ID");
            entity.Property(e => e.ChallangeNo).HasColumnName("ChallangeNO");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("difficulty");
            entity.Property(e => e.LevelDescription)
                .IsUnicode(false)
                .HasColumnName("level_description");
            entity.Property(e => e.LevelName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("level_name");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.WorldId).HasColumnName("world_ID");

            entity.HasOne(d => d.World).WithMany(p => p.GameLevels)
                .HasForeignKey(d => d.WorldId)
                .HasConstraintName("FK__GameLevel__world__72C60C4A");
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => new { e.GameId, e.LevelId, e.ChildId }).HasName("PK__GameSess__93D519606FC81C5C");

            entity.ToTable("GameSession");

            entity.Property(e => e.GameId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("game_ID");
            entity.Property(e => e.LevelId).HasColumnName("level_ID");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.AttemptData)
                .IsUnicode(false)
                .HasColumnName("attemptData");
            entity.Property(e => e.Score)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("score");
            entity.Property(e => e.TimeTaken).HasColumnName("time_taken");

            entity.HasOne(d => d.Child).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameSessi__child__778AC167");

            entity.HasOne(d => d.Level).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameSessi__level__76969D2E");
        });

        modelBuilder.Entity<GameWorld>(entity =>
        {
            entity.HasKey(e => e.WorldId).HasName("PK__GameWorl__A13DB9063CD7D3FA");

            entity.ToTable("GameWorld");

            entity.Property(e => e.WorldId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("world_ID");
            entity.Property(e => e.UnlockRequirement)
                .IsUnicode(false)
                .HasColumnName("unlock_Requirement");
            entity.Property(e => e.WorldDescription)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("world_description");
            entity.Property(e => e.WorldName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("world_name");
        });

        modelBuilder.Entity<IntelligenceProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__Intellig__49B0D4D9E06338BB");

            entity.ToTable("IntelligenceProgress");

            entity.Property(e => e.ProgressId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("progress_ID");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.IntelligenceId).HasColumnName("intelligence_ID");
            entity.Property(e => e.LevelId).HasColumnName("level_ID");
            entity.Property(e => e.RecordDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("record_date");
            entity.Property(e => e.Score)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("score");

            entity.HasOne(d => d.Child).WithMany(p => p.IntelligenceProgresses)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK__Intellige__child__03F0984C");

            entity.HasOne(d => d.Intelligence).WithMany(p => p.IntelligenceProgresses)
                .HasForeignKey(d => d.IntelligenceId)
                .HasConstraintName("FK__Intellige__intel__04E4BC85");

            entity.HasOne(d => d.Level).WithMany(p => p.IntelligenceProgresses)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK__Intellige__level__05D8E0BE");
        });

        modelBuilder.Entity<IntelligenceType>(entity =>
        {
            entity.HasKey(e => e.IntelligenceId).HasName("PK__Intellig__937D372903ABB0A1");

            entity.ToTable("IntelligenceType");

            entity.Property(e => e.IntelligenceId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("intelligence_ID");
            entity.Property(e => e.IconImgPath)
                .IsUnicode(false)
                .HasColumnName("Icon_img_path");
            entity.Property(e => e.IntelligenceName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("intelligence_name");
            entity.Property(e => e.IntelligenceTypeDescription)
                .IsUnicode(false)
                .HasColumnName("intelligenceType_description");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parent__F2D91411D6DF5106");

            entity.ToTable("Parent");

            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("parent_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.ParentName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("parent_name");
            entity.Property(e => e.Password)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Username)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<ParentChild>(entity =>
        {
            entity.HasKey(e => new { e.ParentId, e.ChildId }).HasName("PK__ParentCh__32CCA81D34F0C322");

            entity.ToTable("ParentChild");

            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("parent_ID");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.RelationshipType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("relationship_type");

            entity.HasOne(d => d.Child).WithMany(p => p.ParentChildren)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParentChi__child__5165187F");

            entity.HasOne(d => d.Parent).WithMany(p => p.ParentChildren)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParentChi__paren__5070F446");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Request__18D0B537F67B87B1");

            entity.ToTable("Request");

            entity.Property(e => e.RequestId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("request_ID");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("parent_ID");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("request_status");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Child).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK__Request__child_I__6477ECF3");

            entity.HasOne(d => d.Educator).WithMany(p => p.Requests)
                .HasForeignKey(d => d.EducatorId)
                .HasConstraintName("FK__Request__educato__6383C8BA");

            entity.HasOne(d => d.Parent).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Request__parent___656C112C");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PK__School__27CB60AC67819134");

            entity.ToTable("School");

            entity.Property(e => e.SchoolId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("school_ID");
            entity.Property(e => e.Branch)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("branch");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.SchoolName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("school_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
