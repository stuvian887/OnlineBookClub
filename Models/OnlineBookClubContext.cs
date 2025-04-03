using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineBookClub.Models;

public partial class OnlineBookClubContext : DbContext
{
    public OnlineBookClubContext(DbContextOptions<OnlineBookClubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer_Record> Answer_Record { get; set; }

    public virtual DbSet<Book> Book { get; set; }

    public virtual DbSet<BookPlan> BookPlan { get; set; }

    public virtual DbSet<Learn> Learn { get; set; }

    public virtual DbSet<Members> Members { get; set; }

    public virtual DbSet<Notice> Notice { get; set; }

    public virtual DbSet<PlanMembers> PlanMembers { get; set; }

    public virtual DbSet<Post> Post { get; set; }

    public virtual DbSet<Post_Report> Post_Report { get; set; }

    public virtual DbSet<ProgressTracking> ProgressTracking { get; set; }

    public virtual DbSet<Reply> Reply { get; set; }

    public virtual DbSet<Reply_Report> Reply_Report { get; set; }

    public virtual DbSet<Statistic> Statistic { get; set; }

    public virtual DbSet<Topic> Topic { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer_Record>(entity =>
        {
            entity.HasKey(e => e.AR_Id).HasName("PK__Answer_R__003ED5F200B194C2");

            entity.Property(e => e.Answer)
                .IsRequired()
                .HasMaxLength(1);
            entity.Property(e => e.AnswerDate).HasColumnType("datetime");

            entity.HasOne(d => d.Learn).WithMany(p => p.Answer_Record)
                .HasForeignKey(d => d.Learn_Id)
                .HasConstraintName("FK__Answer_Re__Learn__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.Answer_Record)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer_Re__User___5FB337D6");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Book_Id).HasName("PK__Book__C223F3B45E6536D8");

            entity.Property(e => e.BookName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Link).IsRequired();

            entity.HasOne(d => d.Plan).WithMany(p => p.Book)
                .HasForeignKey(d => d.Plan_Id)
                .HasConstraintName("FK__Book__Plan_Id__5629CD9C");
        });

        modelBuilder.Entity<BookPlan>(entity =>
        {
            entity.HasKey(e => e.Plan_Id).HasName("PK__BookPlan__9BAF9B0351C75CDE");

            entity.Property(e => e.Plan_Goal)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Plan_Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Plan_Type)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Plan_suject)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.BookPlan)
                .HasForeignKey(d => d.User_Id)
                .HasConstraintName("FK__BookPlan__User_I__4CA06362");
        });

        modelBuilder.Entity<Learn>(entity =>
        {
            entity.HasKey(e => e.Learn_Id).HasName("PK__Learn__31999300BF643019");

            entity.Property(e => e.DueTime).HasColumnType("datetime");
            entity.Property(e => e.Learn_Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Plan).WithMany(p => p.Learn)
                .HasForeignKey(d => d.Plan_Id)
                .HasConstraintName("FK__Learn__Plan_Id__59063A47");
        });

        modelBuilder.Entity<Members>(entity =>
        {
            entity.HasKey(e => e.User_Id).HasName("PK__Members__206D9170026C53EE");

            entity.HasIndex(e => e.Email, "UQ__Members__A9D10534F7B3454D").IsUnique();

            entity.Property(e => e.AuthCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Notice>(entity =>
        {
            entity.HasKey(e => e.Notice_Id).HasName("PK__Notice__E9930CABBB2E44A7");

            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.NoticeTime).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Notice)
                .HasForeignKey(d => d.User_Id)
                .HasConstraintName("FK__Notice__User_Id__534D60F1");
        });

        modelBuilder.Entity<PlanMembers>(entity =>
        {
            entity.HasKey(e => new { e.User_Id, e.Plan_Id }).HasName("PK__PlanMemb__09D768C031119E03");

            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasOne(d => d.Plan).WithMany(p => p.PlanMembers)
                .HasForeignKey(d => d.Plan_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlanMembe__Plan___70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.PlanMembers)
                .HasForeignKey(d => d.User_Id)
                .HasConstraintName("FK__PlanMembe__User___6FE99F9F");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Post_Id).HasName("PK__Post__5875F7AD2DEF8D8D");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Plan).WithMany(p => p.Post)
                .HasForeignKey(d => d.Plan_Id)
                .HasConstraintName("FK__Post__Plan_Id__4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.Post)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__User_Id__5070F446");
        });

        modelBuilder.Entity<Post_Report>(entity =>
        {
            entity.HasKey(e => e.P_Report_Id).HasName("PK__Post_Rep__AF4E25E3B72A40DD");

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("未審核");
            entity.Property(e => e.Report_text)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(d => d.Post).WithMany(p => p.Post_Report)
                .HasForeignKey(d => d.Post_Id)
                .HasConstraintName("FK__Post_Repo__Post___6C190EBB");
        });

        modelBuilder.Entity<ProgressTracking>(entity =>
        {
            entity.HasKey(e => e.Progress_Id).HasName("PK__Progress__D558797A78B1BA0E");

            entity.Property(e => e.CompletionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Learn).WithMany(p => p.ProgressTracking)
                .HasForeignKey(d => d.Learn_Id)
                .HasConstraintName("FK__ProgressT__Learn__6477ECF3");

            entity.HasOne(d => d.User).WithMany(p => p.ProgressTracking)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgressT__User___6383C8BA");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Reply_Id).HasName("PK__Reply__B66332845B1014D1");

            entity.Property(e => e.ReplyContent)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.ReplyTime).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Reply)
                .HasForeignKey(d => d.Post_Id)
                .HasConstraintName("FK__Reply__Post_Id__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.Reply)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reply__User_Id__5CD6CB2B");
        });

        modelBuilder.Entity<Reply_Report>(entity =>
        {
            entity.HasKey(e => e.R_Report_Id).HasName("PK__Reply_Re__6B66990FC2260C00");

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("未審核");
            entity.Property(e => e.Report_text)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(d => d.Reply).WithMany(p => p.Reply_Report)
                .HasForeignKey(d => d.Reply_Id)
                .HasConstraintName("FK__Reply_Rep__Reply__68487DD7");
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.Statistics_Id).HasName("PK__Statisti__A2EC2FD9F1F7BAB4");

            entity.HasOne(d => d.Plan).WithMany(p => p.Statistic)
                .HasForeignKey(d => d.Plan_Id)
                .HasConstraintName("FK__Statistic__Plan___76969D2E");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Topic_Id).HasName("PK__Topic__8DEAA4050CB88B32");

            entity.Property(e => e.Answer)
                .IsRequired()
                .HasMaxLength(1);
            entity.Property(e => e.Option_A)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Option_B)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Option_C)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Option_D)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Question)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(d => d.Learn).WithMany(p => p.Topic)
                .HasForeignKey(d => d.Learn_Id)
                .HasConstraintName("FK__Topic__Learn_Id__73BA3083");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
