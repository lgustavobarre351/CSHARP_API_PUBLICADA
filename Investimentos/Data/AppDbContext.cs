using Microsoft.EntityFrameworkCore;
using ProjetoInvestimentos.Models;

namespace ProjetoInvestimentos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Investimento> Investimentos { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Investimento>(entity =>
        {
            entity.ToTable("investimentos", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserCpf).HasColumnName("user_cpf").HasMaxLength(11).IsRequired();
            entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.Operacao).HasColumnName("operacao").HasMaxLength(20).IsRequired();
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.AlteradoEm).HasColumnName("alterado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relacionamento com UserProfile
            entity.HasOne<UserProfile>()
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.Cpf).HasColumnName("cpf").HasMaxLength(11).IsRequired();
            entity.Property(e => e.Dados).HasColumnName("dados").HasColumnType("jsonb");
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.AlteradoEm).HasColumnName("alterado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Índices
            entity.HasIndex(e => e.Cpf).IsUnique();
            
            // Nome não é mapeado para coluna - é usado apenas para lógica de negócio
            entity.Ignore(e => e.Nome);
        });

        base.OnModelCreating(modelBuilder);
    }
}