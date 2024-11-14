﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using src.Data;

#nullable disable

namespace se24.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("src.Games.FinderGame.Level", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Difficulty")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GivenTime")
                        .HasColumnType("integer");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FinderLevels");
                });

            modelBuilder.Entity("src.Games.ReadingGame.ReadingLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<int>("ReadingTime")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ReadingLevels");
                });

            modelBuilder.Entity("src.Games.ReadingGame.ReadingQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string[]>("Answers")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<int>("CorrectAnswer")
                        .HasColumnType("integer");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ReadingLevelId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReadingLevelId");

                    b.ToTable("ReadingQuestions");
                });

            modelBuilder.Entity("src.Shared.GameObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsFound")
                        .HasColumnType("boolean");

                    b.Property<int?>("LevelId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LevelId");

                    b.ToTable("FinderLevelGameObjects");
                });

            modelBuilder.Entity("src.Games.ReadingGame.ReadingQuestion", b =>
                {
                    b.HasOne("src.Games.ReadingGame.ReadingLevel", "ReadingLevel")
                        .WithMany("Questions")
                        .HasForeignKey("ReadingLevelId");

                    b.Navigation("ReadingLevel");
                });

            modelBuilder.Entity("src.Shared.GameObject", b =>
                {
                    b.HasOne("src.Games.FinderGame.Level", "Level")
                        .WithMany("GameObjects")
                        .HasForeignKey("LevelId");

                    b.OwnsOne("src.Shared.Position", "Position", b1 =>
                        {
                            b1.Property<int>("GameObjectId")
                                .HasColumnType("integer");

                            b1.Property<int>("X")
                                .HasColumnType("integer");

                            b1.Property<int>("Y")
                                .HasColumnType("integer");

                            b1.HasKey("GameObjectId");

                            b1.ToTable("FinderLevelGameObjects");

                            b1.WithOwner()
                                .HasForeignKey("GameObjectId");
                        });

                    b.Navigation("Level");

                    b.Navigation("Position")
                        .IsRequired();
                });

            modelBuilder.Entity("src.Games.FinderGame.Level", b =>
                {
                    b.Navigation("GameObjects");
                });

            modelBuilder.Entity("src.Games.ReadingGame.ReadingLevel", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
