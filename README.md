# 🍽️ Restaurant Management System API
Built with **ASP.NET Core 8** + **SQLite** + **Entity Framework Core**

---

## 🚀 How to Run

### Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Steps
```bash
# 1. Clone or extract the project
cd RestaurantAPI

# 2. Restore packages
dotnet restore

# 3. Run the app
dotnet run

# 4. Open Swagger UI
# http://localhost:5000/swagger
```

---

## 📁 Project Structure

```
RestaurantAPI/
├── Controllers/
│   ├── MenuController.cs         # Menu CRUD
│   ├── OrdersController.cs       # Place & manage orders
│   ├── TablesController.cs       # Table availability
│   ├── ReservationsController.cs # Reserve tables
│   ├── InventoryController.cs    # Stock management
│   └── ReportsController.cs      # Sales & analytics
├── Models/
│   ├── MenuItem.cs
│   ├── Order.cs                  # Order + OrderItem
│   ├── RestaurantTable.cs
│   ├── Reservation.cs
│   └── InventoryItem.cs
├── Data/
│   └── AppDbContext.cs           # EF Core DbContext + Seed Data
├── DTOs/
│   └── Dtos.cs                   # All request/response models
└── Program.cs
```

---

## 📡 API Endpoints

### 🍕 Menu `/api/menu`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/menu` | Get all items (optional: `?category=Main`) |
| GET | `/api/menu/{id}` | Get item by ID |
| POST | `/api/menu` | Add new menu item |
| PUT | `/api/menu/{id}` | Update menu item |
| DELETE | `/api/menu/{id}` | Delete menu item |

### 🪑 Tables `/api/tables`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tables` | Get all tables (optional: `?available=true`) |
| PATCH | `/api/tables/{id}/availability` | Set table availability |

### 🛒 Orders `/api/orders`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders (optional: `?status=Pending`) |
| GET | `/api/orders/{id}` | Get order details |
| POST | `/api/orders` | Place a new order |
| PATCH | `/api/orders/{id}/status` | Update order status |

**Order Status values:** `Pending` → `Preparing` → `Ready` → `Served` / `Cancelled`

### 📅 Reservations `/api/reservations`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reservations` | Get all reservations |
| POST | `/api/reservations` | Create reservation |
| DELETE | `/api/reservations/{id}` | Cancel reservation |

### 📦 Inventory `/api/inventory`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/inventory` | Get all items (optional: `?lowStock=true`) |
| POST | `/api/inventory` | Add inventory item |
| PATCH | `/api/inventory/{id}` | Update quantity (auto-alert if low) |
| GET | `/api/inventory/alerts` | Get all low-stock items |

### 📊 Reports `/api/reports`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reports/daily-sales` | Daily sales (optional: `?date=2024-01-15`) |
| GET | `/api/reports/popular-items` | Top 5 most ordered items |

---

## 📝 Example Requests

### Place an Order
```json
POST /api/orders
{
  "tableId": 1,
  "notes": "No onions please",
  "items": [
    { "menuItemId": 2, "quantity": 2 },
    { "menuItemId": 4, "quantity": 1 }
  ]
}
```

### Make a Reservation
```json
POST /api/reservations
{
  "tableId": 2,
  "customerName": "Ahmed Ali",
  "customerPhone": "01234567890",
  "reservationTime": "2024-12-25T19:00:00",
  "guestCount": 3
}
```

### Update Inventory
```json
PATCH /api/inventory/1
{
  "quantity": 3
}
// Response will include ⚠️ low stock alert if below threshold
```
