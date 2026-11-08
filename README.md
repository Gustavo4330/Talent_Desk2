# Talent Desk

Sistema web de **gestão de currículos** para equipes de recrutamento: cadastra candidatos, busca e filtra perfis, acompanha o processo seletivo e mostra indicadores em um dashboard.

![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core_MVC-5C2D91?style=flat-square&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white)
![BCrypt](https://img.shields.io/badge/Senhas-BCrypt-2D3748?style=flat-square)

> Projeto acadêmico em grupo (Técnico em Desenvolvimento de Sistemas, ETEC). Todos os currículos do sistema são **fictícios**, criados apenas para demonstração.


## O que o sistema resolve

Equipes de RH perdem tempo procurando currículos em planilhas e pastas. O Talent Desk reúne os candidatos em um só lugar, permite encontrar o perfil certo com filtros combinados e mostra em que etapa do processo seletivo cada pessoa está.

Este projeto é a **evolução do trabalho do bimestre anterior**: mantivemos a ideia original e melhoramos a base técnica, levando os dados para um banco SQLite e as regras de negócio para o C#.

## Funcionalidades

**Currículos**
- Cadastro, edição e exclusão, com validação dos campos
- Painel de detalhes com resumo, experiências, formação e competências
- Favoritos e status do processo seletivo: Novo, Em análise, Entrevista, Aprovado e Reprovado

**Busca e filtros**
- Filtros por nome, cargo, competência, cidade, área, formação, experiência, faixa etária e status
- Chips dos filtros ativos, com remoção individual
- Ordenação por data, nome ou experiência
- Planejamento da funcionalidade em [`spec/filtros-e-busca.md`](spec/filtros-e-busca.md) e [`plan/filtros-e-busca.md`](plan/filtros-e-busca.md)

**Tela inicial e dashboard**
- Indicadores: total de currículos, novos da semana, favoritos e candidatos em processo seletivo
- Dashboard gerencial com filtros de período e área
- Total, em processo, taxa de aprovação e perfil médio
- Evolução dos cadastros e funil do processo seletivo
- Mapa do Brasil com candidatos por cidade e região

**Acesso**
- Login com senha protegida por BCrypt
- Perfis de acesso: Candidato, Recrutador e Administrador

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Linguagem e plataforma | C# e .NET 10 |
| Web | ASP.NET Core MVC e Razor |
| Banco de dados | SQLite (Microsoft.Data.Sqlite) |
| Segurança | BCrypt.Net-Next |
| Interface | HTML e CSS |

## Decisões técnicas

- **MySQL para SQLite:** o banco anterior (XAMPP) exigia instalar e ligar um servidor. Com o SQLite, o projeto roda com um único comando e o banco é criado sozinho na primeira execução.
- **Regras de negócio no C#:** a lógica saiu das telas e foi para o código, o que facilita testar e manter.
- **Senhas com BCrypt:** nenhuma senha é guardada em texto puro.
- **Banco fora da pasta do projeto:** evita que o arquivo seja versionado ou sincronizado por serviços de nuvem enquanto está em uso.
- **Planejamento antes do código:** as funcionalidades maiores têm uma especificação (`spec/`) e um plano (`plan/`).

## Como rodar

1. Instale o [.NET 10 SDK](https://dotnet.microsoft.com/download).
2. Na pasta do projeto, rode:

   ```bash
   dotnet run
   ```

3. Acesse http://localhost:5216.

Não é preciso instalar nem ligar nenhum servidor de banco de dados. Na primeira execução o sistema cria o banco, com as tabelas, os perfis de acesso e alguns currículos de exemplo.

Se o `dotnet run` reclamar que não encontrou o pacote `Microsoft.Data.Sqlite`, a origem do NuGet não está configurada no computador. Rode uma vez:

```bash
dotnet restore --source https://api.nuget.org/v3/index.json
```

## Banco de dados

O banco é um arquivo SQLite criado automaticamente em:

```
%LOCALAPPDATA%\TalentDesk\talentdesk.db
```

Por exemplo: `C:\Users\SeuUsuario\AppData\Local\TalentDesk\talentdesk.db`.

- O caminho está em `Models/Conexao.cs` e as tabelas em `Repositories/BancoDeDados.cs`.
- Para recomeçar do zero, feche o sistema e apague o arquivo `talentdesk.db`. Ele é recriado na próxima execução.
- Os usuários são criados pela tela de cadastro. Nenhum usuário de teste vem pronto.
- O arquivo `talent_desk.sql` é só histórico: é o dump do MySQL usado antes da migração e não é mais usado pelo sistema.




## Próximos passos

- [ ] Adicionar prints e um GIF de demonstração
- [ ] Testes automatizados das regras de negócio
- [ ] Extração de dados de currículos com IA e saída estruturada, usando apenas currículos fictícios
