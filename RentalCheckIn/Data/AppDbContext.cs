﻿namespace RentalCheckIn.Data;

public partial class AppDbContext : DbContext
{
    private readonly IConfiguration configuration;

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        this.configuration = configuration;
    }

    public virtual DbSet<Apartment> Apartments { get; set; }
    public virtual DbSet<ApartmentTranslation> ApartmentTranslations { get; set; }
    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<LHost> LHosts { get; set; }

    public virtual DbSet<Quest> Quests { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationTranslation> ReservationTranslations { get; set; }
    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }
    public DbSet<StatusTranslation> StatusTranslations { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<LHostCredential> LHostCredentials { get; set; }

    // It is said that OnConfiguring could be removed and we rely on the registered DbContext connection in program.cs. Evaluate.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            // Enable lazy loading;
            //.UseLazyLoadingProxies()
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.ApartmentId).HasName("PRIMARY");

            entity.ToTable("apartment");

            entity.Property(e => e.ApartmentName).HasMaxLength(80);
            entity.Property(e => e.ApartmentType).HasMaxLength(40);
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.ChannelId).HasName("PRIMARY");

            entity.ToTable("channel");

            entity.Property(e => e.ChannelLabel).HasMaxLength(40);
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PRIMARY");

            entity.ToTable("country");

            entity.Property(e => e.CountryIso2)
                .HasMaxLength(2)
                .HasColumnName("CountryISO2")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.CreationDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Dial)
                .HasMaxLength(10)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Nationality)
                .HasMaxLength(100)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PRIMARY");

            entity.ToTable("currency");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.CurrencyLabel).HasMaxLength(10);
            entity.Property(e => e.CurrencySymbol).HasMaxLength(50);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PRIMARY");

            entity.ToTable("language");

            entity.Property(e => e.CreationDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Culture)
                .HasMaxLength(20)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.LanguageCode)
                .HasMaxLength(2)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.LanguageName)
                .HasMaxLength(50)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Svg)
                .HasColumnType("text")
                .HasColumnName("svg")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<LHost>(entity =>
        {
            entity.HasKey(e => e.HostId).HasName("PRIMARY");

            entity.ToTable("lhost");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.IsBlockedSince).HasColumnType("datetime");
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.MailAddress).HasMaxLength(200);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(200)
                .UseCollation("utf8mb3_bin")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Username)
                .HasMaxLength(200)
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.QuestId).HasName("PRIMARY");

            entity.ToTable("quest");

            entity.HasIndex(e => e.CountryId, "FK_quest_country_countryId");

            entity.HasIndex(e => e.LanguageId, "FK_quest_language_languageId");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MailAddress).HasMaxLength(200);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.PassportNr).HasMaxLength(60);

            entity.HasOne(d => d.Country).WithMany(p => p.Quests)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_quest_country_countryId");

            entity.HasOne(d => d.Language).WithMany(p => p.Quests)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK_quest_language_languageId");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PRIMARY");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.ApartmentId, "FK_reservation_apartment_apartmentId");

            entity.HasIndex(e => e.ChannelId, "FK_reservation_channel_channelId");

            entity.HasIndex(e => e.CurrencyId, "FK_reservation_currency_currencyId");

            entity.HasIndex(e => e.HostId, "FK_reservation_host_hostId");

            entity.HasIndex(e => e.QuestId, "FK_reservation_quest_questId");

            entity.HasIndex(e => e.StatusId, "FK_reservation_status_statusId");

            entity.Property(e => e.ApartmentFee).HasPrecision(9, 2);
            entity.Property(e => e.CheckInTime).HasMaxLength(40);
            entity.Property(e => e.CheckOutTime).HasMaxLength(40);
            entity.Property(e => e.CostsPerXtraKwh).HasPrecision(5, 2);
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SecurityDeposit).HasPrecision(9, 2);
            entity.Property(e => e.SignatureQuest).HasColumnType("text");
            entity.Property(e => e.TotalPrice).HasPrecision(9, 2);
            // Changed the default precision
            entity.Property(e => e.CostsXtraRemotes).HasPrecision(18,2);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reservation_listing_listingId");

            entity.HasOne(d => d.Channel).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reservation_channel_channelId");

            entity.HasOne(d => d.Currency).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reservation_currency_currencyId");

            entity.HasOne(d => d.Host).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("FK_reservation_host_hostId");

            entity.HasOne(d => d.Quest).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.QuestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reservation_quest_questId");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_reservation_status_statusId");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PRIMARY");

            entity.ToTable("setting");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.MaxLoginAttempts).HasDefaultValueSql("'5'");
            entity.Property(e => e.RowsPerPage).HasDefaultValueSql("'5'");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("status");

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.StatusLabel).HasMaxLength(20);
        });

        // Define the relationship between Host and RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            // Renaming the table to change it to singular format
            entity.ToTable("RefreshToken");
            entity.HasKey(rt => rt.Id);

            entity.HasOne(rt => rt.LHost)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.HostId)
                // Delete tokens if Host is deleted
                .OnDelete(DeleteBehavior.Cascade); 
        });

        modelBuilder.Entity<LHostCredential>(entity =>
        {
            entity.ToTable("LHostCredential");
            entity.HasOne(c => c.Host)
                .WithMany(h => h.Credentials)
                .HasForeignKey(c => c.HostId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

        });

        // ApartmentTranslation configuration
        modelBuilder.Entity<ApartmentTranslation>(entity =>
        {
            entity.ToTable("ApartmentTranslation");

            entity.HasKey(e => e.ApartmentTranslationId);

            entity.Property(e => e.ApartmentName)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.HasIndex(e => new { e.ApartmentId, e.LanguageId })
                  .IsUnique();

            entity.HasOne(e => e.Apartment)
                  .WithMany(a => a.ApartmentTranslations)
                  .HasForeignKey(e => e.ApartmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Language)
                  .WithMany(l => l.ApartmentTranslations)
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // StatusTranslation configuration
        modelBuilder.Entity<StatusTranslation>(entity =>
        {
            entity.ToTable("StatusTranslation");

            entity.HasKey(e => e.StatusTranslationId);

            entity.Property(e => e.StatusLabel)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.HasIndex(e => new { e.StatusId, e.LanguageId })
                  .IsUnique();

            entity.HasOne(e => e.Status)
                  .WithMany(s => s.StatusTranslations)
                  .HasForeignKey(e => e.StatusId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Language)
                  .WithMany(l => l.StatusTranslations)
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ReservationTranslation configuration
        modelBuilder.Entity<ReservationTranslation>(entity =>
        {
            entity.ToTable("ReservationTranslation");

            entity.HasKey(e => e.ReservationTranslationId);

            entity.Property(e => e.CheckInTime)
                  .HasMaxLength(50);

            entity.Property(e => e.CheckOutTime)
                  .HasMaxLength(50);

            entity.HasIndex(e => new { e.ReservationId, e.LanguageId })
                  .IsUnique();

            entity.HasOne(e => e.Reservation)
                  .WithMany(r => r.ReservationTranslations)
                  .HasForeignKey(e => e.ReservationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Language)
                  .WithMany(l => l.ReservationTranslations)
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.Restrict);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
