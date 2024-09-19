using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Application.Services.Interfaces;
using WebApp.Components;
using WebApp.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddScoped(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return clientFactory.CreateClient("ApiGatewayHttpClient");
});
builder.Services.AddHttpClient("ApiGatewayHttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyMethod());
})
builder.Services.AddTransient<IIdentityService, WebApp.Application.Services.Interfaces.IdentityService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors("CorsPolicy");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
