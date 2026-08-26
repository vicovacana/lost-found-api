using System;
using Lost_Found.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Lost_Found.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260826191147_DodajKategorijuIGraduOglas")]
    partial class DodajKategorijuIGraduOglas
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Lost_Found.Models.Korisnik", b =>
                {
                    b.Property<int>("KorisnikId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("korisnikID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("KorisnikId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<string>("KorisnickoIme")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("korisnickoIme");

                    b.Property<string>("LozinkaHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("lozinka");

                    b.Property<int>("TipKorisnika")
                        .HasColumnType("integer")
                        .HasColumnName("tipKorisnika");

                    b.Property<DateTime>("VremeKreiranja")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("vremeKreiranja")
                        .HasDefaultValueSql("now()");

                    b.HasKey("KorisnikId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("KorisnickoIme")
                        .IsUnique();

                    b.ToTable("Korisnik", null, t =>
                        {
                            t.HasCheckConstraint("CK_Korisnik_TipKorisnika", "\"tipKorisnika\" IN (0, 1)");
                        });

                    b.HasDiscriminator<int>("TipKorisnika");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Lost_Found.Models.Oglas", b =>
                {
                    b.Property<int>("OglasId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("oglasID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OglasId"));

                    b.Property<int?>("AdminId")
                        .HasColumnType("integer")
                        .HasColumnName("adminID");

                    b.Property<DateTime>("DatumKreiranja")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumKreiranja")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Fotografija")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("fotografija");

                    b.Property<string>("Grad")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("grad");

                    b.Property<string>("Kategorija")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("kategorija");

                    b.Property<int>("KreatorId")
                        .HasColumnType("integer")
                        .HasColumnName("kreatorID");

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("decimal(10,8)")
                        .HasColumnName("latitude");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("decimal(11,8)")
                        .HasColumnName("longitude");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("naziv");

                    b.Property<string>("Opis")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)")
                        .HasColumnName("opis");

                    b.Property<int>("Tip")
                        .HasColumnType("integer")
                        .HasColumnName("tip");

                    b.HasKey("OglasId");

                    b.HasIndex("AdminId");

                    b.HasIndex("Grad");

                    b.HasIndex("Kategorija");

                    b.HasIndex("KreatorId");

                    b.HasIndex("Tip");

                    b.ToTable("Oglas", null, t =>
                        {
                            t.HasCheckConstraint("CK_Oglas_Latitude", "\"latitude\" IS NULL OR (\"latitude\" >= -90 AND \"latitude\" <= 90)");

                            t.HasCheckConstraint("CK_Oglas_Longitude", "\"longitude\" IS NULL OR (\"longitude\" >= -180 AND \"longitude\" <= 180)");

                            t.HasCheckConstraint("CK_Oglas_Tip", "\"tip\" IN (0, 1)");
                        });
                });

            modelBuilder.Entity("Lost_Found.Models.Poruka", b =>
                {
                    b.Property<int>("PorukaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("porukaID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PorukaId"));

                    b.Property<DateTime>("DatumKreiranja")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumKreiranja")
                        .HasDefaultValueSql("now()");

                    b.Property<int>("KorisnikId")
                        .HasColumnType("integer")
                        .HasColumnName("korisnikID");

                    b.Property<int>("RazgovorId")
                        .HasColumnType("integer")
                        .HasColumnName("razgovorID");

                    b.Property<string>("Sadrzaj")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)")
                        .HasColumnName("sadrzaj");

                    b.HasKey("PorukaId");

                    b.HasIndex("KorisnikId");

                    b.HasIndex("RazgovorId", "DatumKreiranja");

                    b.ToTable("Poruka", (string)null);
                });

            modelBuilder.Entity("Lost_Found.Models.Potrazivanje", b =>
                {
                    b.Property<int>("KorisnikId")
                        .HasColumnType("integer")
                        .HasColumnName("korisnikID");

                    b.Property<int>("OglasId")
                        .HasColumnType("integer")
                        .HasColumnName("oglasID");

                    b.Property<DateTime>("DatumKreiranja")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumKreiranja")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("DatumRazresavanja")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumRazresavanja");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("KorisnikId", "OglasId");

                    b.HasIndex("OglasId");

                    b.ToTable("Potrazivanje", null, t =>
                        {
                            t.HasCheckConstraint("CK_Potrazivanje_Status", "\"status\" IN (0, 1, 2)");
                        });
                });

            modelBuilder.Entity("Lost_Found.Models.Razgovor", b =>
                {
                    b.Property<int>("RazgovorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("razgovorID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RazgovorId"));

                    b.Property<DateTime>("DatumKreiranja")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datumKreiranja")
                        .HasDefaultValueSql("now()");

                    b.Property<int>("OglasId")
                        .HasColumnType("integer")
                        .HasColumnName("oglasID");

                    b.Property<int>("StatusRazgovora")
                        .HasColumnType("integer")
                        .HasColumnName("statusRazgovora");

                    b.HasKey("RazgovorId");

                    b.HasIndex("OglasId")
                        .IsUnique();

                    b.ToTable("Razgovor", null, t =>
                        {
                            t.HasCheckConstraint("CK_Razgovor_Status", "\"statusRazgovora\" IN (0, 1)");
                        });
                });

            modelBuilder.Entity("Lost_Found.Models.Admin", b =>
                {
                    b.HasBaseType("Lost_Found.Models.Korisnik");

                    b.ToTable(t =>
                        {
                            t.HasCheckConstraint("CK_Korisnik_TipKorisnika", "\"tipKorisnika\" IN (0, 1)");
                        });

                    b.HasDiscriminator().HasValue(1);

                    b.HasData(
                        new
                        {
                            KorisnikId = 1,
                            Email = "admin@lostfound.local",
                            KorisnickoIme = "admin",
                            LozinkaHash = "$2a$11$dp3ZWPpuZlt46loF6AoALutXdQDkMvmCRkRm6Kk5UZKbIRbtFEd8a",
                            VremeKreiranja = new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        });
                });

            modelBuilder.Entity("Lost_Found.Models.StandardniKorisnik", b =>
                {
                    b.HasBaseType("Lost_Found.Models.Korisnik");

                    b.ToTable(t =>
                        {
                            t.HasCheckConstraint("CK_Korisnik_TipKorisnika", "\"tipKorisnika\" IN (0, 1)");
                        });

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Lost_Found.Models.Oglas", b =>
                {
                    b.HasOne("Lost_Found.Models.Admin", "Admin")
                        .WithMany("NadgledaniOglasi")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Lost_Found.Models.StandardniKorisnik", "Kreator")
                        .WithMany("Oglasi")
                        .HasForeignKey("KreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");

                    b.Navigation("Kreator");
                });

            modelBuilder.Entity("Lost_Found.Models.Poruka", b =>
                {
                    b.HasOne("Lost_Found.Models.Korisnik", "Korisnik")
                        .WithMany("Poruke")
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lost_Found.Models.Razgovor", "Razgovor")
                        .WithMany("Poruke")
                        .HasForeignKey("RazgovorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Korisnik");

                    b.Navigation("Razgovor");
                });

            modelBuilder.Entity("Lost_Found.Models.Potrazivanje", b =>
                {
                    b.HasOne("Lost_Found.Models.StandardniKorisnik", "Korisnik")
                        .WithMany("Potrazivanja")
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lost_Found.Models.Oglas", "Oglas")
                        .WithMany("Potrazivanja")
                        .HasForeignKey("OglasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Korisnik");

                    b.Navigation("Oglas");
                });

            modelBuilder.Entity("Lost_Found.Models.Razgovor", b =>
                {
                    b.HasOne("Lost_Found.Models.Oglas", "Oglas")
                        .WithOne("Razgovor")
                        .HasForeignKey("Lost_Found.Models.Razgovor", "OglasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Oglas");
                });

            modelBuilder.Entity("Lost_Found.Models.Korisnik", b =>
                {
                    b.Navigation("Poruke");
                });

            modelBuilder.Entity("Lost_Found.Models.Oglas", b =>
                {
                    b.Navigation("Potrazivanja");

                    b.Navigation("Razgovor");
                });

            modelBuilder.Entity("Lost_Found.Models.Razgovor", b =>
                {
                    b.Navigation("Poruke");
                });

            modelBuilder.Entity("Lost_Found.Models.Admin", b =>
                {
                    b.Navigation("NadgledaniOglasi");
                });

            modelBuilder.Entity("Lost_Found.Models.StandardniKorisnik", b =>
                {
                    b.Navigation("Oglasi");

                    b.Navigation("Potrazivanja");
                });
#pragma warning restore 612, 618
        }
    }
}
