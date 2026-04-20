using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Domain.Core.Models;
using InteractHub.Domain.Enums;

namespace InteractHub.Infrastructure.Data
{
    public class InteractHubDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public InteractHubDbContext(DbContextOptions<InteractHubDbContext> options) : base(options)
        { }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(e =>
            {
                e.Property(u => u.FirstName).HasMaxLength(256);
                e.Property(u => u.LastName).HasMaxLength(256);
                e.Property(u => u.Status).HasDefaultValue(UserStatus.Active);
                e.Property(u => u.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<Post>(e =>
            {
                e.Property(p => p.Content).HasMaxLength(3000).IsRequired();
                e.Property(p => p.ImageUrl).HasMaxLength(2000);
                e.Property(p => p.LikeCount).HasDefaultValue(0);
                e.Property(p => p.CommentCount).HasDefaultValue(0);
                e.Property(p => p.IsDeleted).HasDefaultValue(false);

                e.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PostLike>(e =>
            {
                e.Property(p => p.CreatedOn).HasDefaultValueSql("NOW()");
                e.HasIndex(p => new { p.PostId, p.UserId }).IsUnique();

                e.HasOne(p => p.Post)
                    .WithMany(x => x.Likes)
                    .HasForeignKey(p => p.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            ApplySoftDeleteQueryFilters(builder);
        }

        private static void ApplySoftDeleteQueryFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType == null || !typeof(ISoftDeleteEntity).IsAssignableFrom(clrType) || clrType.IsAbstract)
                    continue;

                var method = typeof(InteractHubDbContext).GetMethod(
                    nameof(SetSoftDeleteQueryFilter),
                    BindingFlags.NonPublic | BindingFlags.Static);
                var generic = method!.MakeGenericMethod(clrType);
                generic.Invoke(null, new object[] { builder });
            }
        }

        private static void SetSoftDeleteQueryFilter<TEntity>(ModelBuilder builder)
            where TEntity : class, ISoftDeleteEntity
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTimeOffset.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTimeOffset.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
