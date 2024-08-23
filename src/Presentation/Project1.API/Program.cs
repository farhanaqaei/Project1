using Project1.API.ActionFilters.AuditlogFilters;
using Project1.Infrastructure.Data;
using Project1.IOC;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Rate Limiter Policy to use arbitrary
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name
                          ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                          ?? httpContext.Connection.RemoteIpAddress?.ToString()
                          ?? "anonymous",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromSeconds(10),
                AutoReplenishment = true,
                QueueLimit = 0,
            }));
});

// Global Rate Limiter
//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: httpContext.User.Identity?.Name 
//                          ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
//                          ?? httpContext.Connection.RemoteIpAddress?.ToString() 
//                          ?? "anonymous",
//            factory: key => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 3,
//                Window = TimeSpan.FromSeconds(10),
//                AutoReplenishment = true,
//                //QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                QueueLimit = 0 // Optional: requests can be queued when limit is reached
//            }));
//});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionLoggingFilter>();
});

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await ApplicationDbContextSeed.SeedRolesAndAdminUserAsync(services);
    }
    catch (Exception ex)
    {
        // log ex
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
