@echo off
set DirName=%~dp0
pushd "%DirName%"
for /f "delims=" %%f in ('dir /ah/a-d/b/s') do (
if %%~zf gtr 52428800 (
echo ����50M���ļ���   %%f  ��СΪ%%~zf)
)
echo.&pause