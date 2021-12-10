using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore;

namespace Proyecto2.Controllers
{
    public class FacturarController : Controller
    {
        private readonly Proyecto2Context _context;
        private readonly IEmailSender _emailSender;
        public FacturarController(Proyecto2Context context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

        Usuarios GetUsuario()
        {
            string json = HttpContext.Session.GetString("Usuario");

            if (!String.IsNullOrEmpty(json))
            {
                Usuarios miUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuarios>(json);

                return miUsuario;
            }
            else
                return null; 
        }



        public IActionResult Index()
        {
            if(!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");


            List<Detalle> lista = getLista();

            return View(lista);
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

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int? id)
        {
            List<Detalle> lista = getLista();


            if (id != null)
            { 
                var detalle = lista.Where(x => x.IdPlatillo == id).FirstOrDefault();

                if (detalle != null)
                    lista.Remove(detalle);

                HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(lista));
            }

            return View("Index", lista);
        }



        [HttpPost, ActionName("Facturar")]
        [ValidateAntiForgeryToken]
        public IActionResult Facturar()
        {
            List<Detalle> lista = getLista();


            if(lista != null)
            {
                Usuarios usuario = GetUsuario();


                Factura factura = new Factura()
                { 
                    Fecha = DateTime.Now,
                    IdUsuario = usuario.Id, 
                    TotalFactura = lista.Sum(x => x.Total)
                };
                 

                var Saved = _context.Add(factura);
                _context.SaveChanges();


                if(Saved.Entity.Id > 0)
                {
                    foreach (var item in lista)
                    {
                        item.IdFactura = Saved.Entity.Id;
                        _context.Entry(item.IdPlatilloNavigation).State = EntityState.Unchanged; 

                        _context.Add(item);
                        _context.SaveChanges();


                        item.IdPlatilloNavigation.Stock -= item.Cantidad;
                        _context.Update<Platillo>(item.IdPlatilloNavigation);
                        _context.SaveChanges();
                    }

                    EnviarCorreo(usuario, Saved.Entity);
                    return RedirectToAction("Index", "RestauranteFinal");
                }

               
                return RedirectToAction("Index", "RestauranteFinal");
            }
            else
            {
                return View("Index", lista);
            } 
             
        }


        void EnviarCorreo(Usuarios user, Factura fact)
        {
            string HTML = "";

            //https://code-maze.com/aspnetcore-send-email/
            //http://www.binaryintellect.net/articles/e30d07c6-6f57-43e7-a2ce-6d2d67ebf403.aspx

            fact.Detalle = _context.Detalle.Include(d => d.IdPlatilloNavigation)
                .Include(d => d.IdPlatilloNavigation.IdMenuNavigation)
                .Include(d => d.IdPlatilloNavigation.IdMenuNavigation.IdRestauranteNavigation)
                .Include(d => d.IdRestauranteNavigation)
                .Where(x => x.IdFactura == fact.Id).ToList();

            var lista = fact.Detalle.GroupBy(x => x.IdRestauranteNavigation.Nombre);

            HTML += $@"<table style=""border: 1px solid black;"">
                      <thead>
                        <tr> 
                          <th style=""border: 1px solid black;"">Restaurante</th> 
                        </tr>
                      </thead>
                      <tbody> 
                       ";

            foreach (var item in lista)
            {
                HTML += Environment.NewLine;
                HTML += Environment.NewLine;

                HTML += $@"<tr> 
                          <td style=""border: 1px solid black; font-size: 16px; font-weight: bold;"">{item.Key}</td > 
                        </tr >";

                foreach (Detalle detalle in item)
                {
                    HTML += Environment.NewLine;
                    HTML += Environment.NewLine;
                    HTML += $@"<table style=""border: 1px solid black;"">
                              <thead>
                                <tr>
                                  <th style=""border: 1px solid black;"">Producto</th>
                                  <th style=""border: 1px solid black;"">Cantidad</th>
                                  <th style=""border: 1px solid black;"">Total</th>
                                </tr>
                              </thead>
                              <tbody>
                                <tr> 
                                  <td style=""border: 1px solid black;"">{detalle.IdPlatilloNavigation.Nombre}</td>
                                  <td style=""border: 1px solid black;"">{detalle.Cantidad}</td>
                                  <td style=""border: 1px solid black;"">{detalle.Total.ToString("N2")}</td>
                                </tr> 
                              </tbody>
                            </table>";
                }

                HTML += Environment.NewLine;
                HTML += Environment.NewLine;
            }

            HTML += @"</tbody >
                    </table >";




            var message = new Message(new string[] { user.Correo }, "Factura Realizada", HTML);
            _emailSender.SendEmailHTML(message);

        }


    }
}
