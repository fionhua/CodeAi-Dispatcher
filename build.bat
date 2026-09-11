@echo off
echo ============================================================================
echo   CodeAi Dispatcher - Native Build Script
echo   Architect: Quantum Court Defensive Engineer Adjudicator (L2.6)
echo ============================================================================
echo.

set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist %CSC% set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist %CSC% (
    echo [ERROR] csc.exe not found on this system.
    pause
    exit /b 1
)

if not exist bin mkdir bin

echo [*] Compiling src\CodeAiDispatcher.cs ...
%CSC% /target:winexe /out:bin\CodeAiDispatcher.exe /lib:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF","C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /r:UIAutomationClient.dll,UIAutomationTypes.dll,WindowsBase.dll,System.Web.Extensions.dll src\CodeAiDispatcher.cs

if errorlevel 1 (
    echo [FAIL] Compilation failed.
    pause
    exit /b 1
)

if exist dispatcher_nodes.json copy /y dispatcher_nodes.json bin\dispatcher_nodes.json >nul

echo.
echo [SUCCESS] Build succeeded: bin\CodeAiDispatcher.exe
echo.
