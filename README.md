# SharpDevelopMVC4

A light-weight solid starting point for developing ASP.NET MVC application in portable [SharpDevelop](https://portable.info.pl/sharpdevelop-portable/). Great for student learning and small medium size projects!

## Features and Libraries

- EntityFramework.SharpDevelop (SQL Server)
- Dapper (MS Access support)
- AutoMapper
- Hangfire Core / Hangfire.MemoryStorage
- FluentValidation
- ~~TuesPechkin PDF~~
- SimpleExcelImport
- Image/File Upload
- Email / SMTP
- SimpleLogger

### Why it's lite?

- No OWIN
- No ASP.NET Identity

### Web.config missing?

Just copy Web.github.config then rename to Web.config

### Build Error? Could not resolve reference... Could not locate assembly...

- Run nuget.bat
- or execute this command `nuget restore`

### Running in IIS Express

1. Click Project Menu > Project Options
2. In **Web** tab, choose **[Use IIS Express Web Server]**
3. Enter a port number higher than `8001`
4. Click **[Create application/virtual directory]** button
   > \*\*\* Error indicating duplicate entry of type 'site' with unique key attributes...
5. Goto `Documents\IISExpress\config` folder
6. In `applicationhost.config`, delete `<site name=...>` entries in `<sites>...</sites>`
7. Repeat step 1.

### Portable Asp.Net Web Server

Run the project without SharpDevelop in this tiny web server!

### Database Browsers

You can browse the database using SQL Server Management Studio (SSMS) or portable [Database.NET](https://bit.ly/30tqqxU). To enable (LocalDB)\MSSQLLocalDB, install [SQL Server Express LocalDB](https://bit.ly/2Mlijj1).

### Free ASP.NET Hosting

- [Somee.com](https://somee.com/FreeAspNetHosting.aspx)
- [Smarterasp.net](https://www.smarterasp.net/secured_signup?plantype=FREE)
- [myasp.net](https://www.myasp.net/freeaspnethosting)

### Learning Slides

- See folder for PPTs
- [Entity Framework 6 Code-First Tutorial](https://bernardgabon.com/blog/entity-framework-tutorial/)

### Warning

- Do not load the project in Visual Studio
- Do not add Nuget packages which supports .NET Standard

## Contributors

- [Bernard Gabon](https://bernardgabon.com)
