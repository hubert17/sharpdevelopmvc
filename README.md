# SharpDevelopMVC4

A light-weight solid starting point for developing ASP.NET MVC application in portable [SharpDevelop](https://portable.info.pl/sharpdevelop-portable/). Great for student learning and small medium size projects!

## Features and Libraries

- Bootstrap 4.5 theme
- EntityFramework.SharpDevelop (SQL Server)
- Dapper (MS Access support)
- Simple Forms Authentication
- AutoMapper
- Hangfire Core / Hangfire.MemoryStorage
- FluentValidation
- Image/File Upload
- Email / SMTP
- SimpleLogger
- ~~TuesPechkin PDF~~
- SimpleExcelImport

### Why it's lite?

- No OWIN
- No ASP.NET Identity

### Web.config missing?

Just copy Web.github.config then rename to Web.config

### Build Error? Could not resolve reference... Could not locate assembly... The underlying connection was closed...
- Option 1: Run (Merge) [nugetfix.reg](https://stackoverflow.com/a/53677845/1281209)
- Option 2: run nuget.bat
- Option 3: execute this command `nuget restore`
- Option 4: download the offline [nuget packages here](https://drive.google.com/file/d/1_BPJqxucppNr5WX337RRxpl8jv7YB8Kd/view?usp=sharing). Extract and add as a local source, Tools > Option > Package Management > Package Sources

### Running in IIS Express

1. Click Project Menu > Project Options
2. In **Web** tab, choose **[Use IIS Express Web Server]**
3. Enter a port number higher than `8001`
4. Click **[Create application/virtual directory]** button. Done! You can now run/debug the web app.
   > \*\*\* Error indicating duplicate entry of type 'site' with unique key attributes...
5. Goto `Documents\IISExpress\config` folder
6. Open `applicationhost.config` in a text editor, delete `<site name=...>` entries in `<sites>...</sites>`
7. Repeat step 1. 

### Local IIS or IIS Express was not found

[Download](https://www.microsoft.com/en-us/download/details.aspx?id=48264) and install [IIS Express](https://www.microsoft.com/en-us/download/details.aspx?id=48264)

### Database Browsers

You can browse the database using SQL Server Management Studio (SSMS) or portable [Database.NET](https://bit.ly/30tqqxU). To enable (LocalDB)\MSSQLLocalDB, install [SQL Server Express LocalDB](https://bit.ly/2Mlijj1).

### Free ASP.NET Hosting

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

### Warning

- Do not load the project in Visual Studio
- Do not add Nuget packages which supports .NET Standard 

## Contributors

- [Bernard Gabon](https://bernardgabon.com)
