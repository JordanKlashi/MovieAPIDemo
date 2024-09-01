using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MovieAPIDemo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("AlloAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Ajouter la configuration de DbContext
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"C:\Users\jorda\Desktop\formation developpeur web\CSharp\New Project ASP + React\StaticImage"),
    RequestPath = "/StaticImage"
});


app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();