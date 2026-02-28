using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

[Table("REFERANSLAR")]
public partial class Referanslar
{
    [Key]
    public int Id { get; set; }

    [Column("Ad_Soyad")]
    [StringLength(300)]
    public string AdSoyad { get; set; } = null!;

    [InverseProperty("Referans")]
    public virtual ICollection<Bagislar> Bagislars { get; set; } = new List<Bagislar>();
}
