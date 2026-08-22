@echo off
setlocal
chcp 65001 >nul

echo Disabling display / sleep timeouts ...
echo.

powercfg /change monitor-timeout-ac 0 || goto :failed
powercfg /change monitor-timeout-dc 0 || goto :failed
powercfg /change standby-timeout-ac 0 || goto :failed
powercfg /change standby-timeout-dc 0 || goto :failed

echo Disabling secure screen saver ...
echo.

reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d 0 /f >nul || goto :failed

REM reg alone only takes effect after re-login; SPI_SETSCREENSAVEACTIVE applies it to the running session.
powershell -NoProfile -Command "$q=[char]34; Add-Type -Namespace Spi -Name N -MemberDefinition ('[DllImport('+$q+'user32.dll'+$q+', SetLastError=true)] public static extern bool SystemParametersInfo(uint a, uint b, System.IntPtr c, uint d);'); [void][Spi.N]::SystemParametersInfo(17,0,[System.IntPtr]::Zero,3)" || goto :failed

echo   Plugged in    display: never   sleep: never
echo   On battery    display: never   sleep: never
echo   Screen saver  lock: off
echo.
echo [OK] Auto display-off, auto sleep and idle screen lock are now disabled.
goto :done

:failed
echo.
echo [FAILED] errorlevel %errorlevel%.
echo          Try again via "Run as administrator".

:done
echo.
echo %cmdcmdline% | find /i "%~nx0" >nul && pause
endlocal
