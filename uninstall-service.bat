@echo off
cls

NET FILE 1>NUL 2>NUL
if '%errorlevel%' == '0' ( goto gotPrivileges ) else ( goto getPrivileges )

:getPrivileges
  echo You have no admin rights! You need to start your CMD in admin mode to uninstall the windows service
  goto end

:gotPrivileges
  net stop "AbpWindowsService" 
  taskkill /F /IM mmc.exe 
  sc delete "AbpWindowsService"

:end
  pause






