@echo protocol file generator...
@echo off
for /r %%i in (*.proto) do (
	@echo general %%~ni.proto
	protoc.exe  --cpp_out=../src/event %%~ni.proto
)

copy ..\src\event\*.cc ..\src\event\*.cpp /Y
del ..\src\event\*.cc

@echo done!
@pause

