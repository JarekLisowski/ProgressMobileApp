using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Progress.Database;

public partial class NavireoDbContext : DbContext
{
    public NavireoDbContext()
    {
    }

    public NavireoDbContext(DbContextOptions<NavireoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdrEmail> AdrEmails { get; set; }

    public virtual DbSet<AdrEwid> AdrEwids { get; set; }

    public virtual DbSet<AdrHistorium> AdrHistoria { get; set; }

    public virtual DbSet<DksKasa> DksKasas { get; set; }

    public virtual DbSet<DokDokument> DokDokuments { get; set; }

    public virtual DbSet<DokPozycja> DokPozycjas { get; set; }

    public virtual DbSet<DokVat> DokVats { get; set; }

    public virtual DbSet<IfxApiFormaPlatnosci> IfxApiFormaPlatnoscis { get; set; }

    public virtual DbSet<IfxApiPromocjaGrupa> IfxApiPromocjaGrupas { get; set; }

    public virtual DbSet<IfxApiPromocjaGrupaZestaw> IfxApiPromocjaGrupaZestaws { get; set; }

    public virtual DbSet<IfxApiPromocjaPozycja> IfxApiPromocjaPozycjas { get; set; }

    public virtual DbSet<IfxApiPromocjaPozycjaTowar> IfxApiPromocjaPozycjaTowars { get; set; }

    public virtual DbSet<IfxApiPromocjaZestaw> IfxApiPromocjaZestaws { get; set; }

    public virtual DbSet<IfxApiSposobDostawy> IfxApiSposobDostawies { get; set; }

    public virtual DbSet<IfxApiUzytkownik> IfxApiUzytkowniks { get; set; }

    public virtual DbSet<IfxApiUzytkownikPoziomyCenowe> IfxApiUzytkownikPoziomyCenowes { get; set; }

    public virtual DbSet<KhAdresyDostawy> KhAdresyDostawies { get; set; }

    public virtual DbSet<KhCechaKh> KhCechaKhs { get; set; }

    public virtual DbSet<KhKontrahent> KhKontrahents { get; set; }

    public virtual DbSet<KhPracownik> KhPracowniks { get; set; }

    public virtual DbSet<NzFinanse> NzFinanses { get; set; }

    public virtual DbSet<PdPodmiot> PdPodmiots { get; set; }

    public virtual DbSet<PdUzytkownik> PdUzytkowniks { get; set; }

    public virtual DbSet<SlBank> SlBanks { get; set; }

    public virtual DbSet<SlCechaKh> SlCechaKhs { get; set; }

    public virtual DbSet<SlCechaTw> SlCechaTws { get; set; }

    public virtual DbSet<SlGrupaKh> SlGrupaKhs { get; set; }

    public virtual DbSet<SlGrupaTw> SlGrupaTws { get; set; }

    public virtual DbSet<SlKategorium> SlKategoria { get; set; }

    public virtual DbSet<SlMagazyn> SlMagazyns { get; set; }

    public virtual DbSet<SlStawkaVat> SlStawkaVats { get; set; }

    public virtual DbSet<SlWlasny> SlWlasnies { get; set; }

    public virtual DbSet<TwCechaTw> TwCechaTws { get; set; }

    public virtual DbSet<TwCena> TwCenas { get; set; }

    public virtual DbSet<TwJednMiary> TwJednMiaries { get; set; }

    public virtual DbSet<TwKodKreskowy> TwKodKreskowies { get; set; }

    public virtual DbSet<TwKomplet> TwKomplets { get; set; }

    public virtual DbSet<TwStan> TwStans { get; set; }

    public virtual DbSet<TwTowar> TwTowars { get; set; }

    public virtual DbSet<TwZdjecieTw> TwZdjecieTws { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Trusted_Connection=True;Database=PH_Progress;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Polish_CI_AS");

        modelBuilder.Entity<AdrEmail>(entity =>
        {
            entity.HasKey(e => e.AmId);

            entity.ToTable("adr_Email", tb =>
                {
                    tb.HasTrigger("tr_AdrEmail_Deleting");
                    tb.HasTrigger("tr_AdrEmail_Inserting");
                    tb.HasTrigger("tr_AdrEmail_Updating");
                });

            entity.Property(e => e.AmId)
                .ValueGeneratedNever()
                .HasColumnName("am_Id");
            entity.Property(e => e.AmEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("am_Email");
            entity.Property(e => e.AmIdAdres).HasColumnName("am_IdAdres");
            entity.Property(e => e.AmPodstawowy).HasColumnName("am_Podstawowy");
            entity.Property(e => e.AmRodzaj)
                .HasDefaultValue(1)
                .HasColumnName("am_Rodzaj");

            entity.HasOne(d => d.AmIdAdresNavigation).WithMany(p => p.AdrEmails)
                .HasForeignKey(d => d.AmIdAdres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adr_Email_adr__Ewid");
        });

        modelBuilder.Entity<AdrEwid>(entity =>
        {
            entity.HasKey(e => e.AdrId);

            entity.ToTable("adr__Ewid", tb =>
                {
                    tb.HasTrigger("tr_AdrEwidPK_Updating");
                    tb.HasTrigger("tr_AdrEwid_Deleting");
                    tb.HasTrigger("tr_AdrEwid_Inserting");
                    tb.HasTrigger("tr_AdrEwid_Updating");
                });

            entity.HasIndex(e => new { e.AdrIdObiektu, e.AdrTypAdresu }, "IX_adr__Ewid").IsUnique();

            entity.HasIndex(e => new { e.AdrId, e.AdrNip }, "IX_adr__Ewid_1");

            entity.HasIndex(e => e.AdrMiejscowosc, "IX_adr__Ewid_Miejscowosc");

            entity.HasIndex(e => e.AdrTypAdresu, "IX_adr__Ewid_Podmiot");

            entity.Property(e => e.AdrId)
                .ValueGeneratedNever()
                .HasColumnName("adr_Id");
            entity.Property(e => e.AdrAdres)
                .HasMaxLength(82)
                .IsUnicode(false)
                .HasComputedColumnSql("(case when [adr_NrLokalu]<>'' then ((([adr_Ulica]+' ')+[adr_NrDomu])+'/')+[adr_NrLokalu] else ([adr_Ulica]+' ')+[adr_NrDomu] end)", false)
                .HasColumnName("adr_Adres");
            entity.Property(e => e.AdrDataZmiany)
                .HasColumnType("datetime")
                .HasColumnName("adr_DataZmiany");
            entity.Property(e => e.AdrFaks)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Faks");
            entity.Property(e => e.AdrGmina)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Gmina");
            entity.Property(e => e.AdrIdGminy).HasColumnName("adr_IdGminy");
            entity.Property(e => e.AdrIdObiektu).HasColumnName("adr_IdObiektu");
            entity.Property(e => e.AdrIdPanstwo).HasColumnName("adr_IdPanstwo");
            entity.Property(e => e.AdrIdWersja).HasColumnName("adr_IdWersja");
            entity.Property(e => e.AdrIdWewPodmiot3KseF)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("adr_IdWewPodmiot3KSeF");
            entity.Property(e => e.AdrIdWojewodztwo).HasColumnName("adr_IdWojewodztwo");
            entity.Property(e => e.AdrIdZmienil).HasColumnName("adr_IdZmienil");
            entity.Property(e => e.AdrKod)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Kod");
            entity.Property(e => e.AdrMiejscowosc)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Miejscowosc");
            entity.Property(e => e.AdrNazwa)
                .HasMaxLength(53)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Nazwa");
            entity.Property(e => e.AdrNazwaPelna)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_NazwaPelna");
            entity.Property(e => e.AdrNip)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_NIP");
            entity.Property(e => e.AdrNrDomu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_NrDomu");
            entity.Property(e => e.AdrNrEori)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_NrEORI");
            entity.Property(e => e.AdrNrLokalu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_NrLokalu");
            entity.Property(e => e.AdrNrUrzeduSkarbowego)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("adr_NrUrzeduSkarbowego");
            entity.Property(e => e.AdrPoczta)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Poczta");
            entity.Property(e => e.AdrPowiat)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Powiat");
            entity.Property(e => e.AdrSkrytka)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Skrytka");
            entity.Property(e => e.AdrSymbol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Symbol");
            entity.Property(e => e.AdrTelefon)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Telefon");
            entity.Property(e => e.AdrTypAdresu).HasColumnName("adr_TypAdresu");
            entity.Property(e => e.AdrUlica)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adr_Ulica");
        });

        modelBuilder.Entity<AdrHistorium>(entity =>
        {
            entity.HasKey(e => e.AdrhId);

            entity.ToTable("adr_Historia");

            entity.HasIndex(e => new { e.AdrhIdAdresu, e.AdrhId }, "IX_adr_Historia");

            entity.Property(e => e.AdrhId)
                .ValueGeneratedNever()
                .HasColumnName("adrh_Id");
            entity.Property(e => e.AdrhAdres)
                .HasMaxLength(82)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Adres");
            entity.Property(e => e.AdrhDataZmiany)
                .HasColumnType("datetime")
                .HasColumnName("adrh_DataZmiany");
            entity.Property(e => e.AdrhFaks)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Faks");
            entity.Property(e => e.AdrhGmina)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Gmina");
            entity.Property(e => e.AdrhIdAdresu).HasColumnName("adrh_IdAdresu");
            entity.Property(e => e.AdrhIdGminy).HasColumnName("adrh_IdGminy");
            entity.Property(e => e.AdrhIdPanstwo)
                .HasDefaultValue(1)
                .HasColumnName("adrh_IdPanstwo");
            entity.Property(e => e.AdrhIdWersja).HasColumnName("adrh_IdWersja");
            entity.Property(e => e.AdrhIdWewPodmiot3KseF)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("adrh_IdWewPodmiot3KSeF");
            entity.Property(e => e.AdrhIdWojewodztwo).HasColumnName("adrh_IdWojewodztwo");
            entity.Property(e => e.AdrhIdZmienil).HasColumnName("adrh_IdZmienil");
            entity.Property(e => e.AdrhKod)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Kod");
            entity.Property(e => e.AdrhMiejscowosc)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Miejscowosc");
            entity.Property(e => e.AdrhNazwa)
                .HasMaxLength(53)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Nazwa");
            entity.Property(e => e.AdrhNazwaPelna)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("adrh_NazwaPelna");
            entity.Property(e => e.AdrhNip)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_NIP");
            entity.Property(e => e.AdrhNrDomu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_NrDomu");
            entity.Property(e => e.AdrhNrEori)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_NrEORI");
            entity.Property(e => e.AdrhNrLokalu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_NrLokalu");
            entity.Property(e => e.AdrhNrUrzeduSkarbowego)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("adrh_NrUrzeduSkarbowego");
            entity.Property(e => e.AdrhPoczta)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Poczta");
            entity.Property(e => e.AdrhPowiat)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Powiat");
            entity.Property(e => e.AdrhSkrytka)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Skrytka");
            entity.Property(e => e.AdrhSymbol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Symbol");
            entity.Property(e => e.AdrhTelefon)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Telefon");
            entity.Property(e => e.AdrhUlica)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("adrh_Ulica");

            entity.HasOne(d => d.AdrhIdAdresuNavigation).WithMany(p => p.AdrHistoria)
                .HasForeignKey(d => d.AdrhIdAdresu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adr_Historia_adr__Ewid");
        });

        modelBuilder.Entity<DksKasa>(entity =>
        {
            entity.HasKey(e => e.KsId).HasName("PK_ks_Kasa");

            entity.ToTable("dks_Kasa", tb => tb.HasTrigger("tr_dks_Kasa_Deleting"));

            entity.HasIndex(e => e.KsSymbol, "UX_Symbol").IsUnique();

            entity.Property(e => e.KsId)
                .ValueGeneratedNever()
                .HasColumnName("ks_Id");
            entity.Property(e => e.KsAnalityka)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("ks_Analityka");
            entity.Property(e => e.KsGlowna).HasColumnName("ks_Glowna");
            entity.Property(e => e.KsNazwa)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("ks_Nazwa");
            entity.Property(e => e.KsOpis)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ks_Opis");
            entity.Property(e => e.KsRkAutoDodawanie)
                .HasDefaultValue(1)
                .HasColumnName("ks_RK_AutoDodawanie");
            entity.Property(e => e.KsRkFormatNumeru).HasColumnName("ks_RK_FormatNumeru");
            entity.Property(e => e.KsRkKategoria).HasColumnName("ks_RK_Kategoria");
            entity.Property(e => e.KsRkKategoriaKorekty).HasColumnName("ks_RK_KategoriaKorekty");
            entity.Property(e => e.KsRkOkres)
                .HasDefaultValue(1)
                .HasColumnName("ks_RK_Okres");
            entity.Property(e => e.KsRkPersonalizacja)
                .HasDefaultValue(true)
                .HasColumnName("ks_RK_Personalizacja");
            entity.Property(e => e.KsRkWieleZaOkres).HasColumnName("ks_RK_WieleZaOkres");
            entity.Property(e => e.KsSymbol)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("ks_Symbol");
            entity.Property(e => e.KsWaluta)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ks_Waluta");
            entity.Property(e => e.KsWielowalutowa)
                .HasDefaultValue(true)
                .HasColumnName("ks_Wielowalutowa");
            entity.Property(e => e.KsZapisKptjakoOczekujace).HasColumnName("ks_ZapisKPTJakoOczekujace");

            entity.HasOne(d => d.KsRkKategoriaNavigation).WithMany(p => p.DksKasaKsRkKategoriaNavigations)
                .HasForeignKey(d => d.KsRkKategoria)
                .HasConstraintName("FK_dks_Kasa_sl_Kategoria_Raportu");

            entity.HasOne(d => d.KsRkKategoriaKorektyNavigation).WithMany(p => p.DksKasaKsRkKategoriaKorektyNavigations)
                .HasForeignKey(d => d.KsRkKategoriaKorekty)
                .HasConstraintName("FK_dks_Kasa_sl_Kategoria_KasowyKorygujacy");
        });

        modelBuilder.Entity<DokDokument>(entity =>
        {
            entity.HasKey(e => e.DokId);

            entity.ToTable("dok__Dokument", tb =>
                {
                    tb.HasTrigger("IF_Upd_Ins_Del_DokZmiany");
                    tb.HasTrigger("IF_Upd_Ins_Del_Kontrahent_Data");
                    tb.HasTrigger("IFx_DokIns");
                    tb.HasTrigger("tr_DokDokument_CheckUniqueNumerKSeF");
                    tb.HasTrigger("tr_DokDokument_CheckUniqueWegielNumerOswiadczenia");
                    tb.HasTrigger("tr_DokDokument_Deleting");
                    tb.HasTrigger("tr_DokDokument_Inserting");
                    tb.HasTrigger("tr_ZapisCzasu");
                    tb.HasTrigger("tr_dok__Dokument_dok_Id_dok__Dokument_dok_Id_DELETE");
                    tb.HasTrigger("tr_dok__Dokument_dok_Id_dok__Dokument_dok_Id_UPDATE");
                });

            entity.HasIndex(e => new { e.DokDoDokId, e.DokTyp, e.DokPodtyp }, "IX_dok__Dokument");

            entity.HasIndex(e => e.DokDataWyst, "IX_dok__Dokument_1");

            entity.HasIndex(e => new { e.DokTyp, e.DokMagId, e.DokNr, e.DokNrRoz, e.DokDataWyst }, "IX_dok__Dokument_2");

            entity.HasIndex(e => e.DokDataOtrzym, "IX_dok__Dokument_3");

            entity.HasIndex(e => new { e.DokTyp, e.DokPodtyp, e.DokNrPelnyOryg, e.DokPlatnikId }, "IX_dok__Dokument_4");

            entity.HasIndex(e => new { e.DokId, e.DokNrPelny, e.DokDoDokNrPelny, e.DokOdbiorcaAdreshId }, "IX_dok__Dokument_5");

            entity.HasIndex(e => new { e.DokStatus, e.DokId }, "IX_dok__Dokument_6");

            entity.HasIndex(e => new { e.DokTyp, e.DokId }, "IX_dok__Dokument_7");

            entity.HasIndex(e => new { e.DokPlatnikId, e.DokId, e.DokOdbiorcaId }, "IX_dok__Dokument_8");

            entity.HasIndex(e => e.DokDoDokId, "IX_rtnet_DoDokId");

            entity.HasIndex(e => e.DokNrPelny, "IX_rtnet_NrPelny");

            entity.HasIndex(e => e.DokDoDokNrPelny, "IX_rtnet_dok_DoDokNrPelny");

            entity.Property(e => e.DokId)
                .ValueGeneratedNever()
                .HasColumnName("dok_Id");
            entity.Property(e => e.DokAdresDostawyAdreshId).HasColumnName("dok_AdresDostawyAdreshId");
            entity.Property(e => e.DokAdresDostawyId).HasColumnName("dok_AdresDostawyId");
            entity.Property(e => e.DokAkcyzaZwolnienieId).HasColumnName("dok_AkcyzaZwolnienieId");
            entity.Property(e => e.DokAlgorytm).HasColumnName("dok_Algorytm");
            entity.Property(e => e.DokBladKseF)
                .IsUnicode(false)
                .HasColumnName("dok_BladKSeF");
            entity.Property(e => e.DokCenyDataKursu)
                .HasColumnType("datetime")
                .HasColumnName("dok_CenyDataKursu");
            entity.Property(e => e.DokCenyIdBanku).HasColumnName("dok_CenyIdBanku");
            entity.Property(e => e.DokCenyKurs)
                .HasColumnType("money")
                .HasColumnName("dok_CenyKurs");
            entity.Property(e => e.DokCenyLiczbaJednostek)
                .HasDefaultValue(1)
                .HasColumnName("dok_CenyLiczbaJednostek");
            entity.Property(e => e.DokCenyNarzut)
                .HasColumnType("money")
                .HasColumnName("dok_CenyNarzut");
            entity.Property(e => e.DokCenyPoziom).HasColumnName("dok_CenyPoziom");
            entity.Property(e => e.DokCenyRodzajKursu).HasColumnName("dok_CenyRodzajKursu");
            entity.Property(e => e.DokCenyTyp).HasColumnName("dok_CenyTyp");
            entity.Property(e => e.DokCesjaPlatnikOdbiorca).HasColumnName("dok_CesjaPlatnikOdbiorca");
            entity.Property(e => e.DokCzasPrzewozuTransportu)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_CzasPrzewozuTransportu");
            entity.Property(e => e.DokCzasWysylkiTransportu)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_CzasWysylkiTransportu");
            entity.Property(e => e.DokCzekaNaKseF).HasColumnName("dok_CzekaNaKSeF");
            entity.Property(e => e.DokDataMag)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataMag");
            entity.Property(e => e.DokDataNumeruKseF)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataNumeruKSeF");
            entity.Property(e => e.DokDataNumeruKseForyg)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("dok_DataNumeruKSeFOryg");
            entity.Property(e => e.DokDataOtrzym)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataOtrzym");
            entity.Property(e => e.DokDataRozpoczeciaPrzetwarzaniaKseF)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataRozpoczeciaPrzetwarzaniaKSeF");
            entity.Property(e => e.DokDataUjeciaKorekty)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataUjeciaKorekty");
            entity.Property(e => e.DokDataWyst)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataWyst");
            entity.Property(e => e.DokDataWystawieniaKseF)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataWystawieniaKSeF");
            entity.Property(e => e.DokDataZakonczenia)
                .HasColumnType("datetime")
                .HasColumnName("dok_DataZakonczenia");
            entity.Property(e => e.DokDefiniowalnyId).HasColumnName("dok_DefiniowalnyId");
            entity.Property(e => e.DokDoDokDataWyst)
                .HasColumnType("datetime")
                .HasColumnName("dok_DoDokDataWyst");
            entity.Property(e => e.DokDoDokId).HasColumnName("dok_DoDokId");
            entity.Property(e => e.DokDoDokNrPelny)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_DoDokNrPelny");
            entity.Property(e => e.DokDoNumerKseF)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("dok_DoNumerKSeF");
            entity.Property(e => e.DokDodatkoweInfoRodzajuTransportu)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_DodatkoweInfoRodzajuTransportu");
            entity.Property(e => e.DokDokumentFiskalnyDlaPodatnikaVat).HasColumnName("dok_DokumentFiskalnyDlaPodatnikaVat");
            entity.Property(e => e.DokDstNr).HasColumnName("dok_DstNr");
            entity.Property(e => e.DokDstNrPelny)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("dok_DstNrPelny");
            entity.Property(e => e.DokDstNrRoz)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("dok_DstNrRoz");
            entity.Property(e => e.DokFakturaUproszczona).HasColumnName("dok_FakturaUproszczona");
            entity.Property(e => e.DokFiskalizacjaData)
                .HasColumnType("datetime")
                .HasColumnName("dok_FiskalizacjaData");
            entity.Property(e => e.DokFiskalizacjaIdUrzadzenia)
                .HasMaxLength(40)
                .HasColumnName("dok_FiskalizacjaIdUrzadzenia");
            entity.Property(e => e.DokFiskalizacjaNumer)
                .HasMaxLength(60)
                .HasColumnName("dok_FiskalizacjaNumer");
            entity.Property(e => e.DokFormaDokumentowania).HasColumnName("dok_FormaDokumentowania");
            entity.Property(e => e.DokIdPanstwaKonsumenta).HasColumnName("dok_IdPanstwaKonsumenta");
            entity.Property(e => e.DokIdPanstwaRozpoczeciaWysylki).HasColumnName("dok_IdPanstwaRozpoczeciaWysylki");
            entity.Property(e => e.DokIdPrzetwarzaniaKseF)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("dok_IdPrzetwarzaniaKSeF");
            entity.Property(e => e.DokIdPrzyczynyZwolnieniaZvat).HasColumnName("dok_IdPrzyczynyZwolnieniaZVAT");
            entity.Property(e => e.DokIdSesjiKasowej)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("dok_IdSesjiKasowej");
            entity.Property(e => e.DokInformacjeDodatkowe)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("dok_InformacjeDodatkowe");
            entity.Property(e => e.DokJestHop).HasColumnName("dok_JestHOP");
            entity.Property(e => e.DokJestRuchMag).HasColumnName("dok_JestRuchMag");
            entity.Property(e => e.DokJestTylkoDoOdczytu).HasColumnName("dok_JestTylkoDoOdczytu");
            entity.Property(e => e.DokJestVatAuto).HasColumnName("dok_JestVatAuto");
            entity.Property(e => e.DokJestVatNaEksport).HasColumnName("dok_JestVatNaEksport");
            entity.Property(e => e.DokJestZmianaDatyDokKas)
                .HasDefaultValue(true)
                .HasColumnName("dok_JestZmianaDatyDokKas");
            entity.Property(e => e.DokKartaId).HasColumnName("dok_KartaId");
            entity.Property(e => e.DokKatId).HasColumnName("dok_KatId");
            entity.Property(e => e.DokKodRodzajuTransakcji).HasColumnName("dok_KodRodzajuTransakcji");
            entity.Property(e => e.DokKodRodzajuTransportu).HasColumnName("dok_KodRodzajuTransportu");
            entity.Property(e => e.DokKorektaDanychNabywcy).HasColumnName("dok_KorektaDanychNabywcy");
            entity.Property(e => e.DokKredytId).HasColumnName("dok_KredytId");
            entity.Property(e => e.DokKwDoZaplaty)
                .HasColumnType("money")
                .HasColumnName("dok_KwDoZaplaty");
            entity.Property(e => e.DokKwGotowka)
                .HasColumnType("money")
                .HasColumnName("dok_KwGotowka");
            entity.Property(e => e.DokKwGotowkaPrzedplata)
                .HasColumnType("money")
                .HasColumnName("dok_KwGotowkaPrzedplata");
            entity.Property(e => e.DokKwKarta)
                .HasColumnType("money")
                .HasColumnName("dok_KwKarta");
            entity.Property(e => e.DokKwKartaPrzedplata)
                .HasColumnType("money")
                .HasColumnName("dok_KwKartaPrzedplata");
            entity.Property(e => e.DokKwKredyt)
                .HasColumnType("money")
                .HasColumnName("dok_KwKredyt");
            entity.Property(e => e.DokKwPrzelew)
                .HasColumnType("money")
                .HasColumnName("dok_KwPrzelew");
            entity.Property(e => e.DokKwPrzelewPrzedplata)
                .HasColumnType("money")
                .HasColumnName("dok_KwPrzelewPrzedplata");
            entity.Property(e => e.DokKwReszta)
                .HasColumnType("money")
                .HasColumnName("dok_KwReszta");
            entity.Property(e => e.DokKwWartosc)
                .HasDefaultValue(0m)
                .HasColumnType("money")
                .HasColumnName("dok_KwWartosc");
            entity.Property(e => e.DokMagId).HasColumnName("dok_MagId");
            entity.Property(e => e.DokMechanizmPodzielonejPlatnosci).HasColumnName("dok_MechanizmPodzielonejPlatnosci");
            entity.Property(e => e.DokMetodaKasowa).HasColumnName("dok_MetodaKasowa");
            entity.Property(e => e.DokMscWyst)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_MscWyst");
            entity.Property(e => e.DokNaliczajFundusze).HasColumnName("dok_NaliczajFundusze");
            entity.Property(e => e.DokNr).HasColumnName("dok_Nr");
            entity.Property(e => e.DokNrIdentNabywcy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("dok_NrIdentNabywcy");
            entity.Property(e => e.DokNrPelny)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_NrPelny");
            entity.Property(e => e.DokNrPelnyOryg)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_NrPelnyOryg");
            entity.Property(e => e.DokNrRachunkuBankowegoPdm).HasColumnName("dok_NrRachunkuBankowegoPdm");
            entity.Property(e => e.DokNrRoz)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength()
                .HasColumnName("dok_NrRoz");
            entity.Property(e => e.DokNumerKseF)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("dok_NumerKSeF");
            entity.Property(e => e.DokObiektGt).HasColumnName("dok_ObiektGT");
            entity.Property(e => e.DokObslugaDokDost).HasColumnName("dok_ObslugaDokDost");
            entity.Property(e => e.DokOdbiorcaAdreshId).HasColumnName("dok_OdbiorcaAdreshId");
            entity.Property(e => e.DokOdbiorcaId).HasColumnName("dok_OdbiorcaId");
            entity.Property(e => e.DokOdebral)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_Odebral");
            entity.Property(e => e.DokPersonelId).HasColumnName("dok_PersonelId");
            entity.Property(e => e.DokPlatId).HasColumnName("dok_PlatId");
            entity.Property(e => e.DokPlatTermin)
                .HasColumnType("datetime")
                .HasColumnName("dok_PlatTermin");
            entity.Property(e => e.DokPlatnikAdreshId).HasColumnName("dok_PlatnikAdreshId");
            entity.Property(e => e.DokPlatnikId).HasColumnName("dok_PlatnikId");
            entity.Property(e => e.DokPodlegaOplSpec)
                .HasDefaultValue(true)
                .HasColumnName("dok_PodlegaOplSpec");
            entity.Property(e => e.DokPodpisanoElektronicznie).HasColumnName("dok_PodpisanoElektronicznie");
            entity.Property(e => e.DokPodsumaVatFszk).HasColumnName("dok_PodsumaVatFSzk");
            entity.Property(e => e.DokPodtyp).HasColumnName("dok_Podtyp");
            entity.Property(e => e.DokPodtytul)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_Podtytul");
            entity.Property(e => e.DokProceduraMarzy).HasColumnName("dok_ProceduraMarzy");
            entity.Property(e => e.DokPromoZenCardStatus).HasColumnName("dok_PromoZenCardStatus");
            entity.Property(e => e.DokPrzetworzonoZkwZd).HasColumnName("dok_PrzetworzonoZKwZD");
            entity.Property(e => e.DokRabatProc)
                .HasColumnType("money")
                .HasColumnName("dok_RabatProc");
            entity.Property(e => e.DokRejId).HasColumnName("dok_RejId");
            entity.Property(e => e.DokRodzajOperacjiVat).HasColumnName("dok_RodzajOperacjiVat");
            entity.Property(e => e.DokRolaOdbiorcyKseF)
                .HasDefaultValue(-1)
                .HasColumnName("dok_RolaOdbiorcyKSeF");
            entity.Property(e => e.DokRozliczony).HasColumnName("dok_Rozliczony");
            entity.Property(e => e.DokSelloData)
                .HasColumnType("datetime")
                .HasColumnName("dok_SelloData");
            entity.Property(e => e.DokSelloId).HasColumnName("dok_SelloId");
            entity.Property(e => e.DokSelloSymbol)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("dok_SelloSymbol");
            entity.Property(e => e.DokSesjaKseF)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("dok_SesjaKSeF");
            entity.Property(e => e.DokSesjaKseFid).HasColumnName("dok_SesjaKSeFId");
            entity.Property(e => e.DokSrodowiskoKseF).HasColumnName("dok_SrodowiskoKSeF");
            entity.Property(e => e.DokStatus).HasColumnName("dok_Status");
            entity.Property(e => e.DokStatusBlok).HasColumnName("dok_StatusBlok");
            entity.Property(e => e.DokStatusEx)
                .HasDefaultValue(0)
                .HasColumnName("dok_StatusEx");
            entity.Property(e => e.DokStatusFiskal).HasColumnName("dok_StatusFiskal");
            entity.Property(e => e.DokStatusKseF).HasColumnName("dok_StatusKSeF");
            entity.Property(e => e.DokStatusKsieg).HasColumnName("dok_StatusKsieg");
            entity.Property(e => e.DokSzybkaPlatnosc).HasColumnName("dok_SzybkaPlatnosc");
            entity.Property(e => e.DokTermPlatIdKonfig).HasColumnName("dok_TermPlatIdKonfig");
            entity.Property(e => e.DokTermPlatIdZadania).HasColumnName("dok_TermPlatIdZadania");
            entity.Property(e => e.DokTermPlatStatus).HasColumnName("dok_TermPlatStatus");
            entity.Property(e => e.DokTermPlatTerminalId)
                .HasMaxLength(40)
                .HasColumnName("dok_TermPlatTerminalId");
            entity.Property(e => e.DokTermPlatTransId)
                .HasMaxLength(128)
                .HasColumnName("dok_TermPlatTransId");
            entity.Property(e => e.DokTerminRealizacji)
                .HasColumnType("datetime")
                .HasColumnName("dok_TerminRealizacji");
            entity.Property(e => e.DokTransakcjaData)
                .HasColumnType("datetime")
                .HasColumnName("dok_TransakcjaData");
            entity.Property(e => e.DokTransakcjaId).HasColumnName("dok_TransakcjaId");
            entity.Property(e => e.DokTransakcjaJednolitaId).HasColumnName("dok_TransakcjaJednolitaId");
            entity.Property(e => e.DokTransakcjaSymbol)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("dok_TransakcjaSymbol");
            entity.Property(e => e.DokTyp).HasColumnName("dok_Typ");
            entity.Property(e => e.DokTypDatyUjeciaKorekty).HasColumnName("dok_TypDatyUjeciaKorekty");
            entity.Property(e => e.DokTypNrIdentNabywcy).HasColumnName("dok_TypNrIdentNabywcy");
            entity.Property(e => e.DokTytul)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_Tytul");
            entity.Property(e => e.DokUwagi)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_Uwagi");
            entity.Property(e => e.DokUwagiExt)
                .HasMaxLength(3500)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_UwagiExt");
            entity.Property(e => e.DokVatMarza).HasColumnName("dok_VatMarza");
            entity.Property(e => e.DokVatMetodaLiczenia).HasColumnName("dok_VatMetodaLiczenia");
            entity.Property(e => e.DokVatRozliczanyPrzezUslugobiorce).HasColumnName("dok_VatRozliczanyPrzezUslugobiorce");
            entity.Property(e => e.DokVenderoData)
                .HasColumnType("datetime")
                .HasColumnName("dok_VenderoData");
            entity.Property(e => e.DokVenderoId).HasColumnName("dok_VenderoId");
            entity.Property(e => e.DokVenderoStatus).HasColumnName("dok_VenderoStatus");
            entity.Property(e => e.DokVenderoSymbol)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("dok_VenderoSymbol");
            entity.Property(e => e.DokWaluta)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("dok_Waluta");
            entity.Property(e => e.DokWalutaDataKursu)
                .HasColumnType("datetime")
                .HasColumnName("dok_WalutaDataKursu");
            entity.Property(e => e.DokWalutaIdBanku).HasColumnName("dok_WalutaIdBanku");
            entity.Property(e => e.DokWalutaKurs)
                .HasDefaultValue(1m)
                .HasColumnType("money")
                .HasColumnName("dok_WalutaKurs");
            entity.Property(e => e.DokWalutaLiczbaJednostek)
                .HasDefaultValue(1)
                .HasColumnName("dok_WalutaLiczbaJednostek");
            entity.Property(e => e.DokWalutaRodzajKursu).HasColumnName("dok_WalutaRodzajKursu");
            entity.Property(e => e.DokWartBrutto)
                .HasColumnType("money")
                .HasColumnName("dok_WartBrutto");
            entity.Property(e => e.DokWartMag)
                .HasColumnType("money")
                .HasColumnName("dok_WartMag");
            entity.Property(e => e.DokWartMagP)
                .HasColumnType("money")
                .HasColumnName("dok_WartMagP");
            entity.Property(e => e.DokWartMagR)
                .HasColumnType("money")
                .HasColumnName("dok_WartMagR");
            entity.Property(e => e.DokWartNetto)
                .HasColumnType("money")
                .HasColumnName("dok_WartNetto");
            entity.Property(e => e.DokWartOpWyd)
                .HasColumnType("money")
                .HasColumnName("dok_WartOpWyd");
            entity.Property(e => e.DokWartOpZwr)
                .HasColumnType("money")
                .HasColumnName("dok_WartOpZwr");
            entity.Property(e => e.DokWartOplRecykl)
                .HasColumnType("money")
                .HasColumnName("dok_WartOplRecykl");
            entity.Property(e => e.DokWartTwBrutto)
                .HasColumnType("money")
                .HasColumnName("dok_WartTwBrutto");
            entity.Property(e => e.DokWartTwNetto)
                .HasColumnType("money")
                .HasColumnName("dok_WartTwNetto");
            entity.Property(e => e.DokWartUsBrutto)
                .HasColumnType("money")
                .HasColumnName("dok_WartUsBrutto");
            entity.Property(e => e.DokWartUsNetto)
                .HasColumnType("money")
                .HasColumnName("dok_WartUsNetto");
            entity.Property(e => e.DokWartVat)
                .HasColumnType("money")
                .HasColumnName("dok_WartVat");
            entity.Property(e => e.DokWegielNumerOswiadczenia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dok_WegielNumerOswiadczenia");
            entity.Property(e => e.DokWystawil)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("dok_Wystawil");
            entity.Property(e => e.DokXmlHashKseF)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("dok_XmlHashKSeF");
            entity.Property(e => e.DokZaimportowanoDoEwidencjiAkcyzowej).HasColumnName("dok_ZaimportowanoDoEwidencjiAkcyzowej");
            entity.Property(e => e.DokZlecenieId).HasColumnName("dok_ZlecenieId");
            entity.Property(e => e.DokZnacznikiGtunaPozycji).HasColumnName("dok_ZnacznikiGTUNaPozycji");

            entity.HasOne(d => d.DokKat).WithMany(p => p.DokDokuments)
                .HasForeignKey(d => d.DokKatId)
                .HasConstraintName("FK_dok__Dokument_sl_Kategoria");
        });

        modelBuilder.Entity<DokPozycja>(entity =>
        {
            entity.HasKey(e => e.ObId);

            entity.ToTable("dok_Pozycja");

            entity.HasIndex(e => e.ObDokHanId, "IX_dok_Pozycja");

            entity.HasIndex(e => e.ObDokMagId, "IX_dok_Pozycja_1");

            entity.HasIndex(e => e.ObDoId, "IX_dok_Pozycja_2");

            entity.HasIndex(e => e.ObTowId, "IX_dok_Pozycja_3");

            entity.HasIndex(e => new { e.ObId, e.ObDokHanId, e.ObDokMagId, e.ObIlosc, e.ObCenaNetto, e.ObCenaBrutto, e.ObWartNetto, e.ObWartBrutto }, "IX_dok_Pozycja_4");

            entity.HasIndex(e => new { e.ObId, e.ObDoId, e.ObZnak, e.ObStatus, e.ObDokHanId, e.ObDokMagId, e.ObTowId, e.ObIlosc, e.ObIloscMag, e.ObCenaWaluta, e.ObCenaNetto, e.ObCenaBrutto, e.ObVatProc }, "IX_dok_Pozycja_5");

            entity.HasIndex(e => new { e.ObTowId, e.ObDokHanId, e.ObId, e.ObDoId }, "IX_dok_Pozycja_6");

            entity.Property(e => e.ObId)
                .ValueGeneratedNever()
                .HasColumnName("ob_Id");
            entity.Property(e => e.ObAkcyza).HasColumnName("ob_Akcyza");
            entity.Property(e => e.ObAkcyzaKwota)
                .HasColumnType("money")
                .HasColumnName("ob_AkcyzaKwota");
            entity.Property(e => e.ObAkcyzaWartosc)
                .HasColumnType("money")
                .HasColumnName("ob_AkcyzaWartosc");
            entity.Property(e => e.ObCenaBrutto)
                .HasColumnType("money")
                .HasColumnName("ob_CenaBrutto");
            entity.Property(e => e.ObCenaMag)
                .HasColumnType("money")
                .HasColumnName("ob_CenaMag");
            entity.Property(e => e.ObCenaNabycia)
                .HasColumnType("money")
                .HasColumnName("ob_CenaNabycia");
            entity.Property(e => e.ObCenaNetto)
                .HasColumnType("money")
                .HasColumnName("ob_CenaNetto");
            entity.Property(e => e.ObCenaPobranaZcennika).HasColumnName("ob_CenaPobranaZCennika");
            entity.Property(e => e.ObCenaWaluta)
                .HasColumnType("money")
                .HasColumnName("ob_CenaWaluta");
            entity.Property(e => e.ObDoId).HasColumnName("ob_DoId");
            entity.Property(e => e.ObDokHanId).HasColumnName("ob_DokHanId");
            entity.Property(e => e.ObDokHanLp).HasColumnName("ob_DokHanLp");
            entity.Property(e => e.ObDokMagId).HasColumnName("ob_DokMagId");
            entity.Property(e => e.ObDokMagLp).HasColumnName("ob_DokMagLp");
            entity.Property(e => e.ObIlosc)
                .HasColumnType("money")
                .HasColumnName("ob_Ilosc");
            entity.Property(e => e.ObIloscMag)
                .HasColumnType("money")
                .HasColumnName("ob_IloscMag");
            entity.Property(e => e.ObJm)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ob_Jm");
            entity.Property(e => e.ObKategoriaId).HasColumnName("ob_KategoriaId");
            entity.Property(e => e.ObKsefUuid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ob_KsefUUID");
            entity.Property(e => e.ObMagId).HasColumnName("ob_MagId");
            entity.Property(e => e.ObNumerSeryjny)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ob_NumerSeryjny");
            entity.Property(e => e.ObOpis)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ob_Opis");
            entity.Property(e => e.ObOplCukrowaCukierZawartoscEx)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaCukierZawartoscEx");
            entity.Property(e => e.ObOplCukrowaKwCukier)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaKwCukier");
            entity.Property(e => e.ObOplCukrowaKwCukierEx)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaKwCukierEx");
            entity.Property(e => e.ObOplCukrowaKwKofeina)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaKwKofeina");
            entity.Property(e => e.ObOplCukrowaKwSuma)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaKwSuma");
            entity.Property(e => e.ObOplCukrowaObj)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaObj");
            entity.Property(e => e.ObOplCukrowaParametry).HasColumnName("ob_OplCukrowaParametry");
            entity.Property(e => e.ObOplCukrowaPodlega)
                .HasDefaultValue(false)
                .HasColumnName("ob_OplCukrowaPodlega");
            entity.Property(e => e.ObOplCukrowaWartCukier)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaWartCukier");
            entity.Property(e => e.ObOplCukrowaWartKofeina)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaWartKofeina");
            entity.Property(e => e.ObOplCukrowaWartSuma)
                .HasColumnType("money")
                .HasColumnName("ob_OplCukrowaWartSuma");
            entity.Property(e => e.ObOznaczenieGtu).HasColumnName("ob_OznaczenieGTU");
            entity.Property(e => e.ObPrzyczynaKorektyId).HasColumnName("ob_PrzyczynaKorektyId");
            entity.Property(e => e.ObRabat)
                .HasColumnType("money")
                .HasColumnName("ob_Rabat");
            entity.Property(e => e.ObStatus).HasColumnName("ob_Status");
            entity.Property(e => e.ObSyncId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ob_SyncId");
            entity.Property(e => e.ObTermin)
                .HasColumnType("datetime")
                .HasColumnName("ob_Termin");
            entity.Property(e => e.ObTowId).HasColumnName("ob_TowId");
            entity.Property(e => e.ObTowKodCn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ob_TowKodCN");
            entity.Property(e => e.ObTowPkwiu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ob_TowPkwiu");
            entity.Property(e => e.ObTowRodzaj)
                .HasDefaultValue(1)
                .HasColumnName("ob_TowRodzaj");
            entity.Property(e => e.ObVatId).HasColumnName("ob_VatId");
            entity.Property(e => e.ObVatProc)
                .HasColumnType("money")
                .HasColumnName("ob_VatProc");
            entity.Property(e => e.ObWartBrutto)
                .HasColumnType("money")
                .HasColumnName("ob_WartBrutto");
            entity.Property(e => e.ObWartMag)
                .HasColumnType("money")
                .HasColumnName("ob_WartMag");
            entity.Property(e => e.ObWartNabycia)
                .HasColumnType("money")
                .HasColumnName("ob_WartNabycia");
            entity.Property(e => e.ObWartNetto)
                .HasColumnType("money")
                .HasColumnName("ob_WartNetto");
            entity.Property(e => e.ObWartVat)
                .HasColumnType("money")
                .HasColumnName("ob_WartVat");
            entity.Property(e => e.ObWegielDataWprowadzeniaLubNabycia)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ob_WegielDataWprowadzeniaLubNabycia");
            entity.Property(e => e.ObWegielOpisPochodzenia)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ob_WegielOpisPochodzenia");
            entity.Property(e => e.ObZnak)
                .HasDefaultValue((short)1)
                .HasColumnName("ob_Znak");

            entity.HasOne(d => d.ObDo).WithMany(p => p.InverseObDo)
                .HasForeignKey(d => d.ObDoId)
                .HasConstraintName("FK_dok_Pozycja_dok_Pozycja");

            entity.HasOne(d => d.ObDokHan).WithMany(p => p.DokPozycjaObDokHans)
                .HasForeignKey(d => d.ObDokHanId)
                .HasConstraintName("FK_dok_Pozycja_dok__Dokument");

            entity.HasOne(d => d.ObDokMag).WithMany(p => p.DokPozycjaObDokMags)
                .HasForeignKey(d => d.ObDokMagId)
                .HasConstraintName("FK_dok_Pozycja_dok__Dokument1");

            entity.HasOne(d => d.ObTow).WithMany(p => p.DokPozycjas)
                .HasForeignKey(d => d.ObTowId)
                .HasConstraintName("FK_dok_Pozycja_tw__Towar");
        });

        modelBuilder.Entity<DokVat>(entity =>
        {
            entity.HasKey(e => e.VtId);

            entity.ToTable("dok_Vat");

            entity.HasIndex(e => e.VtDokId, "IX_dok_Vat");

            entity.Property(e => e.VtId)
                .ValueGeneratedNever()
                .HasColumnName("vt_Id");
            entity.Property(e => e.VtDokId).HasColumnName("vt_DokId");
            entity.Property(e => e.VtVatId).HasColumnName("vt_VatId");
            entity.Property(e => e.VtVatProc)
                .HasColumnType("money")
                .HasColumnName("vt_VatProc");
            entity.Property(e => e.VtWartBrutto)
                .HasColumnType("money")
                .HasColumnName("vt_WartBrutto");
            entity.Property(e => e.VtWartBruttoWaluta)
                .HasColumnType("money")
                .HasColumnName("vt_WartBruttoWaluta");
            entity.Property(e => e.VtWartNetto)
                .HasColumnType("money")
                .HasColumnName("vt_WartNetto");
            entity.Property(e => e.VtWartNettoWaluta)
                .HasColumnType("money")
                .HasColumnName("vt_WartNettoWaluta");
            entity.Property(e => e.VtWartVat)
                .HasColumnType("money")
                .HasColumnName("vt_WartVat");
            entity.Property(e => e.VtWartVatWaluta)
                .HasColumnType("money")
                .HasColumnName("vt_WartVatWaluta");

            entity.HasOne(d => d.VtDok).WithMany(p => p.DokVats)
                .HasForeignKey(d => d.VtDokId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dok_Vat_dok__Dokument");

            entity.HasOne(d => d.VtVat).WithMany(p => p.DokVats)
                .HasForeignKey(d => d.VtVatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dok_Vat_sl_StawkaVAT");
        });

        modelBuilder.Entity<IfxApiFormaPlatnosci>(entity =>
        {
            entity.ToTable("IFx_ApiFormaPlatnosci");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FpId).HasColumnName("fp_Id");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IfxApiPromocjaGrupa>(entity =>
        {
            entity.ToTable("IFx_ApiPromocjaGrupa");

            entity.Property(e => e.Nazwa)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IfxApiPromocjaGrupaZestaw>(entity =>
        {
            entity.ToTable("IFx_ApiPromocjaGrupaZestaw");

            entity.HasOne(d => d.PromocjaGrupa).WithMany(p => p.IfxApiPromocjaGrupaZestaws)
                .HasForeignKey(d => d.PromocjaGrupaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IFx_ApiPromocjaGrupaZestaw_IFx_ApiPromocjaGrupa");

            entity.HasOne(d => d.PromocjaZestaw).WithMany(p => p.IfxApiPromocjaGrupaZestaws)
                .HasForeignKey(d => d.PromocjaZestawId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IFx_ApiPromocjaGrupaZestaw_IFx_ApiPromocjaZestaw");
        });

        modelBuilder.Entity<IfxApiPromocjaPozycja>(entity =>
        {
            entity.ToTable("IFx_ApiPromocjaPozycja");

            entity.Property(e => e.CenaNetto).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.MinCena).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StawkaVat)
                .HasDefaultValue(23m)
                .HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.Zestaw).WithMany(p => p.IfxApiPromocjaPozycjas)
                .HasForeignKey(d => d.ZestawId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IFx_ApiPromocjaPozycja_IFx_ApiPromocjaZestaw");
        });

        modelBuilder.Entity<IfxApiPromocjaPozycjaTowar>(entity =>
        {
            entity.HasKey(e => new { e.PozycjaId, e.TwSymbol }).HasName("PK_IFx_ApiPromocjaPozycjaTowar_1");

            entity.ToTable("IFx_ApiPromocjaPozycjaTowar");

            entity.Property(e => e.TwSymbol)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Pozycja).WithMany(p => p.IfxApiPromocjaPozycjaTowars)
                .HasForeignKey(d => d.PozycjaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IFx_ApiPromocjaPozycjaTowar_IFx_ApiPromocjaPozycja");
        });

        modelBuilder.Entity<IfxApiPromocjaZestaw>(entity =>
        {
            entity.ToTable("IFx_ApiPromocjaZestaw");

            entity.Property(e => e.DataDo).HasColumnType("datetime");
            entity.Property(e => e.DataOd).HasColumnType("datetime");
            entity.Property(e => e.Img).HasColumnType("image");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Zdjecie)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ZmData)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("zm_Data");
            entity.Property(e => e.ZmGrupaData).HasColumnName("zm_GrupaData");
        });

        modelBuilder.Entity<IfxApiSposobDostawy>(entity =>
        {
            entity.ToTable("IFx_ApiSposobDostawy");

            entity.Property(e => e.Nazwa)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TwId).HasColumnName("tw_Id");
        });

        modelBuilder.Entity<IfxApiUzytkownik>(entity =>
        {
            entity.HasKey(e => e.UzId);

            entity.ToTable("IFx_ApiUzytkownik");

            entity.Property(e => e.UzId)
                .ValueGeneratedNever()
                .HasColumnName("uz_Id");
            entity.Property(e => e.CechaId).HasColumnName("cecha_Id");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("deviceId");
            entity.Property(e => e.Fsdodaj).HasColumnName("FSDodaj");
            entity.Property(e => e.MaxPlatnoscOdroczona).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Padodaj).HasColumnName("PADodaj");
            entity.Property(e => e.Typ).HasDefaultValue((byte)0);
            entity.Property(e => e.Zkdodaj).HasColumnName("ZKDodaj");
            entity.Property(e => e.ZmData)
                .HasColumnType("datetime")
                .HasColumnName("zm_Data");
            entity.Property(e => e.Zmmdodaj).HasColumnName("ZMMDodaj");
        });

        modelBuilder.Entity<IfxApiUzytkownikPoziomyCenowe>(entity =>
        {
            entity.ToTable("IFx_ApiUzytkownik_PoziomyCenowe");

            entity.Property(e => e.CenaId).HasColumnName("cena_Id");
            entity.Property(e => e.Primary).HasColumnName("primary");
            entity.Property(e => e.UzId).HasColumnName("uz_Id");

            entity.HasOne(d => d.Uz).WithMany(p => p.IfxApiUzytkownikPoziomyCenowes)
                .HasForeignKey(d => d.UzId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IFx_ApiUzytkownik_PoziomyCenowe_IFx_ApiUzytkownik");
        });

        modelBuilder.Entity<KhAdresyDostawy>(entity =>
        {
            entity.HasKey(e => e.AdkId);

            entity.ToTable("kh_AdresyDostawy");

            entity.Property(e => e.AdkId).HasColumnName("adk_Id");
            entity.Property(e => e.AdkIdAdresuHist).HasColumnName("adk_IdAdresuHist");
            entity.Property(e => e.AdkIdKhnt).HasColumnName("adk_IdKhnt");

            entity.HasOne(d => d.AdkIdAdresuHistNavigation).WithMany(p => p.KhAdresyDostawies)
                .HasForeignKey(d => d.AdkIdAdresuHist)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_kh_AdresyDostawy_adr_Historia");

            entity.HasOne(d => d.AdkIdKhntNavigation).WithMany(p => p.KhAdresyDostawies)
                .HasForeignKey(d => d.AdkIdKhnt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_kh_AdresyDostawy_kh__Kontrahent");
        });

        modelBuilder.Entity<KhCechaKh>(entity =>
        {
            entity.HasKey(e => e.CkId).HasName("PK_kh_GrupaKh");

            entity.ToTable("kh_CechaKh");

            entity.Property(e => e.CkId)
                .ValueGeneratedNever()
                .HasColumnName("ck_Id");
            entity.Property(e => e.CkIdCecha).HasColumnName("ck_IdCecha");
            entity.Property(e => e.CkIdKhnt).HasColumnName("ck_IdKhnt");

            entity.HasOne(d => d.CkIdCechaNavigation).WithMany(p => p.KhCechaKhs)
                .HasForeignKey(d => d.CkIdCecha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_kh_CechaKh_sl_CechaKh");

            entity.HasOne(d => d.CkIdKhntNavigation).WithMany(p => p.KhCechaKhs)
                .HasForeignKey(d => d.CkIdKhnt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_kh_CechaKh_kh__Kontrahent");
        });

        modelBuilder.Entity<KhKontrahent>(entity =>
        {
            entity.HasKey(e => e.KhId);

            entity.ToTable("kh__Kontrahent", tb =>
                {
                    tb.HasTrigger("IF_Upd_Ins_Kontrahent_Data");
                    tb.HasTrigger("tr_KhKontrahent_Deleting");
                    tb.HasTrigger("tr_KhKontrahent_Inserting");
                    tb.HasTrigger("tr_KhKontrahent_Updating");
                });

            entity.Property(e => e.KhId)
                .ValueGeneratedNever()
                .HasColumnName("kh_Id");
            entity.Property(e => e.KhAdresDostawy).HasColumnName("kh_AdresDostawy");
            entity.Property(e => e.KhAdresKoresp).HasColumnName("kh_AdresKoresp");
            entity.Property(e => e.KhAkcyza)
                .HasDefaultValue(1)
                .HasColumnName("kh_Akcyza");
            entity.Property(e => e.KhBrakPpdlaRozrachunkowAuto).HasColumnName("kh_BrakPPDlaRozrachunkowAuto");
            entity.Property(e => e.KhCelZakupu).HasColumnName("kh_CelZakupu");
            entity.Property(e => e.KhCena).HasColumnName("kh_Cena");
            entity.Property(e => e.KhCentrumAut).HasColumnName("kh_CentrumAut");
            entity.Property(e => e.KhCrm).HasColumnName("kh_CRM");
            entity.Property(e => e.KhCzyKomunikat).HasColumnName("kh_CzyKomunikat");
            entity.Property(e => e.KhCzynnyPodatnikVat).HasColumnName("kh_CzynnyPodatnikVAT");
            entity.Property(e => e.KhDataDodania)
                .HasColumnType("datetime")
                .HasColumnName("kh_DataDodania");
            entity.Property(e => e.KhDataOkolicznosciowa)
                .HasColumnType("datetime")
                .HasColumnName("kh_DataOkolicznosciowa");
            entity.Property(e => e.KhDataVat)
                .HasColumnType("datetime")
                .HasColumnName("kh_DataVAT");
            entity.Property(e => e.KhDataWaznosciVat)
                .HasColumnType("datetime")
                .HasColumnName("kh_DataWaznosciVAT");
            entity.Property(e => e.KhDataWyd)
                .HasColumnType("datetime")
                .HasColumnName("kh_DataWyd");
            entity.Property(e => e.KhDataZmiany)
                .HasColumnType("datetime")
                .HasColumnName("kh_DataZmiany");
            entity.Property(e => e.KhDomena)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Domena");
            entity.Property(e => e.KhDomyslnaFormaDokumentowaniaSprzedaz).HasColumnName("kh_DomyslnaFormaDokumentowaniaSprzedaz");
            entity.Property(e => e.KhDomyslnaTransVatsprzedaz).HasColumnName("kh_DomyslnaTransVATSprzedaz");
            entity.Property(e => e.KhDomyslnaTransVatsprzedazFw).HasColumnName("kh_DomyslnaTransVATSprzedazFW");
            entity.Property(e => e.KhDomyslnaTransVatzakup).HasColumnName("kh_DomyslnaTransVATZakup");
            entity.Property(e => e.KhDomyslnaTransVatzakupFw).HasColumnName("kh_DomyslnaTransVATZakupFW");
            entity.Property(e => e.KhDomyslnaWaluta)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("kh_DomyslnaWaluta");
            entity.Property(e => e.KhDomyslnaWalutaMode).HasColumnName("kh_DomyslnaWalutaMode");
            entity.Property(e => e.KhDomyslnyRachBankowyId).HasColumnName("kh_DomyslnyRachBankowyId");
            entity.Property(e => e.KhDomyslnyRachBankowyIdMode).HasColumnName("kh_DomyslnyRachBankowyIdMode");
            entity.Property(e => e.KhDomyslnyTypCeny).HasColumnName("kh_DomyslnyTypCeny");
            entity.Property(e => e.KhEfakturyData)
                .HasColumnType("datetime")
                .HasColumnName("kh_EFakturyData");
            entity.Property(e => e.KhEfakturyZgoda).HasColumnName("kh_EFakturyZgoda");
            entity.Property(e => e.KhEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_EMail");
            entity.Property(e => e.KhEwVatspMcOdliczenia).HasColumnName("kh_EwVATSpMcOdliczenia");
            entity.Property(e => e.KhEwVatzakMcOdliczenia).HasColumnName("kh_EwVATZakMcOdliczenia");
            entity.Property(e => e.KhEwVatzakRodzaj)
                .HasDefaultValue(2)
                .HasColumnName("kh_EwVATZakRodzaj");
            entity.Property(e => e.KhEwVatzakSposobOdliczenia).HasColumnName("kh_EwVATZakSposobOdliczenia");
            entity.Property(e => e.KhGaduGadu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_GaduGadu");
            entity.Property(e => e.KhGrafika)
                .HasColumnType("image")
                .HasColumnName("kh_Grafika");
            entity.Property(e => e.KhIdBranza).HasColumnName("kh_IdBranza");
            entity.Property(e => e.KhIdDodal).HasColumnName("kh_IdDodal");
            entity.Property(e => e.KhIdEwVatsp).HasColumnName("kh_IdEwVATSp");
            entity.Property(e => e.KhIdEwVatspKateg).HasColumnName("kh_IdEwVATSpKateg");
            entity.Property(e => e.KhIdEwVatzak).HasColumnName("kh_IdEwVATZak");
            entity.Property(e => e.KhIdEwVatzakKateg).HasColumnName("kh_IdEwVATZakKateg");
            entity.Property(e => e.KhIdFormaP).HasColumnName("kh_IdFormaP");
            entity.Property(e => e.KhIdGrupa).HasColumnName("kh_IdGrupa");
            entity.Property(e => e.KhIdKategoriaKh).HasColumnName("kh_IdKategoriaKH");
            entity.Property(e => e.KhIdNabywca).HasColumnName("kh_IdNabywca");
            entity.Property(e => e.KhIdOdbiorca).HasColumnName("kh_IdOdbiorca");
            entity.Property(e => e.KhIdOpiekun).HasColumnName("kh_IdOpiekun");
            entity.Property(e => e.KhIdOsobaDo).HasColumnName("kh_IdOsobaDO");
            entity.Property(e => e.KhIdOstatniWpisWeryfikacjiStatusuVat).HasColumnName("kh_IdOstatniWpisWeryfikacjiStatusuVAT");
            entity.Property(e => e.KhIdOstatniWpisWeryfikacjiStatusuVies).HasColumnName("kh_IdOstatniWpisWeryfikacjiStatusuVIES");
            entity.Property(e => e.KhIdOstatniWpisWeryfikacjiWykazPodatnikowVat).HasColumnName("kh_IdOstatniWpisWeryfikacjiWykazPodatnikowVAT");
            entity.Property(e => e.KhIdPozyskany).HasColumnName("kh_IdPozyskany");
            entity.Property(e => e.KhIdRabat).HasColumnName("kh_IdRabat");
            entity.Property(e => e.KhIdRachKategPrzychod).HasColumnName("kh_IdRachKategPrzychod");
            entity.Property(e => e.KhIdRachKategRozchod).HasColumnName("kh_IdRachKategRozchod");
            entity.Property(e => e.KhIdRachunkuWirtualnego).HasColumnName("kh_IdRachunkuWirtualnego");
            entity.Property(e => e.KhIdRegion).HasColumnName("kh_IdRegion");
            entity.Property(e => e.KhIdRodzajKontaktu).HasColumnName("kh_IdRodzajKontaktu");
            entity.Property(e => e.KhIdZmienil).HasColumnName("kh_IdZmienil");
            entity.Property(e => e.KhImie)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Imie");
            entity.Property(e => e.KhInstKredytowa).HasColumnName("kh_InstKredytowa");
            entity.Property(e => e.KhJednorazowy).HasColumnName("kh_Jednorazowy");
            entity.Property(e => e.KhKlientSklepuInternetowego).HasColumnName("kh_KlientSklepuInternetowego");
            entity.Property(e => e.KhKomunikat)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Komunikat");
            entity.Property(e => e.KhKomunikatOd)
                .HasColumnType("datetime")
                .HasColumnName("kh_KomunikatOd");
            entity.Property(e => e.KhKomunikatZawsze)
                .HasDefaultValue(true)
                .HasColumnName("kh_KomunikatZawsze");
            entity.Property(e => e.KhKontakt)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Kontakt");
            entity.Property(e => e.KhKorygowanieKup).HasColumnName("kh_KorygowanieKUP");
            entity.Property(e => e.KhKorygowanieVatsp).HasColumnName("kh_KorygowanieVATSp");
            entity.Property(e => e.KhKorygowanieVatzak).HasColumnName("kh_KorygowanieVATZak");
            entity.Property(e => e.KhKrs)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("kh_KRS");
            entity.Property(e => e.KhKsefDodatkoweInfoPoprzedzEtykietaPola).HasColumnName("kh_KsefDodatkoweInfoPoprzedzEtykietaPola");
            entity.Property(e => e.KhKsefDomyslnyEtapPrzetwarzania).HasColumnName("kh_KsefDomyslnyEtapPrzetwarzania");
            entity.Property(e => e.KhKsefEksportObslugaPolaDodatkoweInformacje).HasColumnName("kh_KsefEksportObslugaPolaDodatkoweInformacje");
            entity.Property(e => e.KhKsefImportObslugaPolaDodatkoweInformacje).HasColumnName("kh_KsefImportObslugaPolaDodatkoweInformacje");
            entity.Property(e => e.KhKsefMagazynDlaEfaktur).HasColumnName("kh_KsefMagazynDlaEFaktur");
            entity.Property(e => e.KhKsefPoleDodatkoweInformacjeEksportNaPodstSql).HasColumnName("kh_KsefPoleDodatkoweInformacjeEksportNaPodstSql");
            entity.Property(e => e.KhKsefPoleDodatkoweInformacjeEksportSql)
                .HasColumnType("text")
                .HasColumnName("kh_KsefPoleDodatkoweInformacjeEksportSql");
            entity.Property(e => e.KhLiczbaPrac).HasColumnName("kh_LiczbaPrac");
            entity.Property(e => e.KhLokalizacja)
                .HasMaxLength(256)
                .HasColumnName("kh_Lokalizacja");
            entity.Property(e => e.KhMalyPojazd).HasColumnName("kh_MalyPojazd");
            entity.Property(e => e.KhMaxDniSp).HasColumnName("kh_MaxDniSp");
            entity.Property(e => e.KhMaxDokKred).HasColumnName("kh_MaxDokKred");
            entity.Property(e => e.KhMaxWartDokKred)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("money")
                .HasColumnName("kh_MaxWartDokKred");
            entity.Property(e => e.KhMaxWartKred)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("money")
                .HasColumnName("kh_MaxWartKred");
            entity.Property(e => e.KhMetodaKasowa).HasColumnName("kh_MetodaKasowa");
            entity.Property(e => e.KhNaliczajOplSpec)
                .HasDefaultValue(true)
                .HasColumnName("kh_NaliczajOplSpec");
            entity.Property(e => e.KhNazwisko)
                .HasMaxLength(51)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Nazwisko");
            entity.Property(e => e.KhNrAkcyzowy)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("kh_NrAkcyzowy");
            entity.Property(e => e.KhNrAnalitykaD)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_NrAnalitykaD");
            entity.Property(e => e.KhNrAnalitykaO)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_NrAnalitykaO");
            entity.Property(e => e.KhNrDowodu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_NrDowodu");
            entity.Property(e => e.KhOdbDet).HasColumnName("kh_OdbDet");
            entity.Property(e => e.KhOdbiorcaCesjaPlatnosci).HasColumnName("kh_OdbiorcaCesjaPlatnosci");
            entity.Property(e => e.KhOpisOperacji)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_OpisOperacji");
            entity.Property(e => e.KhOrganWyd)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_OrganWyd");
            entity.Property(e => e.KhOsoba).HasColumnName("kh_Osoba");
            entity.Property(e => e.KhOsobaVat).HasColumnName("kh_OsobaVAT");
            entity.Property(e => e.KhOstrzezenieTerminPlatnosciPrzekroczony).HasColumnName("kh_OstrzezenieTerminPlatnosciPrzekroczony");
            entity.Property(e => e.KhPesel)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_PESEL");
            entity.Property(e => e.KhPlatOdroczone).HasColumnName("kh_PlatOdroczone");
            entity.Property(e => e.KhPlatPrzelew).HasColumnName("kh_PlatPrzelew");
            entity.Property(e => e.KhPodVatzarejestrowanyWue).HasColumnName("kh_PodVATZarejestrowanyWUE");
            entity.Property(e => e.KhPodatekCukrowyNaliczaj).HasColumnName("kh_PodatekCukrowyNaliczaj");
            entity.Property(e => e.KhPole1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole1");
            entity.Property(e => e.KhPole2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole2");
            entity.Property(e => e.KhPole3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole3");
            entity.Property(e => e.KhPole4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole4");
            entity.Property(e => e.KhPole5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole5");
            entity.Property(e => e.KhPole6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole6");
            entity.Property(e => e.KhPole7)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole7");
            entity.Property(e => e.KhPole8)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pole8");
            entity.Property(e => e.KhPotencjalny).HasColumnName("kh_Potencjalny");
            entity.Property(e => e.KhPowitanie)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Powitanie");
            entity.Property(e => e.KhPracownik)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Pracownik");
            entity.Property(e => e.KhPrefKontakt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_PrefKontakt");
            entity.Property(e => e.KhProcGotowka)
                .HasColumnType("money")
                .HasColumnName("kh_ProcGotowka");
            entity.Property(e => e.KhProcKarta)
                .HasColumnType("money")
                .HasColumnName("kh_ProcKarta");
            entity.Property(e => e.KhProcKredyt)
                .HasColumnType("money")
                .HasColumnName("kh_ProcKredyt");
            entity.Property(e => e.KhProcPozostalo)
                .HasColumnType("money")
                .HasColumnName("kh_ProcPozostalo");
            entity.Property(e => e.KhProcPrzelew)
                .HasColumnType("money")
                .HasColumnName("kh_ProcPrzelew");
            entity.Property(e => e.KhProducentRolny).HasColumnName("kh_ProducentRolny");
            entity.Property(e => e.KhPrzypadekSzczegolnyPit).HasColumnName("kh_PrzypadekSzczegolnyPIT");
            entity.Property(e => e.KhRegon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_REGON");
            entity.Property(e => e.KhRodzaj).HasColumnName("kh_Rodzaj");
            entity.Property(e => e.KhRolaOdbiorcyKseF)
                .HasDefaultValue(-1)
                .HasColumnName("kh_RolaOdbiorcyKSeF");
            entity.Property(e => e.KhSkype)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Skype");
            entity.Property(e => e.KhStatusAkcyza).HasColumnName("kh_StatusAkcyza");
            entity.Property(e => e.KhStawkaVatprzychod).HasColumnName("kh_StawkaVATPrzychod");
            entity.Property(e => e.KhStawkaVatwydatek).HasColumnName("kh_StawkaVATWydatek");
            entity.Property(e => e.KhStosujIndywidualnyCennikWsklepieInternetowym).HasColumnName("kh_StosujIndywidualnyCennikWSklepieInternetowym");
            entity.Property(e => e.KhStosujRabatWmultistore).HasColumnName("kh_StosujRabatWMultistore");
            entity.Property(e => e.KhStosujSzybkaPlatnosc)
                .HasDefaultValue(true)
                .HasColumnName("kh_StosujSzybkaPlatnosc");
            entity.Property(e => e.KhSymbol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("kh_Symbol");
            entity.Property(e => e.KhTransakcjaVatsp).HasColumnName("kh_TransakcjaVATSp");
            entity.Property(e => e.KhTransakcjaVatzak).HasColumnName("kh_TransakcjaVATZak");
            entity.Property(e => e.KhUpowaznienieVat).HasColumnName("kh_UpowaznienieVAT");
            entity.Property(e => e.KhUwagi)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_Uwagi");
            entity.Property(e => e.KhVatRozliczanyPrzezUslugobiorce).HasColumnName("kh_VatRozliczanyPrzezUslugobiorce");
            entity.Property(e => e.KhVatRozliczanyPrzezUslugobiorceFw).HasColumnName("kh_VatRozliczanyPrzezUslugobiorceFW");
            entity.Property(e => e.KhWartoscNettoCzyBrutto).HasColumnName("kh_WartoscNettoCzyBrutto");
            entity.Property(e => e.KhWww)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kh_WWW");
            entity.Property(e => e.KhWzwIdCrmTransakcja).HasColumnName("kh_WzwIdCrmTransakcja");
            entity.Property(e => e.KhWzwIdFs).HasColumnName("kh_WzwIdFS");
            entity.Property(e => e.KhWzwIdKfs).HasColumnName("kh_WzwIdKFS");
            entity.Property(e => e.KhWzwIdWz).HasColumnName("kh_WzwIdWZ");
            entity.Property(e => e.KhWzwIdWzvat).HasColumnName("kh_WzwIdWZVAT");
            entity.Property(e => e.KhWzwIdZd).HasColumnName("kh_WzwIdZD");
            entity.Property(e => e.KhWzwIdZk).HasColumnName("kh_WzwIdZK");
            entity.Property(e => e.KhWzwIdZkzal).HasColumnName("kh_WzwIdZKZAL");
            entity.Property(e => e.KhZablokowany).HasColumnName("kh_Zablokowany");
            entity.Property(e => e.KhZgodaDo).HasColumnName("kh_ZgodaDO");
            entity.Property(e => e.KhZgodaEmail).HasColumnName("kh_ZgodaEMail");
            entity.Property(e => e.KhZgodaMark).HasColumnName("kh_ZgodaMark");
            entity.Property(e => e.KhZgodaNewsletterVendero).HasColumnName("kh_ZgodaNewsletterVendero");

            entity.HasOne(d => d.KhIdEwVatspKategNavigation).WithMany(p => p.KhKontrahentKhIdEwVatspKategNavigations)
                .HasForeignKey(d => d.KhIdEwVatspKateg)
                .HasConstraintName("FK_kh__Kontrahent_sl_Kategoria1");

            entity.HasOne(d => d.KhIdEwVatzakKategNavigation).WithMany(p => p.KhKontrahentKhIdEwVatzakKategNavigations)
                .HasForeignKey(d => d.KhIdEwVatzakKateg)
                .HasConstraintName("FK_kh__Kontrahent_sl_Kategoria2");

            entity.HasOne(d => d.KhIdGrupaNavigation).WithMany(p => p.KhKontrahents)
                .HasForeignKey(d => d.KhIdGrupa)
                .HasConstraintName("FK_kh__Kontrahent_sl_GrupaKh");

            entity.HasOne(d => d.KhIdKategoriaKhNavigation).WithMany(p => p.KhKontrahentKhIdKategoriaKhNavigations)
                .HasForeignKey(d => d.KhIdKategoriaKh)
                .HasConstraintName("FK_kh__Kontrahent_sl_Kategoria");

            entity.HasOne(d => d.KhIdNabywcaNavigation).WithMany(p => p.InverseKhIdNabywcaNavigation)
                .HasForeignKey(d => d.KhIdNabywca)
                .HasConstraintName("FK_kh__Kontrahent_kh__Kontrahent_Nabywca");

            entity.HasOne(d => d.KhIdOdbiorcaNavigation).WithMany(p => p.InverseKhIdOdbiorcaNavigation)
                .HasForeignKey(d => d.KhIdOdbiorca)
                .HasConstraintName("FK_kh__Kontrahent_kh__Kontrahent");

            entity.HasOne(d => d.KhIdOpiekunNavigation).WithMany(p => p.KhKontrahents)
                .HasForeignKey(d => d.KhIdOpiekun)
                .HasConstraintName("FK_kh__Kontrahent_pd_Uzytkownik");

            entity.HasOne(d => d.KhIdRachKategPrzychodNavigation).WithMany(p => p.KhKontrahentKhIdRachKategPrzychodNavigations)
                .HasForeignKey(d => d.KhIdRachKategPrzychod)
                .HasConstraintName("FK_kh__Kontrahent_sl_Kategoria3");

            entity.HasOne(d => d.KhIdRachKategRozchodNavigation).WithMany(p => p.KhKontrahentKhIdRachKategRozchodNavigations)
                .HasForeignKey(d => d.KhIdRachKategRozchod)
                .HasConstraintName("FK_kh__Kontrahent_sl_Kategoria4");
        });

        modelBuilder.Entity<KhPracownik>(entity =>
        {
            entity.HasKey(e => e.PkId);

            entity.ToTable("kh_Pracownik", tb =>
                {
                    tb.HasTrigger("tr_KhPracownik_Deleting");
                    tb.HasTrigger("tr_KhPracownik_Inserting");
                    tb.HasTrigger("tr_KhPracownik_Updating");
                });

            entity.Property(e => e.PkId)
                .ValueGeneratedNever()
                .HasColumnName("pk_Id");
            entity.Property(e => e.PkAdresDostawy).HasColumnName("pk_AdresDostawy");
            entity.Property(e => e.PkAdresKoresp).HasColumnName("pk_AdresKoresp");
            entity.Property(e => e.PkDataDodania)
                .HasColumnType("datetime")
                .HasColumnName("pk_DataDodania");
            entity.Property(e => e.PkDataOkolicznosciowa)
                .HasColumnType("datetime")
                .HasColumnName("pk_DataOkolicznosciowa");
            entity.Property(e => e.PkDataZmiany)
                .HasColumnType("datetime")
                .HasColumnName("pk_DataZmiany");
            entity.Property(e => e.PkDomena)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Domena");
            entity.Property(e => e.PkEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_EMail");
            entity.Property(e => e.PkGaduGadu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_GaduGadu");
            entity.Property(e => e.PkGrafika)
                .HasColumnType("image")
                .HasColumnName("pk_Grafika");
            entity.Property(e => e.PkIdBranza).HasColumnName("pk_IdBranza");
            entity.Property(e => e.PkIdDodal).HasColumnName("pk_IdDodal");
            entity.Property(e => e.PkIdDzial).HasColumnName("pk_IdDzial");
            entity.Property(e => e.PkIdGrupa).HasColumnName("pk_IdGrupa");
            entity.Property(e => e.PkIdKh).HasColumnName("pk_IdKh");
            entity.Property(e => e.PkIdOpiekun).HasColumnName("pk_IdOpiekun");
            entity.Property(e => e.PkIdPozyskany).HasColumnName("pk_IdPozyskany");
            entity.Property(e => e.PkIdRegion).HasColumnName("pk_IdRegion");
            entity.Property(e => e.PkIdRodzajKontaktu).HasColumnName("pk_IdRodzajKontaktu");
            entity.Property(e => e.PkIdZmienil).HasColumnName("pk_IdZmienil");
            entity.Property(e => e.PkImie)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Imie");
            entity.Property(e => e.PkLokalizacja)
                .HasMaxLength(256)
                .HasColumnName("pk_Lokalizacja");
            entity.Property(e => e.PkNazwisko)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("pk_Nazwisko");
            entity.Property(e => e.PkPodstaw).HasColumnName("pk_Podstaw");
            entity.Property(e => e.PkPowitanie)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Powitanie");
            entity.Property(e => e.PkSkype)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Skype");
            entity.Property(e => e.PkStanowisko)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Stanowisko");
            entity.Property(e => e.PkTelKomorka)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_TelKomorka");
            entity.Property(e => e.PkTelefon)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Telefon");
            entity.Property(e => e.PkUwagi)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pk_Uwagi");
            entity.Property(e => e.PkZablokowany).HasColumnName("pk_Zablokowany");
            entity.Property(e => e.PkZgodaDo).HasColumnName("pk_ZgodaDO");
            entity.Property(e => e.PkZgodaEmail).HasColumnName("pk_ZgodaEMail");
            entity.Property(e => e.PkZgodaMark).HasColumnName("pk_ZgodaMark");

            entity.HasOne(d => d.PkIdGrupaNavigation).WithMany(p => p.KhPracowniks)
                .HasForeignKey(d => d.PkIdGrupa)
                .HasConstraintName("FK_kh_Pracownik_sl_GrupaKh");

            entity.HasOne(d => d.PkIdKhNavigation).WithMany(p => p.KhPracowniks)
                .HasForeignKey(d => d.PkIdKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_kh_Pracownik_kh__Kontrahent");

            entity.HasOne(d => d.PkIdOpiekunNavigation).WithMany(p => p.KhPracowniks)
                .HasForeignKey(d => d.PkIdOpiekun)
                .HasConstraintName("FK_kh_Pracownik_pd_Uzytkownik");
        });

        modelBuilder.Entity<NzFinanse>(entity =>
        {
            entity.HasKey(e => e.NzfId).IsClustered(false);

            entity.ToTable("nz__Finanse", tb =>
                {
                    tb.HasTrigger("tr_NzFinanse_DokKas");
                    tb.HasTrigger("tr_NzFinanse_Inserting");
                    tb.HasTrigger("tr_NzFinanse_OpBank");
                    tb.HasTrigger("tr_NzFinanse_Updating");
                });

            entity.HasIndex(e => new { e.NzfIdDokumentAuto, e.NzfPodtyp }, "IX_nz__Finanse");

            entity.HasIndex(e => new { e.NzfTypObiektu, e.NzfIdObiektu }, "IX_nz__Finanse_1");

            entity.HasIndex(e => e.NzfData, "IX_nz__Finanse_2");

            entity.HasIndex(e => new { e.NzfData, e.NzfTyp, e.NzfIdKasy, e.NzfNumer }, "IX_nz__Finanse_3");

            entity.HasIndex(e => e.NzfIdRozliczenia, "IX_nz__Finanse_nzf_IdRozliczenia");

            entity.HasIndex(e => e.NzfNumer, "IX_nz__Finanse_nzf_Numer");

            entity.Property(e => e.NzfId)
                .ValueGeneratedNever()
                .HasColumnName("nzf_Id");
            entity.Property(e => e.NzfData)
                .HasColumnType("datetime")
                .HasColumnName("nzf_Data");
            entity.Property(e => e.NzfDataKursu)
                .HasColumnType("datetime")
                .HasColumnName("nzf_DataKursu");
            entity.Property(e => e.NzfDataOstatniejSplaty)
                .HasColumnType("datetime")
                .HasColumnName("nzf_DataOstatniejSplaty");
            entity.Property(e => e.NzfDataUzgodnienia)
                .HasColumnType("datetime")
                .HasColumnName("nzf_DataUzgodnienia");
            entity.Property(e => e.NzfDlaNieznany).HasColumnName("nzf_DlaNieznany");
            entity.Property(e => e.NzfFlaga).HasColumnName("nzf_Flaga");
            entity.Property(e => e.NzfFlagaZmianaCzas)
                .HasColumnType("datetime")
                .HasColumnName("nzf_FlagaZmianaCzas");
            entity.Property(e => e.NzfFlagaZmianaPersonelId).HasColumnName("nzf_FlagaZmianaPersonelId");
            entity.Property(e => e.NzfGenerujTytulem).HasColumnName("nzf_GenerujTytulem");
            entity.Property(e => e.NzfGotowkowa).HasColumnName("nzf_Gotowkowa");
            entity.Property(e => e.NzfIdAdresu).HasColumnName("nzf_IdAdresu");
            entity.Property(e => e.NzfIdBanku).HasColumnName("nzf_IdBanku");
            entity.Property(e => e.NzfIdDokumentAuto).HasColumnName("nzf_IdDokumentAuto");
            entity.Property(e => e.NzfIdHistoriiAdresu).HasColumnName("nzf_IdHistoriiAdresu");
            entity.Property(e => e.NzfIdKarta).HasColumnName("nzf_IdKarta");
            entity.Property(e => e.NzfIdKasy).HasColumnName("nzf_IdKasy");
            entity.Property(e => e.NzfIdKategorii).HasColumnName("nzf_IdKategorii");
            entity.Property(e => e.NzfIdObiektu).HasColumnName("nzf_IdObiektu");
            entity.Property(e => e.NzfIdRachObiekt).HasColumnName("nzf_IdRachObiekt");
            entity.Property(e => e.NzfIdRachObiektHist).HasColumnName("nzf_IdRachObiektHist");
            entity.Property(e => e.NzfIdRachunku).HasColumnName("nzf_IdRachunku");
            entity.Property(e => e.NzfIdRachunkuHist).HasColumnName("nzf_IdRachunkuHist");
            entity.Property(e => e.NzfIdRachunkuWirtualnego)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nzf_IdRachunkuWirtualnego");
            entity.Property(e => e.NzfIdRozliczenia).HasColumnName("nzf_IdRozliczenia");
            entity.Property(e => e.NzfIdRozrachunku).HasColumnName("nzf_IdRozrachunku");
            entity.Property(e => e.NzfIdSesjiKasowej)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nzf_IdSesjiKasowej");
            entity.Property(e => e.NzfIdSyncDivision)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nzf_IdSyncDivision");
            entity.Property(e => e.NzfIdTransakcjiVat).HasColumnName("nzf_IdTransakcjiVat");
            entity.Property(e => e.NzfIdWaluty)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("nzf_IdWaluty");
            entity.Property(e => e.NzfIdWystawil).HasColumnName("nzf_IdWystawil");
            entity.Property(e => e.NzfImport)
                .HasDefaultValue(0)
                .HasColumnName("nzf_Import");
            entity.Property(e => e.NzfIzbaCelna).HasColumnName("nzf_IzbaCelna");
            entity.Property(e => e.NzfKorekta).HasColumnName("nzf_Korekta");
            entity.Property(e => e.NzfKurs)
                .HasDefaultValue(1m)
                .HasColumnType("money")
                .HasColumnName("nzf_Kurs");
            entity.Property(e => e.NzfKwotaRachunkuVat)
                .HasDefaultValue(0m)
                .HasColumnType("money")
                .HasColumnName("nzf_KwotaRachunkuVAT");
            entity.Property(e => e.NzfLiczbaJednostek)
                .HasDefaultValue(1)
                .HasColumnName("nzf_LiczbaJednostek");
            entity.Property(e => e.NzfMechanizmPodzielonejPlatnosci).HasColumnName("nzf_MechanizmPodzielonejPlatnosci");
            entity.Property(e => e.NzfMetodaKasowa).HasColumnName("nzf_MetodaKasowa");
            entity.Property(e => e.NzfNota).HasColumnName("nzf_Nota");
            entity.Property(e => e.NzfNrFaktury)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasColumnName("nzf_NrFaktury");
            entity.Property(e => e.NzfNumer).HasColumnName("nzf_Numer");
            entity.Property(e => e.NzfNumerPelny)
                .HasMaxLength(144)
                .IsUnicode(false)
                .HasColumnName("nzf_NumerPelny");
            entity.Property(e => e.NzfNumerWyciagu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nzf_NumerWyciagu");
            entity.Property(e => e.NzfObslugaRachunku).HasColumnName("nzf_ObslugaRachunku");
            entity.Property(e => e.NzfOdebral)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nzf_Odebral");
            entity.Property(e => e.NzfOpis)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nzf_Opis");
            entity.Property(e => e.NzfPlatnoscKartaWoddziale).HasColumnName("nzf_PlatnoscKartaWOddziale");
            entity.Property(e => e.NzfPodIdentyfikacjaZobowiazania)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nzf_PodIdentyfikacjaZobowiazania");
            entity.Property(e => e.NzfPodOkres)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_PodOkres");
            entity.Property(e => e.NzfPodSymbol)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_PodSymbol");
            entity.Property(e => e.NzfPodtyp).HasColumnName("nzf_Podtyp");
            entity.Property(e => e.NzfPodtypPp).HasColumnName("nzf_PodtypPP");
            entity.Property(e => e.NzfPowiazanie).HasColumnName("nzf_Powiazanie");
            entity.Property(e => e.NzfPrb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nzf_PRB");
            entity.Property(e => e.NzfProgram)
                .HasDefaultValue(8)
                .HasColumnName("nzf_Program");
            entity.Property(e => e.NzfPrzedplataOddzial).HasColumnName("nzf_PrzedplataOddzial");
            entity.Property(e => e.NzfPrzeniesiony).HasColumnName("nzf_Przeniesiony");
            entity.Property(e => e.NzfRodzajKursu).HasColumnName("nzf_RodzajKursu");
            entity.Property(e => e.NzfRodzajSplaty).HasColumnName("nzf_RodzajSplaty");
            entity.Property(e => e.NzfSplata)
                .HasColumnType("money")
                .HasColumnName("nzf_Splata");
            entity.Property(e => e.NzfSplataWaluta)
                .HasColumnType("money")
                .HasColumnName("nzf_SplataWaluta");
            entity.Property(e => e.NzfStatus)
                .HasDefaultValue(1)
                .HasColumnName("nzf_Status");
            entity.Property(e => e.NzfStopaOdsetek)
                .HasDefaultValue(0m)
                .HasColumnType("money")
                .HasColumnName("nzf_StopaOdsetek");
            entity.Property(e => e.NzfTermPlatStatus).HasColumnName("nzf_TermPlatStatus");
            entity.Property(e => e.NzfTermPlatTerminalId)
                .HasMaxLength(40)
                .HasColumnName("nzf_TermPlatTerminalId");
            entity.Property(e => e.NzfTermPlatTransId)
                .HasMaxLength(128)
                .HasColumnName("nzf_TermPlatTransId");
            entity.Property(e => e.NzfTerminPlatnosci)
                .HasColumnType("datetime")
                .HasColumnName("nzf_TerminPlatnosci");
            entity.Property(e => e.NzfTransakcja)
                .HasMaxLength(144)
                .IsUnicode(false)
                .HasColumnName("nzf_Transakcja");
            entity.Property(e => e.NzfTransfer).HasColumnName("nzf_Transfer");
            entity.Property(e => e.NzfTyp).HasColumnName("nzf_Typ");
            entity.Property(e => e.NzfTypKorekty)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nzf_TypKorekty");
            entity.Property(e => e.NzfTypObiektu).HasColumnName("nzf_TypObiektu");
            entity.Property(e => e.NzfTypOdsetek).HasColumnName("nzf_TypOdsetek");
            entity.Property(e => e.NzfTypPrzelewu)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("nzf_TypPrzelewu");
            entity.Property(e => e.NzfTytulem)
                .HasMaxLength(144)
                .IsUnicode(false)
                .HasColumnName("nzf_Tytulem");
            entity.Property(e => e.NzfVatRozliczanyPrzezUslugobiorce).HasColumnName("nzf_VatRozliczanyPrzezUslugobiorce");
            entity.Property(e => e.NzfVatpierwotny)
                .HasColumnType("money")
                .HasColumnName("nzf_VATPierwotny");
            entity.Property(e => e.NzfVatpierwotnyWaluta)
                .HasColumnType("money")
                .HasColumnName("nzf_VATPierwotnyWaluta");
            entity.Property(e => e.NzfVatpozostalo)
                .HasColumnType("money")
                .HasColumnName("nzf_VATPozostalo");
            entity.Property(e => e.NzfVatpozostaloWaluta)
                .HasColumnType("money")
                .HasColumnName("nzf_VATPozostaloWaluta");
            entity.Property(e => e.NzfWartosc)
                .HasColumnType("money")
                .HasColumnName("nzf_Wartosc");
            entity.Property(e => e.NzfWartoscPierwotna)
                .HasColumnType("money")
                .HasColumnName("nzf_WartoscPierwotna");
            entity.Property(e => e.NzfWartoscPierwotnaWaluta)
                .HasColumnType("money")
                .HasColumnName("nzf_WartoscPierwotnaWaluta");
            entity.Property(e => e.NzfWartoscWaluta)
                .HasColumnType("money")
                .HasColumnName("nzf_WartoscWaluta");
            entity.Property(e => e.NzfWydrukowana).HasColumnName("nzf_Wydrukowana");
            entity.Property(e => e.NzfWyslanaHb).HasColumnName("nzf_WyslanaHB");
            entity.Property(e => e.NzfWystawil)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("nzf_Wystawil");
            entity.Property(e => e.NzfZaliczka).HasColumnName("nzf_Zaliczka");
            entity.Property(e => e.NzfZrodlo).HasColumnName("nzf_Zrodlo");
            entity.Property(e => e.NzfZusdeklaracja)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_ZUSDeklaracja");
            entity.Property(e => e.NzfZusnrDecyzji)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nzf_ZUSNrDecyzji");
            entity.Property(e => e.NzfZusnrDeklaracji)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_ZUSNrDeklaracji");
            entity.Property(e => e.NzfZuspodIdentyfikator)
                .HasMaxLength(14)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_ZUSPodIdentyfikator");
            entity.Property(e => e.NzfZuspodTypIdentyfikatora)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_ZUSPodTypIdentyfikatora");
            entity.Property(e => e.NzfZustypWplaty)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nzf_ZUSTypWplaty");

            entity.HasOne(d => d.NzfFlagaZmianaPersonel).WithMany(p => p.NzFinanses)
                .HasForeignKey(d => d.NzfFlagaZmianaPersonelId)
                .HasConstraintName("FK_nz__Finanse_pd_Uzytkownik");

            entity.HasOne(d => d.NzfIdAdresuNavigation).WithMany(p => p.NzFinanses)
                .HasForeignKey(d => d.NzfIdAdresu)
                .HasConstraintName("FK_nz__Finanse_adr__Ewid");

            entity.HasOne(d => d.NzfIdDokumentAutoNavigation).WithMany(p => p.NzFinanses)
                .HasForeignKey(d => d.NzfIdDokumentAuto)
                .HasConstraintName("FK_nz__Finanse_dok__Dokument");

            entity.HasOne(d => d.NzfIdHistoriiAdresuNavigation).WithMany(p => p.NzFinanses)
                .HasForeignKey(d => d.NzfIdHistoriiAdresu)
                .HasConstraintName("FK_nz__Finanse_adr_Historia");

            entity.HasOne(d => d.NzfIdKasyNavigation).WithMany(p => p.NzFinanses)
                .HasForeignKey(d => d.NzfIdKasy)
                .HasConstraintName("FK_nz__Finanse_dks_Kasa");

            entity.HasOne(d => d.NzfIdKategoriiNavigation).WithMany(p => p.NzFinanses)
                .HasForeignKey(d => d.NzfIdKategorii)
                .HasConstraintName("FK_nz__Finanse_sl_Kategoria");

            entity.HasOne(d => d.NzfIdRozrachunkuNavigation).WithMany(p => p.InverseNzfIdRozrachunkuNavigation)
                .HasForeignKey(d => d.NzfIdRozrachunku)
                .HasConstraintName("FK_nz__Finanse_nz__Finanse");
        });

        modelBuilder.Entity<PdPodmiot>(entity =>
        {
            entity.HasKey(e => e.PdId);

            entity.ToTable("pd__Podmiot");

            entity.Property(e => e.PdId)
                .ValueGeneratedNever()
                .HasColumnName("pd_Id");
            entity.Property(e => e.PdAbonPokazDymki)
                .HasDefaultValue(true)
                .HasColumnName("pd_AbonPokazDymki");
            entity.Property(e => e.PdAnkDemo).HasColumnName("pd_AnkDemo");
            entity.Property(e => e.PdCrm).HasColumnName("pd_CRM");
            entity.Property(e => e.PdCzyUstawionoDateRozpDzial).HasColumnName("pd_CzyUstawionoDateRozpDzial");
            entity.Property(e => e.PdDaneDemo).HasColumnName("pd_DaneDemo");
            entity.Property(e => e.PdDataRejestracji)
                .HasColumnType("datetime")
                .HasColumnName("pd_DataRejestracji");
            entity.Property(e => e.PdDataRezygnacjiZryczaltu)
                .HasColumnType("datetime")
                .HasColumnName("pd_DataRezygnacjiZRyczaltu");
            entity.Property(e => e.PdDataRozpDzial)
                .HasColumnType("datetime")
                .HasColumnName("pd_DataRozpDzial");
            entity.Property(e => e.PdDisableScripts)
                .HasDefaultValue(true)
                .HasColumnName("pd_DisableScripts");
            entity.Property(e => e.PdDmn).HasColumnName("pd_DMN");
            entity.Property(e => e.PdDocumentsPortalLastSeenActive).HasColumnName("pd_DocumentsPortalLastSeenActive");
            entity.Property(e => e.PdDokMagDyspWylaczKasowanie).HasColumnName("pd_DokMagDyspWylaczKasowanie");
            entity.Property(e => e.PdDzHandlowa).HasColumnName("pd_DzHandlowa");
            entity.Property(e => e.PdDzProdukcyjna).HasColumnName("pd_DzProdukcyjna");
            entity.Property(e => e.PdDzUslugowa).HasColumnName("pd_DzUslugowa");
            entity.Property(e => e.PdEbankSaldo).HasColumnName("pd_EBankSaldo");
            entity.Property(e => e.PdEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_Email");
            entity.Property(e => e.PdEmailWystawca)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_EmailWystawca");
            entity.Property(e => e.PdFormaPrawna).HasColumnName("pd_FormaPrawna");
            entity.Property(e => e.PdIdWlasciciela).HasColumnName("pd_IdWlasciciela");
            entity.Property(e => e.PdInfoHtml)
                .HasMaxLength(6000)
                .HasDefaultValueSql("(CONVERT([varbinary](6000),'',(0)))")
                .HasColumnName("pd_InfoHtml");
            entity.Property(e => e.PdKasa).HasColumnName("pd_Kasa");
            entity.Property(e => e.PdKatDkr).HasColumnName("pd_KatDkr");
            entity.Property(e => e.PdKlientEmail).HasColumnName("pd_KlientEmail");
            entity.Property(e => e.PdKmr).HasColumnName("pd_KMR");
            entity.Property(e => e.PdKodEkd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_KodEKD");
            entity.Property(e => e.PdKodKgn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_KodKGN");
            entity.Property(e => e.PdKonto).HasColumnName("pd_Konto");
            entity.Property(e => e.PdLicCentrala).HasColumnName("pd_LicCentrala");
            entity.Property(e => e.PdLicEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicEmail");
            entity.Property(e => e.PdLicFaks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicFaks");
            entity.Property(e => e.PdLicIdGminy).HasColumnName("pd_LicIdGminy");
            entity.Property(e => e.PdLicInneDane).HasColumnName("pd_LicInneDane");
            entity.Property(e => e.PdLicKod)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicKod");
            entity.Property(e => e.PdLicKorInneDane)
                .IsRequired()
                .HasDefaultValueSql("('')")
                .HasColumnName("pd_LicKorInneDane");
            entity.Property(e => e.PdLicKorKod)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicKorKod");
            entity.Property(e => e.PdLicKorMiejscowosc)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicKorMiejscowosc");
            entity.Property(e => e.PdLicKorNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicKorNazwa");
            entity.Property(e => e.PdLicKorOsobaOdp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicKorOsobaOdp");
            entity.Property(e => e.PdLicKorUlica)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicKorUlica");
            entity.Property(e => e.PdLicKorWojewodztwo)
                .HasDefaultValue(-1)
                .HasColumnName("pd_LicKorWojewodztwo");
            entity.Property(e => e.PdLicMiejscowosc)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicMiejscowosc");
            entity.Property(e => e.PdLicNazwaFirmy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicNazwaFirmy");
            entity.Property(e => e.PdLicNip)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicNIP");
            entity.Property(e => e.PdLicOsobaOdp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicOsobaOdp");
            entity.Property(e => e.PdLicTelefon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicTelefon");
            entity.Property(e => e.PdLicUlica)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicUlica");
            entity.Property(e => e.PdLicUzInneDane).HasColumnName("pd_LicUzInneDane");
            entity.Property(e => e.PdLicUzKod)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicUzKod");
            entity.Property(e => e.PdLicUzMiejscowosc)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicUzMiejscowosc");
            entity.Property(e => e.PdLicUzOsobaOdp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicUzOsobaOdp");
            entity.Property(e => e.PdLicUzTelefon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicUzTelefon");
            entity.Property(e => e.PdLicUzUlica)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_LicUzUlica");
            entity.Property(e => e.PdLicWojewodztwo)
                .HasDefaultValue(-1)
                .HasColumnName("pd_LicWojewodztwo");
            entity.Property(e => e.PdMiesiacPierwszejWyplaty)
                .HasColumnType("datetime")
                .HasColumnName("pd_MiesiacPierwszejWyplaty");
            entity.Property(e => e.PdNazwaDlaPlatnika)
                .HasMaxLength(31)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_NazwaDlaPlatnika");
            entity.Property(e => e.PdNazwaRejestru)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_NazwaRejestru");
            entity.Property(e => e.PdNipue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_NIPUE");
            entity.Property(e => e.PdNrBdo)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_NrBDO");
            entity.Property(e => e.PdNrKrs)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_NrKRS");
            entity.Property(e => e.PdNrPdmPosredniczacego)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("pd_NrPdmPosredniczacego");
            entity.Property(e => e.PdNumerRejestru)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_NumerRejestru");
            entity.Property(e => e.PdObraz)
                .HasColumnType("image")
                .HasColumnName("pd_Obraz");
            entity.Property(e => e.PdOpisDzialalnosci)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_OpisDzialalnosci");
            entity.Property(e => e.PdOrganRejestrowy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_OrganRejestrowy");
            entity.Property(e => e.PdOrganZaloz)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_OrganZaloz");
            entity.Property(e => e.PdOsobaFizyczna).HasColumnName("pd_OsobaFizyczna");
            entity.Property(e => e.PdPlanKontWylaczAkt).HasColumnName("pd_PlanKontWylaczAkt");
            entity.Property(e => e.PdPlatKarSprzedaz).HasColumnName("pd_PlatKarSprzedaz");
            entity.Property(e => e.PdPlatKarZakup).HasColumnName("pd_PlatKarZakup");
            entity.Property(e => e.PdPlatOdrocz).HasColumnName("pd_PlatOdrocz");
            entity.Property(e => e.PdPodVatzarejestrowanyWue).HasColumnName("pd_PodVATZarejestrowanyWUE");
            entity.Property(e => e.PdRachBankowy).HasColumnName("pd_RachBankowy");
            entity.Property(e => e.PdRegon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("pd_Regon");
            entity.Property(e => e.PdRodzajPowiazaniaWystawcy)
                .HasDefaultValue(7)
                .HasColumnName("pd_RodzajPowiazaniaWystawcy");
            entity.Property(e => e.PdRodzajZmianyFormyOpodatkowania).HasColumnName("pd_RodzajZmianyFormyOpodatkowania");
            entity.Property(e => e.PdRokBoHopGrat).HasColumnName("pd_RokBoHopGrat");
            entity.Property(e => e.PdTelemetriaPlusZgoda).HasColumnName("pd_TelemetriaPlusZgoda");
            entity.Property(e => e.PdTelemetriaPlusZgodaPytano)
                .HasColumnType("datetime")
                .HasColumnName("pd_TelemetriaPlusZgodaPytano");
            entity.Property(e => e.PdTelemetriaZgoda).HasColumnName("pd_TelemetriaZgoda");
            entity.Property(e => e.PdTelemetriaZgodaPytano)
                .HasColumnType("datetime")
                .HasColumnName("pd_TelemetriaZgodaPytano");
            entity.Property(e => e.PdTypDzialalnosci).HasColumnName("pd_TypDzialalnosci");
            entity.Property(e => e.PdTypKadr).HasColumnName("pd_TypKadr");
            entity.Property(e => e.PdTypKsiegowosci).HasColumnName("pd_TypKsiegowosci");
            entity.Property(e => e.PdTypPdmAkcyzowego).HasColumnName("pd_TypPdmAkcyzowego");
            entity.Property(e => e.PdUrzSkarbowy).HasColumnName("pd_UrzSkarbowy");
            entity.Property(e => e.PdUtworzonoZbazy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pd_UtworzonoZBazy");
            entity.Property(e => e.PdUtworzonoZpodmiotu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pd_UtworzonoZPodmiotu");
            entity.Property(e => e.PdUzywajNipJakoNrAkcyzowy).HasColumnName("pd_UzywajNipJakoNrAkcyzowy");
            entity.Property(e => e.PdWww)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("pd_WWW");
        });

        modelBuilder.Entity<PdUzytkownik>(entity =>
        {
            entity.HasKey(e => e.UzId);

            entity.ToTable("pd_Uzytkownik", tb =>
                {
                    tb.HasTrigger("pd_Uzytkownik_DodajUzMag");
                    tb.HasTrigger("pd_Uzytkownik_DodajUzOkr");
                    tb.HasTrigger("pd_Uzytkownik_DodajUzRok");
                    tb.HasTrigger("tr_pd_Uzytkownik_Deleting");
                    tb.HasTrigger("tr_pd_Uzytkownik_Inserting_Updating");
                });

            entity.HasIndex(e => e.UzIdentyfikator, "IX_pd_Uzytkownik_Ident").IsUnique();

            entity.Property(e => e.UzId)
                .ValueGeneratedNever()
                .HasColumnName("uz_Id");
            entity.Property(e => e.UzAlarmyInterwal).HasColumnName("uz_AlarmyInterwal");
            entity.Property(e => e.UzBlokadaKasy).HasColumnName("uz_BlokadaKasy");
            entity.Property(e => e.UzDataHasla)
                .HasColumnType("datetime")
                .HasColumnName("uz_DataHasla");
            entity.Property(e => e.UzDataPonownegoPrzypomnienia)
                .HasColumnType("datetime")
                .HasColumnName("uz_DataPonownegoPrzypomnienia");
            entity.Property(e => e.UzDomena)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_Domena");
            entity.Property(e => e.UzEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_EMail");
            entity.Property(e => e.UzGaduGadu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_GaduGadu");
            entity.Property(e => e.UzHaslo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_Haslo");
            entity.Property(e => e.UzIdGrupy).HasColumnName("uz_IdGrupy");
            entity.Property(e => e.UzIdKasy).HasColumnName("uz_IdKasy");
            entity.Property(e => e.UzIdKompozycji).HasColumnName("uz_IdKompozycji");
            entity.Property(e => e.UzIdMagazynu).HasColumnName("uz_IdMagazynu");
            entity.Property(e => e.UzIdPracownika).HasColumnName("uz_IdPracownika");
            entity.Property(e => e.UzIdentyfikator)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("uz_Identyfikator");
            entity.Property(e => e.UzImie)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_Imie");
            entity.Property(e => e.UzKlientEmail).HasColumnName("uz_KlientEmail");
            entity.Property(e => e.UzLimitStanowisk).HasColumnName("uz_LimitStanowisk");
            entity.Property(e => e.UzLogin)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_Login");
            entity.Property(e => e.UzLokalizacja)
                .HasMaxLength(256)
                .HasColumnName("uz_Lokalizacja");
            entity.Property(e => e.UzLokalizacjaPokazuj)
                .HasDefaultValue(true)
                .HasColumnName("uz_LokalizacjaPokazuj");
            entity.Property(e => e.UzNazwisko)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("uz_Nazwisko");
            entity.Property(e => e.UzOstatnieKontoEmail).HasColumnName("uz_OstatnieKontoEmail");
            entity.Property(e => e.UzPodnoszenieUprawnienUserId).HasColumnName("uz_PodnoszenieUprawnienUserId");
            entity.Property(e => e.UzPracaZdalna).HasColumnName("uz_PracaZdalna");
            entity.Property(e => e.UzPrefiksOsobisty)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("uz_PrefiksOsobisty");
            entity.Property(e => e.UzProgram)
                .HasDefaultValue(1)
                .HasColumnName("uz_Program");
            entity.Property(e => e.UzRodzajInfoOwierszachListy)
                .HasDefaultValue(1)
                .HasColumnName("uz_RodzajInfoOWierszachListy");
            entity.Property(e => e.UzShowTutorialSms)
                .HasDefaultValue(true)
                .HasColumnName("uz_ShowTutorialSMS");
            entity.Property(e => e.UzSkype)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("uz_Skype");
            entity.Property(e => e.UzStatus)
                .HasDefaultValue(true)
                .HasColumnName("uz_Status");
            entity.Property(e => e.UzStatusPrzypomnieniaZmianyVat)
                .HasDefaultValue(3)
                .HasColumnName("uz_StatusPrzypomnieniaZmianyVAT");
            entity.Property(e => e.UzUruchomCentralke).HasColumnName("uz_UruchomCentralke");
            entity.Property(e => e.UzZmianaHasla).HasColumnName("uz_ZmianaHasla");

            entity.HasOne(d => d.UzIdKasyNavigation).WithMany(p => p.PdUzytkowniks)
                .HasForeignKey(d => d.UzIdKasy)
                .HasConstraintName("FK_pd_Uzytkownik_dks_Kasa");

            entity.HasOne(d => d.UzIdMagazynuNavigation).WithMany(p => p.PdUzytkowniks)
                .HasForeignKey(d => d.UzIdMagazynu)
                .HasConstraintName("FK_pd_Uzytkownik_sl_Magazyn");
        });

        modelBuilder.Entity<SlBank>(entity =>
        {
            entity.HasKey(e => e.BnNrRozliczeniowy);

            entity.ToTable("sl_Bank");

            entity.Property(e => e.BnNrRozliczeniowy)
                .ValueGeneratedNever()
                .HasColumnName("bn_NrRozliczeniowy");
            entity.Property(e => e.BnNazwa)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("bn_Nazwa");
        });

        modelBuilder.Entity<SlCechaKh>(entity =>
        {
            entity.HasKey(e => e.CkhId);

            entity.ToTable("sl_CechaKh");

            entity.Property(e => e.CkhId)
                .ValueGeneratedNever()
                .HasColumnName("ckh_Id");
            entity.Property(e => e.CkhNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ckh_Nazwa");
        });

        modelBuilder.Entity<SlCechaTw>(entity =>
        {
            entity.HasKey(e => e.CtwId);

            entity.ToTable("sl_CechaTw");

            entity.Property(e => e.CtwId)
                .ValueGeneratedNever()
                .HasColumnName("ctw_Id");
            entity.Property(e => e.CtwNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ctw_Nazwa");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rowguid");
        });

        modelBuilder.Entity<SlGrupaKh>(entity =>
        {
            entity.HasKey(e => e.GrkId);

            entity.ToTable("sl_GrupaKh");

            entity.HasIndex(e => e.GrkNazwa, "IX_sl_GrupaKh").IsUnique();

            entity.Property(e => e.GrkId)
                .ValueGeneratedNever()
                .HasColumnName("grk_Id");
            entity.Property(e => e.GrkNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("grk_Nazwa");
        });

        modelBuilder.Entity<SlGrupaTw>(entity =>
        {
            entity.HasKey(e => e.GrtId);

            entity.ToTable("sl_GrupaTw");

            entity.Property(e => e.GrtId)
                .ValueGeneratedNever()
                .HasColumnName("grt_Id");
            entity.Property(e => e.GrtNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("grt_Nazwa");
            entity.Property(e => e.GrtNrAnalityka)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("grt_NrAnalityka");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rowguid");
        });

        modelBuilder.Entity<SlKategorium>(entity =>
        {
            entity.HasKey(e => e.KatId);

            entity.ToTable("sl_Kategoria");

            entity.Property(e => e.KatId)
                .ValueGeneratedNever()
                .HasColumnName("kat_Id");
            entity.Property(e => e.KatNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kat_Nazwa");
            entity.Property(e => e.KatPodtytul)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kat_Podtytul");
            entity.Property(e => e.KatTyp).HasColumnName("kat_Typ");
        });

        modelBuilder.Entity<SlMagazyn>(entity =>
        {
            entity.HasKey(e => e.MagId);

            entity.ToTable("sl_Magazyn", tb =>
                {
                    tb.HasTrigger("tr_MagazynInsMod");
                    tb.HasTrigger("tr_slMagazynInserting");
                    tb.HasTrigger("tr_sl_Magazyn_Deleting");
                });

            entity.Property(e => e.MagId)
                .ValueGeneratedNever()
                .HasColumnName("mag_Id");
            entity.Property(e => e.MagAnalityka)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("mag_Analityka");
            entity.Property(e => e.MagGlowny).HasColumnName("mag_Glowny");
            entity.Property(e => e.MagNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("mag_Nazwa");
            entity.Property(e => e.MagOpis)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("mag_Opis");
            entity.Property(e => e.MagPos).HasColumnName("mag_POS");
            entity.Property(e => e.MagPosadres)
                .HasMaxLength(82)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("mag_POSAdres");
            entity.Property(e => e.MagPosident).HasColumnName("mag_POSIdent");
            entity.Property(e => e.MagPosnazwa)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("mag_POSNazwa");
            entity.Property(e => e.MagStatus)
                .HasDefaultValue(1)
                .HasColumnName("mag_Status");
            entity.Property(e => e.MagSymbol)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("mag_Symbol");
        });

        modelBuilder.Entity<SlStawkaVat>(entity =>
        {
            entity.HasKey(e => e.VatId);

            entity.ToTable("sl_StawkaVAT", tb =>
                {
                    tb.HasTrigger("tr_slStawkiVatDeleting");
                    tb.HasTrigger("tr_slStawkiVatInserting");
                });

            entity.HasIndex(e => e.VatId, "IX_sl_StawkaVAT_Symbol").IsUnique();

            entity.Property(e => e.VatId)
                .ValueGeneratedNever()
                .HasColumnName("vat_Id");
            entity.Property(e => e.VatCzySystemowa).HasColumnName("vat_CzySystemowa");
            entity.Property(e => e.VatCzyWidoczna).HasColumnName("vat_CzyWidoczna");
            entity.Property(e => e.VatIdPanstwo).HasColumnName("vat_IdPanstwo");
            entity.Property(e => e.VatNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("vat_Nazwa");
            entity.Property(e => e.VatPozDomyslna).HasColumnName("vat_PozDomyslna");
            entity.Property(e => e.VatPozRr).HasColumnName("vat_PozRR");
            entity.Property(e => e.VatPozSprzedaz).HasColumnName("vat_PozSprzedaz");
            entity.Property(e => e.VatPozZakup).HasColumnName("vat_PozZakup");
            entity.Property(e => e.VatPozycja).HasColumnName("vat_Pozycja");
            entity.Property(e => e.VatRodzaj).HasColumnName("vat_Rodzaj");
            entity.Property(e => e.VatStawka)
                .HasColumnType("money")
                .HasColumnName("vat_Stawka");
            entity.Property(e => e.VatStawkaZagraniczna).HasColumnName("vat_StawkaZagraniczna");
            entity.Property(e => e.VatStawkaZagranicznaPdst).HasColumnName("vat_StawkaZagranicznaPdst");
            entity.Property(e => e.VatSymbol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("vat_Symbol");
            entity.Property(e => e.VatUePanstwo).HasColumnName("vat_UePanstwo");
        });

        modelBuilder.Entity<SlWlasny>(entity =>
        {
            entity.HasKey(e => e.SwId)
                .HasName("PK__sl_Wlasny")
                .IsClustered(false);

            entity.ToTable("sl_Wlasny");

            entity.HasIndex(e => new { e.SwSlownikId, e.SwId }, "IX_sl_Wlasny")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.SwId)
                .ValueGeneratedNever()
                .HasColumnName("sw_Id");
            entity.Property(e => e.SwNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("sw_Nazwa");
            entity.Property(e => e.SwSlownikId).HasColumnName("sw_SlownikId");
        });

        modelBuilder.Entity<TwCechaTw>(entity =>
        {
            entity.HasKey(e => e.ChtId);

            entity.ToTable("tw_CechaTw", tb =>
                {
                    tb.HasTrigger("tr_tw_CechaTw_Synch_delete");
                    tb.HasTrigger("tr_tw_CechaTw_Synch_insert");
                    tb.HasTrigger("tr_tw_CechaTw_Synch_update");
                });

            entity.Property(e => e.ChtId)
                .ValueGeneratedNever()
                .HasColumnName("cht_Id");
            entity.Property(e => e.ChtIdCecha).HasColumnName("cht_IdCecha");
            entity.Property(e => e.ChtIdTowar).HasColumnName("cht_IdTowar");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rowguid");

            entity.HasOne(d => d.ChtIdCechaNavigation).WithMany(p => p.TwCechaTws)
                .HasForeignKey(d => d.ChtIdCecha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_CechaTw_sl_CechaTw");

            entity.HasOne(d => d.ChtIdTowarNavigation).WithMany(p => p.TwCechaTws)
                .HasForeignKey(d => d.ChtIdTowar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_GrupaTw_tw__Towar");
        });

        modelBuilder.Entity<TwCena>(entity =>
        {
            entity.HasKey(e => e.TcId);

            entity.ToTable("tw_Cena", tb =>
                {
                    tb.HasTrigger("IF_Upd_Ins_TowarCena");
                    tb.HasTrigger("tr_tw_Cena_Updating");
                });

            entity.HasIndex(e => e.TcIdTowar, "IX_tw_Cena").IsUnique();

            entity.Property(e => e.TcId)
                .ValueGeneratedNever()
                .HasColumnName("tc_Id");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rowguid");
            entity.Property(e => e.TcBankKursuWaluty).HasColumnName("tc_BankKursuWaluty");
            entity.Property(e => e.TcCenaBrutto0)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto0");
            entity.Property(e => e.TcCenaBrutto1)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto1");
            entity.Property(e => e.TcCenaBrutto10)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto10");
            entity.Property(e => e.TcCenaBrutto2)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto2");
            entity.Property(e => e.TcCenaBrutto3)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto3");
            entity.Property(e => e.TcCenaBrutto4)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto4");
            entity.Property(e => e.TcCenaBrutto5)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto5");
            entity.Property(e => e.TcCenaBrutto6)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto6");
            entity.Property(e => e.TcCenaBrutto7)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto7");
            entity.Property(e => e.TcCenaBrutto8)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto8");
            entity.Property(e => e.TcCenaBrutto9)
                .HasColumnType("money")
                .HasColumnName("tc_CenaBrutto9");
            entity.Property(e => e.TcCenaNetto0)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto0");
            entity.Property(e => e.TcCenaNetto1)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto1");
            entity.Property(e => e.TcCenaNetto10)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto10");
            entity.Property(e => e.TcCenaNetto2)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto2");
            entity.Property(e => e.TcCenaNetto3)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto3");
            entity.Property(e => e.TcCenaNetto4)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto4");
            entity.Property(e => e.TcCenaNetto5)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto5");
            entity.Property(e => e.TcCenaNetto6)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto6");
            entity.Property(e => e.TcCenaNetto7)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto7");
            entity.Property(e => e.TcCenaNetto8)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto8");
            entity.Property(e => e.TcCenaNetto9)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNetto9");
            entity.Property(e => e.TcCenaNettoWaluta)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNettoWaluta");
            entity.Property(e => e.TcCenaNettoWaluta2)
                .HasColumnType("money")
                .HasColumnName("tc_CenaNettoWaluta2");
            entity.Property(e => e.TcCenaWalutaNarzut)
                .HasColumnType("money")
                .HasColumnName("tc_CenaWalutaNarzut");
            entity.Property(e => e.TcDataKursuWaluty)
                .HasColumnType("datetime")
                .HasColumnName("tc_DataKursuWaluty");
            entity.Property(e => e.TcIdTowar).HasColumnName("tc_IdTowar");
            entity.Property(e => e.TcIdWaluta0)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta0");
            entity.Property(e => e.TcIdWaluta1)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta1");
            entity.Property(e => e.TcIdWaluta10)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta10");
            entity.Property(e => e.TcIdWaluta2)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta2");
            entity.Property(e => e.TcIdWaluta3)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta3");
            entity.Property(e => e.TcIdWaluta4)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta4");
            entity.Property(e => e.TcIdWaluta5)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta5");
            entity.Property(e => e.TcIdWaluta6)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta6");
            entity.Property(e => e.TcIdWaluta7)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta7");
            entity.Property(e => e.TcIdWaluta8)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta8");
            entity.Property(e => e.TcIdWaluta9)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PLN")
                .IsFixedLength()
                .HasColumnName("tc_IdWaluta9");
            entity.Property(e => e.TcIdWalutaKurs).HasColumnName("tc_IdWalutaKurs");
            entity.Property(e => e.TcKursWaluty1).HasColumnName("tc_KursWaluty1");
            entity.Property(e => e.TcKursWaluty10).HasColumnName("tc_KursWaluty10");
            entity.Property(e => e.TcKursWaluty2).HasColumnName("tc_KursWaluty2");
            entity.Property(e => e.TcKursWaluty3).HasColumnName("tc_KursWaluty3");
            entity.Property(e => e.TcKursWaluty4).HasColumnName("tc_KursWaluty4");
            entity.Property(e => e.TcKursWaluty5).HasColumnName("tc_KursWaluty5");
            entity.Property(e => e.TcKursWaluty6).HasColumnName("tc_KursWaluty6");
            entity.Property(e => e.TcKursWaluty7).HasColumnName("tc_KursWaluty7");
            entity.Property(e => e.TcKursWaluty8).HasColumnName("tc_KursWaluty8");
            entity.Property(e => e.TcKursWaluty9).HasColumnName("tc_KursWaluty9");
            entity.Property(e => e.TcMarza1)
                .HasColumnType("money")
                .HasColumnName("tc_Marza1");
            entity.Property(e => e.TcMarza10)
                .HasColumnType("money")
                .HasColumnName("tc_Marza10");
            entity.Property(e => e.TcMarza2)
                .HasColumnType("money")
                .HasColumnName("tc_Marza2");
            entity.Property(e => e.TcMarza3)
                .HasColumnType("money")
                .HasColumnName("tc_Marza3");
            entity.Property(e => e.TcMarza4)
                .HasColumnType("money")
                .HasColumnName("tc_Marza4");
            entity.Property(e => e.TcMarza5)
                .HasColumnType("money")
                .HasColumnName("tc_Marza5");
            entity.Property(e => e.TcMarza6)
                .HasColumnType("money")
                .HasColumnName("tc_Marza6");
            entity.Property(e => e.TcMarza7)
                .HasColumnType("money")
                .HasColumnName("tc_Marza7");
            entity.Property(e => e.TcMarza8)
                .HasColumnType("money")
                .HasColumnName("tc_Marza8");
            entity.Property(e => e.TcMarza9)
                .HasColumnType("money")
                .HasColumnName("tc_Marza9");
            entity.Property(e => e.TcNarzut1)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut1");
            entity.Property(e => e.TcNarzut10)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut10");
            entity.Property(e => e.TcNarzut2)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut2");
            entity.Property(e => e.TcNarzut3)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut3");
            entity.Property(e => e.TcNarzut4)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut4");
            entity.Property(e => e.TcNarzut5)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut5");
            entity.Property(e => e.TcNarzut6)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut6");
            entity.Property(e => e.TcNarzut7)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut7");
            entity.Property(e => e.TcNarzut8)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut8");
            entity.Property(e => e.TcNarzut9)
                .HasColumnType("money")
                .HasColumnName("tc_Narzut9");
            entity.Property(e => e.TcPodstawaKc)
                .HasDefaultValue(0)
                .HasColumnName("tc_PodstawaKC");
            entity.Property(e => e.TcWalutaId)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tc_WalutaId");
            entity.Property(e => e.TcWalutaJedn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tc_WalutaJedn");
            entity.Property(e => e.TcWalutaKurs)
                .HasColumnType("money")
                .HasColumnName("tc_WalutaKurs");
            entity.Property(e => e.TcZysk1)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk1");
            entity.Property(e => e.TcZysk10)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk10");
            entity.Property(e => e.TcZysk2)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk2");
            entity.Property(e => e.TcZysk3)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk3");
            entity.Property(e => e.TcZysk4)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk4");
            entity.Property(e => e.TcZysk5)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk5");
            entity.Property(e => e.TcZysk6)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk6");
            entity.Property(e => e.TcZysk7)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk7");
            entity.Property(e => e.TcZysk8)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk8");
            entity.Property(e => e.TcZysk9)
                .HasColumnType("money")
                .HasColumnName("tc_Zysk9");

            entity.HasOne(d => d.TcIdTowarNavigation).WithOne(p => p.TwCena)
                .HasForeignKey<TwCena>(d => d.TcIdTowar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_Cena_tw__Towar");
        });

        modelBuilder.Entity<TwJednMiary>(entity =>
        {
            entity.HasKey(e => e.JmId);

            entity.ToTable("tw_JednMiary");

            entity.HasIndex(e => new { e.JmIdTowar, e.JmIdJednMiary }, "IX_tw_JednMiary").IsUnique();

            entity.Property(e => e.JmId)
                .ValueGeneratedNever()
                .HasColumnName("jm_Id");
            entity.Property(e => e.JmDlaNaklejek).HasColumnName("jm_DlaNaklejek");
            entity.Property(e => e.JmIdJednMiary)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("jm_IdJednMiary");
            entity.Property(e => e.JmIdTowar).HasColumnName("jm_IdTowar");
            entity.Property(e => e.JmPrzelicznik)
                .HasColumnType("money")
                .HasColumnName("jm_Przelicznik");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rowguid");

            entity.HasOne(d => d.JmIdTowarNavigation).WithMany(p => p.TwJednMiaries)
                .HasForeignKey(d => d.JmIdTowar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_JednMiary_tw__Towar");
        });

        modelBuilder.Entity<TwKodKreskowy>(entity =>
        {
            entity.HasKey(e => e.KkId);

            entity.ToTable("tw_KodKreskowy", tb =>
                {
                    tb.HasTrigger("tr_TwKodKreskowy_Deleting");
                    tb.HasTrigger("tr_TwKodKreskowy_Inserting");
                    tb.HasTrigger("tr_TwKodKreskowy_Updating");
                    tb.HasTrigger("tr_tw_KodKreskowy_Synch_delete");
                    tb.HasTrigger("tr_tw_KodKreskowy_Synch_insert");
                    tb.HasTrigger("tr_tw_KodKreskowy_Synch_update");
                });

            entity.HasIndex(e => new { e.KkIdTowar, e.KkKod }, "IX_tw_KodKreskowy").IsUnique();

            entity.Property(e => e.KkId)
                .ValueGeneratedNever()
                .HasColumnName("kk_Id");
            entity.Property(e => e.KkIdTowar).HasColumnName("kk_IdTowar");
            entity.Property(e => e.KkIlosc)
                .HasColumnType("money")
                .HasColumnName("kk_Ilosc");
            entity.Property(e => e.KkKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("kk_Kod");

            entity.HasOne(d => d.KkIdTowarNavigation).WithMany(p => p.TwKodKreskowies)
                .HasForeignKey(d => d.KkIdTowar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_KodKreskowy_tw__Towar");
        });

        modelBuilder.Entity<TwKomplet>(entity =>
        {
            entity.HasKey(e => e.KplId);

            entity.ToTable("tw_Komplet");

            entity.Property(e => e.KplId)
                .ValueGeneratedNever()
                .HasColumnName("kpl_Id");
            entity.Property(e => e.KplIdKomplet).HasColumnName("kpl_IdKomplet");
            entity.Property(e => e.KplIdSkladnik).HasColumnName("kpl_IdSkladnik");
            entity.Property(e => e.KplLiczba)
                .HasColumnType("money")
                .HasColumnName("kpl_Liczba");

            entity.HasOne(d => d.KplIdSkladnikNavigation).WithMany(p => p.TwKomplets)
                .HasForeignKey(d => d.KplIdSkladnik)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_Komplet_tw__Towar");
        });

        modelBuilder.Entity<TwStan>(entity =>
        {
            entity.HasKey(e => new { e.StTowId, e.StMagId });

            entity.ToTable("tw_Stan", tb => tb.HasTrigger("Zm_stan_ins_upd_Data"));

            entity.HasIndex(e => new { e.StTowId, e.StMagId }, "IX_tw_Stany").IsUnique();

            entity.HasIndex(e => new { e.StTowId, e.StMagId, e.StStan, e.StStanRez }, "IX_tw_Stany_1");

            entity.Property(e => e.StTowId).HasColumnName("st_TowId");
            entity.Property(e => e.StMagId).HasColumnName("st_MagId");
            entity.Property(e => e.StStan)
                .HasColumnType("money")
                .HasColumnName("st_Stan");
            entity.Property(e => e.StStanMax)
                .HasColumnType("money")
                .HasColumnName("st_StanMax");
            entity.Property(e => e.StStanMin)
                .HasColumnType("money")
                .HasColumnName("st_StanMin");
            entity.Property(e => e.StStanRez)
                .HasColumnType("money")
                .HasColumnName("st_StanRez");

            entity.HasOne(d => d.StMag).WithMany(p => p.TwStans)
                .HasForeignKey(d => d.StMagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_Stan_sl_Magazyn");

            entity.HasOne(d => d.StTow).WithMany(p => p.TwStans)
                .HasForeignKey(d => d.StTowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_Stan_tw__Towar");
        });

        modelBuilder.Entity<TwTowar>(entity =>
        {
            entity.HasKey(e => e.TwId);

            entity.ToTable("tw__Towar", tb =>
                {
                    tb.HasTrigger("IF_Upd_Ins_Towar");
                    tb.HasTrigger("TRI_InsSearch_tw__Towar");
                    tb.HasTrigger("TRU_InsSearch_tw__Towar");
                    tb.HasTrigger("tr_TwTowar_Deleting");
                    tb.HasTrigger("tr_TwTowar_Inserting");
                    tb.HasTrigger("tr_TwTowar_Updating");
                    tb.HasTrigger("tr_tw__Towar_Synch_delete");
                    tb.HasTrigger("tr_tw__Towar_Synch_insert");
                    tb.HasTrigger("tr_tw__Towar_Synch_update");
                });

            entity.Property(e => e.TwId)
                .ValueGeneratedNever()
                .HasColumnName("tw_Id");
            entity.Property(e => e.TwAkcyza).HasColumnName("tw_Akcyza");
            entity.Property(e => e.TwAkcyzaKwota)
                .HasColumnType("money")
                .HasColumnName("tw_AkcyzaKwota");
            entity.Property(e => e.TwAkcyzaMarkaWyrobow)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_AkcyzaMarkaWyrobow");
            entity.Property(e => e.TwAkcyzaWielkoscProducenta)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_AkcyzaWielkoscProducenta");
            entity.Property(e => e.TwAkcyzaZaznacz).HasColumnName("tw_AkcyzaZaznacz");
            entity.Property(e => e.TwBloz12)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tw_bloz_12");
            entity.Property(e => e.TwBloz7)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tw_bloz_7");
            entity.Property(e => e.TwCenaOtwarta).HasColumnName("tw_CenaOtwarta");
            entity.Property(e => e.TwCharakter)
                .HasColumnType("text")
                .HasColumnName("tw_Charakter");
            entity.Property(e => e.TwCzasDostawy).HasColumnName("tw_CzasDostawy");
            entity.Property(e => e.TwDataZmianyVatSprzedazy)
                .HasColumnType("datetime")
                .HasColumnName("tw_DataZmianyVatSprzedazy");
            entity.Property(e => e.TwDniWaznosc).HasColumnName("tw_DniWaznosc");
            entity.Property(e => e.TwDodawalnyDoZw).HasColumnName("tw_DodawalnyDoZW");
            entity.Property(e => e.TwDomyslnaKategoria).HasColumnName("tw_DomyslnaKategoria");
            entity.Property(e => e.TwDostSymbol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_DostSymbol");
            entity.Property(e => e.TwGlebokosc)
                .HasColumnType("money")
                .HasColumnName("tw_Glebokosc");
            entity.Property(e => e.TwGrupaJpkVat).HasColumnName("tw_GrupaJpkVat");
            entity.Property(e => e.TwIdFundPromocji).HasColumnName("tw_IdFundPromocji");
            entity.Property(e => e.TwIdGrupa).HasColumnName("tw_IdGrupa");
            entity.Property(e => e.TwIdKoduWyrobuAkcyzowego).HasColumnName("tw_IdKoduWyrobuAkcyzowego");
            entity.Property(e => e.TwIdKrajuPochodzenia).HasColumnName("tw_IdKrajuPochodzenia");
            entity.Property(e => e.TwIdOpakowanie).HasColumnName("tw_IdOpakowanie");
            entity.Property(e => e.TwIdPodstDostawca).HasColumnName("tw_IdPodstDostawca");
            entity.Property(e => e.TwIdProducenta).HasColumnName("tw_IdProducenta");
            entity.Property(e => e.TwIdRabat).HasColumnName("tw_IdRabat");
            entity.Property(e => e.TwIdTypKodu).HasColumnName("tw_IdTypKodu");
            entity.Property(e => e.TwIdUjm).HasColumnName("tw_IdUJM");
            entity.Property(e => e.TwIdVatSp).HasColumnName("tw_IdVatSp");
            entity.Property(e => e.TwIdVatZak).HasColumnName("tw_IdVatZak");
            entity.Property(e => e.TwIsFundPromocji).HasColumnName("tw_IsFundPromocji");
            entity.Property(e => e.TwIsbn)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tw_isbn");
            entity.Property(e => e.TwJakPrzySp)
                .HasDefaultValue(true)
                .HasColumnName("tw_JakPrzySp");
            entity.Property(e => e.TwJednMiary)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_JednMiary");
            entity.Property(e => e.TwJednMiarySprz)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_JednMiarySprz");
            entity.Property(e => e.TwJednMiaryZak)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_JednMiaryZak");
            entity.Property(e => e.TwJednStanMin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_JednStanMin");
            entity.Property(e => e.TwJmsprzInna).HasColumnName("tw_JMSprzInna");
            entity.Property(e => e.TwJmzakInna).HasColumnName("tw_JMZakInna");
            entity.Property(e => e.TwKodTowaru)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tw_KodTowaru");
            entity.Property(e => e.TwKodUproducenta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tw_KodUProducenta");
            entity.Property(e => e.TwKomunikat)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tw_Komunikat");
            entity.Property(e => e.TwKomunikatDokumenty)
                .HasDefaultValue(3)
                .HasColumnName("tw_KomunikatDokumenty");
            entity.Property(e => e.TwKomunikatOd)
                .HasColumnType("datetime")
                .HasColumnName("tw_KomunikatOd");
            entity.Property(e => e.TwKontrolaTw).HasColumnName("tw_KontrolaTW");
            entity.Property(e => e.TwLogo)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("tw_Logo");
            entity.Property(e => e.TwMasa)
                .HasColumnType("money")
                .HasColumnName("tw_Masa");
            entity.Property(e => e.TwMasaNetto)
                .HasColumnType("money")
                .HasColumnName("tw_MasaNetto");
            entity.Property(e => e.TwMechanizmPodzielonejPlatnosci).HasColumnName("tw_MechanizmPodzielonejPlatnosci");
            entity.Property(e => e.TwNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Nazwa");
            entity.Property(e => e.TwObjetosc)
                .HasColumnType("money")
                .HasColumnName("tw_Objetosc");
            entity.Property(e => e.TwObrotMarza).HasColumnName("tw_ObrotMarza");
            entity.Property(e => e.TwOdwrotneObciazenie).HasColumnName("tw_OdwrotneObciazenie");
            entity.Property(e => e.TwOpis)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Opis");
            entity.Property(e => e.TwOplCukrowaInneSlodzace).HasColumnName("tw_OplCukrowaInneSlodzace");
            entity.Property(e => e.TwOplCukrowaKofeinaKwota)
                .HasColumnType("money")
                .HasColumnName("tw_OplCukrowaKofeinaKwota");
            entity.Property(e => e.TwOplCukrowaKofeinaPodlega).HasColumnName("tw_OplCukrowaKofeinaPodlega");
            entity.Property(e => e.TwOplCukrowaKwota)
                .HasColumnType("money")
                .HasColumnName("tw_OplCukrowaKwota");
            entity.Property(e => e.TwOplCukrowaKwotaPowyzej)
                .HasColumnType("money")
                .HasColumnName("tw_OplCukrowaKwotaPowyzej");
            entity.Property(e => e.TwOplCukrowaNapojWeglElektr).HasColumnName("tw_OplCukrowaNapojWeglElektr");
            entity.Property(e => e.TwOplCukrowaObj)
                .HasColumnType("money")
                .HasColumnName("tw_OplCukrowaObj");
            entity.Property(e => e.TwOplCukrowaPodlega).HasColumnName("tw_OplCukrowaPodlega");
            entity.Property(e => e.TwOplCukrowaSok).HasColumnName("tw_OplCukrowaSok");
            entity.Property(e => e.TwOplCukrowaZawartoscCukru)
                .HasColumnType("money")
                .HasColumnName("tw_OplCukrowaZawartoscCukru");
            entity.Property(e => e.TwPkwiU)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_PKWiU");
            entity.Property(e => e.TwPlu).HasColumnName("tw_PLU");
            entity.Property(e => e.TwPodlegaOplacieNaFunduszOchronyRolnictwa).HasColumnName("tw_PodlegaOplacieNaFunduszOchronyRolnictwa");
            entity.Property(e => e.TwPodstKodKresk)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_PodstKodKresk");
            entity.Property(e => e.TwPole1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole1");
            entity.Property(e => e.TwPole2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole2");
            entity.Property(e => e.TwPole3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole3");
            entity.Property(e => e.TwPole4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole4");
            entity.Property(e => e.TwPole5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole5");
            entity.Property(e => e.TwPole6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole6");
            entity.Property(e => e.TwPole7)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole7");
            entity.Property(e => e.TwPole8)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Pole8");
            entity.Property(e => e.TwProgKwotowyOo).HasColumnName("tw_ProgKwotowyOO");
            entity.Property(e => e.TwPrzezWartosc)
                .HasDefaultValue(true)
                .HasColumnName("tw_PrzezWartosc");
            entity.Property(e => e.TwRodzaj)
                .HasDefaultValue(1)
                .HasColumnName("tw_Rodzaj");
            entity.Property(e => e.TwSerwisAukcyjny).HasColumnName("tw_SerwisAukcyjny");
            entity.Property(e => e.TwSklepInternet).HasColumnName("tw_SklepInternet");
            entity.Property(e => e.TwSprzedazMobilna).HasColumnName("tw_SprzedazMobilna");
            entity.Property(e => e.TwStanMaks)
                .HasColumnType("money")
                .HasColumnName("tw_StanMaks");
            entity.Property(e => e.TwStanMin)
                .HasColumnType("money")
                .HasColumnName("tw_StanMin");
            entity.Property(e => e.TwSww)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_SWW");
            entity.Property(e => e.TwSymbol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Symbol");
            entity.Property(e => e.TwSzerokosc)
                .HasColumnType("money")
                .HasColumnName("tw_Szerokosc");
            entity.Property(e => e.TwUrzNazwa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_UrzNazwa");
            entity.Property(e => e.TwUsuniety).HasColumnName("tw_Usuniety");
            entity.Property(e => e.TwUwagi)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_Uwagi");
            entity.Property(e => e.TwWagaEtykiet)
                .HasDefaultValue(false)
                .HasColumnName("tw_WagaEtykiet");
            entity.Property(e => e.TwWegielOpisPochodzenia)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_WegielOpisPochodzenia");
            entity.Property(e => e.TwWegielPodlegaOswiadczeniu).HasColumnName("tw_WegielPodlegaOswiadczeniu");
            entity.Property(e => e.TwWww)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("tw_WWW");
            entity.Property(e => e.TwWysokosc)
                .HasColumnType("money")
                .HasColumnName("tw_Wysokosc");
            entity.Property(e => e.TwZablokowany).HasColumnName("tw_Zablokowany");
            entity.Property(e => e.TwZnakiAkcyzy).HasColumnName("tw_ZnakiAkcyzy");

            entity.HasOne(d => d.TwIdGrupaNavigation).WithMany(p => p.TwTowars)
                .HasForeignKey(d => d.TwIdGrupa)
                .HasConstraintName("FK_tw__Towar_sl_GrupaTw");

            entity.HasOne(d => d.TwIdOpakowanieNavigation).WithMany(p => p.InverseTwIdOpakowanieNavigation)
                .HasForeignKey(d => d.TwIdOpakowanie)
                .HasConstraintName("FK_tw__Towar_tw__Towar");

            entity.HasOne(d => d.TwIdPodstDostawcaNavigation).WithMany(p => p.TwTowarTwIdPodstDostawcaNavigations)
                .HasForeignKey(d => d.TwIdPodstDostawca)
                .HasConstraintName("FK_tw__Towar_kh__Kontrahent_PodstawowyDostawca");

            entity.HasOne(d => d.TwIdProducentaNavigation).WithMany(p => p.TwTowarTwIdProducentaNavigations)
                .HasForeignKey(d => d.TwIdProducenta)
                .HasConstraintName("FK_tw__Towar_kh__Kontrahent_Producent");

            entity.HasOne(d => d.TwIdVatSpNavigation).WithMany(p => p.TwTowarTwIdVatSpNavigations)
                .HasForeignKey(d => d.TwIdVatSp)
                .HasConstraintName("FK_tw__Towar_sl_StawkaVAT");

            entity.HasOne(d => d.TwIdVatZakNavigation).WithMany(p => p.TwTowarTwIdVatZakNavigations)
                .HasForeignKey(d => d.TwIdVatZak)
                .HasConstraintName("FK_tw__Towar_sl_StawkaVAT1");
        });

        modelBuilder.Entity<TwZdjecieTw>(entity =>
        {
            entity.HasKey(e => e.ZdId);

            entity.ToTable("tw_ZdjecieTw");

            entity.Property(e => e.ZdId)
                .ValueGeneratedNever()
                .HasColumnName("zd_Id");
            entity.Property(e => e.ZdCrc)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("zd_CRC");
            entity.Property(e => e.ZdGlowne).HasColumnName("zd_Glowne");
            entity.Property(e => e.ZdIdTowar).HasColumnName("zd_IdTowar");
            entity.Property(e => e.ZdZdjecie)
                .HasColumnType("image")
                .HasColumnName("zd_Zdjecie");

            entity.HasOne(d => d.ZdIdTowarNavigation).WithMany(p => p.TwZdjecieTws)
                .HasForeignKey(d => d.ZdIdTowar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tw_ZdjecieTw_tw__Towar");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
