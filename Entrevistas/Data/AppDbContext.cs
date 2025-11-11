using Microsoft.EntityFrameworkCore;
using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Models.Enums;

namespace ProjetoEntrevistas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Empregador> Empregadores { get; set; }
    public DbSet<Candidato> Candidatos { get; set; }
    public DbSet<Entrevista> Entrevistas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração da tabela Empregadores (employers)
        modelBuilder.Entity<Empregador>(entity =>
        {
            entity.ToTable("employers", "public");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("name").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(200);
            entity.Property(e => e.Telefone).HasColumnName("phone").HasMaxLength(20);
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasMany(e => e.Entrevistas)
                .WithOne(e => e.Empregador)
                .HasForeignKey(e => e.IdEmpregador)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da tabela Candidatos (candidates)
        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.ToTable("candidates", "public");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("name").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(200);
            entity.Property(e => e.Telefone).HasColumnName("phone").HasMaxLength(20);
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasMany(e => e.Entrevistas)
                .WithOne(e => e.Candidato)
                .HasForeignKey(e => e.IdCandidato)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da tabela Entrevistas (interviews)
        modelBuilder.Entity<Entrevista>(entity =>
        {
            entity.ToTable("interviews", "public");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEmpregador).HasColumnName("employer_id");
            entity.Property(e => e.IdCandidato).HasColumnName("candidate_id");
            entity.Property(e => e.DataHora).HasColumnName("starts_at").IsRequired();
            entity.Property(e => e.DuracaoMinutos).HasColumnName("duration_minutes").IsRequired().HasDefaultValue(60);
            entity.Property(e => e.LinkReuniao).HasColumnName("meeting_link").HasMaxLength(500);
            entity.Property(e => e.Local).HasColumnName("location").HasMaxLength(500);
            entity.Property(e => e.Observacoes).HasColumnName("notes").HasMaxLength(1000);
            entity.Property(e => e.CriadoEm).HasColumnName("created_at").HasDefaultValueSql("now()");
            
            // ✅ Enums armazenados como TEXT no PostgreSQL (sem validação de tipo)
            entity.Property(e => e.Tipo)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();
            
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(StatusEntrevista.scheduled)
                .IsRequired();
            
            // Índices
            entity.HasIndex(e => e.DataHora);
            entity.HasIndex(e => e.IdCandidato);
            entity.HasIndex(e => e.IdEmpregador);
        });

        base.OnModelCreating(modelBuilder);
    }
}