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
            entity.HasKey(e => e.AR_Id).HasName("PK__Answer_R__003ED5F26DB1ECD1");

            entity.Property(e => e.Answer).HasMaxLength(1);
            entity.Property(e => e.AnswerDate).HasColumnType("datetime");

            entity.HasOne(d => d.Learn).WithMany(p => p.Answer_Record)
                .HasForeignKey(d => d.Learn_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer_Re__Learn__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Answer_Record)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answer_Re__User___4D94879B");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Book_Id).HasName("PK__Book__C223F3B4CBA24285");

            entity.Property(e => e.BookName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(d => d.Plan).WithMany(p => p.Book)
                .HasForeignKey(d => d.Plan_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Book__Plan_Id__440B1D61");
        });

        modelBuilder.Entity<BookPlan>(entity =>
        {
            entity.HasKey(e => e.Plan_Id).HasName("PK__BookPlan__9BAF9B031DE0303A");

            entity.Property(e => e.Plan_Goal).HasMaxLength(200);
            entity.Property(e => e.Plan_Name).HasMaxLength(100);
            entity.Property(e => e.Plan_Type).HasMaxLength(100);
            entity.Property(e => e.Plan_suject).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.BookPlan)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookPlan__User_I__3A81B327");
        });

        modelBuilder.Entity<Learn>(entity =>
        {
            entity.HasKey(e => e.Learn_Id).HasName("PK__Learn__319993005E050ACF");

            entity.Property(e => e.DueTime).HasColumnType("datetime");
            entity.Property(e => e.Learn_Name).HasMaxLength(100);

            entity.HasOne(d => d.Plan).WithMany(p => p.Learn)
                .HasForeignKey(d => d.Plan_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Learn__Plan_Id__46E78A0C");
        });

        modelBuilder.Entity<Members>(entity =>
        {
            entity.HasKey(e => e.User_Id).HasName("PK__Members__206D9170C8FFF617");

            entity.HasIndex(e => e.Email, "UQ__Members__A9D10534555AED53").IsUnique();

            entity.Property(e => e.AuthCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<Notice>(entity =>
        {
            entity.HasKey(e => e.Notice_Id).HasName("PK__Notice__E9930CABD96445D5");

            entity.Property(e => e.Message).HasMaxLength(200);
            entity.Property(e => e.NoticeTime).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Notice)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notice__User_Id__412EB0B6");
        });

        modelBuilder.Entity<PlanMembers>(entity =>
        {
            entity.HasKey(e => new { e.User_Id, e.Plan_Id }).HasName("PK__PlanMemb__09D768C06B5D99B1");

            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(10);

            entity.HasOne(d => d.Plan).WithMany(p => p.PlanMembers)
                .HasForeignKey(d => d.Plan_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlanMembe__Plan___5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.PlanMembers)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlanMembe__User___5DCAEF64");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Post_Id).HasName("PK__Post__5875F7AD1838C391");

            entity.Property(e => e.Content).HasMaxLength(100);
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Plan).WithMany(p => p.Post)
                .HasForeignKey(d => d.Plan_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__Plan_Id__3D5E1FD2");

            entity.HasOne(d => d.User).WithMany(p => p.Post)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__User_Id__3E52440B");
        });

        modelBuilder.Entity<Post_Report>(entity =>
        {
            entity.HasKey(e => e.P_Report_Id).HasName("PK__Post_Rep__AF4E25E3E831EA16");

            entity.Property(e => e.Action)
                .HasMaxLength(10)
                .HasDefaultValue("未審核");
            entity.Property(e => e.Report_text).HasMaxLength(200);

            entity.HasOne(d => d.Post).WithMany(p => p.Post_Report)
                .HasForeignKey(d => d.Post_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post_Repo__Post___59FA5E80");
        });

        modelBuilder.Entity<ProgressTracking>(entity =>
        {
            entity.HasKey(e => e.Progress_Id).HasName("PK__Progress__D558797A60C7671D");

            entity.Property(e => e.CompletionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Learn).WithMany(p => p.ProgressTracking)
                .HasForeignKey(d => d.Learn_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgressT__Learn__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.ProgressTracking)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgressT__User___5165187F");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Reply_Id).HasName("PK__Reply__B6633284E3C88026");

            entity.Property(e => e.ReplyContent).HasMaxLength(100);
            entity.Property(e => e.ReplyTime).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Reply)
                .HasForeignKey(d => d.Post_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reply__Post_Id__49C3F6B7");

            entity.HasOne(d => d.User).WithMany(p => p.Reply)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reply__User_Id__4AB81AF0");
        });

        modelBuilder.Entity<Reply_Report>(entity =>
        {
            entity.HasKey(e => e.R_Report_Id).HasName("PK__Reply_Re__6B66990FED16DBBA");

            entity.Property(e => e.Action)
                .HasMaxLength(10)
                .HasDefaultValue("未審核");
            entity.Property(e => e.Report_text).HasMaxLength(200);

            entity.HasOne(d => d.Reply).WithMany(p => p.Reply_Report)
                .HasForeignKey(d => d.Reply_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reply_Rep__Reply__5629CD9C");
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.Statistics_Id).HasName("PK__Statisti__A2EC2FD9D49FAB21");

            entity.HasOne(d => d.Plan).WithMany(p => p.Statistic)
                .HasForeignKey(d => d.Plan_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Statistic__Plan___6477ECF3");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Topic_Id).HasName("PK__Topic__8DEAA40577172212");

            entity.Property(e => e.Answer).HasMaxLength(1);
            entity.Property(e => e.Option_A).HasMaxLength(200);
            entity.Property(e => e.Option_B).HasMaxLength(200);
            entity.Property(e => e.Option_C).HasMaxLength(200);
            entity.Property(e => e.Option_D).HasMaxLength(200);
            entity.Property(e => e.Question).HasMaxLength(200);

            entity.HasOne(d => d.Learn).WithMany(p => p.Topic)
                .HasForeignKey(d => d.Learn_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Topic__Learn_Id__619B8048");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
