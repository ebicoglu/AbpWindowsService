# ABP Windows Service

This is a simple **Windows Service** built on **ABP Framework (open source)** (.NET5 and ABP 4.4.2). The Windows Service uses a local SQL database.

Project Structure:

*   **AbpWindowsService**: This is the Windows Service. You will install this service as Windows Service.
*   **AbpWindowsService.DbMigrator**: You can create your database and update it with DbMigrator.
*   **AbpWindowsService.Domain**: Your domain objects like your entities are in this project.
*   **AbpWindowsService.Domain.Shared**: Shared objects like your entity consts, enums are in this project.
*   **AbpWindowsService.EntityFrameworkCore**: The EFCore referenced project has DbContext and Migrations.

For more information see https://docs.abp.io/en/commercial/latest/startup-templates/application/solution-structure

## How to install and run?

Clone [this repository](https://github.com/ebicoglu/AbpWindowsService) to your disk.

Build the solution.

Run the `DbMigrator` project to create the database.

Start a command prompt as an administrator and run `install-service.bat`. This will install the Window Service. Your service name will be `AbpWindowsService`. You can change the service name [from this code line](https://github.com/ebicoglu/AbpWindowsService/blob/main/src/AbpWindowsService/Program.cs#L52).

![](https://user-images.githubusercontent.com/9526587/132733812-a042f301-d766-4e6e-95c5-5a80aa58deb2.png)

The database connection string is

```bash
Server=(LocalDb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True
```

The `Trusted_Connection` allows connecting the database with your active Windows user account. But the default user account for Windows Services is a local `System` user. Therefore you need to set your active account to the service. To do this; Open the _Services_ window by running `C:\Windows\System32\services.msc` and right-click your service, go to "_Properties_". Go to the "_LogOn_" tab, choose "_This Account_" and write your _username & password_. To learn your logon username, start a command prompt and write `whoami`.

![](https://user-images.githubusercontent.com/9526587/132734210-d6982cc9-fda5-4dd4-b49f-eb59401e9445.png)

Start the service. And that's it!

To check if it's working, open the following logfile

```shell
 src\AbpWindowsService\bin\Debug\net5.0\win-x64\Logs\Log.txt
```

I already added 4 database records (books) with [the data seeder](https://github.com/ebicoglu/AbpWindowsService/blob/main/src/AbpWindowsService.Domain/IdentityServer/IdentityServerDataSeedContributor.cs#L69), so you'll see the following lines in the logs when the service starts. These are the records fetched from the `Books` table.

```
[INF] *** Book name: George Orwell 1984
[INF] *** Book name: Lord of the Flies
[INF] *** Book name: Harry Potter
[INF] *** Book name: The Great Gatsby
```

## How to uninstall?

Start a command prompt as an administrator and run `uninstall-service.bat`. This will uninstall the Window Service. This batch runs the following operations:

*   Stops the service if it's running
*   Closes `mmc.exe` if it's open. (_because the service cannot be uninstalled if this window is open_)
*   Removes service with `sc delete` command

![](https://user-images.githubusercontent.com/9526587/132734406-a1204e90-d66d-491e-b294-d3fea95a852b.png)

## Creating your own Windows Service in .NET CORE using ABP Framework

### Create an ABP console application

To create your own Windows Service, you can start with creating an ABP console application with the following command:

```bash
abp new AbpWindowsService -t console -csf
```

### Add Windows Service middleware

Add the [Microsoft.Extensions.Hosting.WindowsServices](https://www.nuget.org/packages/Microsoft.Extensions.Hosting.WindowsServices) package to your console application by adding the following line to the csproj:

```xml
<PackageReference Include="Microsoft.Extensions.Hosting.WindowsServices" Version="5.0.1" />
```

Open the **Program.cs** and add the `UseWindowsService()` to `CreateHostBuilder` method. You can also set a service name to be shown in the services window.

```c#
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

`UseWindowsService` makes 3 things:

*   Sets the host lifetime to _WindowsServiceLifetime_
*   Sets the Content Root
*   Enables logging to the event log with the application name as the default source name

### Set log file path

By default, Windows Service runs as a local System account and Serilog will write the logs under `C:\Windows\System32`. To overcome this, you can set the log file as the static path. The below code lets Serilog write to the Logs folder under your Windows Service installation directory.

```
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
.Enrich.FromLogContext()
.WriteTo.Async(c => c.File(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs/Log.txt")))
.CreateLogger();
```

### Write your business logic

In your Windows Service, there is an ABP module class `AbpWindowsServiceModule.cs`. This class adds the hosted service `*HostedService.cs`. Hosted service is an implementation of `IHostedService`. It has `StartAsync()` and `StopAsync()` methods. `StartAsync` the method will run when you start your Windows Service and `StopAsync` will run when you stop your Windows Service. The application will not end unless you stop it. You can create a domain service from `IDomainService`. And write your code in this domain service. Call your method inside the Hosted Service `StartAsync` method. Dependency Injection will work without any problem as long as you implement your classes from `ITransientDependency` or `ISingletonDependency`.

### Accessing the database

To access your database and repositories you can move your Windows Service into your main ABP Web Application solution. To do this, copy the service project to the src folder of your main web solution. Add project reference of \***.EntityFrameworkCore** project to your Windows Service as shown below:

```xml
<ProjectReference Include="..\AbpWindowsService.EntityFrameworkCore\AbpWindowsService.EntityFrameworkCore.csproj" />
```

Add the `DependsOn` attribute of `EntityFramework` module to your Windows Service as shown below:

```csharp
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

Don't forget to add the database connection string to the **appsettings.json** of your Windows Service.

```xml
{
  "ConnectionStrings": {
    "Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True"
  }
}
```

Note that we are using `Trusted_Connection=True` which means, Windows Authentication (the active Windows user) will be used.

> `Trusted_Connection=True` will cause a small problem! The default user of a Windows Service is local System account. Therefore you cannot connect your database with the System account. To solve this, apply the **5th** step of **How to install and run?** section in this document. Alternative way; You can create a user in your database and use the UserId-Password credential in the connection string.

And you are ready to install your new service! See the **How to install and run?** section to install your new service.

And happy coding!
