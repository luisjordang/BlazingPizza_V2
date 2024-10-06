using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Data;

// Controlador API para gestionar las operaciones relacionadas con los pedidos
[Route("orders")]
[ApiController]
public class OrdersController : Controller
{
    private readonly PizzaStoreContext _db; // Contexto de base de datos para acceder a los datos

    // Constructor que inyecta el contexto de la base de datos
    public OrdersController(PizzaStoreContext db)
    {
        _db = db;
    }

    // Método para obtener la lista de pedidos con su estado
    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        // Obtiene los pedidos de la base de datos, incluyendo las pizzas, sus especialidades y toppings
        var orders = await _db.Orders
         .Include(o => o.Pizzas).ThenInclude(p => p.Special)
         .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
         .OrderByDescending(o => o.CreatedTime) // Ordena los pedidos por fecha de creación descendente
         .ToListAsync();

        // Convierte los pedidos a su representación con estado y los devuelve
        return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
    }

    // Método para realizar un nuevo pedido
    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        order.CreatedTime = DateTime.Now; // Establece la fecha de creación del pedido

        // Enforce existence of Pizza.SpecialId and Topping.ToppingId
        // in the database - prevent the submitter from making up
        // new specials and toppings
        foreach (var pizza in order.Pizzas)
        {
            pizza.SpecialId = pizza.Special.Id; // Asigna el ID de la especialidad
            pizza.Special = null; // Elimina la referencia a la especialidad para evitar duplicados
        }

        _db.Orders.Attach(order); // Adjunta el pedido al contexto de seguimiento de cambios
        await _db.SaveChangesAsync(); // Guarda los cambios en la base de datos

        return order.OrderId; // Devuelve el ID del pedido creado
    }

    // Método para obtener un pedido específico con su estado
    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
    {
        // Busca el pedido por ID, incluyendo las pizzas, sus especialidades y toppings
        var order = await _db.Orders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
            .SingleOrDefaultAsync();

        if (order == null)
        {
            return NotFound(); // Devuelve un 404 si el pedido no se encuentra
        }

        return OrderWithStatus.FromOrder(order); // Devuelve el pedido con su estado
    }
}