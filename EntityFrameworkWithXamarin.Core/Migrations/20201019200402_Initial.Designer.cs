// <auto-generated />
using System;
using EntityFrameworkWithXamarin.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkWithXamarin.Core.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20201019200402_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("EntityFrameworkWithXamarin.Core.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("EntityFrameworkWithXamarin.Core.Reguest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("RGSt")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Reguests");
                });

            modelBuilder.Entity("EntityFrameworkWithXamarin.Core.TableItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("A")
                        .HasColumnType("TEXT");

                    b.Property<string>("Cost")
                        .HasColumnType("TEXT");

                    b.Property<string>("Locacion")
                        .HasColumnType("TEXT");

                    b.Property<string>("Quele")
                        .HasColumnType("TEXT");

                    b.Property<int>("ReguestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Txt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ReguestId");

                    b.HasIndex("UserId");

                    b.ToTable("TableItems");
                });

            modelBuilder.Entity("EntityFrameworkWithXamarin.Core.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EntityFrameworkWithXamarin.Core.Reguest", b =>
                {
                    b.HasOne("EntityFrameworkWithXamarin.Core.User", "User")
                        .WithMany("Reguests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EntityFrameworkWithXamarin.Core.TableItem", b =>
                {
                    b.HasOne("EntityFrameworkWithXamarin.Core.Reguest", "Reguest")
                        .WithMany("TdItems")
                        .HasForeignKey("ReguestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkWithXamarin.Core.User", null)
                        .WithMany("tbItems")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
