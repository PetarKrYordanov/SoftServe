using Microsoft.EntityFrameworkCore;
using ImageServe.Data.Configurations;
using ImageServe.Models;
//using ImageServe.Common;
using Microsoft.EntityFrameworkCore.Proxies;

namespace ImageServe.Data
{
    public class ImageServeDbContext : DbContext
    {
        public DbSet<Image> Images { get; set; }

        public DbSet<ImageTag> ImageTags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<MainComment> MainComments { get; set; }

        public DbSet<SubComment> SubComments { get; set; }

        public DbSet<ImageLike> Likes { get; set; }

        public ImageServeDbContext(DbContextOptions<ImageServeDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            //if (!builder.IsConfigured)
            //{
            //
            //    builder.UseLazyLoadingProxies(useLazyLoadingProxies: true)
            //      .UseSqlServer(ConnectionInformation.DatabaseConnectionString);
            //
            //}
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration<User>(new UserConfiguration());

            builder.ApplyConfiguration<Image>(new ImageConfiguration());

            builder.ApplyConfiguration<Friendship>(new FriendshipConfiguration());

            builder.ApplyConfiguration<ImageTag>(new ImageTagConfiguration());

            builder.ApplyConfiguration<ImageLike>(new ImageLikeConfiguration());

            builder.ApplyConfiguration<MainComment>(new MainCommentConfiguration());

            
        }
    }
}
