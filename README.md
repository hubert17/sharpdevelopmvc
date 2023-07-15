

# SharpDevelopMVC4

A light-weight solid starting point for developing ASP.NET MVC and Web API application in [portable](https://portable.info.pl/sharpdevelop-portable/) [SharpDevelop](https://mega.nz/file/sJIHBbyY#O80dgllefCf07TIesoM1IMxsTqomVhLVt6_t9WG-hXA). Great for student learning and small medium size projects!

![https://portable.info.pl/sharpdevelop-portable/](https://portable.info.pl/wp-content/uploads/2017/10/SharpDevelop_Softables.png)

## Features and Libraries

- Bootstrap 4.6 with bonus [Bootswatch](https://bootswatch.com/4/) themes and [Bootbox.js](https://bootboxjs.com/examples.html)
- EntityFramework.SharpDevelop (SQL Server)
- Dapper (MS Access support)
- [EFUtilities.SharpDevelop](https://github.com/MikaelEliasson/EntityFramework.Utilities)
- Simple Forms Authentication
- AutoMapper
- FluentValidation
- Image / File Upload
- Email / SMTP
- SimpleLogger
- Hangfire Core / Hangfire.MemoryStorage
- CsvHelper
- SimpleExcelImport
- [petite-vue](https://github.com/vuejs/petite-vue)
- ~~OpenHtmlToPdf~~
- Simple JWT Authentication
- Swagger / Swashbuckle
- SignalR 1 [(ChatDemo)](https://github.com/aspdotnetgabs/sharpdevelopmvc/tree/chatdemo-signalr-petitevue)

### Why it's lite?

- No OWIN
- No ASP .NET Identity

### Web.config missing?

Just `Build` then reload the project. Or copy _web.config then rename to Web.config

### Build Error? Could not resolve reference... Could not locate assembly... The underlying connection was closed...
- Option 1: Run (Merge) [nugetfix.reg](https://stackoverflow.com/a/53677845/1281209)
- Option 2: run nuget.bat
- Option 3: In SharpDevelop, run Restore packages
- Option 4: execute this command `nuget restore`

### Running in IIS Express

1. Click Project Menu > Project Options
2. In **Web** tab, choose **[Use IIS Express Web Server]**
3. Enter a port number higher than `8001`
4. Click **[Create application/virtual directory]** button. Done! You can now run/debug the web app.

   > \*\*\* Error indicating duplicate entry of type 'site' with unique key attributes...
5. Just right-click, rename and `Clean` the project. Ex: **ASPNETWebApp45** --> **NewNameOfMyWebApp**
6. Repeat step 1. 

### Local IIS or IIS Express was not found

[Download](https://www.microsoft.com/en-us/download/details.aspx?id=48264) and install [IIS Express](https://www.microsoft.com/en-us/download/details.aspx?id=48264)

### Database Browsers

You can browse the database using SQL Server Management Studio (SSMS) or [portable](https://bit.ly/30tqqxU) [Database.NET](https://fishcodelib.com/files/DatabaseNet4.zip). To enable (LocalDB)\MSSQLLocalDB, install [SQL Server Express LocalDB](https://bit.ly/2Mlijj1).

### Free ASP .NET Hosting

- [Somee.com](https://somee.com/FreeAspNetHosting.aspx)
- [Smarterasp.net](https://www.smarterasp.net/secured_signup?plantype=FREE)
- [myasp.net](https://www.myasp.net/freeaspnethosting)
- [Azure For Students](https://azure.microsoft.com/en-us/free/students/) (Requires school .edu email address)
- Local hosting with [Ngrok](https://github.com/hubert17/ngrok-redirector)

### Github Actions (CD/CI to Somee.com)

Autodeploy your webapp to Somee.com. Just create these [Secrets](https://docs.github.com/en/actions/reference/encrypted-secrets) (Settings > Secrets) and supply with your Somee username and password: 
- SOMEE_USERNAME
- SOMEE_PASSWORD

### Learning Slides

- See folder for PPTs
- [Entity Framework 6 Code-First Tutorial](https://bernardgabon.com/blog/entity-framework-tutorial/)
- JSON
- ASP.NET Web API

### ASP .NET MVC Tutorial

1. [Creating your first aspnet mvc application (until 5:07)](https://www.youtube.com/watch?v=KvTy_FAYjks)

2. [Controllers in an mvc application](https://www.youtube.com/watch?v=duQ1Pvr-oW0)

3. [Views in an mvc application](https://www.youtube.com/watch?v=N6srbKfNcV4)

4. [ViewData and ViewBag in mvc](https://www.youtube.com/watch?v=KrdMO2akohE)

5. [Models in an mvc application](https://www.youtube.com/watch?v=KYOMgtZ4k3w)

6. [Data access in mvc using Entity Framework](https://www.youtube.com/watch?v=Lrr66APUwBk)

7. [Generate hyperlinks using actionlink html helper](https://www.youtube.com/watch?v=It_X8Br2rmY)

8. [Creating a view to insert data using mvc](https://www.youtube.com/watch?v=OX69gRT7azs)

9. [Model Binding in mvc](https://www.youtube.com/watch?v=uXwmyuvrn1E)

10. [Insert update delete in mvc using entity framework](https://www.youtube.com/watch?v=8f4P8U1a2TI)

11. [Partial views in mvc](https://www.youtube.com/watch?v=SABg7RyjX-4)

12. [Razor views in mvc](https://www.youtube.com/watch?v=PRLGP_S9_K8)

13. [Layout view in mvc](https://www.youtube.com/watch?v=VyQhEArGTNs)

14. [Authorize and AllowAnonymous action filters in mvc](https://www.youtube.com/watch?v=ColwQX-dRJY)

### ENTITY FRAMEWORK Tutorial
01. [What is Entity Framework](https://www.youtube.com/watch?v=Z7713GBhi4k) 

02. [Entity Framework Code First Approach](https://www.youtube.com/watch?v=kbH-rqMl8cE)

03. [Customizing table, column and foreign key column names](https://www.youtube.com/watch?v=XIlTTKjRzO4)

### Warning

- Do not load the project in Visual Studio
- Do not add Nuget packages which supports .NET Standard 

### Visual Studio 201x version

- [Download here](https://github.com/aspdotnetgabs/sharpdevelopmvc/archive/refs/heads/visual-studio-201x-version.zip) the same project comptible with Visual Studio 2012-2019. 

## Contributors

- [Bernard Gabon](https://bernardgabon.com)
