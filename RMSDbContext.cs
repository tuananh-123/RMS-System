using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RMS.Entities;

namespace RMS;

public class RMSDbContext(DbContextOptions<RMSDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<RecipeHistory> RecipeHistories { get; set; }
    public DbSet<TagForRecipe> TagForRecipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1 - n Recipe - RecipeHistory
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.RecipeHistories)
            .WithOne(rh => rh.Recipe)
            .HasForeignKey(rh => rh.RecipeID)
            .OnDelete(DeleteBehavior.Restrict);

        // định nghĩa enum dưới dạng int trong database
        modelBuilder.Entity<Recipe>()
            .Property(p => p.Nation).HasColumnType("int");
        modelBuilder.Entity<Recipe>()
            .Property(p => p.Difficulty).HasColumnType("int");

        modelBuilder.Entity<RecipeHistory>()
            .Property(p => p.Nation).HasColumnType("int");
        modelBuilder.Entity<RecipeHistory>()
            .Property(p => p.Difficulty).HasColumnType("int");

        // n - n Recipe - Ingredient
        // 1 - n Recipe - RecipeIngredient
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.RecipeIngredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeID)
            .OnDelete(DeleteBehavior.Cascade);
        // 1 - n Ingredient - RecipeIngredient
        modelBuilder.Entity<Ingredient>()
            .HasMany(i => i.RecipeIngredients)
            .WithOne(ri => ri.Ingredient)
            .HasForeignKey(ri => ri.IngredientID)
            .OnDelete(DeleteBehavior.Cascade);

        // n - n Recipe - Tag
        // 1 - n Recipe - RecipeTag
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.TagForRecipes)
            .WithOne(rt => rt.Recipe)
            .HasForeignKey(rt => rt.RecipeID)
            .OnDelete(DeleteBehavior.Cascade);
        // 1 - n Tag - RecipeTag
        modelBuilder.Entity<Tag>()
            .HasMany(t => t.TagForRecipes)
            .WithOne(rt => rt.Tag)
            .HasForeignKey(rt => rt.TagID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recipe>()
            .Property(r => r.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken().HasColumnType("bytea");

        var searchKeywordConverter = new ValueConverter<SearchKeyword?, string>(
            sk => JsonSerializer.Serialize(sk),
            json => JsonSerializer.Deserialize<SearchKeyword>(json));

        // khai báo SearchKeyword là jsonb
        modelBuilder.Entity<Recipe>().Property(r => r.SearchKeyword).HasColumnType("jsonb").HasConversion(searchKeywordConverter);
        
        modelBuilder.Entity<Ingredient>().Property(i => i.SearchKeyword).HasColumnType("jsonb").HasConversion(searchKeywordConverter);

        // Composite key cho RecipeIngredient
        modelBuilder.Entity<RecipeIngredient>().HasKey(ri => new { ri.RecipeID, ri.IngredientID });

        // Composite key cho RecipeTag
        modelBuilder.Entity<TagForRecipe>().HasKey(rt => new { rt.RecipeID, rt.TagID });
            
    }
}