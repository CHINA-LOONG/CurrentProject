@echo protocol file generator...
@echo off
for /r %%i in (./Protocol/*.proto) do (

	@echo general ./Protocol/%%~ni.proto

	protoc.exe --java_out=./Protobuf/Java/src ./Protocol/%%~ni.proto --proto_path=./Protocol

)
@echo done!
pause