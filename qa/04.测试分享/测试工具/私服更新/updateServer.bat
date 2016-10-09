set localDir=E:\work\rd\_server\
set serverDir=\\192.168.199.122\HSshare\_server
set serverId=11


cd /d %localDir%
del /f /q /s *.*
for /d %%i in (*) do rd /s /q %%i

xcopy %serverDir%\*.* %local% /s /e

cd %localDir%/release/GameServer/cfg
@echo off
(for /f "tokens=*" %%i in (gs.cfg) do echo %%i|findstr /ib serverId>nul&&echo serverId = %serverId%||echo %%i)>temp.cfg
move /y temp.cfg gs.cfg

pause