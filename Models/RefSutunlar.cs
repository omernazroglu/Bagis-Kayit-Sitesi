using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UFUKDER_BAGIS.Models;

[Table("REF_SUTUNLAR")]
public partial class RefSutunlar
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ACIKLAMA")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Aciklama { get; set; }

    [InverseProperty("Sutunlar")]
    public virtual ICollection<BagisBilgileri> BagisBilgileris { get; set; } = new List<BagisBilgileri>();
}
