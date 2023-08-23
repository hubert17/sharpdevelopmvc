

# SharpDevelopMVC5

A lightweight, solid starting point for developing ASP.NET MVC and Web API applications in [portable](https://portable.info.pl/sharpdevelop-portable/) [SharpDevelop](https://mega.nz/file/sJIHBbyY#O80dgllefCf07TIesoM1IMxsTqomVhLVt6_t9WG-hXA). Great for student learning and small to medium-sized projects!

![SharpDevelop](https://portable.info.pl/wp-content/uploads/2017/10/SharpDevelop_Softables.png)

See the [Demo](http://sharpdevelopmvc.somee.com)

## Features and Libraries

- Bootstrap 4.6 with bonus [Bootswatch](https://bootswatch.com/4/) themes and [Bootbox.js](https://bootboxjs.com/examples.html)
- EntityFramework.SharpDevelop (SQL Server, now with MS Access MDB support!)
- Dapper / Dapper.Contrib
- [EFUtilities.SharpDevelop](https://github.com/MikaelEliasson/EntityFramework.Utilities)
- Simple Forms Authentication
- AutoMapper
- FluentValidation
- Image/File Upload
- Email/SMTP
- SimpleLogger
- Hangfire Core/Hangfire.MemoryStorage
- CsvHelper
- SimpleExcelImport
- [petite-vue](https://github.com/vuejs/petite-vue)
- ~~OpenHtmlToPdf~~
- Simple JWT Authentication
- Swagger/Swashbuckle
- SignalR 1 [(ChatDemo)](https://github.com/aspdotnetgabs/sharpdevelopmvc/tree/chatdemo-signalr-petitevue)

### Why it's light?

- No OWIN
- No ASP .NET Identity

### Web.config missing?

Simply `Build` and then reload the project, or copy _web.config and rename it to Web.config.

#### Encountering Build Errors? Could not resolve reference... Could not locate assembly... The underlying connection was closed...
- Option 1: Run (Merge) [nugetfix.reg](https://stackoverflow.com/a/53677845/1281209)
- Option 2: Run nuget.bat
- Option 3: In SharpDevelop, run Restore packages
- Option 4: Execute this command `nuget restore`

### Running in IIS Express

1. Click the Project Menu > Project Options
2. In the **Web** tab, choose **[Use IIS Express Web Server]**
3. Enter any port number, e.g., `55353` or higher
4. Click the **[Create application/virtual directory]** button. Done! You can now run/debug the web app.

   > \*\*\* Error indicating duplicate entry of type 'site' with unique key attributes...
5. Right-click, rename, and `Clean` the project. Example: **ASPNETWebApp45** -> **NewNameOfMyWebApp**
6. Repeat step 1. 

#### To enable localhost SSL/HTTPS
Add a port `443xx` https binding in ***My Documents\IISExpress\config\applicationhost.config***

        <bindings>
		    <binding protocol="http" bindingInformation="*:55353:localhost" />
			<binding protocol="https" bindingInformation="*:44353:localhost" />
        </bindings>

#### Local IIS or IIS Express was not found

[Download](https://www.microsoft.com/en-us/download/details.aspx?id=48264) and install [IIS Express](https://www.microsoft.com/en-us/download/details.aspx?id=48264)

### Database Browsers

You can browse the database using SQL Server Management Studio (SSMS) or [portable](https://bit.ly/30tqqxU) [Database.NET](https://fishcodelib.com/files/DatabaseNet4.zip). To enable (LocalDB)\MSSQLLocalDB, install [SQL Server Express LocalDB](https://bit.ly/2Mlijj1).

### Publish Files (Ready to zip and upload)

Run `Rebuild` to generate publish files. See the _publish folder. *Make sure to back up `App_Data` folder in your production server before zip-deploying your site.*

### Free ASP .NET Hosting

- [Somee.com](https://somee.com/FreeAspNetHosting.aspx)
- [Smarterasp.net](https://www.smarterasp.net/secured_signup?plantype=FREE)
- [myasp.net](https://www.myasp.net/freeaspnethosting)
- [Azure](https://azure.microsoft.com/en-us/free/students/) (Requires a school .edu email address for students, or an ATM/Debit Card)
- Local hosting with [Ngrok](https://github.com/hubert17/ngrok-redirector)

### Github Actions (CD/CI to Somee.com)

Autodeploy your web app to Somee.com. Just create these [Secrets](https://docs.github.com/en/actions/reference/encrypted-secrets) [(Settings > Secrets)](https://github.com/aspdotnetgabs/sharpdevelopmvc/settings/secrets/actions) and Variables, and supply your Somee sitename, username and password: 
- SOMEE_SITENAME
- SOMEE_USERNAME
- SOMEE_PASSWORD

### Learning Slides

- See the folder for PPTs
- [Entity Framework 6 Code-First Tutorial](https://bernardgabon.com/blog/entity-framework-tutorial/)
- JSON
- ASP.NET Web API

### ASP .NET MVC Tutorial

1. [Creating your first ASP.NET MVC application (until 5:07)](https://www.youtube.com/watch?v=KvTy_FAYjks)

2. [Controllers in an MVC application](https://www.youtube.com/watch?v=duQ1Pvr-oW0)

3. [Views in an MVC application](https://www.youtube.com/watch?v=N6srbKfNcV4)

4. [ViewData and ViewBag in MVC](https://www.youtube.com/watch?v=KrdMO2akohE)

5. [Models in an MVC application](https://www.youtube.com/watch?v=KYOMgtZ4k3w)

6. [Data access in MVC using Entity Framework](https://www.youtube.com/watch?v=Lrr66APUwBk)

7. [Generate hyperlinks using actionlink HTML helper](https://www.youtube.com/watch?v=It_X8Br2rmY)

8. [Creating a view to insert data using MVC](https://www.youtube.com/watch?v=OX69gRT7azs)

9. [Model Binding in MVC](https://www.youtube.com/watch?v=uXwmyuvrn1E)

10. [Insert, update, delete in MVC using Entity Framework](https://www.youtube.com/watch?v=8f4P8U1a2TI)

11. [Partial views in MVC](https://www.youtube.com/watch?v=SABg7RyjX-4)

12. [Razor views in MVC](https://www.youtube.com/watch?v=PRLGP_S9_K8)

13. [Layout view in MVC](https://www.youtube.com/watch?v=VyQhEArGTNs)

14. [Authorize and AllowAnonymous action filters in MVC](https://www.youtube.com/watch?v=ColwQX-dRJY)

### ENTITY FRAMEWORK Tutorial
01. [What is Entity Framework](https://www.youtube.com/watch?v=Z7713GBhi4k) 

02. [Entity Framework Code First Approach](https://www.youtube.com/watch?v=kbH-rqMl8cE)

03. [Customizing table, column, and foreign key column names](https://www.youtube.com/watch?v=XIlTTKjRzO4)

### Warning

- Do not load the project in Visual Studio
- Do not add Nuget packages that support .NET Standard 

### Visual Studio 201x version

- [Download here](https://github.com/aspdotnetgabs/sharpdevelopmvc/archive/refs/heads/visual-studio-201x-version.zip) the same project compatible with Visual Studio 2012-2019. 

## Contributors

- [Bernard Gabon](https://bernardgabon.com)
