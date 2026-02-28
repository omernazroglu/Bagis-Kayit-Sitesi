using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BagisBilgileri> BagisBilgileris { get; set; }

    public virtual DbSet<BagisGrup> BagisGrups { get; set; }

    public virtual DbSet<Bagislar> Bagislars { get; set; }

    public virtual DbSet<Kullanici> Kullanicis { get; set; }

    public virtual DbSet<RefSutunlar> RefSutunlars { get; set; }

    public virtual DbSet<Referanslar> Referanslars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=UFUKDER_BAGIS;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BagisBilgileri>(entity =>
        {
            entity.HasOne(d => d.Bagislar).WithMany(p => p.BagisBilgileris).HasConstraintName("FK_BAGIS_BILGILERI_BAGISLAR");

            entity.HasOne(d => d.Sutunlar).WithMany(p => p.BagisBilgileris).HasConstraintName("FK_BAGIS_BILGILERI_SUTUNLAR");
        });

        modelBuilder.Entity<BagisGrup>(entity =>
        {
            entity.HasOne(d => d.Bagislar).WithMany(p => p.BagisGrups).HasConstraintName("FK_BAGIS_GRUP_BAGISLAR");
        });

        modelBuilder.Entity<Bagislar>(entity =>
        {
            entity.HasOne(d => d.Kullanici).WithMany(p => p.Bagislars)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BAGISLAR_KULLANICI");

            entity.HasOne(d => d.Referans).WithMany(p => p.Bagislars).HasConstraintName("FK_Bagislar_Referanslar");
        });

        modelBuilder.Entity<Referanslar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Referanslar");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
