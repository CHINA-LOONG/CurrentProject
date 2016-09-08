#!/bin/sh
#读取外部传入参数
port=${1}
scriptPort=${2}
dbHost=${3}
dbPort=${4}
dbName=${5}
dbUser=${6}
dbPwd=${7}
workPath=${8}
pid=${9}
#echo port:${port}, scriptPort:${scriptPort}, dbHost:${dbHost}, dbPort:${dbPort}, dbName:${dbName}, dbUser:${dbUser}, dbPwd:${dbPwd}, workPath:${workPath}, pid:${pid}

line=`netstat -nltp | grep ${port} | awk '{print $7}' | awk -F / '{print $1}' | wc -l`
if [ ${line} -gt 0 ];then
	echo 'server running'
else
	echo 'server stoped'
fi
