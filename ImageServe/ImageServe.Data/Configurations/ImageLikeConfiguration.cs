using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ImageServe.Models;

namespace ImageServe.Data.Configurations
{
    public class ImageLikeConfiguration : IEntityTypeConfiguration<ImageLike>
    {
        public void Configure(EntityTypeBuilder<ImageLike> builder)
        {
            builder.HasKey(r => new { r.ImageID, r.UserId });
        }
    }
}
