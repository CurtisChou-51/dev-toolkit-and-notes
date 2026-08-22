@echo off
setlocal
chcp 65001 >nul

echo Restoring display / sleep timeouts ...
echo.

powercfg /change monitor-timeout-ac 5 || goto :failed
powercfg /change monitor-timeout-dc 3 || goto :failed
powercfg /change standby-timeout-ac 5 || goto :failed
powercfg /change standby-timeout-dc 3 || goto :failed

echo Restoring secure screen saver ...
echo.

reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive    /t REG_SZ /d 1   /f >nul || goto :failed
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveTimeOut   /t REG_SZ /d 300 /f >nul || goto :failed
reg add "HKCU\Control Panel\Desktop" /v ScreenSaverIsSecure /t REG_SZ /d 1   /f >nul || goto :failed

REM reg alone only takes effect after re-login; SPI_SETSCREENSAVETIMEOUT / SPI_SETSCREENSAVEACTIVE apply it to the running session.
powershell -NoProfile -Command "$q=[char]34; Add-Type -Namespace Spi -Name N -MemberDefinition ('[DllImport('+$q+'user32.dll'+$q+', SetLastError=true)] public static extern bool SystemParametersInfo(uint a, uint b, System.IntPtr c, uint d);'); [void][Spi.N]::SystemParametersInfo(15,300,[System.IntPtr]::Zero,3); [void][Spi.N]::SystemParametersInfo(17,1,[System.IntPtr]::Zero,3)" || goto :failed

echo   Plugged in    display: 5 min   sleep: 5 min
echo   On battery    display: 3 min   sleep: 3 min
echo   Screen saver  lock: on   timeout: 5 min
echo.
echo [OK] Original settings restored.
goto :done

:failed
echo.
echo [FAILED] errorlevel %errorlevel%.
echo          Try again via "Run as administrator".

:done
echo.
echo %cmdcmdline% | find /i "%~nx0" >nul && pause
endlocal
