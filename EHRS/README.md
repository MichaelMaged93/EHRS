# 🏥 EHRS – Electronic Health Record System (Backend)

Electronic Health Record System (EHRS) is a backend RESTful API built using ASP.NET Core (.NET 8) following Clean Architecture principles.  
It manages healthcare data for Patients and Doctors with secure authentication, localization, file handling, and structured querying.

---

## 🚀 Features

### 🔐 Authentication & Authorization
- JWT-based authentication
- Role-based authorization (Patient / Doctor)
- Secure password hashing
- Doctor approval workflow (Pending / Approved / Rejected)

### 👨‍⚕️ Doctor Module
- Register & Login
- Doctor Profile (update with image & certificates upload)
- View Today Dashboard
- Manage Appointments
- Create & Manage Medical Records
- Upload Prescriptions & Radiology files

### 👩‍🦱 Patient Module
- Register & Login
- Profile Management (with image upload)
- Dashboard (BMI, upcoming appointments, sensor data)
- Book Appointment (Area → Specialty → Doctor → Date)
- Cancel Appointment
- View Medical History
- Manage Surgeries
- View Imaging & Radiology
- View & Download Prescriptions

---

## 🌍 Localization
- Full Arabic & English support
- Language selected via `Accept-Language` header
- Centralized resource-based messages (`Messages.resx`)

---

## 📁 File Handling
- Secure upload & storage under `wwwroot/uploads`
- Prescription & Radiology file management
- Controlled file access

---

## 📊 Pagination & Filtering
- Appointments
- Medical Records
- Prescriptions
- Imaging

---

## 🏗 Architecture

This project follows Clean Architecture:

```text
EHRS.sln
│
├── EHRS.Api
│   ├── Controllers
│   ├── Services
│   ├── Localization
│   └── Resources
│
├── EHRS.Core
│   ├── DTOs
│   ├── Requests
│   ├── Interfaces
│   └── Abstractions (Queries)
│
└── EHRS.Infrastructure
    ├── Persistence (DbContext + Entities)
    ├── Queries (Implementations)
    └── Services
    
    
    
Design Patterns Used

Clean Architecture

Queries Pattern

Dependency Injection

JWT Authentication

Role-based Authorization

Resource-based Localization

Pagination Pattern

🔑 Authentication Flow

User registers (Patient or Doctor)

Doctor accounts require approval

On login, JWT token is generated

Role & UserId are extracted using ClaimsHelper

Controllers authorize using [Authorize(Roles="...")]

🌍 Localization

Language controlled by header:

Accept-Language: ar
Accept-Language: en


Messages are stored in:

Resources/Messages.resx
Resources/Messages.ar.resx

🛠 Technologies Used

ASP.NET Core (.NET 8)

Entity Framework Core

SQL Server (Database First)

JWT Bearer Authentication

Resource-based Localization

Swagger

▶️ How to Run

Clone repository:

git clone https://github.com/MichaelMaged93/EHRS.git


Update connection string in appsettings.json

Ensure database exists

Run project:

dotnet run


Open Swagger:

https://localhost:{port}/swagger

📦 Example Endpoints
Patient
POST   /api/PatientAuth/register
POST   /api/PatientAuth/login
GET    /api/PatientDashboard
POST   /api/PatientAppointments/{id}/cancel

Doctor
POST   /api/DoctorAuth/register
POST   /api/DoctorAuth/login
GET    /api/Dashboard/today
POST   /api/MedicalRecords

🔒 Security Notes

Passwords stored as hashed values

Role-based authorization enforced

File paths validated

Ownership checks applied on sensitive endpoints

📌 Future Improvements

Global Exception Middleware

Unified Error Response Model

FluentValidation Integration

Logging Layer

Unit & Integration Testing

👨‍💻 Author

Michael Maged
Backend Developer – ASP.NET Core
Electrical Engineer (Communications & Electronics)