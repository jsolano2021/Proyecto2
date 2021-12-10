using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Proyecto2.Models
{
    public partial class Restaurante
    {
        public Restaurante()
        {
            Detalle = new HashSet<Detalle>();
            Menu = new HashSet<Menu>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public bool? Estado { get; set; }

        public virtual ICollection<Detalle> Detalle { get; set; }
        public virtual ICollection<Menu> Menu { get; set; }
    }
}
