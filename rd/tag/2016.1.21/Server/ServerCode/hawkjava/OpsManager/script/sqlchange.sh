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
sqlFile=${10}
#echo port:${port}, scriptPort:${scriptPort}, dbHost:${dbHost}, dbPort:${dbPort}, dbName:${dbName}, dbUser:${dbUser}, dbPwd:${dbPwd}, workPath:${workPath}, pid:${pid}

sqlPath=${workPath}${sqlFile}
if [ -f "${sqlPath}" ];then
	mysql -h${dbHost} -P${dbPort} -u${dbUser} -p${dbPwd} ${dbName} < ${sqlPath}
	echo 'sqlchange success: '${sqlFile}
else
	echo 'sqlfile not found: '${sqlFile}
fi
