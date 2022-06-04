using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MANU_AUTO.Models;

namespace MANU_AUTO.Data
{
    public partial class LAMANU_AUTOContext : DbContext
    {
        public LAMANU_AUTOContext()
        {
        }

        public LAMANU_AUTOContext(DbContextOptions<LAMANU_AUTOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categorie> Categories { get; set; } = null!;
        public virtual DbSet<Tutoriel> Tutoriels { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-9UHQOE3\\SQLEXPRESS;Database=LAMANU_AUTO;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categorie>(entity =>
            {
                entity.ToTable("Categorie");

                entity.Property(e => e.Label)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasMany(d => d.IdTutoriels)
                    .WithMany(p => p.Ids)
                    .UsingEntity<Dictionary<string, object>>(
                        "TutorielCategorie",
                        l => l.HasOne<Tutoriel>().WithMany().HasForeignKey("IdTutoriel").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Tutoriel_Categorie_Tutoriel0_FK"),
                        r => r.HasOne<Categorie>().WithMany().HasForeignKey("Id").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Tutoriel_Categorie_Categorie_FK"),
                        j =>
                        {
                            j.HasKey("Id", "IdTutoriel").HasName("Tutoriel_Categorie_PK");

                            j.ToTable("Tutoriel_Categorie");

                            j.IndexerProperty<int>("IdTutoriel").HasColumnName("Id_Tutoriel");
                        });
            });

            modelBuilder.Entity<Tutoriel>(entity =>
            {
                entity.ToTable("Tutoriel");

                entity.Property(e => e.Contenu).HasColumnType("text");

                entity.Property(e => e.Dcc)
                    .HasColumnType("datetime")
                    .HasColumnName("DCC");

                entity.Property(e => e.Dml)
                    .HasColumnType("datetime")
                    .HasColumnName("DML");

                entity.Property(e => e.Titre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VideoLink)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
