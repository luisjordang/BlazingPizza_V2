
// Esta clase crea un contexto de base de datos que se puede usar para registrar un servicio de base de datos. El contexto también nos permite tener un controlador que accede a la base de datos.
using Microsoft.EntityFrameworkCore;

// Declaración del espacio de nombres para organizar las clases relacionadas con la capa de datos
namespace BlazingPizza.Data; // it is only this what was giving errors

// Clase que representa el contexto de la base de datos para la aplicación de la pizzería
public class PizzaStoreContext : DbContext
{
    // Constructor que recibe las opciones de configuración del DbContext
    public PizzaStoreContext(
        DbContextOptions options) : base(options)
    {
    }

    // Conjunto de entidades que representa los pedidos en la base de datos
    public DbSet<Order> Orders { get; set; }
    // Conjunto de entidades que representa las pizzas en la base de datos
    public DbSet<Pizza> Pizzas { get; set; }
    // Conjunto de entidades que representa las especialidades de pizza en la base de datos
    public DbSet<PizzaSpecial> Specials { get; set; }
    // Conjunto de entidades que representa los ingredientes adicionales (toppings) en la base de datos
    public DbSet<Topping> Toppings { get; set; }

    // Método que se llama al crear el modelo de base de datos, utilizado para configurar relaciones y restricciones
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring a many-to-many special -> topping relationship that is friendly for serialization
        modelBuilder.Entity<PizzaTopping>().HasKey(pst => new { pst.PizzaId, pst.ToppingId });
        modelBuilder.Entity<PizzaTopping>().HasOne<Pizza>().WithMany(ps => ps.Toppings);
        modelBuilder.Entity<PizzaTopping>().HasOne(pst => pst.Topping).WithMany();
    }

}
