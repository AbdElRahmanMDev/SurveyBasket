# 📊 Survey Basket API

**Survey Basket** is a full-featured RESTful API for managing online surveys. Built with best practices in mind, this project enables authenticated users to create and manage polls, define questions, collect answers, and view aggregated results. The system includes user role management, authentication, auditing, background jobs, and more.

---

## 🔧 Tech Stack

- **.NET Core / ASP.NET**
- **Entity Framework Core**
- **JWT Authentication & Refresh Tokens**
- **Swagger / OpenAPI**
- **SQL Server / PostgreSQL**
- **Mapster**
- **Hangfire** (for background jobs)
- **Serilog** (for logging)


---

## 🧠 Features

### 🗳 Poll Management
- Create, update, delete, and list polls
- Send notifications to participants when new polls are available

### ❓ Questions Module
- Attach multiple questions to a poll
- Full CRUD (Create, Read, Update, Delete) operations

### 📝 Answer Collection
- Submit answers to specific questions
- Supports listing and updating responses

### ✅ Participation Tracking
- Register user participation in available polls

### 📈 Results Aggregation
- Aggregate and display poll results
- Filterable by poll, date, and user

### 👥 User Management
- Admins can add, update, lock/unlock users
- Manage roles and permissions
- Reset passwords

### 🔐 Authentication & Authorization
- User registration & login
- Account confirmation
- JWT-based authentication
- Refresh tokens for secure session management

### 🧾 Account Management
- Update profile
- Change password
- Password reset flow

### ⚙ Admin Features
- Role-based access control (RBAC)
- User activity logging
- Background jobs (e.g., send notifications)
- Health checks and diagnostics

### 🚀 Performance & Scalability
- Pagination, filtering, sorting on all endpoints
- Caching with Redis
- Background processing with Hangfire
- Rate limiting to prevent abuse

---

## 🗂 Modules Overview (Based on Architecture Diagram)

```text
Survey Basket
│
├── Polls
│   ├── List / Add / Update / Delete
│   └── Send Notifications
│
├── Questions
│   ├── List / Add / Update / Delete
│
├── Answers
│   ├── List / Add / Update / Delete
│
├── Participates
│   └── Add Participation
│
├── Results
│   └── View Aggregated Results
│
├── Users
│   ├── Add / Update
│   ├── Manage Roles
│   ├── Lock/Unlock / Reset Password
│
└── Account Management
    ├── Register / Confirm / Login
    ├── Update Profile / Change Password
