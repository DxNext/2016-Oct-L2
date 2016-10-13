@echo off
%~d0
cd "%~dp0"

IF EXIST %WINDIR%\SysWow64 (
set powerShellDir=%WINDIR%\SysWow64\windowspowershell\v1.0
) ELSE (
set powerShellDir=%WINDIR%\system32\windowspowershell\v1.0
)

call %powerShellDir%\powershell.exe -Command Set-ExecutionPolicy unrestricted

:MENU
ECHO.
ECHO Select operation to perform
ECHO ===========================
ECHO.
ECHO 1 - Generate sample data files
ECHO 2 - Upload sample files to Azure Storage
ECHO 3 - Create SQL Data Warehouse
ECHO 4 - Create HDI cluster
ECHO 5 - EXIT
ECHO.

SET /P M=Type option number then press ENTER: 

IF %M%==1 GOTO GENDATA
IF %M%==2 GOTO UPLOAD
IF %M%==3 GOTO DW
IF %M%==4 GOTO HDI
GOTO EOF

:GENDATA
call %powerShellDir%\powershell.exe -Command "&'.\generateSampleData.ps1'
GOTO MENU

:UPLOAD
call %powerShellDir%\powershell.exe -Command "&'.\uploadFiles.ps1'
GOTO MENU

:DW
call %powerShellDir%\powershell.exe -Command "&'.\createDataWarehouse.ps1'
GOTO MENU

:HDI
call %powerShellDir%\powershell.exe -Command "&'.\createHDICluster.ps1'
GOTO MENU

pause

:EOF