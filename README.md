# 🛒 PStore

PStore é uma aplicação de **e-commerce distribuído** construída em **.NET 8** com microsserviços, comunicação assíncrona via **RabbitMQ** e persistência em **SQL Server**.  

Foi utilizado no projeto arquitetura moderna com serviços desacoplados, mensageria resiliente (Outbox Pattern) e infraestrutura orquestrada via **Docker Compose**.

---

## 🚀 Tecnologias

- **Backend**
  - ASP.NET Core 8 (Minimal APIs)
  - Clean Architecture em cada microsserviço
  - Entity Framework Core + SQL Server
  - RabbitMQ (mensageria)
  - Outbox Pattern (consistência assíncrona)

- **Infra**
  - Docker e Docker Compose
  - SQL Server
  - Migrations versionadas no Git

---

## 📦 Arquitetura

A aplicação é composta por **microsserviços independentes**, cada um responsável por uma parte do domínio:

- **Catalog.Api**  
  Gerencia os produtos do catálogo. Expõe endpoints para consulta por SKU e mantém os dados em banco SQL Server.

- **Orders.Api**  
  Responsável pela criação de pedidos. Consulta o catálogo para validar os SKUs e grava a ordem no banco. Utiliza o padrão **Outbox** para publicar eventos `OrderCreated`.

- **Payments.Worker**  
  Consome os eventos de criação de pedidos no RabbitMQ, processa pagamentos de forma simulada (90% sucesso, 10% falha) e publica eventos de confirmação ou rejeição (`payments.confirmed.v1` e `payments.rejected.v1`).

- **Orders.Worker**  
  Consome os eventos de pagamento e atualiza o status dos pedidos no banco (`Paid` ou `Rejected`).

- **RabbitMQ**  
  Faz a comunicação assíncrona entre os serviços através de exchanges e filas.

- **SQL Server**  
  Cada contexto (Catalog, Orders) tem seu próprio banco isolado, garantindo baixo acoplamento.

---

## 📂 Estrutura do Projeto

```
services/
  Catalog/
    PStore.Catalog.Api/
    PStore.Catalog.Domain/
    PStore.Catalog.Infrastructure/
  Orders/
    PStore.Orders.Api/
    PStore.Orders.Domain/
    PStore.Orders.Infrastructure/
    PStore.Orders.Worker/
  Payments/
    PStore.Payments.Worker/
docker-compose.yml
```

---

## 🐳 Rodando com Docker Compose

### 1. Build e start
```bash
docker compose up --build
```

### 2. Endpoints disponíveis
- Catalog API → [http://localhost:5001/api/products](http://localhost:5001/api/products)  
- Orders API → [http://localhost:5002/api/orders](http://localhost:5002/api/orders)  
- RabbitMQ → [http://localhost:15672](http://localhost:15672) (guest/guest)

---

## 🧪 Testando o fluxo

### 1. Criar pedido
```bash
curl -k --request POST "http://localhost:5002/api/orders"   --header "Content-Type: application/json"   --data '{
    "items": [
      { "sku": "SKU-001", "quantity": 2 },
      { "sku": "SKU-002", "quantity": 1 }
    ]
  }'
```

### 2. Ver status inicial
```bash
curl -k http://localhost:5002/api/orders/{id}
# status: Created
```

### 3. Worker de pagamentos processa e publica resultado  
### 4. Orders.Worker atualiza status  
```bash
curl -k http://localhost:5002/api/orders/{id}
# status: Paid (ou Rejected)
```

---

## 📜 Licença
MIT
