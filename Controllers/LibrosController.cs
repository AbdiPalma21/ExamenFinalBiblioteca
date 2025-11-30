using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContexto _contexto;
        private readonly IWebHostEnvironment _entornoWeb;

        public LibrosController(BibliotecaContexto contexto, IWebHostEnvironment entornoWeb)
        {
            _contexto = contexto;
            _entornoWeb = entornoWeb;
        }

        // GET: Libros/Inicio
        public async Task<IActionResult> Inicio(string criterioBusqueda)
        {
            ViewData["CriterioActual"] = criterioBusqueda;

            var consultaLibros = from l in _contexto.Libros
                                 select l;

            if (!string.IsNullOrEmpty(criterioBusqueda))
            {
                consultaLibros = consultaLibros
                    .Where(l => l.TituloLibro.Contains(criterioBusqueda));
            }

            var listaLibros = await consultaLibros.ToListAsync();
            return View(listaLibros);
        }

        // GET: Libros/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _contexto.Libros
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libros/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Libros/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Libro libro)
        {
            if (ModelState.IsValid)
            {
                // Guardar archivo de portada si viene uno
                if (libro.ArchivoPortada != null && libro.ArchivoPortada.Length > 0)
                {
                    var nombreArchivo = Guid.NewGuid().ToString() +
                                        Path.GetExtension(libro.ArchivoPortada.FileName);

                    var rutaCarpeta = Path.Combine(_entornoWeb.WebRootPath, "portadas");
                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);
                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await libro.ArchivoPortada.CopyToAsync(stream);
                    }

                    // Guardamos la ruta relativa en la propiedad Portada
                    libro.Portada = "/portadas/" + nombreArchivo;
                }

                _contexto.Add(libro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction(nameof(Inicio));
            }

            return View(libro);
        }


        // GET: Libros/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _contexto.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var libroExistente = await _contexto.Libros.FindAsync(id);
                    if (libroExistente == null)
                    {
                        return NotFound();
                    }

                    // Actualizar campos básicos
                    libroExistente.TituloLibro = libro.TituloLibro;
                    libroExistente.Autor = libro.Autor;
                    libroExistente.Disponible = libro.Disponible;
                    libroExistente.FechaDePublicacion = libro.FechaDePublicacion;
                    libroExistente.DescripcionLibro = libro.DescripcionLibro;

                    // Si subieron nueva portada, la guardamos y reemplazamos la ruta
                    if (libro.ArchivoPortada != null && libro.ArchivoPortada.Length > 0)
                    {
                        var nombreArchivo = Guid.NewGuid().ToString() +
                                            Path.GetExtension(libro.ArchivoPortada.FileName);

                        var rutaCarpeta = Path.Combine(_entornoWeb.WebRootPath, "portadas");
                        if (!Directory.Exists(rutaCarpeta))
                        {
                            Directory.CreateDirectory(rutaCarpeta);
                        }

                        var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);
                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await libro.ArchivoPortada.CopyToAsync(stream);
                        }

                        libroExistente.Portada = "/portadas/" + nombreArchivo;
                    }

                    await _contexto.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExiste(libro.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Inicio));
            }

            return View(libro);
        }


        // GET: Libros/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _contexto.Libros
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var libro = await _contexto.Libros.FindAsync(id);
            if (libro != null)
            {
                _contexto.Libros.Remove(libro);
                await _contexto.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Inicio));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarDisponibilidad(int id)
        {
            var libro = await _contexto.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            // Si está "Sí", lo marcamos como "No"; si no, lo marcamos como "Sí"
            libro.Disponible = (libro.Disponible == "Sí") ? "No" : "Sí";

            await _contexto.SaveChangesAsync();

            return RedirectToAction(nameof(Inicio));
        }


        private bool LibroExiste(int id)
        {
            return _contexto.Libros.Any(e => e.Id == id);
        }
    }
}
