using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class MemeConfiguration : IEntityTypeConfiguration<Meme>
{
    public void Configure(EntityTypeBuilder<Meme> builder)
    {
        builder.ToTable("Memes");
    }
}
