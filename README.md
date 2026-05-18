# Food Ordering System

A complete runnable implementation based on the provided architecture image:

- **Customer Web App**: HTML, CSS, JavaScript
- **Backend API**: Java 17 Spring Boot REST API
- **Database**: H2 for quick local testing, MySQL profile for production-style use
- **Cashier/Admin Desktop App**: C# Windows Forms (.NET 8, Windows)

## Quick start: API + Web App

### Requirements
- Java 17+
- Maven 3.9+
- A modern browser

### Run the backend
```bash
cd api
mvn spring-boot:run
```

The API starts at:

```text
http://localhost:8080
```

The customer web app is served directly by Spring Boot at:

```text
http://localhost:8080
```

H2 database console:

```text
http://localhost:8080/h2-console
JDBC URL: jdbc:h2:mem:fooddb
User: sa
Password: password
```

## Run with MySQL using Docker

### Requirements
- Docker
- Java 17+
- Maven 3.9+

Start MySQL:

```bash
docker compose up -d mysql
```

Run API using the MySQL profile:

```bash
cd api
mvn spring-boot:run -Dspring-boot.run.profiles=mysql
```

MySQL connection used by the app:

```text
Host: localhost
Port: 3306
Database: food_ordering
User: food_user
Password: food_password
```

## Run the C# Windows Forms Cashier/Admin App

### Requirements
- Windows
- .NET 8 SDK
- Backend API running at `http://localhost:8080`

```powershell
cd cashier-desktop
 dotnet run
```

The desktop app lets the cashier/admin:

- View current orders
- Select an order
- See order/payment/receipt details
- Update order status
- Print a text receipt file

## API Endpoints

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/menu` | Get active menu items |
| POST | `/api/orders` | Place customer order |
| GET | `/api/orders` | List orders for cashier/admin |
| GET | `/api/orders/{id}` | Get one order |
| PATCH | `/api/orders/{id}/status` | Update order status |
| GET | `/api/receipts/{orderId}` | Get receipt by order ID |
| GET | `/api/health` | Health check |

## Default sample menu

The API automatically seeds:

- Margherita Pizza
- Beef Burger
- Carbonara Pasta
- Chicken Wings
- Iced Coffee
- French Fries

## Project structure

```text
food-ordering-system/
├── api/                    Java Spring Boot backend + hosted web static files
├── cashier-desktop/        C# WinForms cashier/admin application
├── database/               MySQL schema and seed SQL
├── web-standalone/         Standalone web copy for direct editing/testing
├── docker-compose.yml      MySQL service
└── README.md
```

## Notes

- For easiest testing, use the default H2 in-memory database.
- For a real deployment, use the MySQL profile and externalize passwords via environment variables.
- The C# app is source-ready and runs on Windows with .NET 8.
