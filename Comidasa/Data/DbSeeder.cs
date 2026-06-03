using Comidasa.Models;
using Microsoft.EntityFrameworkCore;

namespace Comidasa.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Esperar a que la DB se haya creado/migrado
            await context.Database.MigrateAsync();

            if (await context.Products.AnyAsync())
            {
                return; // Ya hay datos
            }

            var products = new List<Product>
            {
                new Product
                {
                    NameProduct = "Pan a la Brasa",
                    Price = 8.00m,
                    Descrip = "Masa madre artesanal, mantequilla de ceniza y sal de mar recolectada a mano.",
                    ImagenProduct = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=600&h=400&fit=crop",
                    Category = "Entradas"
                },
                new Product
                {
                    NameProduct = "Zanahorias Baby",
                    Price = 11.50m,
                    Descrip = "Glaseadas en miel de agave, hummus de remolacha y pistachos tostados.",
                    ImagenProduct = "https://images.unsplash.com/photo-1582515073490-39981397c445?w=600&h=400&fit=crop",
                    Category = "Entradas"
                },
                new Product
                {
                    NameProduct = "Tartar de Atún",
                    Price = 16.00m,
                    Descrip = "Aleta amarilla, emulsión de wasabi, aguacate y aire de cilantro cítrico.",
                    ImagenProduct = "https://images.unsplash.com/photo-1541592106381-b31e9677c0e5?w=600&h=400&fit=crop",
                    Category = "Entradas"
                },
                new Product
                {
                    NameProduct = "Entrecot de Ternera",
                    Price = 34.00m,
                    Descrip = "350g de corte premium, madurado 45 días, acompañado de milhojas de papa y chimichurri de la casa.",
                    ImagenProduct = "https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=600&h=400&fit=crop",
                    Category = "Platos Fuertes"
                },
                new Product
                {
                    NameProduct = "Salmón del Pacífico",
                    Price = 29.50m,
                    Descrip = "A la brasa con costra de hierbas, puré de guisantes a la menta y espárragos trigueros.",
                    ImagenProduct = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600&h=400&fit=crop",
                    Category = "Platos Fuertes"
                },
                new Product
                {
                    NameProduct = "Volcán de Cacao",
                    Price = 9.00m,
                    Descrip = "70% cacao orgánico y helado de vainilla.",
                    ImagenProduct = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=600&h=400&fit=crop",
                    Category = "Postres"
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
