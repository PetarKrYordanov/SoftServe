using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ImageServe.Models;

namespace ImageServe.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.UserFriends)
                 .WithOne(f => f.User)
                 .HasForeignKey(f => f.UserId)
                 .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(u => u.FriendUsers)
                .WithOne(f => f.Friend)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(u => u.Images)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(i => i.Likes)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
