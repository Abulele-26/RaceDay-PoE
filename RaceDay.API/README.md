# 🏃‍♂️ RaceDay Platform - Portfolio of Evidence (PoE)

![.NET Core CI](https://github.com/Abulele-26/RaceDay-PoE/actions/workflows/validate.yml/badge.svg)

## 📖 Project Overview
The **RaceDay Platform** is a comprehensive full-stack event management system designed for the South African road running, walking, and cycling community. This repository contains the complete Portfolio of Evidence (PoE) spanning from system planning and database design to API development and a fully functional MVC web application.

## 📂 Repository Structure
*   `RaceDay.API/` - The Part 2 RESTful Web API (ASP.NET Core Web API).
*   `RaceDay.Web/` - The Part 3 Frontend Application (ASP.NET Core MVC).
*   `RaceDay.Tests/` - Unit tests for the API.
*   `docs/` - Part 1 documentation (ERD, API plan, SQL script).
*   `.github/workflows/` - CI/CD pipeline configuration.

---

## 📝 Part 1: System Planning & Database
The initial planning phase is documented in the `docs/` folder. 
*   **Database Design:** A relational SQL database consisting of 6+ entities (Users, Events, Categories, Enrollments, Results, etc.).
*   **Entity Relationship Diagram (ERD):** Available in `docs/ERD.png`.
*   **API Endpoint Plan:** Available in `docs/API_Plan.md`.
*   **SQL Script:** The complete `CREATE TABLE` and `INSERT` scripts are available in `docs/database_script.sql`.

## ⚙️ Part 2: The RESTful API (Backend)
The backend is built using **ASP.NET Core Web API** and **Entity Framework Core**.
*   **Authentication:** JWT (JSON Web Tokens) and role-based authorization (Organiser / Participant).
*   **Database:** SQL Server.
*   **Cloud Storage:** Integrated with **Azure Blob Storage** for uploading event banners and user profile pictures.
*   **API Documentation:** Swagger UI is enabled for easy endpoint testing.

### How to run the API locally:
1. Open `RaceDay.API` in Visual Studio.
2. Ensure your `appsettings.json` is configured with your local SQL Server connection string and Azure Blob Storage credentials.
3. Press **F5** to run. The API will launch at `http://localhost:5291` (or check your `launchSettings.json` for the exact port).
4. Access Swagger at `http://localhost:5291/swagger`.

---

## 🖥️ Part 3: The MVC Web Application (Frontend)
The frontend is a **ASP.NET Core MVC** application that consumes the Part 2 API. It does not have direct database access.

*   **Authentication:** Cookie-based session management.
*   **Organiser Interface:** Dashboard, Event CRUD (with image uploads), Category management, Enrolment viewing, and Results entry.
*   **Participant Interface:** Browse upcoming events, view event details, enrol in categories, view personal results, and update profile pictures.
*   **UI/UX:** Built with Bootstrap 5, featuring a sporty theme, colour-coded enrolment statuses, and graceful error handling.

---

## ☁️ Azure Blob Storage Integration
The upload flow is strictly API-driven:
1. The MVC Web App sends the `IFormFile` to the Part 2 API via `MultipartFormDataContent`.
2. The API handles the Azure SDK and uploads the file to Azure Blob Storage.
3. The API saves the Azure URL to the SQL Database.
4. The MVC app displays the image by referencing the returned URL.

---

## 🐳 Docker Setup
The MVC application is fully containerized using a multi-stage `Dockerfile`.

### 1. Build the Image
Navigate to the `RaceDay.Web` directory and run:
```bash
docker build -t raceday-mvc .