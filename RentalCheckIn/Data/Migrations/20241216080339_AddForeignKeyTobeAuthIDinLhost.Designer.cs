﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentalCheckIn.Data;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241216080339_AddForeignKeyTobeAuthIDinLhost")]
    partial class AddForeignKeyTobeAuthIDinLhost
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("RentalCheckIn.Entities.Apartment", b =>
                {
                    b.Property<uint>("ApartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("ApartmentId"));

                    b.Property<string>("ApartmentName")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)");

                    b.Property<string>("ApartmentType")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ApartmentId")
                        .HasName("PRIMARY");

                    b.ToTable("apartment", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.ApartmentTranslation", b =>
                {
                    b.Property<uint>("ApartmentTranslationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("ApartmentTranslationId"));

                    b.Property<uint>("ApartmentId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("ApartmentName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<uint>("LanguageId")
                        .HasColumnType("int unsigned");

                    b.HasKey("ApartmentTranslationId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("ApartmentId", "LanguageId")
                        .IsUnique();

                    b.ToTable("ApartmentTranslation", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Channel", b =>
                {
                    b.Property<uint>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("ChannelId"));

                    b.Property<string>("ChannelLabel")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("ChannelId")
                        .HasName("PRIMARY");

                    b.ToTable("channel", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Country", b =>
                {
                    b.Property<uint>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("CountryId"));

                    b.Property<string>("CountryIso2")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("varchar(2)")
                        .HasColumnName("CountryISO2")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("CountryIso2"), "utf8mb3");

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("CountryName"), "utf8mb3");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    MySqlPropertyBuilderExtensions.UseMySqlComputedColumn(b.Property<DateTime>("CreationDate"));

                    b.Property<string>("Dial")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Dial"), "utf8mb3");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<uint?>("LanguageId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("Nationality")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Nationality"), "utf8mb3");

                    b.HasKey("CountryId")
                        .HasName("PRIMARY");

                    b.ToTable("country", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Currency", b =>
                {
                    b.Property<uint>("CurrencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("CurrencyId"));

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("CurrencyLabel")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("CurrencySymbol")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("CurrencyId")
                        .HasName("PRIMARY");

                    b.ToTable("currency", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.LHost", b =>
                {
                    b.Property<uint>("HostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("HostId"));

                    b.Property<uint>("AuthenticatorId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("AuthenticatorType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("EmailVTokenExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EmailVerificationToken")
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("IsBlockedSince")
                        .HasColumnType("datetime");

                    b.Property<sbyte>("IsDisabled")
                        .HasColumnType("tinyint");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<byte>("LoginAttempts")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("MailAddress")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .UseCollation("utf8mb3_bin");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("PasswordHash"), "utf8mb3");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TotpSecret")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte[]>("UserHandle")
                        .HasColumnType("longblob");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Username"), "utf8mb3");

                    b.HasKey("HostId")
                        .HasName("PRIMARY");

                    b.ToTable("lhost", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.LHostCredential", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("Id"));

                    b.Property<string>("AuthenticatorType")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("CredentialId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<uint>("HostId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("PublicKey")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long>("SignCount")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("HostId");

                    b.ToTable("LHostCredential", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Language", b =>
                {
                    b.Property<uint>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("LanguageId"));

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    MySqlPropertyBuilderExtensions.UseMySqlComputedColumn(b.Property<DateTime>("CreationDate"));

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Culture"), "utf8mb3");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LanguageCode")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("varchar(2)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("LanguageCode"), "utf8mb3");

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("LanguageName"), "utf8mb3");

                    b.Property<string>("Svg")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("svg")
                        .UseCollation("utf8mb3_general_ci");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Svg"), "utf8mb3");

                    b.HasKey("LanguageId")
                        .HasName("PRIMARY");

                    b.ToTable("language", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Quest", b =>
                {
                    b.Property<uint>("QuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("QuestId"));

                    b.Property<uint?>("CountryId")
                        .HasColumnType("int unsigned");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<uint?>("LanguageId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("MailAddress")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Mobile")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("PassportNr")
                        .HasMaxLength(60)
                        .HasColumnType("varchar(60)");

                    b.HasKey("QuestId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "CountryId" }, "FK_quest_country_countryId");

                    b.HasIndex(new[] { "LanguageId" }, "FK_quest_language_languageId");

                    b.ToTable("quest", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.RefreshToken", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime(6)");

                    b.Property<uint>("HostId")
                        .HasColumnType("int unsigned");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("HostId");

                    b.ToTable("RefreshToken", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Reservation", b =>
                {
                    b.Property<uint>("ReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("ReservationId"));

                    b.Property<bool>("AgreeEnergyConsumption")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("AgreeTerms")
                        .HasColumnType("tinyint(1)");

                    b.Property<decimal>("ApartmentFee")
                        .HasPrecision(9, 2)
                        .HasColumnType("decimal(9,2)");

                    b.Property<uint>("ApartmentId")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("ChannelId")
                        .HasColumnType("int unsigned");

                    b.Property<DateOnly>("CheckInDate")
                        .HasColumnType("date");

                    b.Property<string>("CheckInTime")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<DateOnly>("CheckOutDate")
                        .HasColumnType("date");

                    b.Property<string>("CheckOutTime")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<DateTime?>("CheckedInAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CheckedOutAt")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal?>("CostsPerXtraKwh")
                        .HasPrecision(5, 2)
                        .HasColumnType("decimal(5,2)");

                    b.Property<DateTime?>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<uint>("CurrencyId")
                        .HasColumnType("int unsigned");

                    b.Property<uint?>("HostId")
                        .HasColumnType("int unsigned");

                    b.Property<int>("KwhAtCheckIn")
                        .HasColumnType("int");

                    b.Property<int?>("KwhAtCheckOut")
                        .HasColumnType("int");

                    b.Property<int>("KwhPerNightIncluded")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<int>("NumberOfNights")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfQuests")
                        .HasColumnType("int");

                    b.Property<uint>("QuestId")
                        .HasColumnType("int unsigned");

                    b.Property<bool>("ReceivedKeys")
                        .HasColumnType("tinyint(1)");

                    b.Property<decimal>("SecurityDeposit")
                        .HasPrecision(9, 2)
                        .HasColumnType("decimal(9,2)");

                    b.Property<string>("SignatureQuest")
                        .HasColumnType("text");

                    b.Property<uint>("StatusId")
                        .HasColumnType("int unsigned");

                    b.Property<decimal?>("TotalPrice")
                        .HasPrecision(9, 2)
                        .HasColumnType("decimal(9,2)");

                    b.HasKey("ReservationId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "ApartmentId" }, "FK_reservation_apartment_apartmentId");

                    b.HasIndex(new[] { "ChannelId" }, "FK_reservation_channel_channelId");

                    b.HasIndex(new[] { "CurrencyId" }, "FK_reservation_currency_currencyId");

                    b.HasIndex(new[] { "HostId" }, "FK_reservation_host_hostId");

                    b.HasIndex(new[] { "QuestId" }, "FK_reservation_quest_questId");

                    b.HasIndex(new[] { "StatusId" }, "FK_reservation_status_statusId");

                    b.ToTable("reservation", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.ReservationTranslation", b =>
                {
                    b.Property<uint>("ReservationTranslationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("ReservationTranslationId"));

                    b.Property<string>("CheckInTime")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CheckOutTime")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<uint>("LanguageId")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("ReservationId")
                        .HasColumnType("int unsigned");

                    b.HasKey("ReservationTranslationId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("ReservationId", "LanguageId")
                        .IsUnique();

                    b.ToTable("ReservationTranslation", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Setting", b =>
                {
                    b.Property<uint>("SettingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("SettingId"));

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("MaxLoginAttempts")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("'5'");

                    b.Property<uint>("RowsPerPage")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned")
                        .HasDefaultValueSql("'5'");

                    b.HasKey("SettingId")
                        .HasName("PRIMARY");

                    b.ToTable("setting", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Status", b =>
                {
                    b.Property<uint>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("StatusId"));

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("StatusLabel")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("StatusId")
                        .HasName("PRIMARY");

                    b.ToTable("status", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.StatusTranslation", b =>
                {
                    b.Property<uint>("StatusTranslationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<uint>("StatusTranslationId"));

                    b.Property<uint>("LanguageId")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("StatusId")
                        .HasColumnType("int unsigned");

                    b.Property<string>("StatusLabel")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("StatusTranslationId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("StatusId", "LanguageId")
                        .IsUnique();

                    b.ToTable("StatusTranslation", (string)null);
                });

            modelBuilder.Entity("RentalCheckIn.Entities.ApartmentTranslation", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.Apartment", "Apartment")
                        .WithMany("ApartmentTranslations")
                        .HasForeignKey("ApartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RentalCheckIn.Entities.Language", "Language")
                        .WithMany("ApartmentTranslations")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.LHostCredential", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.LHost", "Host")
                        .WithMany("Credentials")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Host");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Quest", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.Country", "Country")
                        .WithMany("Quests")
                        .HasForeignKey("CountryId")
                        .HasConstraintName("FK_quest_country_countryId");

                    b.HasOne("RentalCheckIn.Entities.Language", "Language")
                        .WithMany("Quests")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_quest_language_languageId");

                    b.Navigation("Country");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.RefreshToken", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.LHost", "LHost")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LHost");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Reservation", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.Apartment", "Apartment")
                        .WithMany("Reservations")
                        .HasForeignKey("ApartmentId")
                        .IsRequired()
                        .HasConstraintName("FK_reservation_listing_listingId");

                    b.HasOne("RentalCheckIn.Entities.Channel", "Channel")
                        .WithMany("Reservations")
                        .HasForeignKey("ChannelId")
                        .IsRequired()
                        .HasConstraintName("FK_reservation_channel_channelId");

                    b.HasOne("RentalCheckIn.Entities.Currency", "Currency")
                        .WithMany("Reservations")
                        .HasForeignKey("CurrencyId")
                        .IsRequired()
                        .HasConstraintName("FK_reservation_currency_currencyId");

                    b.HasOne("RentalCheckIn.Entities.LHost", "Host")
                        .WithMany("Reservations")
                        .HasForeignKey("HostId")
                        .HasConstraintName("FK_reservation_host_hostId");

                    b.HasOne("RentalCheckIn.Entities.Quest", "Quest")
                        .WithMany("Reservations")
                        .HasForeignKey("QuestId")
                        .IsRequired()
                        .HasConstraintName("FK_reservation_quest_questId");

                    b.HasOne("RentalCheckIn.Entities.Status", "Status")
                        .WithMany("Reservations")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_reservation_status_statusId");

                    b.Navigation("Apartment");

                    b.Navigation("Channel");

                    b.Navigation("Currency");

                    b.Navigation("Host");

                    b.Navigation("Quest");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.ReservationTranslation", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.Language", "Language")
                        .WithMany("ReservationTranslations")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RentalCheckIn.Entities.Reservation", "Reservation")
                        .WithMany("ReservationTranslations")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.StatusTranslation", b =>
                {
                    b.HasOne("RentalCheckIn.Entities.Language", "Language")
                        .WithMany("StatusTranslations")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RentalCheckIn.Entities.Status", "Status")
                        .WithMany("StatusTranslations")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Apartment", b =>
                {
                    b.Navigation("ApartmentTranslations");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Channel", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Country", b =>
                {
                    b.Navigation("Quests");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Currency", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.LHost", b =>
                {
                    b.Navigation("Credentials");

                    b.Navigation("RefreshTokens");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Language", b =>
                {
                    b.Navigation("ApartmentTranslations");

                    b.Navigation("Quests");

                    b.Navigation("ReservationTranslations");

                    b.Navigation("StatusTranslations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Quest", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Reservation", b =>
                {
                    b.Navigation("ReservationTranslations");
                });

            modelBuilder.Entity("RentalCheckIn.Entities.Status", b =>
                {
                    b.Navigation("Reservations");

                    b.Navigation("StatusTranslations");
                });
#pragma warning restore 612, 618
        }
    }
}
