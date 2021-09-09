# ABP Windows Service

This is a simple **Windows Service** built on **ABP Framework (open source)** (.NET5 and ABP 4.4.2). The Windows Service uses a local SQL database.


Project Structure:

- **AbpWindowsService**: This is the Windows Service. You will install this service as Windows Service.

- **AbpWindowsService.DbMigrator**: You can create your database and update it with DbMigrator.

- **AbpWindowsService.Domain**: Your domain objects like your entities are in this project.

- **AbpWindowsService.Domain.Shared**: Shared objects like your entity consts, enums are in this project.

- **AbpWindowsService.EntityFrameworkCore**: The EFCore referenced project which has DbContext and Migrations.

  For more information see https://docs.abp.io/en/commercial/latest/startup-templates/application/solution-structure 


## How to install and run?

1. Clone this repository to your disk.

2. Build the solution.

3. Run the `DbMigrator` project to create the database.

4. Start a command prompt as an administrator and run `install-service.bat`. This will install the Window Service. Your service name will be `AbpWindowsService`.

5. The database connection string is 

   ```bash
   Server=(LocalDb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True
   ```

   The `Trusted_Connection` allows to connect the database with your active Windows user account. But the default user account for Windows Services is local `System` user. Therefore you need to set your active account to the service. To do this; Open *Services* window by running `C:\Windows\System32\services.msc ` and right click your service, go to "*Properties*". Go to "*LogOn*" tab, choose "*This Account*" and write your *username & password*. To learn your logon username, start a command prompt and write `whoami`. 

6. Start the service. And that's it!



To check if it's working, open the following log file

```
 src\AbpWindowsService\bin\Debug\net5.0\win-x64\Logs\Log.txt
```



I already added 4 database records (books)  with the data seeder, so you'll see the following lines in the logs when the service starts. These are the records fetched from the `Books` table.

```cmd
2021-09-08 20:38:17.713 +03:00 [INF] *** Book name: George Orwell 1984
2021-09-08 20:38:17.713 +03:00 [INF] *** Book name: Lord of the Flies
2021-09-08 20:38:17.713 +03:00 [INF] *** Book name: Harry Potter
2021-09-08 20:38:17.713 +03:00 [INF] *** Book name: The Great Gatsby
```



## How to uninstall?

Start a command prompt as an administrator and run `uninstall-service.bat`. This will uninstall the Window Service. This batch runs the following operations:

- Stops the service if it's running
- Closes `mmc.exe` if it's open. (*Service cannot be uninstalled if this window is open*)
- Removes service with ` sc delete` command.



## Creating your own Windows Service



### Create console application

To create your own Windows Service, create an ABP console application with the following command:

```bash
abp new AbpWindowsService -t console -csf
```



### Add Windows Service middleware

Add the [Microsoft.Extensions.Hosting.WindowsServices](https://www.nuget.org/packages/Microsoft.Extensions.Hosting.WindowsServices) package to your application by adding the following line to the csproj:

```json
<PackageReference Include="Microsoft.Extensions.Hosting.WindowsServices" Version="5.0.1" />
```

Open the **Program.cs** and add  the `UseWindowsService()` to `CreateHostBuilder` method. You can also set a service name to be shown in services window. `UseWindowsService` ;

- Sets the host lifetime to *WindowsServiceLifetime*

- Sets the Content Root

- Enables logging to the event log with the application name as the default source name

  

```
  Host.CreateDefaultBuilder(args)
  .UseAutofac()
  .UseSerilog()
  .UseWindowsService(x => x.ServiceName = "AbpWindowsService") //// add this line /////
  .ConfigureAppConfiguration((context, config) =>
  {
      //setup your additional configuration sources
  })
  .ConfigureServices((hostContext, services) =>
  {
  services.AddApplication<AbpWindowsServiceModule>();
  });
```



### Set log file path

By default Windows Service runs as local System account and Serilog will write the logs under `C:\Windows\System32`. So you can set the log file as static. Below code lets Serilog write to the Logs folder under your Windows Service installation directory.

    Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs/Log.txt")))
    .CreateLogger();


### Write your business logic

In your Windows Service there is an ABP module class  `AbpWindowsServiceModule.cs`. This class adds the hosted service `*HostedService.cs`. Hosted service is an implementation of `IHostedService`. It has `StartAsync()` and `StopAsync()` methods. `StartAsync` method will run when you start your Windows Service and `StopAsync` will run when you stop your Windows Service. The application will not end unless you stop it.  You can create a domain service from `IDomainService`. And write your code in this domain service. Call your method inside the Hosted Service `StartAsync` method. Dependency Injection will work without any problem as long as you implement your classes from `ITransientDependency ` or `ISingletonDependency`.



### Accessing the database

To access your database, entities, repositories and application services you can move your Windows Service into your ABP Web Application solution. To do this, copy the service project to the src folder of your main (web) solution. Add project reference of ***.EntityFrameworkCore**  project to your Windows Service as shown below:

```json
<ProjectReference Include="..\AbpWindowsService.EntityFrameworkCore\AbpWindowsService.EntityFrameworkCore.csproj" />
```

Add the `DependsOn` attribute of EntityFramework module to your Windows Service as shown below:

```
 [DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpWindowsServiceEntityFrameworkCoreModule)) /// added this line ///
 ]
 public class AbpWindowsServiceModule : AbpModule
 {
      //....
 }
```

Now you can inject your repositories into your domain service.

Add your database connection string to the **appsettings.json** of your Windows Service.

```json
{
  "ConnectionStrings": {
    "Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True"
  }
}
```

Note that we are using `Trusted_Connection=True` which means,  Windows Authentication (the active Windows user) will be used.

> `Trusted_Connection=True`  will cause a small problem! The default user of a Windows Service is local System account. Therefore you cannot connect your database with the System account. To solve this, apply the **5th** step of **How to install and run?** section in this document. Alternative way; You can create a user in your database and use the UserId-Password credential in the connection string.

And you are ready to install your new service to your Windows! See the **How to install and run?** section to install your service.


And happy coding!