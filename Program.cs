using TalentDesk.Filters;
using TalentDesk.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(opcoes =>
{
    // Mostra uma tela amigável quando o MySQL está desligado
    opcoes.Filters.Add<BancoIndisponivelFilter>();
});

// Sessão: guarda o usuário logado
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opcoes =>
{
    opcoes.IdleTimeout = TimeSpan.FromHours(8);
    opcoes.Cookie.HttpOnly = true;
    opcoes.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Curriculo}/{action=Index}/{id?}");

// Cria as tabelas de currículos no MySQL (se ainda não existirem)
BancoDeDados.TentarInicializar(app.Logger);

app.Run();
