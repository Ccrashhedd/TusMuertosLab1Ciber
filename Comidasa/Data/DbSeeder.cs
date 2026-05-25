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
                    ImagenProduct = "https://lh3.googleusercontent.com/aida-public/AB6AXuAJyAHzyTbrHOOB-XYyIRiXGSgcKViN5E-xpXuOACsboNA8Q09aE9QxRUESEOlBIajawDB7ps4ukGUScWMTQY-iR_GLrajnMNubMlLZ9wq8mNUqEeoXSRqycHCLVDCpYAHOgnaO4jfw5cGeyx9QZOsUMOb8IjLHlXw3o0FpRGOQFEw95MNcn1StQhUG5eohRzd9MMKn3KZu8B767qu1h6xn7uzFP8m3JXzy8bkGoOUEA2k2RilnUALquWC7X7yJLzcxdzwSpO65nRA",
                    Category = "Entradas"
                },
                new Product
                {
                    NameProduct = "Zanahorias Baby",
                    Price = 11.50m,
                    Descrip = "Glaseadas en miel de agave, hummus de remolacha y pistachos tostados.",
                    ImagenProduct = "https://lh3.googleusercontent.com/aida-public/AB6AXuALcmPRXVTGtrsiU-LRZDQoxBiZpj9OuIk9f_-WOM1rWpFKaWnokEz3eDhydJCdcJ2vubeYInfcG1fscOIPLCLrFXQDRrKjb5-fpnXEvTJr0-ZWHfj8FAlPF-18GwvArzK1iGAB9_DBNDvu3rmbF8tfFJYaolGCTjMFzyBVz0KNlL2a5a_SyB8gV1BcWFTIbE-p6q0YViFNXn16pK3ML_lbaHfT6YqV3Hot3tZTDwEZIJLvJ33AxftIpo2Bq3iMxzo0T_pLzNJtXQo",
                    Category = "Entradas"
                },
                new Product
                {
                    NameProduct = "Tartar de Atún",
                    Price = 16.00m,
                    Descrip = "Aleta amarilla, emulsión de wasabi, aguacate y aire de cilantro cítrico.",
                    ImagenProduct = "https://lh3.googleusercontent.com/aida-public/AB6AXuApaXNeZ2NdToiWQohoJCwWpRRJv8ArjWKlLDvNywq5hsqdjx6KnhtzOnmNWvxAYU0zEjTBxzcH_3SMD_SDHM3IufL0rxjP3LdqP8OmENXx0-xFFATApTLBUoBx2ULuvt--ovOjVBGkOKrBIettZzM0nY0atSGIT_25IVFcmdg6vEHGHjgxChdl_at7nTyRfGZkj9bZCwpdKZJkm8ZPzDw-uRmA0xykLW4vtOqxHxKZSu13YGNdAw_9nqfXLb3f6t6td0Da8iL4Adk",
                    Category = "Entradas"
                },
                new Product
                {
                    NameProduct = "Entrecot de Ternera",
                    Price = 34.00m,
                    Descrip = "350g de corte premium, madurado 45 días, acompañado de milhojas de papa y chimichurri de la casa.",
                    ImagenProduct = "https://lh3.googleusercontent.com/aida-public/AB6AXuC3HFlCk01x5HXbNCLTUcdlFRX6rXUIJ1G_pL68x9NGjxeMcFo-rPHIEy2LmLqSitueSR9l56EmxgIZvg7MSElL8IOXMOTKCog015Vv_iLs2Zh61Z10ONIx2IGtPW6ruE1vD-W4o_7VBDPipItYoz-hBiTNaNup26rociPIfH34tWgwX23YofbLTqhf7RpnB630L67-H2EjDJH8y3kxD44z2gsFVLhbG_hQ76hFG8aJJPItWSKMeTvDHkXjrIjvR8I1b31OgcSIwFM",
                    Category = "Platos Fuertes"
                },
                new Product
                {
                    NameProduct = "Salmón del Pacífico",
                    Price = 29.50m,
                    Descrip = "A la brasa con costra de hierbas, puré de guisantes a la menta y espárragos trigueros.",
                    ImagenProduct = "https://lh3.googleusercontent.com/aida-public/AB6AXuDw3dPDgbsUiTd5UH4fgFYsoI9QTJFRqnwkA_4irmIBIEqc02p37vi1GKbIaZsL4OSIYzXN1YrBHyyqzf-aseir0eX98GRvoBmPjELbcMn5RWh73_2-ESZ4n_QkRLHfUq_9RXuqeI2G5b91Rp5BvuJq2z9Cjh-JoLT8k4RPYBk9QEXyMb6hX-6h75g6w1shWoSZzLe78pQg5RYxcndbbt0lq3oP9p5kp0MqveqWSlQUxe7XvBgkEDlZmpipnqCjT1xA8q2Gfpk0Gmo",
                    Category = "Platos Fuertes"
                },
                new Product
                {
                    NameProduct = "Volcán de Cacao",
                    Price = 9.00m,
                    Descrip = "70% cacao orgánico y helado de vainilla.",
                    ImagenProduct = "https://lh3.googleusercontent.com/aida-public/AB6AXuBgiwZTniyCllFMnqgPs6LUVXhQ0k3G-x6YnxPfBbQyfO6nrASNmh82n_Op0iFcB2Vbexj9izl1tNhCKhpnEKEngB5ga8ISMIVNhPwGeYK9p1jXd4BK-b3RaXD5VYzivorbUjzUShJrVhmpho55t9BV6FtGDLM8Lpluw_yteVltcmCHZLGPp2H_xtoPWhWaYFn9YfZkfBXZ0uj6JG2aPm0dZG9_hq0WIovrrioP50Yv2qQBsWQrz1xRJH8WjW8y6svHRPgqTsO2NV4",
                    Category = "Postres"
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
