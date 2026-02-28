using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

[Table("BAGIS_GRUP")]
public partial class BagisGrup
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("BAGISLAR_ID")]
    public int? BagislarId { get; set; }

    [Column("GRUP_NO")]
    public int? GrupNo { get; set; }

    [ForeignKey("BagislarId")]
    [InverseProperty("BagisGrups")]
    public virtual Bagislar? Bagislar { get; set; }
}
