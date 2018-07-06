using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyFund.DataModel
{
    public partial class CrowdContext: IdentityDbContext<User, IdentityRole<long>, long>
    {
        public CrowdContext()
        {
        }

        public CrowdContext(DbContextOptions<CrowdContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attatchment> Attatchment { get; set; }
        public virtual DbSet<AttatchmentSet> AttatchmentSet { get; set; }
        public virtual DbSet<BackingPackage> BackingPackage { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectCategory> ProjectCategory { get; set; }
        public virtual DbSet<ProjectUpdate> ProjectUpdate { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserBacking> UserBacking { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attatchment>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.AttatchmentSet)
                    .WithMany(p => p.Attatchments)
                    .HasForeignKey(d => d.AttatchmentSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_203");
            });

            modelBuilder.Entity<AttatchmentSet>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(10);
            });

            modelBuilder.Entity<BackingPackage>(entity =>
            {
                entity.Property(e => e.BackingAmount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PackageDescription).HasMaxLength(255);

                entity.HasOne(d => d.AttatchmentSet)
                    .WithMany(p => p.BackingPackage)
                    .HasForeignKey(d => d.AttatchmentSetId)
                    .HasConstraintName("FK_213");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.BackingPackages)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_50");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.AmountGathered).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.Deadline).HasColumnType("date");

                entity.Property(e => e.Goal).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.MediaUrl)
                    .HasColumnName("MediaURL")
                    .HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasMaxLength(255);

                entity.HasOne(d => d.AttatchmentSet)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.AttatchmentSetId)
                    .HasConstraintName("FK_207");

                entity.HasOne(d => d.ProjectCategory)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.ProjectCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_35");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_25");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_82");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime2(7)")
                    .ValueGeneratedOnUpdate()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("DateCreated")
                    .IsRequired()
                    .HasColumnType("datetime2(7)")
                    .HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<ProjectCategory>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ProjectUpdate>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Text).HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.AttatchmentSet)
                    .WithMany(p => p.ProjectUpdate)
                    .HasForeignKey(d => d.AttatchmentSetId)
                    .HasConstraintName("FK_254");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectUpdates)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_228");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ProjectUpdate)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_267");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UK_Email")
                    .IsUnique();

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Organization).HasMaxLength(100);

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<UserBacking>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.BackingId });

                entity.Property(e => e.Amount).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.Backing)
                    .WithMany(p => p.UserBackings)
                    .HasForeignKey(d => d.BackingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_86");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserBackings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_78");
            });
        }
    }
}
