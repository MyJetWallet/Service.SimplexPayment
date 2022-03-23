﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.SimplexPayment.Postgres;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220323085301_Fee")]
    partial class Fee
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("simplex")
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Service.SimplexPayment.Domain.Models.SimplexIntention", b =>
                {
                    b.Property<string>("QuoteId")
                        .HasColumnType("text");

                    b.Property<decimal>("BaseFiatAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("BlockchainTxHash")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<string>("ClientIdHash")
                        .HasColumnType("text");

                    b.Property<string>("ClientIp")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ErrorText")
                        .HasColumnType("text");

                    b.Property<decimal>("Fee")
                        .HasColumnType("numeric");

                    b.Property<decimal>("FromAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("FromCurrency")
                        .HasColumnType("text");

                    b.Property<string>("OrderId")
                        .HasColumnType("text");

                    b.Property<string>("PaymentId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<decimal>("ToAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("ToAsset")
                        .HasColumnType("text");

                    b.Property<decimal>("TotalFiatAmount")
                        .HasColumnType("numeric");

                    b.HasKey("QuoteId");

                    b.HasIndex("ClientIdHash");

                    b.HasIndex("PaymentId");

                    b.ToTable("intentions", "simplex");
                });
#pragma warning restore 612, 618
        }
    }
}
