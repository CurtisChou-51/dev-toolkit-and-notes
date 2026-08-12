@echo off
setlocal
chcp 65001 >nul

echo Restoring display / sleep timeouts ...
echo.

powercfg /change monitor-timeout-ac 5 || goto :failed
powercfg /change monitor-timeout-dc 3 || goto :failed
powercfg /change standby-timeout-ac 5 || goto :failed
powercfg /change standby-timeout-dc 3 || goto :failed

echo   Plugged in   display: 5 min   sleep: 5 min
echo   On battery   display: 3 min   sleep: 3 min
echo.
echo [OK] Original settings restored.
goto :done

:failed
echo.
echo [FAILED] powercfg returned errorlevel %errorlevel%.
echo          Try again via "Run as administrator".

:done
echo.
echo %cmdcmdline% | find /i "%~nx0" >nul && pause
endlocal
