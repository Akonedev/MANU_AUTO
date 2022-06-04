using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MANU_AUTO.Models;

namespace MANU_AUTO.Data
{
    public partial class LAMANU_AUTOContext : MANU_AUTO_IdentityContext
    {

        public LAMANU_AUTOContext(DbContextOptions<LAMANU_AUTOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categorie> Categories { get; set; } = null!;
        public virtual DbSet<Tutoriel> Tutoriels { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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
