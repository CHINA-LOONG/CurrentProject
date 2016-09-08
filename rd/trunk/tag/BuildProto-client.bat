@echo protocol file generator...
@echo off
for /r %%i in (./Protocol/*.proto) do (
	@echo general ./Protocol/%%~ni
	protogen.exe  -i:./Protocol/%%~ni.proto -o:./Protobuf/C#/%%~ni.cs -ns:PB
)
@pause
@echo done!