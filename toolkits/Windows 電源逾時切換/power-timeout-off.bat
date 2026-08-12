@echo off
setlocal
chcp 65001 >nul

echo Disabling display / sleep timeouts ...
echo.

powercfg /change monitor-timeout-ac 0 || goto :failed
powercfg /change monitor-timeout-dc 0 || goto :failed
powercfg /change standby-timeout-ac 0 || goto :failed
powercfg /change standby-timeout-dc 0 || goto :failed

echo   Plugged in   display: never   sleep: never
echo   On battery   display: never   sleep: never
echo.
echo [OK] Auto display-off and auto sleep are now disabled.
goto :done

:failed
echo.
echo [FAILED] powercfg returned errorlevel %errorlevel%.
echo          Try again via "Run as administrator".

:done
echo.
echo %cmdcmdline% | find /i "%~nx0" >nul && pause
endlocal
