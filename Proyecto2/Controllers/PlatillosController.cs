using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class PlatillosController : Controller
    {
        private readonly Proyecto2Context _context;

        public PlatillosController(Proyecto2Context context)
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

        // GET: Platillos
        public async Task<IActionResult> Index()
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            var proyecto2Context = _context.Platillo.Include(p => p.IdMenuNavigation).Include(p => p.IdMenuNavigation.IdRestauranteNavigation).OrderBy(x => x.IdMenu);



            return View(await proyecto2Context.ToListAsync());
        }

        // GET: Platillos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var platillo = await _context.Platillo
                .Include(p => p.IdMenuNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platillo == null)
            {
                return NotFound();
            }

            return View(platillo);
        }

        // GET: Platillos/Create
        public IActionResult Create()
        {

            try
            {
                if (!ValidarUsuario())
                    return RedirectToAction("Login", "Seguridad");


                var lista = _context.Menu.Include(x => x.IdRestauranteNavigation);

                lista.ForEachAsync(x => { x.Nombre = x.IdRestauranteNavigation.Nombre + " - " + x.Nombre; });

                ViewData["IdMenu"] = new SelectList(lista, "Id", "Nombre");


                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // POST: Platillos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdMenu,Nombre,Descripcion,Precio,Stock,Estado, ImageFile")] Platillo platillo)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (ModelState.IsValid)
            {
                if (platillo.ImageFile != null)
                {
                    if (platillo.ImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            platillo.ImageFile.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            platillo.Imagen = fileBytes;
                        }
                    }
                }


                _context.Add(platillo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
          

            return View(platillo);
        }

        // GET: Platillos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var platillo = await _context.Platillo.FindAsync(id);

            if (platillo == null)
            {
                return NotFound();
            }  

            var lista = _context.Menu.Include(x => x.IdRestauranteNavigation);

            await lista.ForEachAsync(x => { x.Nombre = x.IdRestauranteNavigation.Nombre + " - " + x.Nombre; });

            ViewData["IdMenu"] = new SelectList(lista, "Id", "Nombre");

            return View(platillo);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdMenu,Nombre,Descripcion,Precio,Stock,Estado, ImageFile")] Platillo platillo)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id != platillo.Id)
            {
                return NotFound();
            } 


            if (!string.IsNullOrEmpty(platillo.Nombre) && platillo.Stock >= 0 && platillo.Precio > 0 && platillo.Id > 0)
            {
                try
                {

                    if (platillo.ImageFile != null)
                    {
                        if (platillo.ImageFile.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                platillo.ImageFile.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                platillo.Imagen = fileBytes;
                            } 

                            _context.Update<Platillo>(platillo);
                            await _context.SaveChangesAsync();

                        }
                    }
                    else
                    {
                        _context.Entry(platillo).State = EntityState.Modified;
                        _context.Entry(platillo).Property(x => x.Imagen).IsModified = false; 
                        await _context.SaveChangesAsync();
                    } 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatilloExists(platillo.Id))
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

            var lista = _context.Menu.Include(x => x.IdRestauranteNavigation);

            await lista.ForEachAsync(x => { x.Nombre = x.IdRestauranteNavigation.Nombre + " - " + x.Nombre; });

            ViewData["IdMenu"] = new SelectList(lista, "Id", "Nombre");

            return View(platillo);
        }

        // GET: Platillos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            if (id == null)
            {
                return NotFound();
            }

            var platillo = await _context.Platillo
                .Include(p => p.IdMenuNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platillo == null)
            {
                return NotFound();
            }

            return View(platillo);
        }

        // POST: Platillos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!ValidarUsuario())
                return RedirectToAction("Login", "Seguridad");

            var platillo = await _context.Platillo.FindAsync(id);
            _context.Platillo.Remove(platillo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatilloExists(int id)
        {
            return _context.Platillo.Any(e => e.Id == id);
        }
    }
}
