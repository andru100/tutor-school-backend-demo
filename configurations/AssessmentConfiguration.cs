using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Model.Configurations;

public class AssessmentConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("application_users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name).HasColumnName("Name");
        builder.Property(u => u.Dob).HasColumnName("Dob");
        // builder.Property(u => u.SortCode).HasColumnName("SortCode");
        // builder.Property(u => u.AccountNo).HasColumnName("AccountNo");
        builder.Property(u => u.Stream).HasColumnName("Stream");
        builder.Property(u => u.Adjustments).HasColumnName("Adjustments");
        builder.Property(u => u.Notes).HasColumnName("Notes");
        builder.Property(u => u.ParentName).HasColumnName("ParentName");
    }
}