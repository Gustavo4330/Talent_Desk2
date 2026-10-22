# Spec — Feature "Filtros e busca" (Talent Desk)

## 1. Entender a funcionalidade

**O que será criado?**
Uma nova versão dos filtros e da busca da tela de currículos (`/Curriculo/Index`), usada pelo recrutador para encontrar candidatos rapidamente.

**Qual problema resolve?**
Com muitos currículos cadastrados, o recrutador precisa reduzir a lista até chegar nos candidatos certos, saber quais filtros estão ligados e organizar o resultado.

**Qual o objetivo?**
Encontrar um candidato em poucos cliques, sem precisar rolar a lista inteira.

## 2. Situação atual (antes desta feature)

Analisando o código, o projeto já tinha uma primeira versão:

| Item | Situação |
|------|----------|
| Busca por texto | Funciona: nome, cargo, área e competências, sem diferenciar acentos/maiúsculas |
| Filtros de área, formação, experiência e localização | Funcionam (selects que enviam o formulário sozinhos) |
| Filtro de status | Funciona (em "Mais filtros") |
| Filtro de idade | **Só idade exata** (ex.: 27). Quem procura "entre 26 e 30 anos" precisa testar idade por idade. A classe `FaixaIdade` existia mas não era usada |
| Ordenação | **Não existe**: a lista é sempre "mais recentes primeiro" |
| Filtros ativos | **Não aparecem de forma clara**: o usuário não vê o resumo do que está filtrando e só consegue limpar tudo de uma vez |
| Busca por cidade | **Não**: digitar "Curitiba" na busca não encontra nada |

## 3. Requisitos da nova versão

### RF01 — Filtro por faixa etária
- Trocar o campo numérico "Idade" por um select com as faixas de `FaixaIdade.Todas` (Menor de 18, 18 a 20, 21 a 25, ..., Acima de 60).
- Parâmetro na URL: `idade=26-30`.
- O banco filtra com `c.idade BETWEEN @idadeMin AND @idadeMax`.
- Valor inválido na URL (ex.: `idade=abc` ou o formato antigo `idade=27`) é ignorado.

### RF02 — Ordenação dos resultados
- Select "Ordenar por" com as opções:
  - Mais recentes (padrão) — `data_cadastro DESC`
  - Nome (A–Z) — `nome ASC`
  - Mais experiência — `anos_experiencia DESC`
  - Menos experiência — `anos_experiencia ASC`
- Parâmetro na URL: `ordem=nome` (o padrão não aparece na URL).
- A ordenação é mantida ao trocar de página, ao abrir o painel de detalhes e ao mudar itens por página.
- O `ORDER BY` vem de uma lista fixa no C#, nunca do texto da URL (evita SQL injection).

### RF03 — Chips de filtros ativos
- Abaixo dos filtros aparece uma linha com um "chip" para cada filtro ligado. Ex.: `Busca: java ×`, `Área: Tecnologia ×`, `Idade: 26 a 30 anos ×`.
- Clicar no `×` remove **só aquele filtro**, mantendo os outros.
- Botão "Limpar todos" remove todos os filtros de uma vez (mantém a visão atual: dashboard, currículos ou favoritos).
- Junto aparece o total: "12 currículos encontrados".

### RF04 — Busca também por cidade
- A busca por texto passa a procurar também na cidade (ex.: "sao paulo" encontra "São Paulo").

## 4. Regras

- Toda a regra de filtragem fica no C# (`FiltroCurriculo` e `CurriculoRepository`), não no JavaScript.
- Os filtros ficam na URL (GET), então dá para copiar o link e mandar para outra pessoa com os mesmos filtros.
- Ao mudar qualquer filtro a paginação volta para a página 1.
- Consultas sempre com parâmetros (`@busca`, `@idadeMin`...), nunca concatenando o texto digitado.

## 5. Fora do escopo

- Filtros salvos por usuário.
- Busca com autocompletar enquanto digita.
- Filtro com várias áreas ao mesmo tempo (multi-seleção).

## 6. Critérios de aceite

1. Escolher "26 a 30 anos" mostra só candidatos com idade entre 26 e 30.
2. Escolher "Nome (A–Z)" ordena a lista em ordem alfabética; trocar de página mantém a ordem.
3. Com busca + área ligadas, aparecem dois chips; clicar no `×` da área mantém a busca.
4. "Limpar todos" volta para a lista sem filtros na mesma visão.
5. Buscar "curitiba" encontra os candidatos de Curitiba.
6. `?idade=abc` e `?ordem=xyz` na URL não quebram a tela: são ignorados.
