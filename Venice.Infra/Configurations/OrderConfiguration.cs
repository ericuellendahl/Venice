using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venice.Domain.Entities;

namespace Venice.Infra.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ClienteId)
                  .IsRequired();

            entity.Property(e => e.Data)
                  .IsRequired();

            entity.Property(e => e.Status)
                  .IsRequired();

            entity.Property(e => e.ValorTotal)
                  .HasColumnType("decimal(18,2)");
        }
    }
}
