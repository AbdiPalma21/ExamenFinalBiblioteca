using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data
{
    public class BibliotecaContexto : DbContext
    {
        public BibliotecaContexto(DbContextOptions<BibliotecaContexto> opciones)
            : base(opciones)
        {
        }

        public DbSet<Libro> Libros { get; set; } = default!;
    }
}
