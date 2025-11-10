# GestionEmployes - Employee Management System

## 📖 Project Description

**GestionEmployes** is a comprehensive Windows desktop application built in C# .NET Framework 4.8 for managing employees, suppliers, invoices, and financial transactions. The system provides a complete solution for small to medium businesses to track employee data, manage supplier relationships, handle invoice processing, and generate detailed reports.

## 🎯 Project Goals

- **Employee Management**: Complete CRUD operations for employee records with salary tracking
- **Financial Tracking**: Monitor employee advances, absences, and salary deductions
- **Supplier Management**: Maintain supplier database with contact information and status tracking
- **Invoice Processing**: Handle supplier invoices with payment tracking and status management
- **Transaction Management**: Record and track all financial transactions between employees, suppliers, and invoices
- **Reporting System**: Generate comprehensive weekly reports with Excel export functionality
- **Security**: Implement software activation and user authentication systems

## ✨ Key Features

### 👥 Employee Management
- Add, edit, delete employee records
- Track employee personal information (CIN, Name, Surname, Username)
- Salary management and calculation
- Employee authentication system

### 💰 Financial Management
- **Advances (Avances)**: Track salary advances given to employees
- **Absences**: Record employee absences with penalty calculations
- **Transactions**: Complete transaction history for payments and advances
- **Dashboard**: Real-time financial overview with key metrics

### 🏢 Supplier Management
- Supplier database with contact details
- Active/inactive status tracking
- Invoice relationship management

### 📄 Invoice Processing
- Invoice creation and management
- Payment tracking (advances vs. total amount)
- Status monitoring (Non payée, En cours, Payée)
- Due date tracking

### 📊 Reporting & Analytics
- Weekly employee reports with salary calculations
- Excel export functionality using ClosedXML
- Dashboard with financial summaries
- Transaction history reports

### 🔐 Security Features
- Software activation system with machine-specific keys
- User authentication with license management
- Registry-based activation storage



## 📋 Key Files Explained

### Core Application Files

- **`Program.cs`**: Application entry point with database initialization and activation check
- **`App.config`**: Contains Entity Framework configuration, SQLite connection string, and assembly bindings
- **`GestionEmployes.csproj`**: Project file with NuGet package references and build configuration

### Data Layer

- **`ApplicationDbContext.cs`**: Entity Framework context defining database structure, relationships, and mapping configurations for all entities

### Model Classes

- **`Employe.cs`**: Employee entity with CIN as primary key, personal information, and salary data
- **`Avance.cs`**: Advance payment tracking with amount, date, and employee relationship
- **`Absence.cs`**: Employee absence records with penalty calculations and date tracking
- **`Supplier.cs`**: Supplier information with contact details and active status
- **`Facture.cs`**: Invoice entity with amount, advance payments, remaining balance calculation, and status determination
- **`Transaction.cs`**: Financial transaction records linking invoices, employees, and payment methods
- **`WeeklyReport.cs`**: Report data model with salary calculations and period tracking

### Service Layer

- **`EmployeService.cs`**: Complete employee CRUD operations with async methods and validation
- **`DashboardService.cs`**: Financial data aggregation for dashboard metrics and summaries
- **`ReportService.cs`**: Weekly report generation with Excel export using ClosedXML library
- **`SupplierService.cs`**: Supplier management operations and relationship tracking
- **`FactureService.cs`**: Invoice processing with payment status calculations

### Utility Classes

- **`DatabaseHelper.cs`**: Database creation, initialization, and table structure verification
- **`ActivationManager.cs`**: Software activation using machine-specific keys and registry storage
- **`LicenseManager.cs`**: User authentication with default credentials (admin/12345)
- **`Theme.cs`**: Consistent UI styling and theme application

### User Interface

- **`MainForm.cs`**: Main application window with tabbed interface for different modules
- **`LoginForm.cs`**: User authentication interface with license validation
- **`DashboardForm.cs`**: Financial overview with real-time metrics and charts
- **`EmployeForm.cs`**: Employee management interface with CRUD operations
- **`ReportForm.cs`**: Report generation interface with date selection and Excel export

## 💾 Database Schema

The application uses **SQLite** database with the following main tables:

- **Employe**: Employee records (Primary Key: CIN)
- **Avance**: Advance payments linked to employees
- **Absence**: Employee absences with penalties
- **Supplier**: Supplier information
- **Facture**: Invoice records linked to suppliers
- **PaymentTransaction**: Financial transactions

## 🔧 Technology Stack

- **Framework**: .NET Framework 4.8
- **Language**: C# (Windows Forms)
- **Database**: SQLite with Entity Framework 6
- **ORM**: Entity Framework 6.5.1
- **Excel Export**: ClosedXML 0.105.0
- **Security**: Registry-based activation with SHA256 hashing
- **Package Management**: NuGet packages

## 🚀 Getting Started

### Prerequisites
- .NET Framework 4.8
- Visual Studio 2019 or later
- Windows operating system

### Default Login Credentials
- **Username**: `admin`
- **Password**: `12345`

### Installation
1. Clone or download the project
2. Open `GestionEmployes.sln` in Visual Studio
3. Restore NuGet packages
4. Build and run the application
5. The database will be automatically created on first run

## 📞 Support

For technical support, contact: **0669286543**

---

*This project provides a complete business management solution with employee tracking, financial management, and comprehensive reporting capabilities.*
