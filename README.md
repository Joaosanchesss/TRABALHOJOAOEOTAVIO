# Folha de Pagamento - Minimal API

Alunos: Joao Sanches e Otavio Lunardelli

Solucao: Joao | Projeto: Otavio | Banco: joao_otavio.db

Tecnologias: C# .NET 8, Minimal API, Entity Framework Core, SQLite.

## Como executar

Requisito: .NET 8 SDK.

```
cd Otavio
dotnet restore
dotnet run
```

A API sobe em http://localhost:5000 e o banco joao_otavio.db e criado automaticamente.

## Endpoints

- POST /api/folha/cadastrar
- GET /api/folha/listar
- GET /api/folha/buscar/{cpf}/{mes}/{ano}
- DELETE /api/folha/remover/{cpf}/{mes}/{ano}
- GET /api/folha/total-liquido

## Exemplo de cadastro

```json
{
  "nome": "Ana Souza",
  "cpf": "123.456.789-00",
  "mes": 10,
  "ano": 2025,
  "horasTrabalhadas": 160,
  "valorHora": 50
}
```
