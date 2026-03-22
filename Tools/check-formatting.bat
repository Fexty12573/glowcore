@echo off

dotnet format ./Tools/Lint/GlowCore.Lint.csproj --verify-no-changes --severity error
if %ERRORLEVEL% NEQ 0 (
    echo Code formatting issues detected. Please run 'format.bat' to fix them.
    exit /b 1
) else (
    echo Code formatting is correct.
)
