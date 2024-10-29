using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Products
var products = new List<Product>()
{
    new Product {Id=1, Name="Product 1" , Description="Description for Product 1" , Price=30},
    new Product {Id=1, Name="Product 2" , Description="Description for Product 2" , Price=56},
    new Product {Id=1, Name="Product 3" , Description="Description for Product 3" , Price=33},
    new Product {Id=1, Name="Product 4" , Description="Description for Product 4" , Price=120},
    new Product {Id=1, Name="Product 5" , Description="Description for Product 5" , Price=5}
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// GET /Products
app.MapGet("/Products", () =>  products);

// GET /Products/{id}
app.MapGet("/Products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

// POST /Products
app.MapPost("/Products", (Product product) =>
{
    product.Id = products.Max(p => p.Id) + 1;
    products.Add(product);
    return Results.Created($"/Products/{product.Id}", product);
});

// PUT /Products/{id}
app.MapPut("/Products/{id}", (int id, Product updatedProduct) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    return Results.NoContent();
});

// DELETE /Products/{id}
app.MapDelete("/Products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();

    products.Remove(product);
    return Results.NoContent();
});

app.Run();
