using System.ComponentModel.DataAnnotations.Schema;

namespace Consolidado.Models
{
    [Table("plantillauo")]
    public class UnidadOrganizacional
    {
        [Column("idpuo")]
        public int Id { get; set; }

        [Column("nombreuo")]
        public string? NombreUO { get; set; }

        [Column("clasificacionuo")]
        public string? ClasificacionUO { get; set; }

        [Column("dependenciauo")]
        public string? DependenciaUO { get; set; }
    }
}
