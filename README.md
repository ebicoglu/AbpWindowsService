# ABP Windows Service

This is a simple **Windows Service** built on **ABP Framework** (.NET5 and ABP 4.4.2)

Even the ABP version is old, you can apply the same code to your project to run a Windows Service which uses DB transactions.

Project Structure:

- **AbpWindowsService**: This is the Windows Service. You'll install this service.

- **AbpWindowsService.DbMigrator**: You can create your database and update it.

- **AbpWindowsService.Domain**: Your domain objects like your entities.

- **AbpWindowsService.Domain.Shared**: Shared objects like your entity consts.

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



4 books already added with the data seeder, so you'll see the following lines in the logs. These are the records fetched from the `Books` table.

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