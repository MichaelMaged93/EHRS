# 🏥 EHRS – Electronic Health Record System (Backend)

Electronic Health Record System (EHRS) is a **RESTful Backend API** built using **ASP.NET Core (.NET 8)** following **Clean Architecture** and **Database First approach**.

The system manages healthcare workflows between **Patients and Doctors**, ensuring scalability, maintainability, and real-world readiness.

---

## 🚀 Features

### 🔐 Authentication & Authorization
- Centralized authentication using `UserCredential` table
- Secure password hashing (no password stored in Patient/Doctor tables)
- Role-based system:
  - Patient
  - Doctor
- Doctor approval workflow:
  - `Pending`
  - `Approved`
  - `Rejected`
- Ready for JWT Authentication (next phase)

---

### 👨‍⚕️ Doctor Module
- Full Doctor CRUD (Admin)
- Doctor Profile Management
  - Partial updates
  - Profile image upload
  - Certificates upload (PDF)
- Doctor Dashboard (Today view)
- Appointments Management (Paging + Filtering)
- Access Patient Data using SSN:
  - Medical History
  - Surgeries
- Create Medical Records
- Upload Prescription files

---

### 👩‍🦱 Patient Module
- Patient Profile Management
- Patient Dashboard
- View Appointments
- Smart Appointment Booking
- View Medical History
- View Surgeries
- View Imaging & Radiology
- Full healthcare data overview

---

## 📅 Smart Booking System

### ✔️ Rules
- Booking based on **DateOnly (no time)**
- Multiple patients allowed per doctor per day
- Prevent duplicate booking:
  - Same Patient + Same Doctor + Same Date ❌
- Server-side validation:
  - Doctor must match selected **Area & Specialty**

---

## 📄 Medical Records System
- List Medical Records (Paging + Search)
- Get by Id
- Get by Appointment
- Create Medical Record
- Upload Prescription Image
- Fully linked:
  - Patient
  - Doctor
  - Appointment

---

## 🔍 Doctor Search (SSN-Based)

### Returns:
- Full Name
- Age (calculated)
- Height
- Weight
- Blood Type

### Used in:
- Medical Records
- Surgeries

---

## 📊 Appointment Status Standardization

All statuses are unified into:

- `waiting`
- `completed`
- `cancelled`

Handled via internal mapping method:

---

## 🏗 Architecture
EHRS.sln
│
├── EHRS.Api
│ ├── Controllers (Thin Controllers)
│
├── EHRS.Core
│ ├── DTOs
│ ├── Requests
│ ├── Interfaces
│ └── Abstractions (Queries)
│
└── EHRS.Infrastructure
├── Persistence (DbContext + Entities)
├── Queries (Flat Structure)

---

## 📐 Design Patterns Used

- Clean Architecture
- Queries Pattern
- Dependency Injection
- DTO Pattern
- Separation of Concerns
- Database First Approach

---

## 🔑 Queries Pattern (Core Concept)
Controller → IQueries → Infrastructure/Queries

- Controllers contain no business logic
- Logic implemented inside Queries layer
- Interfaces defined in Core
- Implementations in Infrastructure

---

## 🧠 Key Architectural Decisions

- ✅ Flat Queries Structure (No Feature folders)
- ✅ All Queries inside `Infrastructure/Queries`
- ✅ DTOs used between layers only
- ✅ DoctorId temporarily hardcoded (until JWT)
- ✅ DateOnly used for booking system

---

## 🧾 Core Entities

### Patient
- Personal & medical data
- Height / Weight / BloodType / SSN

### Doctor
- Profile & specialization
- Certificates & license
- ApprovalStatus

### Appointment
- Date-based booking
- Status + IsCancelled

### MedicalRecord
- Diagnosis / Notes / Treatment
- Prescription Image

### SurgeryHistory
- SurgeryType / Date / Notes

### SensorData
- HeartRate / SpO2 / Temperature / Activity

---

## 📦 API Endpoints (Summary)

### 👨‍⚕️ DoctorGET /api/Doctors
POST /api/Doctors
GET /api/Doctors/{id}
PUT /api/Doctors/{id}
DELETE /api/Doctors/{id}
### 👤 Doctor Profile
GET /api/DoctorProfile
PUT /api/DoctorProfile
### 📅 Booking
GET /api/areas
GET /api/specialties
GET /api/doctors
POST /api/booking

### 📄 Medical Records
GET /api/MedicalRecords
GET /api/MedicalRecords/{id}
POST /api/MedicalRecords

---

## 🧪 Testing

- Swagger ✅
- Postman ✅

### Test Data
- 15 Patients
- 15 Doctors (Pending Approval)

---

## 🛠 Technologies Used

- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server (Database First)
- Swagger
- Clean Architecture

---

## ▶️ How to Run

### 1. Clone Repository
```bash
git clone https://github.com/your-username/EHRS.git
2. Configure Database
Update connection string in appsettings.json
Ensure SQL Server is running
3. Run Project
dotnet run
4. Open Swagger
https://localhost:{port}/swagger
🔒 Security Notes
Passwords stored as hashed values
No password stored in Patient/Doctor tables
Role separation enforced
Booking validation applied server-side
Ownership checks on sensitive endpoints
📊 Current Project Status
✅ Completed
Full Backend Architecture
Doctor Module
Patient Module
Booking System
Medical Records
Dashboard Logic
Authentication Database Design
🚧 Next Steps
JWT Authentication
Role-based Authorization Middleware
Frontend Integration
Wearable Device Integration
📌 Future Improvements
Global Exception Handling Middleware
Unified Response Model
FluentValidation Integration
Logging System
Unit & Integration Testing
👨‍💻 Author

Michael
Backend Developer – ASP.NET Core
Electrical Engineer (Communications & Electronics)

📌 Final Note

This project represents a real-world scalable healthcare backend system built with clean design, solid architecture, and production-ready practices.
