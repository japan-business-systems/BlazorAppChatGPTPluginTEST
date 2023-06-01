using BlazorAppChatGPTPluginTEST.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// ** Create CORS policy called OpenAI
// to allow OpenAI to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenAI", policy =>
    {
        policy.WithOrigins(
            "https://chat.openai.com",
            "http://localhost:5200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
// ** Add controllers to the application
builder.Services.AddControllers();
// ** Add Swagger/OpenAPI to the application
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blazor Azure Cognitive Search Plugin",
        Version = "v1",
        Description = "Blazorで動作するAzure Cognitive Search接続用のプラグインです。"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Disable this so that OpenAI can access the API
// without using https  (which is not supported by OpenAI)
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapControllers();

app.MapFallbackToPage("/_Host");


// ** CORS to allow OpenAI to access the API
app.UseCors("OpenAI");

// ** Serve the .well-known folder for OpenAI
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});

// ** UseSwagger
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer> {
                        new OpenApiServer {
                            Url = $"{httpReq.Scheme}://{httpReq.Host.Value}"
                        }
                    };
    });
});

// ** UseSwaggerUI
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint(
        "/swagger/v1/swagger.yaml",
        "Blazor TODO Plugin (no auth)"
        );
});

app.Run();