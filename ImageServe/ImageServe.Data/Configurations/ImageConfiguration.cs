using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ImageServe.Models;

namespace ImageServe.Data.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasMany(i => i.Tags)
                .WithOne(it => it.Image)
                .HasForeignKey(it => it.ImageID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Likes)
                .WithOne(i => i.Image)
                .HasForeignKey(i => i.ImageID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.MainComments)
                .WithOne(it => it.Image)
                .HasForeignKey(it => it.ImageId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(i => i.DateUploaded)
                 .HasDefaultValueSql("GETDATE()");
        }
    }
}
