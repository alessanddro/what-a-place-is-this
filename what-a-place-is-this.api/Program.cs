using what_a_place_is_this.api.Config;
using what_a_place_is_this.api.Data;
using what_a_place_is_this.api.Mappings;
using what_a_place_is_this.api.Services;

var builder = WebApplication.CreateBuilder(args);
// Configs
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DbConfig"));
//
// Add services to the container.
builder.Services.AddSingleton<PlaceService>();
builder.Services.AddControllers();
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(ModelToDTOProfile));

builder.Services.AddSwaggerConfig();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
