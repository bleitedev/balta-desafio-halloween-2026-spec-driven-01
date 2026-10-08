# Plano técnico

## Contexto
Implementação de uma API REST para geração e recuperação de senhas fortes conforme definido no documento de especificação (spec.md) e regido pela constituição do projeto (constitution.md). O sistema opera de forma autônoma e portátil, garantindo persistência estruturada em banco relacional, geração criptograficamente segura com regras estritas de complexidade e segregação completa entre transporte HTTP, orquestração de aplicação, lógica de domínio e persistência de dados.

## Arquitetura
A aplicação adota separação em camadas, orquestração de operações via CQRS manual (sem bibliotecas externas) e controle transacional via Unit of Work:

- Apresentação / Endpoints: Desenvolvidos com ASP.NET Core Minimal APIs. Recebem a requisição HTTP, validam o formato dos parâmetros primitivos (como o formato do GUID na rota) e repassam a execução diretamente ao respectivo Handler de CQRS. Não contêm lógica de negócio nem acessam o banco de dados.
- Aplicação (Application): Contém os contratos de entrada e saída, os Commands (GeneratePasswordCommand) e Queries (GetPasswordByIdQuery), e seus respectivos Handlers (GeneratePasswordCommandHandler e GetPasswordByIdQueryHandler). Coordena o domínio, repositórios e a finalização transacional.
- Domínio (Domain): Camada pura e isolada sem dependência de Entity Framework Core ou ASP.NET Core. Contém a entidade PasswordRecord, as regras do gerador de senhas (PasswordGeneratorService) e as interfaces de persistência e transação (IPasswordRepository e IUnitOfWork).
- Infraestrutura / Dados (Infrastructure.Data): Implementa o acesso ao banco com Entity Framework Core e SQLite. Abriga a classe AppDbContext, o mapeamento relacional das entidades, a implementação concreta de IPasswordRepository e o UnitOfWork que encapsula o salvamento atômico das alterações.

## Decisões
- Runtime .NET 10 e C#: Escolhido por ser o padrão determinado na constituição, oferecendo alto desempenho em tempo de execução, tipagem estrita e suporte maduro a Minimal APIs.
- ASP.NET Core Minimal APIs: Escolhido para manter a camada HTTP limpa, declarativa e direta, eliminando a sobrecarga de controllers clássicos e focando na integração direta com os Handlers de CQRS.
- Persistência com SQLite: Escolhido porque o requisito exige armazenamento persistente em banco relacional, mas o projeto é estritamente lúdico e educacional. O SQLite evita a necessidade de instalar, gerenciar e rodar containers ou serviços externos de banco (Postgres/SQL Server), mantendo o projeto 100% autocontido.
- Entity Framework Core (Microsoft.EntityFrameworkCore.Sqlite): Escolhido para simplificar o mapeamento objeto-relacional, gerenciar a criação do esquema SQLite e fornecer nativamente o padrão Unit of Work através do ciclo de vida do DbContext.
- CQRS Manual (Sem MediatR): A aplicação possui apenas um comando e uma consulta. Adicionar o pacote MediatR violaria a regra de governança da constituição de não introduzir dependências sem justificativa indispensável. Handlers declarados como classes e interfaces C# nativas atendem plenamente ao padrão com complexidade mínima.
- Padrão Unit of Work (IUnitOfWork): O repositório não chama SaveChangesAsync(). A persistência atômica é controlada explicitamente pelo handler via IUnitOfWork.CommitAsync(), assegurando integridade e permitindo transações limpas.
- Entropia com System.Security.Cryptography.RandomNumberGenerator: A geração de senhas utiliza gerador criptograficamente seguro do framework em vez de System.Random, garantindo alta entropia e ausência de previsibilidade matemática.
- Padrão de Erro RFC 7807 (ProblemDetails): Obrigatório pela constituição para padronizar respostas de erro (status 400 em caso de GUIDs com formatação inválida) sem necessidade de pacotes externos.
- Testes com xUnit e Microsoft.AspNetCore.Mvc.Testing: Utilizados para garantir a meta de 100% de cobertura unitária das regras de domínio e fornecer execução in-memory dos endpoints HTTP para os testes de integração exigidos na definição de pronto.

## Modelo de dados
A base SQLite contém uma única tabela denominada Passwords.

| Coluna | Tipo SQLite / EF | Modificadores | Descrição |
| :--- | :--- | :--- | :--- |
| Id | TEXT (System.Guid) | Chave Primária, Não Nulo | Identificador global universal e imutável do registro. |
| Value | TEXT (string) | Não Nulo, Máx 128 | Senha forte gerada em texto simples (sem espaços, com caracteres especiais, >= 16 chars). |
| CreatedAtUtc | TEXT (DateTimeOffset) | Não Nulo | Data e hora em UTC de quando o registro foi persistido. |

Mapeamento via Fluent API:
- builder.ToTable("Passwords");
- builder.HasKey(x => x.Id);
- builder.Property(x => x.Value).IsRequired().HasMaxLength(128);
- builder.Property(x => x.CreatedAtUtc).IsRequired();

## Contratos

### 1. Geração de Senha
- Rota: POST /api/passwords
- Fluxo: Endpoint dispara GeneratePasswordCommand -> processado por GeneratePasswordCommandHandler.
- Corpo da Requisição: Vazio.
- Status Sucesso: 201 Created
- Cabeçalho: Location: /api/passwords/{id}
- Exemplo de Resposta de Sucesso:
  - id: "7f8b9a23-4c12-4d56-8e78-90abcdef1234"
  - password: "aB3!kP9#mZ2$wQ8&"
  - createdAtUtc: "2026-10-08T03:11:54Z"

### 2. Consulta de Senha por GUID
- Rota: GET /api/passwords/{id}
- Fluxo: Endpoint extrai o GUID da rota e dispara GetPasswordByIdQuery(Guid Id) -> processado por GetPasswordByIdQueryHandler.
- Status Sucesso: 200 OK
- Exemplo de Resposta de Sucesso:
  - id: "7f8b9a23-4c12-4d56-8e78-90abcdef1234"
  - password: "aB3!kP9#mZ2$wQ8&"
  - createdAtUtc: "2026-10-08T03:11:54Z"
- Status Não Encontrado: 404 Not Found (quando o GUID é válido, mas não existe no banco).
- Status Erro de Validação: 400 Bad Request com ProblemDetails (quando o parâmetro id não é um GUID válido):
  - type: "https://tools.ietf.org/html/rfc7807"
  - title: "Bad Request"
  - status: 400
  - detail: "O identificador fornecido não é um GUID válido."

## Riscos
- Não atendimento aos requisitos por sorteio aleatório simples: Se caracteres forem sorteados puramente ao acaso em um charset global, há o risco de uma senha sair sem caracteres especiais. Mitigação: O algoritmo garante a inclusão mínima forçada de pelo menos 1 maiúscula, 1 minúscula, 1 dígito e 1 caractere especial antes de preencher o restante até 16 caracteres e realizar o embaralhamento criptográfico.
- Concorrência e bloqueio de arquivo no SQLite: Execuções paralelas da suíte de testes de integração podem travar o arquivo de banco em disco. Mitigação: Isolar as instâncias de teste utilizando SQLite em memória com conexão mantida aberta por fixture (DataSource=:memory:).
- Vazamento de regras de persistência para a camada de aplicação: Chamar SaveChanges de forma desordenada pode quebrar o fluxo do Unit of Work. Mitigação: Apenas a implementação de IUnitOfWork expõe o método de salvamento, e o repositório expõe apenas métodos de adicionar e buscar por ID.