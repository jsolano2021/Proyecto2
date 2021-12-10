using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Proyecto2.Models
{
    public partial class Detalle
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public int IdPlatillo { get; set; }
        public int IdRestaurante { get; set; }
        public double Cantidad { get; set; }
        public double Precio { get; set; }
        public double Total { get; set; }
        public bool? Estado { get; set; }

        public virtual Factura IdFacturaNavigation { get; set; }
        public virtual Platillo IdPlatilloNavigation { get; set; }
        public virtual Restaurante IdRestauranteNavigation { get; set; }
    }
}
