# Modernized .NET 10 MVC & Web API

This project is a modern port and upgrade of the legacy SharpDevelop MVC 4 application to **ASP.NET Core (.NET 10.0)**. It serves as a unified controller-based application combining server-rendered MVC views (Razor) with secure JSON Web Token (JWT) REST APIs.

---

## 🚀 Key Features

*   **Modern Framework:** Built on **.NET 10.0** utilizing standard controller-based architectures (avoiding minimal APIs for MVC/API parity).
*   **MS Access Database Provider:** Utilizes `EntityFrameworkCore.Jet` to interact with an MS Access database file (`MyAccessDb.mdb`). The context registration is structured flexibly to allow future migrations to **PostgreSQL**.
*   **Dual Authentication Pipeline:**
    *   **Cookie Authentication:** For traditional server-side Razor pages (e.g. login forms, product management portals).
    *   **JWT Bearer Tokens:** For API controllers (validated via secure headers).
*   **Interactive API UI:** Features **Scalar** API documentation (accessible at `/scalar/v1`), providing an interactive dashboard and testing playground. Legacy requests to `/swagger` are automatically redirected to Scalar.
*   **High-Speed Seeding:** Reads Billboard dataset CSVs and inserts 10,000+ database rows in **under 2 seconds** using parameterized raw ADO.NET SQL commands executed in a single transaction (bypassing slow EF Core state tracking).
*   **Modern Helper Utilities:**
    *   **Email Dispatch:** Features async SMTP mail and attachment mapping utilizing `IFormFile` inputs.
    *   **Image Optimization:** Modernized image saving with automatic rotation (EXIF 274), bounding-box scaling, and thumbnail generation using GDI+ (`System.Drawing.Common`).

---

## 🛠️ Windows-on-ARM64 Driver Limitation

If you are developing on a Windows ARM64 device (e.g., Qualcomm Snapdragon machines), Microsoft does not publish a native ARM64 OLE DB database engine. To connect to MS Access database files, the process must run under **x64 emulation** to allow Kestrel to load the `Microsoft.ACE.OLEDB` driver natively.

Always launch the application using:
```powershell
dotnet run --arch x64
```

---

## 📂 Project Structure

```text
Dotnet10MvcApi/
├── App_Data/
│   ├── MyAccessDb.mdb               # The MS Access database
│   └── BillboardTo2013.zip          # Billboard template CSV dataset
├── Controllers/
│   ├── Api/
│   │   ├── AccountController.cs    # Token generation, refresh, and registration
│   │   ├── SongController.cs       # Paged API access to Billboard songs
│   │   └── SampleController.cs     # Weather feeds, email dispatch, file uploads
│   ├── AccountController.cs        # MVC Cookie session manager
│   ├── CrudsampleController.cs     # MVC Product listing and CRUD views
│   └── HomeController.cs           # MVC Static page routes
├── Data/
│   └── ApplicationDbContext.cs     # Entity Framework DB Context
├── Helpers/
│   ├── EmailService.cs             # SMTP mailing utility
│   ├── ImageUploadExtension.cs     # Image scale and thumbnail optimizer
│   └── TokenManager.cs             # JWT issuance and validation engine
├── Models/
│   ├── Product.cs                  # Product data model and seed lists
│   ├── Song.cs                     # Billboard song model and raw ADO.NET seed logic
│   └── UserAccount.cs              # DB-backed credentials model and password hasher
├── wwwroot/
│   ├── index.html                  # Glassmorphic homepage served at /
│   └── UploadedImages/             # Public directory for uploaded product media
├── Program.cs                      # Main application startup and configurations
└── appsettings.json                # Database connection string and JWT parameters
```

---

## 🏁 Getting Started

### Prerequisites
*   .NET 10.0 SDK
*   Microsoft Access Database Engine 2016 Redistributable (x64)

### Build the Project
To restore dependencies and build the solution binaries, run:
```powershell
dotnet build
```

### Start the Application
To run the server locally under the x64 architecture environment:
```powershell
dotnet run --arch x64
```

Once started:
*   **Web Portal Homepage:** [http://localhost:5071](http://localhost:5071)
*   **Scalar Interactive API UI:** [http://localhost:5071/scalar/v1](http://localhost:5071/scalar/v1) (or via [http://localhost:5071/swagger](http://localhost:5071/swagger) redirect)
*   **OpenAPI Document Spec:** [http://localhost:5071/openapi/v1.json](http://localhost:5071/openapi/v1.json)
