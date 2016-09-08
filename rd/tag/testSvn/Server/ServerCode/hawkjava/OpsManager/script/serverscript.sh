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
uri=${10}
args=${11}
#echo port:${port}, scriptPort:${scriptPort}, dbHost:${dbHost}, dbPort:${dbPort}, dbName:${dbName}, dbUser:${dbUser}, dbPwd:${dbPwd}, workPath:${workPath}, pid:${pid}

if [ -z ${args} ];then
	curl --connect-timeout 5 -m 10 http://127.0.0.1:${scriptPort}/${uri}?user=hawk
else
	curl --connect-timeout 5 -m 10 http://127.0.0.1:${scriptPort}/${uri}?user=hawk\&params=${args}
fi
