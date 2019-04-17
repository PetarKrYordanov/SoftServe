using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ImageServe.Models;

namespace ImageServe.Data.Configurations
{
    class ImageTagConfiguration : IEntityTypeConfiguration<ImageTag>
    {
        public void Configure(EntityTypeBuilder<ImageTag> builder)
        {
            builder.HasKey(t => new { t.Name, t.ImageID });
        }
    }
}
