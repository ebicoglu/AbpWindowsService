@echo off
cls

NET FILE 1>NUL 2>NUL
if '%errorlevel%' == '0' ( goto gotPrivileges ) else ( goto getPrivileges )

:getPrivileges
  echo You have no admin rights! You need to start your CMD in admin mode to install the windows service
  goto end

:gotPrivileges
  set current=%~dp0
  set bin="%current%src\AbpWindowsService\bin\Debug\net5.0\win-x64\AbpWindowsService.exe"
  echo Creating service with the name "AbpWindowsService"
  sc create "AbpWindowsService" binPath=%bin%

:end
  pause




