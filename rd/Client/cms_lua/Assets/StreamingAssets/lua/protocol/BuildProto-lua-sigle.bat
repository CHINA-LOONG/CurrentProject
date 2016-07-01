@echo protocol file generator...
@echo off
set /p filename=input the filename(without suffix):
@echo general %filename%.proto
_protoc.exe --lua_out=./Protobuf/Lua/ --plugin=protoc-gen-lua=".\Plugin\protoc-gen-lua.bat" %filename%.proto
@echo done!
@pause