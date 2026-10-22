# Plano de implementação — Feature "Filtros e busca"

> Planejamento gerado com IA (Claude Code) antes de escrever o código, seguindo o `workflow.md` e as `rules.md` do professor.
> Requisitos completos em [`spec/filtros-e-busca.md`](../spec/filtros-e-busca.md).

## Etapa 1 — Entender

Melhorar a busca e os filtros da tela de currículos: faixa etária, ordenação, chips de filtros ativos e busca por cidade. A base (busca por texto e filtros de área/formação/experiência/cidade/status) já existe e será reaproveitada.

## Etapa 2 — Planejar (quais arquivos serão alterados)

| Camada | Arquivo | O que muda |
|--------|---------|------------|
| Model | `Models/FiltroCurriculo.cs` | `Idade` vira `string` (valor da faixa); nova propriedade `Ordem` com as opções; método `FiltrosAtivos()` para os chips; `ParaRota()` passa a aceitar um filtro para remover (`sem:`) |
| Model | `Models/FaixaIdade.cs` | Nenhuma mudança: passa a ser usada no filtro |
| Repository | `Repositories/CurriculoRepository.cs` | `MontarWhere`: idade por faixa (`BETWEEN`) e cidade na busca; `Buscar`: `ORDER BY` escolhido por lista fixa |
| Controller | `Controllers/CurriculoController.cs` | Remove o tratamento de erro de conversão da idade (agora é texto, não falha) |
| View | `Views/Curriculo/Index.cshtml` | Select de faixa etária em "Mais filtros"; barra com chips, total e "Ordenar por" |
| CSS | `wwwroot/css/app.css` | Estilos da barra de resultados e dos chips |
| JS | `wwwroot/js/script.js` | Nenhuma mudança: o select de ordem usa o `data-auto-submit` que já existe |

Não precisa de Model novo, nem tabela nova no banco, nem Controller novo.

## Etapa 3 — Implementar em pequenas partes

1. **Model — faixa etária**
   - Trocar `public int? Idade` por `public string? Idade`.
   - Em `Normalizar()`: se `FaixaIdade.ObterPorValor(Idade) == null`, limpar `Idade`.
2. **Model — ordenação**
   - Constantes `OrdemRecentes`, `OrdemNome`, `OrdemMaisExperiencia`, `OrdemMenosExperiencia` e a lista `OpcoesOrdem` (valor + rótulo).
   - Propriedade `Ordem` com padrão `recentes`; `Normalizar()` volta para o padrão se vier um valor desconhecido.
   - `ParaRota()` inclui `ordem` quando não for o padrão.
3. **Model — chips**
   - Record `FiltroAtivo(Chave, Rotulo, Valor)`.
   - `FiltrosAtivos()` devolve a lista dos filtros ligados com rótulos amigáveis (ex.: `26-30` → "26 a 30 anos").
   - `ParaRota(sem: "area")` monta a URL atual sem aquele filtro (link do `×`).
4. **Repository**
   - Idade: `c.idade BETWEEN @idadeMin AND @idadeMax` usando a faixa.
   - Busca: acrescentar `OR sem_acento(c.cidade) LIKE @busca`.
   - Ordem: `switch` em C# que devolve um `ORDER BY` fixo; sempre desempatando por `c.id DESC`.
5. **Controller**
   - Tirar a linha que tratava erro de conversão de `Idade`.
6. **View**
   - Em "Mais filtros", trocar o `<input type="number">` por `<select name="idade">` com as faixas.
   - Nova barra entre os filtros e a tabela: total encontrado, chips com link `×`, "Limpar todos" e `<select name="ordem" form="filtroCurriculos" data-auto-submit>`.
   - Campo hidden de `ordem` no formulário de "Itens por página" já vem do `ParaRota()`.
7. **CSS**
   - `.resultados-barra`, `.chips`, `.chip`, `.chip-remover`, `.ordenacao` seguindo as cores (`--roxo-*`) do projeto; quebra de linha no celular.

## Etapa 4 — Revisar

- Conferir que nenhum texto da URL vai direto para o SQL (só parâmetros e `switch` fixo).
- Conferir que os links de paginação, "Ver mais" e fechar painel mantêm `ordem` e `idade`.
- Nomes em português, no mesmo padrão do projeto.

## Etapa 5 — Testar

| # | Teste | Resultado esperado |
|---|-------|--------------------|
| 1 | `dotnet build` | Compila sem erros |
| 2 | Mais filtros → Idade "26 a 30 anos" → Aplicar | Só idades entre 26 e 30; chip "Idade: 26 a 30 anos" |
| 3 | Ordenar por "Nome (A–Z)" | Lista em ordem alfabética |
| 4 | Com ordem por nome, ir para a página 2 | Continua ordenado por nome |
| 5 | Buscar "java" + Área → clicar no `×` da área | Mantém a busca "java" |
| 6 | "Limpar todos" | Lista sem filtros, mesma visão |
| 7 | Buscar "curitiba" | Aparecem candidatos de Curitiba |
| 8 | URL `?idade=abc&ordem=xyz` | Tela abre normal, sem filtro de idade, ordem padrão |
| 9 | Tela no celular (375px) | Chips e ordenação quebram linha sem rolagem horizontal |

### Resultado dos testes (22/10/2026)

| # | Resultado |
|---|-----------|
| 1 | OK — 0 erros, 0 avisos |
| 2 | OK — 5 candidatos, idades 26, 27, 28, 28 e 30 |
| 3 | OK — Amanda, Gustavo, Juliana, Lucas, Rafael |
| 4 | OK — link da página 2 leva `ordem=nome` |
| 5 | OK — o `×` de cada chip remove só aquele parâmetro da URL |
| 6 | OK — volta para `?visao=curriculos` mantendo a ordem escolhida |
| 7 | OK — "1 currículo encontrado" (Rafael Silva, Curitiba, PR) |
| 8 | OK — 12 currículos, ordem padrão; o formato antigo `idade=27` também é ignorado |
| 9 | OK — largura da página = 375px, chips quebram linha |

## Etapa 6 — Melhorar

- Atualizar o README (seção de funcionalidades).
- Ideias futuras: filtros salvos, multi-seleção de áreas, autocompletar na busca.
