using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Proyecto2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2.Controllers
{
    public class RestauranteFinal : Controller
    {
        private readonly Proyecto2Context _context;

        public RestauranteFinal(Proyecto2Context context)
        {
            _context = context;
        }


        bool ValidarUsuario()
        {
            string json = HttpContext.Session.GetString("Usuario");

            if (!String.IsNullOrEmpty(json))
            {
                Usuarios miUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuarios>(json);
                if (miUsuario == null || miUsuario.IdPerfil != 3)
                    return false;
            }
            else
                return false;

            return true;
        }

        public async Task<IActionResult> Index()
        {

            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");


            var proyecto2Context = _context.Restaurante;
            return View(await proyecto2Context.ToListAsync());
        }


        public IActionResult Menu(int? id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var menu = _context.Menu.Where(x => x.IdRestaurante == id).FirstOrDefault();

            if (menu == null)
            {
                return NotFound();
            }

            var lista = _context.Menu.Include(p => p.IdRestauranteNavigation).Where(x => x.IdRestaurante == id);

            ViewData["idMenu"] = new SelectList(lista, "Id", "Nombre", menu.IdRestaurante);

            return View(menu);
        }



        [HttpPost]
        public IActionResult getMenu([Bind("Id, Nombre, IdRestaurante")] Menu Menu)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (Menu != null && Menu.Id > 0)
            {

                var Lista = _context.Platillo.Where(x => x.IdMenu == Menu.Id).Include(p => p.IdMenuNavigation).Include(p => p.IdMenuNavigation.IdRestauranteNavigation);
                Restaurante restaurante = null;

                if (Lista != null && Lista.Count() > 0)
                    restaurante = Lista.FirstOrDefault().IdMenuNavigation.IdRestauranteNavigation;

                ViewData["idMenu"] = new SelectList(_context.Menu.Where(x => x.IdRestaurante == restaurante.Id), "Id", "Nombre", Menu.IdRestaurante);
                ViewData["ListaDatos"] = Lista;
                return View("Menu");
            }


            ViewData["idMenu"] = new SelectList(_context.Menu.Where(x => x.IdRestaurante == Menu.IdRestaurante), "Id", "Nombre", Menu.IdRestaurante);
            return View("Index");
        }

        List<Detalle> getLista()
        {

            string json = HttpContext.Session.GetString("Carrito");

            if (!String.IsNullOrEmpty(json))
            {
                List<Detalle> detalles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Detalle>>(json);
                return detalles;
            }
            return null;
        }

        [HttpPost, ActionName("Comprar")]
        [ValidateAntiForgeryToken]
        public IActionResult Comprar(int? id, int? cantidad)
        {

            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id != null)
            {
                List<Detalle> detalles = getLista();

                Platillo producto = _context.Platillo.Where(x => x.Id == id).Include(x => x.IdMenuNavigation).Include(x => x.IdMenuNavigation.IdRestauranteNavigation).FirstOrDefault();
                
                double? cantidadN = cantidad.Value;

                if(producto.Stock < cantidad)
                {
                    cantidadN = producto.Stock;
                }


                if(producto.Stock > 0)
                {
                    if (detalles == null)
                        detalles = new List<Detalle>();


                    detalles.Add(new Detalle
                    {
                        IdPlatilloNavigation = producto,
                        Cantidad = (cantidadN != null) ? cantidadN.Value : 1,
                        Precio = producto.Precio,
                        Total = producto.Precio * ((cantidad != null) ? cantidad.Value : 1),
                        IdRestaurante = producto.IdMenuNavigation.IdRestaurante,
                        IdPlatillo = producto.Id,
                    }
                        );
                }
                else
                {
                    ViewData["Mensaje"] = "Sin Stock para facturar";
                }


                _context.Menu.ForEachAsync(x => x.cantidad = 0);

                cantidad = 0;

                HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(detalles));

                var Lista = _context.Platillo.Where(x => x.IdMenu == producto.IdMenu).Include(p => p.IdMenuNavigation);


                ViewData["idMenu"] = new SelectList(_context.Menu.Where(x => x.IdRestaurante == producto.IdMenuNavigation.IdRestaurante), "Id", "Nombre", producto.IdMenuNavigation.IdRestaurante);
                ViewData["ListaDatos"] = Lista;

                return View("Menu");
            }
            else
            {
                return View("Menu");
            } 
        }
    }
}
