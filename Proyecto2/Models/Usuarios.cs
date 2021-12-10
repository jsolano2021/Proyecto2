using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Proyecto2.Models
{
    public partial class Usuarios
    {
        public Usuarios()
        {
            Factura = new HashSet<Factura>();
        }

        public int Id { get; set; }
        public int IdPerfil { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? Estado { get; set; }

        public virtual Perfil IdPerfilNavigation { get; set; }
        public virtual ICollection<Factura> Factura { get; set; }
    }
}
