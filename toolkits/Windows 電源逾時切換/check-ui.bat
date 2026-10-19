@echo off
setlocal
chcp 65001 >nul

control /name Microsoft.PowerOptions /page pagePlanSettings
control desk.cpl,,@screensaver

endlocal
