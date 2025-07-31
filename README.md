# Venice Orders - Sistema de Gerenciamento de Pedidos

## Visão Geral

O Venice Orders é um microserviço desenvolvido em .NET 8 para gerenciar pedidos de forma eficiente e escalável. O sistema utiliza uma arquitetura híbrida com múltiplos bancos de dados e comunicação assíncrona através de filas.

## Arquitetura

### Clean Architecture

O projeto foi desenvolvido seguindo os princípios da **Clean Architecture** (Arquitetura Limpa), que organiza o código em camadas bem definidas com dependências unidirecionais:

```
┌─────────────────┐
│   API Layer    │ ← Controllers, Middleware, Configuration
├─────────────────┤
│Application Layer│ ← Services, DTOs, Interfaces
├─────────────────┤
│   Domain Layer  │ ← Entities, Value Objects, Business Rules
├─────────────────┤
│Infrastructure   │ ← Repositories, External Services, Data Access
└─────────────────┘
```

### Justificativa da Escolha Arquitetural

**Clean Architecture** foi escolhida pelos seguintes motivos:

1. **Separação de Responsabilidades**: Cada camada tem uma responsabilidade específica
2. **Testabilidade**: Facilita a criação de testes unitários ao isolar a lógica de negócio
3. **Flexibilidade**: Permite trocar implementações de infraestrutura sem afetar o domínio
4. **Manutenibilidade**: Código mais organizando e fácil de manter
5. **Independência de Frameworks**: O domínio não depende de tecnologias específicas

### Design Pattern: Repository Pattern

Foi implementado o **Repository Pattern** para:

- Abstrair o acesso aos dados
- Facilitar os testes unitários através de mocks
- Permitir diferentes implementações de persistência
- Manter a lógica de negócio independente da camada de dados

## Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM para SQL Server
- **MongoDB Driver**: Acesso ao MongoDB
- **Redis**: Cache distribuído
- **RabbitMQ**: Message broker para comunicação assíncrona
- **xUnit + Moq**: Framework de testes
- **Docker**: Containerização

## Estrutura do Projeto

```
VeniceOrders/
├── src/
│   ├── VeniceOrders.Domain/          # Regras de negócio e entidades
│   │   ├── Entities/                 # Order
│   │   ├── ValueObjects/             # OrderItem, OrderStatus
│   │   ├── Repositories/             # Interfaces dos repositórios
│   │   └── Events/                   # PedidoCriadoEvent
│   ├── VeniceOrders.Application/     # Casos de uso e serviços
│   │   ├── Services/                 # OrderService
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   └── Interfaces/               # Contratos de serviços
│   ├── VeniceOrders.Infrastructure/  # Implementações de infraestrutura
│   │   ├── Data/                     # Contextos de dados
│   │   ├── Repositories/             # Implementações dos repositórios
│   │   ├── Cache/                    # Implementação do Redis
│   │   └── Messaging/                # Implementação do RabbitMQ
│   └── VeniceOrders.API/             # Camada de apresentação
│       ├── Controllers/              # OrdersController
│       └── Configuration/            # Configurações e DI
└── tests/
    └── VeniceOrders.Tests/           # Testes unitários
```

## Funcionalidades

### 1. Criação de Pedidos (POST /api/orders)

**Endpoint**: `POST /api/orders`

**Request Body**:
```json
{
  "clienteId": "550e8400-e29b-41d4-a716-446655440000",
  "itens": [
    {
      "produto": "Produto A",
      "quantidade": 2,
      "precoUnitario": 25.50
    },
    {
      "produto": "Produto B",
      "quantidade": 1,
      "precoUnitario": 15.00
    }
  ]
}
```

**Response**:
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "clienteId": "550e8400-e29b-41d4-a716-446655440000",
  "data": "2024-01-15T10:30:00Z",
  "status": "Criado",
  "valorTotal": 66.00,
  "itens": [
    {
      "produto": "Produto A",
      "quantidade": 2,
      "precoUnitario": 25.50
    },
    {
      "produto": "Produto B",
      "quantidade": 1,
      "precoUnitario": 15.00
    }
  ]
}
```

### 2. Consulta de Pedidos (GET /api/orders/{id})

**Endpoint**: `GET /api/orders/{id}`

**Response**: Mesmo formato do POST, com dados recuperados dos bancos integrados.

### 3. Armazenamento Híbrido

- **SQL Server**: Dados principais do pedido (ID, ClienteID, Data, Status, ValorTotal)
- **MongoDB**: Lista de itens do pedido (Produto, Quantidade, PrecoUnitario)

### 4. Cache Redis

- Cache de 2 minutos para consultas GET
- Chave no formato: `order:{id}`
- Reduz latência e carga nos bancos de dados

### 5. Mensageria Assíncrona

Após a criação do pedido, um evento `PedidoCriadoEvent` é publicado no RabbitMQ:

```json
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "clienteId": "550e8400-e29b-41d4-a716-446655440000",
  "valorTotal": 66.00,
  "dataCriacao": "2024-01-15T10:30:00Z",
  "itens": [...]
}
```

## Como Executar

### Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose

### Opção 1: Com Docker Compose (Recomendado)

```bash
# Clonar o repositório
git clone [repository-url]
cd VeniceOrders

```

### Acessos aos Serviços

- **API**: http://localhost:8080 (Docker) ou https://localhost:7239 (Local)
- **Swagger**: /swagger
- **SQL Server**: localhost:1433 (sa/YourPassword123!)
- **MongoDB**: localhost:27017 (admin/password123)
- **Redis**: localhost:6379
- **RabbitMQ Management**: http://localhost:15672 (admin/password123)

## Testes

### Executar Testes Unitários

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Testes Implementados

1. **OrderServiceTests**:
   - Criação de pedidos válidos
   - Consulta com cache hit
   - Consulta com cache miss

2. **OrderTests**:
   - Validação de regras de negócio
   - Cálculo de valor total
   - Validação de itens


### Consultar um Pedido

```bash
curl -X GET "http://localhost:8080/api/orders/123e4567-e89b-12d3-a456-426614174000"
```

## Princípios SOLID Aplicados

1. **Single Responsibility**: Cada classe tem uma única responsabilidade
2. **Open/Closed**: Extensível para novos recursos sem modificar código existente
3. **Liskov Substitution**: Implementações podem ser substituídas transparentemente
4. **Interface Segregation**: Interfaces específicas e coesas
5. **Dependency Inversion**: Dependências de abstrações, não de implementações concretas

## Padrões DDD Aplicados

### Entidades
- **Order**: Entidade raiz do agregado com identidade própria
- Comportamentos encapsulados (cálculo de valor total, mudança de status)

### Value Objects
- **OrderItem**: Objeto de valor imutável representando um item do pedido
- **OrderStatus**: Enum representando os possíveis status do pedido

### Agregados
- **Order + OrderItems**: Agregado que mantém consistência transacional

### Domain Events
- **PedidoCriadoEvent**: Evento de domínio disparado na criação do pedido

### Repositories
- Abstrações para persistência seguindo os padrões DDD
- Implementações específicas para cada tipo de armazenamento

## Monitoramento e Observabilidade

### Logs Estruturados
A aplicação utiliza o sistema de logging nativo do .NET com suporte a logs estruturados:

```csharp
// Exemplo de uso nos serviços
_logger.LogInformation("Pedido {PedidoId} criado para cliente {ClienteId}", 
    order.Id, order.ClienteId);
```
