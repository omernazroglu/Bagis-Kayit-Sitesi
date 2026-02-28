using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

[Table("BAGISLAR")]
public partial class Bagislar
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("OLUSTURMA_TARIHI", TypeName = "datetime")]
    public DateTime? OlusturmaTarihi { get; set; }

    [Column("DEGISTIRME_TARIHI", TypeName = "datetime")]
    public DateTime? DegistirmeTarihi { get; set; }

    [Column("AKTIF")]
    public int? Aktif { get; set; }

    [Column("KULLANICI_ID")]
    public int KullaniciId { get; set; }

    public int? ReferansId { get; set; }

    [InverseProperty("Bagislar")]
    public virtual ICollection<BagisBilgileri> BagisBilgileris { get; set; } = new List<BagisBilgileri>();

    [InverseProperty("Bagislar")]
    public virtual ICollection<BagisGrup> BagisGrups { get; set; } = new List<BagisGrup>();

    [ForeignKey("KullaniciId")]
    [InverseProperty("Bagislars")]
    public virtual Kullanici Kullanici { get; set; } = null!;

    [ForeignKey("ReferansId")]
    [InverseProperty("Bagislars")]
    public virtual Referanslar? Referans { get; set; }
}
