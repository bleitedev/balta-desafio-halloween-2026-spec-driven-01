# Constituição do projeto

## Stack
- **Linguagem e Runtime:** C# com .NET 10.
- **Web Framework:** ASP.NET Core Minimal APIs.
- **Acesso a Dados:** Entity Framework Core com provedor SQLite.
- **Testes:** xUnit com `Microsoft.AspNetCore.Mvc.Testing` para testes de integração.

## Arquitetura
- **Separação em Camadas (Domínio, Repositório e Serviço):**
  - **Domínio:** Contém entidades, objetos de valor e regras puras de negócio, sem dependência de banco de dados ou frameworks externos.
  - **Repositório:** Responsável exclusivamente pela persistência e leitura das entidades via interfaces dedicadas. Endpoints e serviços não executam consultas diretas ao `DbContext`.
  - **Serviço:** Orquestra casos de uso, validações e interações com repositórios e domínio.
  - *Justificativa:* Garante alto desacoplamento, responsabilidade única por camada e facilita a testabilidade unitária sem necessidade de infraestrutura ativa.
- **CQRS (Command Query Responsibility Segregation):**
  - Todas as mutações de dados devem ser executadas via **Commands** (`CommandHandler`), e todas as consultas via **Queries** (`QueryHandler`).
  - Minimal APIs apenas recebem requisições HTTP, mapeiam para o respectivo Command/Query e acionam o Handler correspondente.
  - *Justificativa:* Isola a lógica de escrita da de leitura, simplifica fluxos de orquestração e permite evoluir ou otimizar consultas e mutações de forma independente.
- **Unit of Work (UoW):**
  - Métodos de repositório não invocam `SaveChangesAsync()` individualmente.
  - A confirmação das transações é realizada de forma atômica via `IUnitOfWork.CommitAsync()` ao fim do processamento do Command.
  - *Justificativa:* Centraliza o ciclo de vida transacional e garante integridade (atomicidade) nas operações de escrita, impedindo estados inconsistentes no banco em caso de erro.

## Qualidade
- **Cobertura de Regras:** 100% dos geradores de regras de senha, Commands e Handlers devem ser cobertos por testes unitários via xUnit (com mocks/fakes de repositório e UoW).
- **Testes de Integração:** Todo endpoint exposto precisa de ao menos um teste de integração de caminho feliz (`201 Created` para criação e `200 OK` para consulta) e validação de `404 Not Found` para identificadores não encontrados.
- **Tratamento de Erros:** Erros de validação e requisições inválidas devem retornar status HTTP 400 serializado no formato RFC 7807 (`ProblemDetails`).
- **Definição de Pronto (DoD):** Compilação sem warnings (`TreatWarningsAsErrors = true`) e 100% dos testes automatizados passando com sucesso.

## Convenções
- **Nomenclatura:** PascalCase para tipos, classes, propriedades e métodos; camelCase para parâmetros e variáveis locais.
- **Sufixos de CQRS:** Commands nomeados com sufixo `Command`, Handlers com `CommandHandler` (o mesmo padrão aplica-se a `Query` e `QueryHandler`).
- **Identificadores:** Todo identificador de recurso consultável exposto externamente utiliza o tipo `System.Guid`.
- **Regras da Senha:** Comprimento mínimo de 16 caracteres, presença obrigatória de caracteres especiais e ausência de espaços em branco.

## Governança
- **Inclusão de Dependências:** Nenhum pacote NuGet ou biblioteca externa pode ser adicionado sem justificativa técnica explicitamente descrita e aprovada no plano (`plan.md`).
- **Respeito aos Limites Arquiteturais:** Nenhuma implementação pode violar o isolamento de camadas (ex.: invocar repositório ou banco diretamente dentro de endpoints).