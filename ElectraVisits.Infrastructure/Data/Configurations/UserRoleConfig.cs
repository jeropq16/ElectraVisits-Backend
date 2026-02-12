using ElectraVisits.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectraVisits.Infrastructure.Data.Configurations;

public class UserRoleConfig :  IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(u => new { u.UserId, u.RoleId });

        builder.HasOne(u => u.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(u => u.UserId);

        builder.HasOne(u => u.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(u => u.RoleId);
    }
}