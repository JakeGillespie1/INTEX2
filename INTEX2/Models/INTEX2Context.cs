using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace INTEX2.Models;

public partial class INTEX2Context : DbContext
{
    public INTEX2Context()
    {
    }

    public INTEX2Context(DbContextOptions<INTEX2Context> options)
        : base(options)
    {
    }
    public DbSet<AppUser> Users { get; set; }

    public virtual DbSet<AuthUserRecommendation> AuthUserRecommendations { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<LineItem> LineItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    => optionsBuilder.UseSqlServer("Server=tcp:myfreeintex2sqlserver.database.windows.net,1433;Initial Catalog=INTEX2db;Persist Security Info=False;User ID=jake123gillespie;Password=db65!xC#I<3You:);MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUserRecommendation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuthUserRecommendation");

            entity.HasIndex(e => e.Index, "ix_AuthUserRecommendation_index");

            entity.Property(e => e.IfYouBought).IsUnicode(false);
            entity.Property(e => e.Rec1).IsUnicode(false);
            entity.Property(e => e.Rec2).IsUnicode(false);
            entity.Property(e => e.Rec3).IsUnicode(false);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B0ED9CCD6");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasMany(d => d.Products).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategory",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductCa__Produ__2180FB33"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductCa__Categ__208CD6FA"),
                    j =>
                    {
                        j.HasKey("CategoryId", "ProductId").HasName("PK__ProductC__D249F645A626447B");
                        j.ToTable("ProductCategory");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("CategoryID");
                        j.IndexerProperty<int>("ProductId").HasColumnName("ProductID");
                    });
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_Customers_CustomerID");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.BirthDate)
                .HasMaxLength(50)
                .HasColumnName("birth_date");
            entity.Property(e => e.CountryOfResidence)
                .HasMaxLength(50)
                .HasColumnName("country_of_residence");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
        });

        modelBuilder.Entity<LineItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId });

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Order).WithMany(p => p.LineItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderID");

            entity.HasOne(d => d.Product).WithMany(p => p.LineItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductID");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_Orders_OrderID");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Bank)
                .HasMaxLength(50)
                .HasColumnName("bank");
            entity.Property(e => e.CountryOfTransaction)
                .HasMaxLength(50)
                .HasColumnName("country_of_transaction");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Date)
                .HasMaxLength(50)
                .HasColumnName("date");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(50)
                .HasColumnName("day_of_week");
            entity.Property(e => e.EntryMode)
                .HasMaxLength(50)
                .HasColumnName("entry_mode");
            entity.Property(e => e.Fraud).HasColumnName("fraud");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(50)
                .HasColumnName("shipping_address");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.TypeOfCard)
                .HasMaxLength(50)
                .HasColumnName("type_of_card");
            entity.Property(e => e.TypeOfTransaction)
                .HasMaxLength(50)
                .HasColumnName("type_of_transaction");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerID");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.Description)
                .HasMaxLength(2800)
                .HasColumnName("description");
            entity.Property(e => e.ImgLink)
                .HasMaxLength(150)
                .HasColumnName("img_link");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NumParts).HasColumnName("num_parts");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.PrimaryColor)
                .HasMaxLength(50)
                .HasColumnName("primary_color");
            entity.Property(e => e.SecondaryColor)
                .HasMaxLength(50)
                .HasColumnName("secondary_color");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
