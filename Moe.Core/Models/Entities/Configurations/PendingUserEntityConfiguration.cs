using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Moe.Core.Models.Entities.Configurations;

public class PendingUserEntityConfiguration : IEntityTypeConfiguration<PendingUser>
{
    public void Configure(EntityTypeBuilder<PendingUser> builder)
    {
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasFilter("\"Email\" IS NOT NULL");
        builder.HasIndex(e => new {e.Phone, e.PhoneCountryCode})
            .IsUnique()
            .HasFilter("\"Phone\" IS NOT NULL AND \"PhoneCountryCode\" IS NOT NULL");
    }
}