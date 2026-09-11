@echo off
echo ============================================================================
echo   CodeAi Dispatcher - DEV/Candidate Build Script (Process-Isolated)
echo   Target: DEV/TEST sandbox (Port 8788), strictly isolated from PROD
echo ============================================================================
echo.

set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist %CSC% set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist %CSC% (
    echo [ERROR] csc.exe not found on this system.
    exit /b 1
)

if not exist bin-dev mkdir bin-dev

echo [*] Compiling src\CodeAiDispatcher.cs -> bin-dev\CodeAiDispatcher.exe ...
%CSC% /codepage:65001 /target:winexe /out:bin-dev\CodeAiDispatcher.exe /lib:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF","C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /r:UIAutomationClient.dll,UIAutomationTypes.dll,WindowsBase.dll,System.Web.Extensions.dll src\CodeAiDispatcher.cs

if errorlevel 1 (
    echo [FAIL] Compilation failed.
    exit /b 1
)

if exist dispatcher_nodes.json copy /y dispatcher_nodes.json bin-dev\dispatcher_nodes.json >nul

echo.
echo [SUCCESS] Build succeeded: bin-dev\CodeAiDispatcher.exe
echo [NOTE] PROD scripts directory is untouched.
echo.
