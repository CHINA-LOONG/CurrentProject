@echo off
set DirName=%~dp0
pushd "%DirName%"
for /f "delims=" %%f in ('dir /ah/a-d/b/s') do (
if %%~zf gtr 52428800 (
echo 大于50M的文件：   %%f  大小为%%~zf)
)
echo.&pause