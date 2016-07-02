@echo protocol file generator...
@echo off
for /r %%i in (./Protocol/*.proto) do (
	@echo general ./Protocol/%%~ni.proto
	protoc.exe  --java_out=./Protobuf/Java/src  --proto_path=./Protocol ./Protocol/%%~ni.proto
)

@echo done!
@pause

