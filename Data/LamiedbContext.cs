using System;
using System.Collections.Generic;
using LamiePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace LamiePlatform.Data;

public partial class LamiedbContext : DbContext
{
    public LamiedbContext()
    {
    }

    public LamiedbContext(DbContextOptions<LamiedbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityRecommendation> ActivityRecommendations { get; set; }

    public virtual DbSet<AspectResult> AspectResults { get; set; }

    public virtual DbSet<AssessmentItem> AssessmentItems { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<ChildGroup> ChildGroups { get; set; }

    public virtual DbSet<ChildIntelligence> ChildIntelligences { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Educator> Educators { get; set; }

    public virtual DbSet<EducatorPermission> EducatorPermissions { get; set; }

    public virtual DbSet<GameCharacter> GameCharacters { get; set; }

    public virtual DbSet<GameLevel> GameLevels { get; set; }

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<IndicatorResult> IndicatorResults { get; set; }

    public virtual DbSet<IntelligenceType> IntelligenceTypes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<ParentChild> ParentChildren { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=LamieDB;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityRecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId).HasName("PK__Activity__BCABEBB79AAFF1F5");

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
            entity.Property(e => e.ClassId).HasColumnName("class_ID");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.GroupId).HasColumnName("group_ID");

            entity.HasOne(d => d.Child).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ActivityR__child__778AC167");

            entity.HasOne(d => d.Class).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ActivityR__class__7A672E12");

            entity.HasOne(d => d.Educator).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.EducatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ActivityR__educa__787EE5A0");

            entity.HasOne(d => d.Group).WithMany(p => p.ActivityRecommendations)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__ActivityR__group__797309D9");
        });

        modelBuilder.Entity<AspectResult>(entity =>
        {
            entity.HasKey(e => e.AspectResultId).HasName("PK__AspectRe__AB86FFC0B1A6C9B2");

            entity.ToTable("AspectResult");

            entity.Property(e => e.AspectResultId).HasColumnName("aspect_result_id");
            entity.Property(e => e.AspectName)
                .HasMaxLength(100)
                .HasColumnName("aspect_name");
            entity.Property(e => e.AspectRating)
                .HasMaxLength(20)
                .HasColumnName("aspect_rating");
            entity.Property(e => e.AspectScore).HasColumnName("aspect_score");
            entity.Property(e => e.GameSessionId).HasColumnName("gameSession_ID");
            entity.Property(e => e.IntelligenceId).HasColumnName("intelligence_ID");
            entity.Property(e => e.RecordedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("recorded_at");

            entity.HasOne(d => d.GameSession).WithMany(p => p.AspectResults)
                .HasForeignKey(d => d.GameSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspectResult_GameSession");

            entity.HasOne(d => d.Intelligence).WithMany(p => p.AspectResults)
                .HasForeignKey(d => d.IntelligenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspectResult_Intelligence");
        });

        modelBuilder.Entity<AssessmentItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Assessme__5203084565EDBCE3");

            entity.ToTable("AssessmentItem");

            entity.Property(e => e.ItemId).HasColumnName("item_ID");
            entity.Property(e => e.Accuracy).HasColumnName("accuracy");
            entity.Property(e => e.ErrorRate).HasColumnName("error_rate");
            entity.Property(e => e.FinalScore).HasColumnName("final_score");
            entity.Property(e => e.IndicatorResultId).HasColumnName("indicator_result_id");
            entity.Property(e => e.ItemIndex).HasColumnName("item_index");
            entity.Property(e => e.PsychometricPts).HasColumnName("psychometric_pts");
            entity.Property(e => e.Rating)
                .HasMaxLength(20)
                .HasColumnName("rating");
            entity.Property(e => e.RecordedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("recorded_at");
            entity.Property(e => e.SpeedScore).HasColumnName("speed_score");

            entity.HasOne(d => d.IndicatorResult).WithMany(p => p.AssessmentItems)
                .HasForeignKey(d => d.IndicatorResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentItem_Indicator");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.ChildId).HasName("PK__Child__015BC0CD565E06AF");

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
            entity.Property(e => e.ClassId).HasColumnName("class_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.GradeId).HasColumnName("grade_ID");
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
                .HasConstraintName("FK__Child__character__72C60C4A");

            entity.HasOne(d => d.ChildGroup).WithMany(p => p.Children)
                .HasForeignKey(d => d.ChildGroupId)
                .HasConstraintName("FK__Child__child_gro__73BA3083");

            entity.HasOne(d => d.Class).WithMany(p => p.Children)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Child__class_ID__75A278F5");

            entity.HasOne(d => d.Grade).WithMany(p => p.Children)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("FK__Child__grade_ID__76969D2E");

            entity.HasOne(d => d.School).WithMany(p => p.Children)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__Child__school_ID__74AE54BC");
        });

        modelBuilder.Entity<ChildGroup>(entity =>
        {
            entity.HasKey(e => e.ChildGroupId).HasName("PK__ChildGro__37D5622085461BBF");

            entity.ToTable("ChildGroup");

            entity.Property(e => e.ChildGroupId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("child_group_ID");
            entity.Property(e => e.ClassId).HasColumnName("class_ID");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("group_name");
            entity.Property(e => e.MatchScore).HasColumnName("match_score");

            entity.HasOne(d => d.Class).WithMany(p => p.ChildGroups)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_ChildGroup_Class");
        });

        modelBuilder.Entity<ChildIntelligence>(entity =>
        {
            entity.HasKey(e => new { e.ChildId, e.IntelligenceId }).HasName("PK__ChildInt__886C13BFF55ABA40");

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
                .HasConstraintName("FK__ChildInte__child__7B5B524B");

            entity.HasOne(d => d.Intelligence).WithMany(p => p.ChildIntelligences)
                .HasForeignKey(d => d.IntelligenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChildInte__intel__7C4F7684");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__FDF57D8E71771ED5");

            entity.ToTable("Class");

            entity.Property(e => e.ClassId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("class_ID");
            entity.Property(e => e.ClassName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("class_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.GradeId).HasColumnName("grade_ID");

            entity.HasOne(d => d.Educator).WithMany(p => p.Classes)
                .HasForeignKey(d => d.EducatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Class__educator___6FE99F9F");

            entity.HasOne(d => d.Grade).WithMany(p => p.Classes)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("FK__Class__grade_ID__70DDC3D8");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__31E14AF287AA6722");

            entity.ToTable("Conversation");

            entity.Property(e => e.ConversationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("conversation_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EducatorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("educator_ID");
            entity.Property(e => e.ParentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("parent_ID");

            entity.HasOne(d => d.Educator).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.EducatorId)
                .HasConstraintName("FK__Conversat__educa__2B0A656D");

            entity.HasOne(d => d.Parent).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conversat__paren__2BFE89A6");
        });

        modelBuilder.Entity<Educator>(entity =>
        {
            entity.HasKey(e => e.EducatorId).HasName("PK__Educator__5C2A4BF6D06CDF20");

            entity.ToTable("Educator");

            entity.HasIndex(e => e.Email, "UQ_Educator_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_Educator_Phone").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Educator_Username").IsUnique();

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
                .HasMaxLength(256)
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
                .HasConstraintName("FK__Educator__school__71D1E811");
        });

        modelBuilder.Entity<EducatorPermission>(entity =>
        {
            entity.HasKey(e => new { e.RequestId, e.EducatorId }).HasName("PK__Educator__6D1211884F628FEB");

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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__EducatorP__child__7D439ABD");

            entity.HasOne(d => d.Educator).WithMany(p => p.EducatorPermissions)
                .HasForeignKey(d => d.EducatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EducatorP__educa__7E37BEF6");

            entity.HasOne(d => d.Parent).WithMany(p => p.EducatorPermissions)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__EducatorP__paren__7F2BE32F");
        });

        modelBuilder.Entity<GameCharacter>(entity =>
        {
            entity.HasKey(e => e.CharacterId).HasName("PK__GameChar__11D466B6A16FD769");

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
            entity.HasKey(e => e.LevelId).HasName("PK__GameLeve__0345127BE830C1EE");

            entity.ToTable("GameLevel");

            entity.Property(e => e.LevelId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("level_ID");
            entity.Property(e => e.IntelligenceId).HasColumnName("intelligence_ID");
            entity.Property(e => e.LevelDescription)
                .IsUnicode(false)
                .HasColumnName("level_description");
            entity.Property(e => e.LevelName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("level_name");

            entity.HasOne(d => d.Intelligence).WithMany(p => p.GameLevels)
                .HasForeignKey(d => d.IntelligenceId)
                .HasConstraintName("FK_GameLevel_Intelligence");
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.ToTable("GameSession");

            entity.Property(e => e.GameSessionId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("gameSession_ID");
            entity.Property(e => e.ChildId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("child_ID");
            entity.Property(e => e.LevelId).HasColumnName("level_ID");
            entity.Property(e => e.PlayedAt).HasColumnName("played_at");
            entity.Property(e => e.TotalTime).HasColumnName("total_time");

            entity.HasOne(d => d.Child).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK__GameSessi__child__01142BA1");

            entity.HasOne(d => d.Level).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameSessi__level__02084FDA");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grade__3A884EE4937B42B0");

            entity.ToTable("Grade");

            entity.Property(e => e.GradeId)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("grade_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.GradeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("grade_name");
            entity.Property(e => e.SchoolId).HasColumnName("school_ID");

            entity.HasOne(d => d.School).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__Grade__school_ID__6EF57B66");
        });

        modelBuilder.Entity<IndicatorResult>(entity =>
        {
            entity.HasKey(e => e.IndicatorResultId).HasName("PK__Indicato__93B7BE7CE6E47A10");

            entity.ToTable("IndicatorResult");

            entity.Property(e => e.IndicatorResultId).HasColumnName("indicator_result_id");
            entity.Property(e => e.AspectResultId).HasColumnName("aspect_result_id");
            entity.Property(e => e.IndicatorName)
                .HasMaxLength(100)
                .HasColumnName("indicator_name");
            entity.Property(e => e.IndicatorRating)
                .HasMaxLength(20)
                .HasColumnName("indicator_rating");
            entity.Property(e => e.IndicatorScore).HasColumnName("indicator_score");
            entity.Property(e => e.PsychometricPts).HasColumnName("psychometric_pts");
            entity.Property(e => e.RecordedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("recorded_at");

            entity.HasOne(d => d.AspectResult).WithMany(p => p.IndicatorResults)
                .HasForeignKey(d => d.AspectResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IndicatorResult_Aspect");
        });

        modelBuilder.Entity<IntelligenceType>(entity =>
        {
            entity.HasKey(e => e.IntelligenceId).HasName("PK__Intellig__937D372903D4BE82");

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
            entity.Property(e => e.IntelligenceTypeDescription).HasColumnName("intelligenceType_description");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__0BBC6AEED828F13E");

            entity.ToTable("Message");

            entity.Property(e => e.MessageId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("message_ID");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_ID");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.SenderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("sender_ID");
            entity.Property(e => e.SenderType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("sender_type");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("FK__Message__convers__31B762FC");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parent__F2D91411F41356B7");

            entity.ToTable("Parent");

            entity.HasIndex(e => e.Email, "UQ_Parent_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_Parent_Phone").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Parent_Username").IsUnique();

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
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<ParentChild>(entity =>
        {
            entity.HasKey(e => new { e.ParentId, e.ChildId }).HasName("PK__ParentCh__32CCA81DFBF783DE");

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
                .HasConstraintName("FK__ParentChi__child__05D8E0BE");

            entity.HasOne(d => d.Parent).WithMany(p => p.ParentChildren)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParentChi__paren__06CD04F7");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Request__18D0B537E5254934");

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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Request__child_I__07C12930");

            entity.HasOne(d => d.Educator).WithMany(p => p.Requests)
                .HasForeignKey(d => d.EducatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Request__educato__08B54D69");

            entity.HasOne(d => d.Parent).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Request__parent___09A971A2");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PK__School__27CB60ACE42C5A01");

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
