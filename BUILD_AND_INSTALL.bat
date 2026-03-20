@echo off
IF NOT EXIST "..\BepInEx\interop\Assembly-CSharp.dll" (
    echo ERROR: Launch Among Us with BepInEx installed first, then re-run this.
    pause & exit /b 1
)
dotnet build -c Release
IF %ERRORLEVEL% NEQ 0 ( pause & exit /b 1 )
copy /Y "bin\Release\net6.0\PhantomMenu.dll" "."
echo Done! Launch Among Us.
pause
