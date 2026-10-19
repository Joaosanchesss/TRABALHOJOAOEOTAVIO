using Otavio.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=joao_otavio.db"));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.MapPost("/api/folha/cadastrar", async (Folha f, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(f.Cpf)) return Results.BadRequest("Cpf obrigatorio");
    if (f.Mes < 1 || f.Mes > 12) return Results.BadRequest("Mes invalido");
    if (f.Ano < 2000) return Results.BadRequest("Ano invalido");
    if (await db.Folhas.AnyAsync(x => x.Cpf == f.Cpf && x.Mes == f.Mes && x.Ano == f.Ano))
        return Results.BadRequest("Folha ja cadastrada");

    decimal bruto = f.HorasTrabalhadas * f.ValorHora;

    decimal ir;
    if (bruto <= 1903.98m) ir = 0;
    else if (bruto <= 2826.65m) ir = bruto * 0.075m - 142.80m;
    else if (bruto <= 3751.05m) ir = bruto * 0.15m - 354.80m;
    else if (bruto <= 4664.68m) ir = bruto * 0.225m - 636.13m;
    else ir = bruto * 0.275m - 869.36m;

    decimal inss;
    if (bruto <= 1693.72m) inss = bruto * 0.08m;
    else if (bruto <= 2822.90m) inss = bruto * 0.09m;
    else if (bruto <= 5645.80m) inss = bruto * 0.11m;
    else inss = 621.03m;

    decimal fgts = bruto * 0.08m;

    f.Id = 0;
    f.SalarioBruto = Math.Round(bruto, 2);
    f.ImpostoRenda = Math.Round(ir, 2);
    f.Inss = Math.Round(inss, 2);
    f.Fgts = Math.Round(fgts, 2);
    f.SalarioLiquido = Math.Round(bruto - ir - inss, 2);

    db.Folhas.Add(f);
    await db.SaveChangesAsync();
    return Results.Created($"/api/folha/buscar/{f.Cpf}/{f.Mes}/{f.Ano}", f);
});

app.MapGet("/api/folha/listar", async (AppDbContext db) =>
{
    var lista = await db.Folhas.ToListAsync();
    if (lista.Count == 0) return Results.NotFound();
    return Results.Ok(lista);
});

app.MapGet("/api/folha/buscar/{cpf}/{mes}/{ano}", async (string cpf, int mes, int ano, AppDbContext db) =>
{
    var f = await db.Folhas.FirstOrDefaultAsync(x => x.Cpf == cpf && x.Mes == mes && x.Ano == ano);
    if (f == null) return Results.NotFound();
    return Results.Ok(f);
});

app.MapDelete("/api/folha/remover/{cpf}/{mes}/{ano}", async (string cpf, int mes, int ano, AppDbContext db) =>
{
    var f = await db.Folhas.FirstOrDefaultAsync(x => x.Cpf == cpf && x.Mes == mes && x.Ano == ano);
    if (f == null) return Results.NotFound();
    db.Folhas.Remove(f);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/api/folha/total-liquido", async (AppDbContext db) =>
{
    if (!await db.Folhas.AnyAsync()) return Results.NotFound();
    var lista = await db.Folhas.Select(x => x.SalarioLiquido).ToListAsync();
    return Results.Ok(new { totalLiquido = Math.Round(lista.Sum(), 2) });
});

app.Run();
