@echo protocol file generator...
@echo off
for /r %%i in (*.proto) do (
	@echo general %%~ni
	protogen.exe  -i:%%~ni.proto -o:%%~ni.cs -ns:PB
)
@pause
@echo done!