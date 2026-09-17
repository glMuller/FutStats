# ⚽ FutStats API

API RESTful desenvolvida em ASP.NET Core (.NET 8) para gerenciamento de times e jogadores de futebol, aplicando Clean Architecture, Repository Pattern, paginação, otimização de performance e testes automatizados.

Projeto desenvolvido para o Checkpoint 4 — Advanced Business Development with .NET (FIAP, 2026).

## 👥 Integrantes

| Nome | RM |
|---|---|
| [Nome completo] | RM561995 |
| [Nome completo] | RM |
| [Nome completo] | RM |

## 📖 Descrição do projeto

A FutStats API permite cadastrar, consultar, atualizar e remover **Times** e **Jogadores**, com um relacionamento 1:N entre eles (um time possui vários jogadores). O objetivo do projeto é demonstrar, na prática, boas práticas de arquitetura e engenharia de software no ecossistema .NET:

- Separação de responsabilidades em camadas independentes
- Persistência de dados via Entity Framework Core, com banco Oracle
- Otimização de respostas (paginação, compressão)
- Proteção contra abuso de requisições (Rate Limiting)
- Cobertura de testes automatizados (unidade e integração)
- Logging estruturado dos principais fluxos e erros

## 🏗️ Arquitetura e componentes utilizados

O projeto segue os princípios de **Clean Architecture**, dividido em 4 camadas independentes, mais 2 projetos de testes:

```
FutStatsAPI (Solution)
 ├── FutStats.Application       → DTOs, Services (regras de negócio), Mapeamentos (AutoMapper)
 ├── FutStatsAPI.Domain         → Entidades, Interfaces de Repositório, regras puras de domínio
 ├── FutStatsAPI.Infrastructure → EF Core, DbContext, Repositórios, Configurações de mapeamento de tabelas
 ├── FutStats API                → Controllers, Program.cs, Swagger, Rate Limiting, Compressão
 ├── FutStatsAPI.Tests.Unit         → Testes de unidade dos Services (xUnit + Moq)
 └── FutStatsAPI.Tests.Integration  → Testes de integração dos endpoints (xUnit + WebApplicationFactory)
```

**Regra de dependência entre camadas:**

```
API → Application → Domain
         ↑
  Infrastructure
```

O `Domain` não possui nenhuma dependência externa (nem de EF Core, nem de ASP.NET Core) — é código puro de regras de negócio e contratos (interfaces). A `Infrastructure` implementa essas interfaces usando Entity Framework Core; a `API` conecta tudo na inicialização via Injeção de Dependência.

### Principais tecnologias e pacotes

| Tecnologia | Uso |
|---|---|
| .NET 8 / ASP.NET Core | Framework base da API |
| Entity Framework Core 8 | ORM de acesso a dados |
| Oracle.EntityFrameworkCore | Provider de conexão com banco Oracle |
| AutoMapper | Mapeamento entre Entidades e DTOs |
| Swashbuckle.AspNetCore (+ Annotations) | Documentação Swagger/OpenAPI |
| Moq | Mock de dependências nos testes de unidade |
| xUnit | Framework de testes |
| Microsoft.AspNetCore.Mvc.Testing | Testes de integração com API em memória |
| Microsoft.EntityFrameworkCore.InMemory | Banco em memória usado exclusivamente nos testes de integração |

## 🚀 Como rodar o projeto

### Pré-requisitos

- .NET 8 SDK instalado
- Acesso ao banco Oracle da FIAP (usuário e senha)
- Ferramenta `dotnet-ef` instalada globalmente:
```bash
dotnet tool install --global dotnet-ef
```

### 1. Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd FutStatsAPI
```

### 2. Configurar a connection string (User Secrets)

A connection string do Oracle **não fica no `appsettings.json`** por questões de segurança (o repositório é público). Configure via User Secrets, dentro da pasta do projeto da API:

```bash
cd "FutStats API"
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=SEU_RM;Password=SUA_SENHA;"
```

### 3. Aplicar as migrations (criar as tabelas no Oracle)

Na raiz da solution:

```bash
dotnet ef database update --project FutStatsAPI.Infrastructure --startup-project "FutStats API"
```

### 4. Rodar a API

```bash
dotnet run --project "FutStats API"
```

A API sobe por padrão em `https://localhost:{porta}` (a porta exata aparece no console ao iniciar). O Swagger fica disponível em:

```
https://localhost:{porta}/swagger
```

### 5. Executar os testes automatizados

Na raiz da solution:

```bash
dotnet test
```

Isso executa tanto os testes de unidade (`FutStatsAPI.Tests.Unit`) quanto os de integração (`FutStatsAPI.Tests.Integration`, que sobem a API inteira em memória, sem depender do Oracle real).

## 📋 Funcionalidades implementadas

- ✅ Organização em camadas (Domain, Application, Infrastructure, API)
- ✅ Repository Pattern com interfaces genéricas e específicas
- ✅ DTOs e mapeamentos com AutoMapper (entidades nunca são expostas diretamente pela API)
- ✅ Paginação de resultados (`pageNumber`, `pageSize`) nos endpoints de listagem
- ✅ Índices de banco de dados: índice único em `Time.Nome` e índice em `Jogador.TimeId`
- ✅ Compressão de resposta (Gzip/Brotli)
- ✅ Rate Limiting (10 requisições a cada 10 segundos por IP, retornando `429 Too Many Requests` quando excedido)
- ✅ Documentação Swagger com Annotations (`SwaggerOperation`, `ProducesResponseType`)
- ✅ Tratamento centralizado de exceções via Middleware, convertendo erros de negócio em respostas HTTP apropriadas (`400`, `404`, `500`)
- ✅ Logging estruturado (`ILogger`) nos principais fluxos de criação, atualização, remoção e erros
- ✅ Testes de unidade cobrindo as regras de negócio dos Services
- ✅ Testes de integração cobrindo o ciclo completo dos endpoints da API

## 🔌 Endpoints disponíveis

### Times

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/times?pageNumber=1&pageSize=10` | Lista times de forma paginada |
| GET | `/api/times/{id}` | Busca um time por Id, incluindo seus jogadores |
| POST | `/api/times` | Cadastra um novo time |
| PUT | `/api/times/{id}` | Atualiza um time existente |
| DELETE | `/api/times/{id}` | Remove um time |

**Exemplo de requisição — `POST /api/times`:**
```json
{
  "nome": "Corinthians",
  "cidade": "São Paulo",
  "estado": "SP",
  "anoFundacao": 1910
}
```

### Jogadores

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/jogadores/time/{timeId}?pageNumber=1&pageSize=10` | Lista jogadores de um time, paginado |
| GET | `/api/jogadores/{id}` | Busca um jogador por Id |
| POST | `/api/jogadores` | Cadastra um novo jogador vinculado a um time |
| PUT | `/api/jogadores/{id}` | Atualiza um jogador existente |
| DELETE | `/api/jogadores/{id}` | Remove um jogador |

**Exemplo de requisição — `POST /api/jogadores`:**
```json
{
  "nome": "Vinicius Junior",
  "numero": 7,
  "posicao": "Atacante",
  "dataNascimento": "2000-07-12",
  "timeId": 1
}
```

Posições válidas: `Goleiro`, `Zagueiro`, `LateralDireito`, `LateralEsquerdo`, `Volante`, `Meia`, `Atacante`.

## ⚠️ Observações e melhorias futuras

Por restrição de tempo do checkpoint, os seguintes itens do escopo original não foram implementados nesta entrega e ficam como evolução natural do projeto:

- Health Check (`/health`) para monitoramento da API e da conexão com o Oracle
- Integração com Application Insights para tracing e métricas

## 📄 Licença

Projeto acadêmico desenvolvido para fins educacionais — FIAP, Checkpoint 4, 2026.