using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

[Table("KULLANICI")]
public partial class Kullanici
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("AD")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Ad { get; set; }

    [Column("SOYAD")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Soyad { get; set; }

    [Column("EMAIL")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("SIFRE")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Sifre { get; set; }

    [Column("AKTIF")]
    public int? Aktif { get; set; }

    [InverseProperty("Kullanici")]
    public virtual ICollection<Bagislar> Bagislars { get; set; } = new List<Bagislar>();
}
