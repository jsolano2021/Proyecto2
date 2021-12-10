using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly Proyecto2Context _context;

        public RestauranteController(Proyecto2Context context)
        {
            _context = context;
        }

        bool ValidarUsuario()
        {
            string json = HttpContext.Session.GetString("Usuario");

            if (!String.IsNullOrEmpty(json))
            {
                Usuarios miUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuarios>(json);
                if (miUsuario == null || miUsuario.IdPerfil != 1)
                    return false;
            }
            else
                return false;

            return true;
        }

        // GET: Restaurante
        public async Task<IActionResult> Index()
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            return View(await _context.Restaurante.ToListAsync());
        }

        // GET: Restaurante/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var restaurante = await _context.Restaurante
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurante == null)
            {
                return NotFound();
            }

            return View(restaurante);
        }

        // GET: Restaurante/Create
        public IActionResult Create()
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            return View();
        }

        // POST: Restaurante/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,Telefono,Estado")] Restaurante restaurante)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (ModelState.IsValid)
            {
                _context.Add(restaurante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurante);
        }

        // GET: Restaurante/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var restaurante = await _context.Restaurante.FindAsync(id);
            if (restaurante == null)
            {
                return NotFound();
            }
            return View(restaurante);
        }

        // POST: Restaurante/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Direccion,Telefono,Estado")] Restaurante restaurante)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id != restaurante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestauranteExists(restaurante.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(restaurante);
        }

        // GET: Restaurante/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!ValidarUsuario())

                return RedirectToAction("Login", "Seguridad");
            if (id == null)
            {
                return NotFound();
            }

            var restaurante = await _context.Restaurante
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurante == null)
            {
                return NotFound();
            }

            return View(restaurante);
        }

        // POST: Restaurante/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            var restaurante = await _context.Restaurante.FindAsync(id);
            _context.Restaurante.Remove(restaurante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestauranteExists(int id)
        {
            return _context.Restaurante.Any(e => e.Id == id);
        }
    }
}
