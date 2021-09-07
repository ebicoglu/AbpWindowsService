net stop "AbpWindowsService"
taskkill /F /IM mmc.exe
sc delete "AbpWindowsService"