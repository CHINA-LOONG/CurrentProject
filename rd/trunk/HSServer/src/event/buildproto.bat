@echo protocol file generator...
@echo off
for /r %%i in (*.proto) do (
	@echo general %%~ni.proto
	protoc.exe  --cpp_out=../src/event %%~ni.proto
)

copy *.cc *.cpp /Y
del *.cc

@echo done!
@pause

