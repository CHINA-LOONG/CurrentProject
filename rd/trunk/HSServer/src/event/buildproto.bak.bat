protoc.exe *.proto --cpp_out=. -I=../ProtoMessage/;./  

copy *.cc *.cpp /Y

del *.cc

pause


