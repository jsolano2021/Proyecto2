using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Proyecto2.Models
{
    public partial class Menu
    {
        public Menu()
        {
            Platillo = new HashSet<Platillo>();
        }

        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DisplayName("Restaurante")]
        public int IdRestaurante { get; set; }
        public string Nombre { get; set; }
        public bool? Estado { get; set; }

        [System.ComponentModel.DisplayName("Restaurante")]
        public virtual Restaurante IdRestauranteNavigation { get; set; }
        public virtual ICollection<Platillo> Platillo { get; set; }

        [NotMapped] 
        [DisplayName("Cantidad")]
        public int cantidad { get; set; }
    }
}
