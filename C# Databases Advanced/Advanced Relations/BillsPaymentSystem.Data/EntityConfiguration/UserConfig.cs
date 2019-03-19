using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfiguration
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(x => x.PaymentMethods)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(80)
                .IsUnicode(false);

            builder.Property(x => x.Password)
                .IsUnicode(false)
                .IsRequired();
        }
    }
}