// Se agregan los espacios de nombres necesarios

using BlazingPizza.Data;
using BlazingPizza.Services;

var builder = WebApplication.CreateBuilder(args); // Se crea una nueva instancia de WebApplicationBuilder

// Se registran servicios para la aplicación
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Este código registra dos servicios. La primera instrucción AddHttpClient permite que la aplicación acceda a comandos HTTP. La aplicación usa HttpClient para obtener el JSON de los especiales de pizza. La segunda instrucción registra el nuevo elemento PizzaStoreContext y proporciona el nombre de archivo de la base de datos SQLite.
// Agrega soporte para HttpClient
builder.Services.AddHttpClient();
// Registra el contexto de la base de datos SQLite
builder.Services.AddSqlite<PizzaStoreContext>("Data Source=pizza.db");

// Registra OrderState como un servicio con ámbito
builder.Services.AddScoped<OrderState>();

var app = builder.Build(); // Se construye la aplicación

// Configura el manejo de excepciones
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles(); // Habilita el servicio de archivos estáticos
app.UseRouting(); // Configura el enrutamiento

app.MapRazorPages(); // Agrega las páginas Razor
app.MapBlazorHub(); // Agrega Blazor
app.MapFallbackToPage("/_Host"); // Configura la página de error en caso de no encontrar una página

// Agrega rutas de controlador
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

// Initialize the database
// Este cambio crea un ámbito de base de datos con PizzaStoreContext. Si no hay una base de datos ya creada, llama a la clase estática SeedData para crear una.
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>(); // Obtiene el servicio de fábrica de ámbito
using (var scope = scopeFactory.CreateScope()) // Crea un nuevo ámbito
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>(); // Obtiene el contexto de la base de datos
    if (db.Database.EnsureCreated()) // Verifica si la base de datos ya existe
    {
        SeedData.Initialize(db); // Inicializa la base de datos con datos de ejemplo
    }
}

// Inicia la ejecución de la aplicación
// app.Run();
await app.RunAsync();