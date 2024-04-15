using System;
using System.Collections.Generic;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DotnetCoding.Infrastructure
{
    public partial class ProductDBContext : DbContext
    {
        public ProductDBContext()
        {
        }

        public ProductDBContext(DbContextOptions<ProductDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ProductDetail> ProductDetails { get; set; } = null!;
        public virtual DbSet<ProductQueue> ProductQueues { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=DESKTOP-4DH234D\\SQLEXPRESS; database=ProductDB;Trusted_Connection=True;Encrypt=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.Property(e => e.ProductDescription)
                    .HasMaxLength(225)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .HasMaxLength(225)
                    .IsUnicode(false);

                entity.Property(e => e.ProductPostDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductQueue>(entity =>
            {
                entity.ToTable("ProductQueue");

                entity.Property(e => e.ProductDescription)
                    .HasMaxLength(225)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .HasMaxLength(225)
                    .IsUnicode(false);

                entity.Property(e => e.ProductPostDate).HasColumnType("datetime");

                entity.Property(e => e.QueueOperation)
                    .HasMaxLength(225)
                    .IsUnicode(false);

                entity.Property(e => e.QueuePostDate).HasColumnType("datetime");

                entity.Property(e => e.QueueReason)
                    .HasMaxLength(225)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
