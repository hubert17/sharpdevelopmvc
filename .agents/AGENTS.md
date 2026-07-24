# Workspace Rules - SharpDevelopMVC Modernization

These instructions govern all future modifications, tests, and task executions performed by AI agents in this repository.

---

## 💻 Environment & Run Requirements

*   **x64 Emulation Constraint:** This application runs on Windows ARM64 but connects to an MS Access database via OLE DB drivers, which are exclusively compiled for x64/x86 architectures.
    *   **Rule:** Always run, debug, or build the project using the x64 architecture flag:
        ```powershell
        dotnet run --arch x64
        ```
    *   **Failure Mode:** Running without `--arch x64` results in `assembly not found` or `provider not registered` exceptions during database connection handshakes.

---

## 🗄️ Database & Queries (MS Access Jet / EF Core)

*   **Database Provider:** The project uses `EntityFrameworkCore.Jet` for database connections. Maintain compatibility for easy future shifts to **PostgreSQL**. Do not use MS SQL Server.
*   **Scalar Queries (#Dual):** The Jet provider translates LINQ evaluations like `.Any()` into SQL containing `FROM #Dual`. 
    *   **Rule:** The database must contain a helper table named `[#Dual]` with exactly one row. This table is automatically checked and seeded on startup in `Program.cs`. Do not delete or alter this table.
*   **Bulk Ingest Seeding:** Row-by-row EF Core change-tracked inserts for thousands of records are too slow for the Jet database engine.
    *   **Rule:** Seeding of large lists (like the Billboard songs database) must be executed using raw parameterized ADO.NET commands inside a single transaction (refer to `Song.Seed(...)`).

---

## 🔐 Hybrid Authentication Model

*   **Dual Authentication Schemas:** The project registers both Cookie and JWT Bearer schemes in `Program.cs`. The default scheme is Cookies.
    *   **MVC Pages:** Use standard `[Authorize]` attributes (which default to redirection to `/Account/Login`).
    *   **Web APIs:** Must explicitly request JWT Bearer authentication to check header authorizations:
        ```csharp
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        ```

---

## 📷 GDI+ & Image Processing

*   **Windows Target Platform:** GDI+ library calls (`System.Drawing.Common`) are used for image rotation, scaling, and thumbnail rendering in `ImageUploadExtension.cs`.
    *   **Rule:** Platform compatibility warning `CA1416` (not supported on non-Windows platforms) can be ignored or suppressed, as the project is platform-locked to Windows due to OLE DB driver constraints.

---

## 📄 API Documentation

*   **API Reference Route:** The OpenAPI docs and visual playground are powered by **Scalar** (served at `/scalar/v1`).
    *   **Rule:** Ensure that the legacy redirection endpoint `/swagger` in `Program.cs` remains mapped to `/scalar/v1` for convenience.

---

## 🌐 HTML-First View Implementation

*   **Markup Style:** Prefer standard HTML5 markup over legacy ASP.NET MVC Razor helpers (e.g., `@Html.BeginForm`, `@Html.TextBoxFor`, `@Html.LabelFor`).
    *   **Rule:** Implement views using clean, raw HTML form controls and Bootstrap 4 classes (`<form action="..." method="...">`, `<input id="..." name="..." class="form-control" />`). Use Razor syntax for essential dynamic control flow (loops, conditionals) and model properties rather than HTML helper abstractions.

