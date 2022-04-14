using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OnlineShopBack.Models
{
    public partial class OnlineShopContext : DbContext
    {
        public OnlineShopContext()
        {
        }

        public OnlineShopContext(DbContextOptions<OnlineShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TAccount> TAccount { get; set; }
        public virtual DbSet<TMember> TMember { get; set; }
        public virtual DbSet<TMyFavourite> TMyFavourite { get; set; }
        public virtual DbSet<TProduct> TProduct { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TAccount>(entity =>
            {
                entity.HasKey(e => new { e.FId, e.FAcc });

                entity.ToTable("t_account");

                entity.Property(e => e.FId)
                    .HasColumnName("f_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FAcc)
                    .HasColumnName("f_acc")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FCreateDate)
                    .HasColumnName("f_createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FLevel).HasColumnName("f_level");

                entity.Property(e => e.FPwd)
                    .IsRequired()
                    .HasColumnName("f_pwd")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FUpdateDate)
                    .HasColumnName("f_updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TMember>(entity =>
            {
                entity.HasKey(e => new { e.FId, e.FAcc })
                    .HasName("PK_t_member_1");

                entity.ToTable("t_member");

                entity.Property(e => e.FId)
                    .HasColumnName("f_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FAcc)
                    .HasColumnName("f_acc")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FAddress)
                    .HasColumnName("f_address")
                    .HasMaxLength(100);

                entity.Property(e => e.FCreateDate)
                    .HasColumnName("f_createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FLevel).HasColumnName("f_level");

                entity.Property(e => e.FMail)
                    .IsRequired()
                    .HasColumnName("f_mail")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FName)
                    .HasColumnName("f_name")
                    .HasMaxLength(4);

                entity.Property(e => e.FPhone)
                    .IsRequired()
                    .HasColumnName("f_phone")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FPwd)
                    .IsRequired()
                    .HasColumnName("f_pwd")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FShopGold).HasColumnName("f_shopGold");

                entity.Property(e => e.FSuspension).HasColumnName("f_suspension");

                entity.Property(e => e.FUpdateDate)
                    .HasColumnName("f_updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TMyFavourite>(entity =>
            {
                entity.HasKey(e => new { e.FAcc, e.FProductId });

                entity.ToTable("t_myFavourite");

                entity.Property(e => e.FAcc)
                    .HasColumnName("f_acc")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FProductId).HasColumnName("f_productId");

                entity.Property(e => e.FCreateDate)
                    .HasColumnName("f_createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TProduct>(entity =>
            {
                entity.HasKey(e => new { e.FId, e.FNum })
                    .HasName("PK_t_product_1");

                entity.ToTable("t_product");

                entity.Property(e => e.FId)
                    .HasColumnName("f_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FNum)
                    .HasColumnName("f_num")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FCategory).HasColumnName("f_category");

                entity.Property(e => e.FContent)
                    .HasColumnName("f_content")
                    .HasMaxLength(500);

                entity.Property(e => e.FCreateDate)
                    .HasColumnName("f_createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FImg)
                    .IsRequired()
                    .HasColumnName("f_img")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("f_name")
                    .HasMaxLength(20);

                entity.Property(e => e.FPrice).HasColumnName("f_price");

                entity.Property(e => e.FStatus).HasColumnName("f_status");

                entity.Property(e => e.FStock).HasColumnName("f_stock");

                entity.Property(e => e.FUpdateDate)
                    .HasColumnName("f_updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
