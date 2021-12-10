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
    public class MenuController : Controller
    {
        private readonly Proyecto2Context _context;


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

        public MenuController(Proyecto2Context context)
        {
            _context = context;
        }

        // GET: Menu
        public async Task<IActionResult> Index()
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");


            var proyecto2Context = _context.Menu.Include(m => m.IdRestauranteNavigation);
            return View(await proyecto2Context.ToListAsync());
        }

        // GET: Menu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu
                .Include(m => m.IdRestauranteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menu/Create
        public IActionResult Create()
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            ViewData["IdRestaurante"] = new SelectList(_context.Restaurante, "Id", "Nombre");
            return View();
        }

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdRestaurante,Nombre,Estado")] Menu menu)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurante, "Id", "Direccion", menu.IdRestaurante);
            return View(menu);
        }

        // GET: Menu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");


            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurante, "Id", "Direccion", menu.IdRestaurante);
            return View(menu);
        }

        // POST: Menu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdRestaurante,Nombre,Estado")] Menu menu)
        {

            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");



            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
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
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurante, "Id", "Direccion", menu.IdRestaurante);
            return View(menu);
        }

        // GET: Menu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");


            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu
                .Include(m => m.IdRestauranteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            var menu = await _context.Menu.FindAsync(id);
            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menu.Any(e => e.Id == id);
        }
    }
}
