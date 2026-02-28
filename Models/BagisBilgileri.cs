using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

[Table("BAGIS_BILGILERI")]
public partial class BagisBilgileri
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("BAGISLAR_ID")]
    public int? BagislarId { get; set; }

    [Column("SUTUNLAR_ID")]
    public int? SutunlarId { get; set; }

    [Column("ACIKLAMA")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Aciklama { get; set; }

    [ForeignKey("BagislarId")]
    [InverseProperty("BagisBilgileris")]
    public virtual Bagislar? Bagislar { get; set; }

    [ForeignKey("SutunlarId")]
    [InverseProperty("BagisBilgileris")]
    public virtual RefSutunlar? Sutunlar { get; set; }
}
