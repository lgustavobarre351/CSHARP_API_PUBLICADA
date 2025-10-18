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
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserCpf).HasColumnName("user_cpf"); // Mapear para coluna user_cpf
            entity.Property(e => e.Tipo).HasColumnName("tipo");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("numeric(12,2)");
            entity.Property(e => e.Operacao).HasColumnName("operacao");
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em");
            entity.Property(e => e.AlteradoEm).HasColumnName("alterado_em");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Cpf).HasColumnName("cpf");
            entity.Property(e => e.Dados).HasColumnName("dados").HasColumnType("jsonb");
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em");
            entity.Property(e => e.AlteradoEm).HasColumnName("alterado_em");
            
            // Nome não é mapeado para coluna - é usado apenas para lógica de negócio
            entity.Ignore(e => e.Nome);
        });

        base.OnModelCreating(modelBuilder);
    }
}