# Product Catalog App - Full Stack

A complete e-commerce application built with multiple frameworks.

## Architecture

- **Frontend (React)**: `frontend-react/` - Shopping cart, product filtering, checkout
- **Frontend (Angular)**: `frontend-angular/` - Alternative UI for the same app
- **Backend API**: `backend-api/` - C# / .NET Core REST API
- **Database**: SQL Server - Product and order persistence

## Quick Start

### Backend (API)
```bash
cd backend-api
dotnet run
# Runs on http://localhost:5135
# Swagger docs: http://localhost:5135/swagger/index.html
```

### Frontend (React)
```bash
cd frontend-react
npm install
npm run dev
# Runs on http://localhost:5173
```

### Frontend (Angular)
```bash
cd frontend-angular
npm install
ng serve
# Runs on http://localhost:4200
```

## Features

- Product catalog with search and filtering
- Shopping cart system
- Order management
- REST API with validation
- SQL Server persistence
- Swagger API documentation

## Tech Stack

- **Frontend**: React/Angular with TypeScript
- **Backend**: C# / .NET Core
- **Database**: SQL Server
- **API Docs**: Swagger/OpenAPI