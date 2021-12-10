using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2.Controllers
{
   public class SeguridadController : Controller
    {
        private readonly Proyecto2Context _context;

        public SeguridadController(Proyecto2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuarios user)
        {

            if (user != null)
            {
                Usuarios miUsuario = _context.Usuarios.Where(x => x.UserName.ToUpper().Equals(user.UserName.ToUpper())
                             && x.Password.Equals(user.Password)).FirstOrDefault();

                if (miUsuario != null)
                {
                    HttpContext.Session.SetString("Usuario", JsonConvert.SerializeObject(miUsuario));

                    if (miUsuario.IdPerfil == 1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "RestauranteFinal");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Seguridad");
                }
            }

            return View();
        }
         
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Usuario");
            return RedirectToAction("Login", "Seguridad");
        }


    }
}
