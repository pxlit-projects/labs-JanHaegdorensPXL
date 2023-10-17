using DevOps.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Infrastructure
{
    internal class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
    {
        public void Configure(EntityTypeBuilder<Developer> builder)
        {
            builder.Property(d => d.Id)
            .HasMaxLength(11)
            .IsRequired();

            builder.Property(d => d.FirstName)
                .IsRequired();

            builder.Property(d => d.LastName)
                .IsRequired();

            builder.Property(d => d.Rating)
            .HasConversion(
                percentage => (double) percentage,               // Conversion from Percentage to double (no conversion needed)
                v => new Percentage(v) // Conversion from double to Percentage
            );
        }
    }
}
