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

count=0
line=`netstat -nltp | grep ${port} | awk '{print $7}' | awk -F / '{print $1}' | wc -l`
if [ ${line} -gt 0 ];then
	pid=`netstat -nltp | grep ${port} | awk '{print $7}' | awk -F / '{print $1}'`
	kill ${pid}
	while [ `ps -elf | grep 'java' | grep '${pid}' | grep -v grep | wc -l` -gt 0 ]
	do
		count=`expr $count + 1`
		if [ $count -ge 10 ];then
			break
		fi
        sleep 1
	done
fi

echo 'stop success'
