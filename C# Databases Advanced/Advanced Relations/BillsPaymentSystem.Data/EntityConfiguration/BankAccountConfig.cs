namespace BillPaymentSystem.Data.EntityConfigurations
{
    using BillsPaymentSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.Property(x => x.BankName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.SWIFT)
                .IsUnicode(false)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}