# Invoice Management System

A multi-tenant invoice management system built with ASP.NET Core Web API and Blazor Server, featuring JWT authentication and company-specific data isolation.

## 🚀 Features

- **Multi-Tenant Architecture**: Complete data isolation per company
- **JWT Authentication**: Secure token-based login system
- **Role-Based Access**: Admin and User roles
- **Invoice Management**: Create, edit, and track invoices with line items
- **Client Management**: Manage customer information per company
- **Dashboard Analytics**: Visual charts for revenue and invoice status
- **Auto-Generated Invoice Numbers**: Sequential numbering (INV-2024-001)

## 🛠️ Tech Stack

**Backend**
- ASP.NET Core 8.0 Web API
- Entity Framework Core
- SQL Server
- FluentValidation
- JWT Bearer Authentication
- BCrypt.Net

**Frontend**
- Blazor Server
- MudBlazor
- Blazored.LocalStorage

**Architecture**
- Service Layer Pattern
- Dependency Injection
- Claims-Based Authorization

## 🔐 Security

- **Password Hashing**: BCrypt for secure storage
- **JWT Tokens**: Company ID and role embedded in claims
- **Tenant Isolation**: Automatic data filtering by company
- **Protected Endpoints**: Role-based authorization

## 📊 Key Features

### Multi-Tenancy
Each company's data is completely isolated. The Company ID from the JWT token automatically filters all queries, ensuring users only see their company's data.

### Invoice Management
- Line items with quantity and pricing
- Tax and delivery calculations
- Multiple status tracking (Paid, Unpaid, Pending, Overdue)
- Client association

### Dashboard
- Monthly revenue charts
- Status distribution
- Real-time analytics

## 👨‍💻 Author

**Your Name**
- GitHub: [@aisyah388](https://github.com/aisyah388)

---

⭐ Star this repo if you find it useful!
