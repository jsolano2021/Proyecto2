using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Proyecto2.Models
{
    public partial class Platillo
    {
        public Platillo()
        {
            Detalle = new HashSet<Detalle>();
        }

        public int Id { get; set; }

        [Required]
        [DisplayName("Menu")]
        public int IdMenu { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public double Precio { get; set; }
        public byte[] Imagen { get; set; }
        [Required]
        public double Stock { get; set; }
        public bool? Estado { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("Subir Imagen...")]
        public IFormFile ImageFile { get; set; }

        [DisplayName("Menu")]
        public virtual Menu IdMenuNavigation { get; set; }
        public virtual ICollection<Detalle> Detalle { get; set; }
    }
}
