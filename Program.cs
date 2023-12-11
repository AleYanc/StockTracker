using Microsoft.EntityFrameworkCore;
using StockTracker.Database;
using StockTracker.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<DatabaseConnection>(
    opt =>
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IFileSaver, LocalFileSaver>();
builder.Services.AddSingleton<IUriService>(o =>
{
    IHttpContextAccessor accessor = o.GetRequiredService<IHttpContextAccessor>();
    HttpRequest request = accessor.HttpContext.Request;
    string uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriService(uri);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
    builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
