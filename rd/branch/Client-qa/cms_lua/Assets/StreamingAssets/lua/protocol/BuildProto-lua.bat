@echo protocol file generator...
@echo off
for /r %%i in (*.proto) do (
	@echo general %%~ni.proto
	_protoc.exe --lua_out=../pblua/ --plugin=protoc-gen-lua=".\plugin\protoc-gen-lua.bat" %%~ni.proto
)
@echo done!
rem pause