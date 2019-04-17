using ImageServe.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageServe.Data.Configurations
{
    class MainCommentConfiguration : IEntityTypeConfiguration<MainComment>
    {
        public void Configure(EntityTypeBuilder<MainComment> builder)
        {
            builder.HasMany(i => i.SubComments)
                .WithOne(it => it.MainComment)
                .HasForeignKey(it => it.MainCommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
