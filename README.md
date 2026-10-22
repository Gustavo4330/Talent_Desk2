# Talent Desk

Sistema de gestão de currículos feito em **C# com ASP.NET Core MVC** e banco de dados **SQLite**.

Este projeto é a evolução do trabalho do bimestre passado: mantivemos a ideia original e focamos em melhorá-lo, levando os dados para o banco e as regras de negócio para o C#.

## Integrantes

- Gustavo Calderon
- Rafael Sodré
- Felipe Nunes
- Pedro Henrique
- Kauã Vinicius

## Funcionalidades

- **Tela Inicial** com indicadores: total de currículos, novos da semana, favoritos e candidatos em processo seletivo
- **Busca e filtros** por nome, cargo, competência, cidade, área, formação, experiência, faixa etária e status, com chips dos filtros ativos (remoção individual) e ordenação por data, nome ou experiência. Planejamento da feature em [`spec/filtros-e-busca.md`](spec/filtros-e-busca.md) e [`plan/filtros-e-busca.md`](plan/filtros-e-busca.md)
- **Painel de detalhes** com resumo, experiências, formação e competências do candidato
- **Cadastro, edição e exclusão** de currículos, com validação dos campos
- **Favoritos** e **status** do processo seletivo (Novo, Em análise, Entrevista, Aprovado, Reprovado)
- **Dashboard** gerencial com filtros de período e área, indicadores (total, em processo, taxa de aprovação, perfil médio), evolução dos cadastros, funil do processo seletivo e mapa do Brasil com os candidatos por cidade e região
- **Login** com senha protegida por BCrypt

## Tecnologias

- C# e .NET 10
- ASP.NET Core MVC e Razor
- SQLite com Microsoft.Data.Sqlite
- BCrypt.Net-Next
- HTML e CSS

## Como rodar o projeto

1. Instale o [.NET 10 SDK](https://dotnet.microsoft.com/download).
2. Na pasta do projeto, rode:

   ```bash
   dotnet run
   ```

3. Acesse http://localhost:5216.

Não é preciso instalar nem ligar nenhum servidor de banco de dados. Na primeira execução o sistema cria o banco sozinho, com as tabelas, os perfis (Candidato, Recrutador e Administrador) e alguns currículos de exemplo.

Se o `dotnet run` reclamar que não encontrou o pacote `Microsoft.Data.Sqlite`, o computador está sem a origem do NuGet configurada. Rode uma vez:

```bash
dotnet restore --source https://api.nuget.org/v3/index.json
```

## Banco de dados

O banco é um arquivo SQLite criado automaticamente em:

```
%LOCALAPPDATA%\TalentDesk\talentdesk.db
```

(por exemplo, `C:\Users\SeuUsuario\AppData\Local\TalentDesk\talentdesk.db`). Ele fica fora da pasta do projeto para não ser versionado nem sincronizado por serviços de nuvem enquanto está em uso. O caminho está em `Models/Conexao.cs` e as tabelas em `Repositories/BancoDeDados.cs`.

Para recomeçar do zero, feche o sistema e apague o arquivo `talentdesk.db`: ele é recriado na próxima execução. Os usuários são criados pela tela de cadastro; nenhum usuário de teste vem pronto.

O arquivo `talent_desk.sql` é apenas histórico: é o dump do banco MySQL (XAMPP) usado antes da migração para o SQLite e não é mais usado pelo sistema.
