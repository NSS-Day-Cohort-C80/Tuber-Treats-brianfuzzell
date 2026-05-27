using TuberTreats.Models;
using TuberTreats.Models.DTOs;

List<Customer> customers = new List<Customer>
{
    new Customer() { Id = 1, Name = "Homer Simpson", Address = "742 Evergreen Terrace" },
    new Customer() { Id = 2, Name = "Marge Simpson", Address = "742 Evergreen Terrace" },
    new Customer() { Id = 3, Name = "Ned Flanders", Address = "744 Evergreen Terrace" },
    new Customer() { Id = 4, Name = "Moe Szyslak", Address = "57 Walnut Street" },
    new Customer() { Id = 5, Name = "Bart Simpson", Address = "742 Evergreen Terrace" }
};

List<Topping> toppings = new List<Topping>
{
    new Topping() { Id = 1, Name = "Butter" },
    new Topping() { Id = 2, Name = "Sour Cream" },
    new Topping() { Id = 3, Name = "Cheddar Cheese" },
    new Topping() { Id = 4, Name = "Bacon Bits" },
    new Topping() { Id = 5, Name = "Chives" }
};

List<TuberDriver> tuberDrivers = new List<TuberDriver>
{
    new TuberDriver() { Id = 1, Name = "Apu Nahasapeemapetilon", TuberDeliveries = new List<TuberOrder>() },
    new TuberDriver() { Id = 2, Name = "Krusty the Clown", TuberDeliveries = new List<TuberOrder>() },
    new TuberDriver() { Id = 3, Name = "Barney Gumble", TuberDeliveries = new List<TuberOrder>() }
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping() { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping() { Id = 2, TuberOrderId = 1, ToppingId = 3 },
    new TuberTopping() { Id = 3, TuberOrderId = 2, ToppingId = 2 },
    new TuberTopping() { Id = 4, TuberOrderId = 2, ToppingId = 4 },
    new TuberTopping() { Id = 5, TuberOrderId = 3, ToppingId = 1 },
    new TuberTopping() { Id = 6, TuberOrderId = 3, ToppingId = 5 }
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder()
    {
        Id = 1,
        OrderPlacedOnDate = new DateTime(2024, 1, 15),
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = new DateTime(2024, 1, 15),
        Toppings = new List<Topping>()
    },
    new TuberOrder()
    {
        Id = 2,
        OrderPlacedOnDate = new DateTime(2024, 2, 20),
        CustomerId = 3,
        TuberDriverId = 2,
        DeliveredOnDate = new DateTime(2024, 2, 20),
        Toppings = new List<Topping>()
    },
    new TuberOrder()
    {
        Id = 3,
        OrderPlacedOnDate = new DateTime(2024, 3, 10),
        CustomerId = 5,
        TuberDriverId = null,
        DeliveredOnDate = null,
        Toppings = new List<Topping>()
    }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here
app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new ToppingDTO
    {
        Id = t.Id,
        Name = t.Name
    });
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);

    if (topping == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new ToppingDTO
    {
        Id = topping.Id,
        Name = topping.Name
    });
});

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    if (customer == null)
    {
        return Results.NotFound();
    }

    List<TuberOrder> orders = tuberOrders.Where(to => to.CustomerId == id).ToList();

    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        TuberOrders = orders.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId,
            DeliveredOnDate = o.DeliveredOnDate
        }).ToList()
    });
});

app.MapPost("/customers", (Customer customer) =>
{
    if (customers.Count == 0)
    {
        customer.Id = 1;
    }
    else
    {
        customer.Id = customers.Max(c => c.Id) + 1;
    }

    customers.Add(customer);

    return Results.Created($"/customers/{customer.Id}", new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address
    });
});

app.MapDelete("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    if (customer == null)
    {
        return Results.NotFound();
    }

    customers.Remove(customer);

    return Results.NoContent();
});

app.MapGet("/tuberdrivers", () =>
{
    return tuberDrivers.Select(td => new TuberDriverDTO
    {
        Id = td.Id,
        Name = td.Name
    });
});

app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == id);

    if (tuberDriver == null)
    {
        return Results.NotFound();
    }

    List<TuberOrder> orders = tuberOrders.Where(to => to.TuberDriverId == id).ToList();

    return Results.Ok(new TuberDriverDTO
    {
        Id = tuberDriver.Id,
        Name = tuberDriver.Name,
        TuberDeliveries = orders.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId,
            DeliveredOnDate = o.DeliveredOnDate
        }).ToList()
    });
});

app.MapGet("/tuberorders", () =>
{
    return tuberOrders.Select(to => new TuberOrderDTO
    {
        Id = to.Id,
        OrderPlacedOnDate = to.OrderPlacedOnDate,
        CustomerId = to.CustomerId,
        TuberDriverId = to.TuberDriverId,
        DeliveredOnDate = to.DeliveredOnDate,
        Toppings = tuberToppings
            .Where(tt => tt.TuberOrderId == to.Id)
            .Select(tt => toppings.First(t => t.Id == tt.ToppingId))
            .Select(t => new ToppingDTO { Id = t.Id, Name = t.Name })
            .ToList()
    });
});

app.MapGet("/tuberorders/{id}", (int id) =>
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(t => t.Id == id);
    if (tuberOrder == null)
    {
        return Results.NotFound();
    }

    Customer customer = customers.FirstOrDefault(c => c.Id == tuberOrder.CustomerId);

    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == tuberOrder.TuberDriverId);

    return Results.Ok(new TuberOrderDTO
    {
        Id = tuberOrder.Id,
        OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
        CustomerId = tuberOrder.CustomerId,
        Customer = customer == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        TuberDriverId = tuberOrder.TuberDriverId,
        TuberDriver = tuberDriver == null ? null : new TuberDriverDTO
        {
            Id = tuberDriver.Id,
            Name = tuberDriver.Name
        },
        DeliveredOnDate = tuberOrder.DeliveredOnDate,
        Toppings = tuberToppings
            .Where(tt => tt.TuberOrderId == tuberOrder.Id)
            .Select(tt => toppings.First(t => t.Id == tt.ToppingId))
            .Select(t => new ToppingDTO { Id = t.Id, Name = t.Name })
            .ToList()
    });
});

app.MapPost("/tuberorders", (TuberOrder tuberOrder) =>
{
    if (tuberOrders.Count == 0)
    {
        tuberOrder.Id = 1;
    }
    else
    {
       tuberOrder.Id = tuberOrders.Max(to => to.Id) + 1; 
    }
    
    tuberOrder.OrderPlacedOnDate = DateTime.Now;

    foreach (Topping topping in tuberOrder.Toppings)
    {
        tuberToppings.Add(new TuberTopping
        {
            Id = tuberToppings.Max(tt => tt.Id) + 1,
            TuberOrderId = tuberOrder.Id,
            ToppingId = topping.Id
        });
    }

    tuberOrders.Add(tuberOrder);

    return Results.Created($"/tuberorders/{tuberOrder.Id}", new TuberOrderDTO
    {
        Id = tuberOrder.Id,
        OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
        CustomerId = tuberOrder.CustomerId,
        TuberDriverId = tuberOrder.TuberDriverId,
        DeliveredOnDate = tuberOrder.DeliveredOnDate,
        Toppings = tuberToppings
            .Where(tt => tt.TuberOrderId == tuberOrder.Id)
            .Select(tt => toppings.First(t => t.Id == tt.ToppingId))
            .Select(t => new ToppingDTO { Id = t.Id, Name = t.Name })
            .ToList()
    });
});

app.MapPut("/tuberorders/{id}", (int id, TuberOrder tuberOrder) =>
{
    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(to => to.Id == id);

    if (orderToUpdate == null)
    {
        return Results.NotFound();
    }

    orderToUpdate.TuberDriverId = tuberOrder.TuberDriverId;

    return Results.NoContent();
});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    TuberOrder orderToComplete = tuberOrders.FirstOrDefault(to => to.Id == id);

    orderToComplete.DeliveredOnDate = DateTime.Today;
});

app.Run();
//don't touch or move this!
public partial class Program { }