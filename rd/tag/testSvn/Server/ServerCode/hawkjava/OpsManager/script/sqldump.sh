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

dateStr=`date "+%Y%m%d-%H:%M:%S"`
mkdir -p ${workPath}sqlDump
mysqldump -h${dbHost} -P${dbPort} -u${dbUser} -p${dbPwd} ${dbName} > ${workPath}sqlDump/${dbName}.sql.${dateStr}

echo 'sqldump success'
