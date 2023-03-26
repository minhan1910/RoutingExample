using RoutingExample.CustomConstraints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("months", typeof(MonthsCustomConstraint));
});  

var app = builder.Build();

app.Use(async (context, next) =>
{
    Endpoint? endPoint = context.GetEndpoint();

    if (endPoint != null)
    {
        await context.Response.WriteAsync($"Endpoint: {endPoint.DisplayName}\n");
    }

    await next(context);
});
// enabled routing

app.UseRouting();

app.Use(async (context, next) =>
{
    Endpoint? endPoint = context.GetEndpoint();

    if (endPoint != null)
    {
        await context.Response.WriteAsync($"Endpoint: {endPoint.DisplayName}\n");
    }

    await next(context);
});

// crating endpoints
app.UseEndpoints(endpoints =>
{
    // add your endpoints here
    endpoints.Map("files/{filename}.{extension}", async (context) => {
        string? fileName = Convert.ToString(context.Request.RouteValues["filename"]);
        string? extension = Convert.ToString(context.Request.RouteValues["extension"]);
        
        await context.Response.WriteAsync($"In files - {fileName}.{extension}");
    });

    endpoints.Map("employees/profile/{EmployeeName:length(4,7):alpha=minan}", async (context) =>
    {
        string? employeeName = Convert.ToString(context.Request.RouteValues["EmployeeName"]);

        await context.Response.WriteAsync($"In Employee Profile - {employeeName}");
    });

    endpoints.Map("products/details/{id:int:range(1,1000)?}", async (context) =>
    {
        if (context.Request.RouteValues.ContainsKey("id"))
        {
            int idProduct = Convert.ToInt32(context.Request.RouteValues["id"]);

            await context.Response.WriteAsync($"Products Details - {idProduct}");
        } else
        {
            await context.Response.WriteAsync($"Products Details - id is not supplied");
        }
    });

    // Eg: daily-digest-report/{reportname}
    endpoints.Map("daily-digest-report/{reportdate:datetime}", async context =>
    {
        DateTime reportDate = Convert.ToDateTime(context.Request.RouteValues["reportdate"]);

        await context.Response.WriteAsync($"In daily - digist-report - {reportDate.ToShortDateString()}");
    });

    // Eg: cities/cityid
    endpoints.Map("cities/{cityid:guid}", async context =>
    {
        // ! is can not be null
        Guid cityId = Guid.Parse(Convert.ToString(context.Request.RouteValues["cityid"])!);

        await context.Response.WriteAsync($"City information - {cityId}");
    });

    // Eg: sales-report/2030/apr
    endpoints.Map("sales-report/{year:int:min(1900)}/{month:months}", async context => 
    {
        int year = Convert.ToInt32(context.Request.RouteValues["year"]);
        string? month = Convert.ToString(context.Request.RouteValues["month"]);

        var months = new List<string>()
        {
            "oct", "apr", "jul", "jan"
        };

        // for not constraint will be notify the meaningful of response
        if (!string.IsNullOrEmpty(month) && months.Contains(month)) {
            await context.Response.WriteAsync($"sales report - {year} - {month}");
        } else
        {
            await context.Response.WriteAsync($"{month} is not allowed for sales report");
        }
    });

    // Eg: sales-report/2024/jan
    // This endpoints has more precedence than sales-report/{year:int:min(1900)}/{month:months}
    endpoints.Map("sales-report/2024/jan", async context =>
    {
        await context.Response.WriteAsync("Sales report exclusively for 2024 - jan");
    });
});

app.Run(async context =>
{
    await context.Response.WriteAsync($"Request received at {context.Request.Path}");
});

app.Run();
