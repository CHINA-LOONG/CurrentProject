dbHost=${3}
dbPort=${4}
dbName=${5}
dbUser=${6}
dbPwd=${7}
workPath=${8}
pid=${9}
updateUrl=${10}
#echo port:${port}, scriptPort:${scriptPort}, dbHost:${dbHost}, dbPort:${dbPort}, dbName:${dbName}, dbUser:${dbUser}, dbPwd:${dbPwd}, workPath:${workPath}, pid:${pid}

#测试用
#workPath='/data/java/legend_test_sever/trunk/jar/ops/'
#updateUrl='http://123.59.62.235/qmby_sync/'

filelist=${updateUrl}filelist.txt
filePath=${workPath}filelist.txt
syncLog=${workPath}synclog.txt

function update(){
	fileName=$1
	md5=$2

	pos=`echo "$fileName" | awk -F / '{printf "%d", length($0)-length($NF)}'`
	if [ $pos -gt 0 ];then
		mkdir -p ${workPath}${fileName:0:$pos-1}
	fi

	fileMd5='none'
	if [ -f "${workPath}${fileName}" ];then
		fileMd5=`md5sum ${workPath}${fileName} | cut -d ' ' -f1`
	fi

	if [ ${md5} != ${fileMd5} ];then
		curl --connect-timeout 5 -m 10 -s -o ${workPath}${fileName} ${updateUrl}${fileName}
		echo 'sync file: '${fileName} >> ${syncLog}
	fi

	newMd5=`md5sum ${workPath}${fileName} | cut -d ' ' -f1`
	if [ ${md5} != ${newMd5} ];then
		echo 'md5sum failed: '${fileName} >> ${syncLog}
	fi
}

rm -rf ${filePath}
>${syncLog}
curl -s -o ${filePath} ${filelist}

if [ -f "${filePath}" ];then
        cat ${filePath} | while read line
        do
                eval $(echo $line | awk {'printf("fileName=%s;md5=%s",$1,$2)'})
                update $fileName $md5
        done
	echo 'sync success'
	cat ${syncLog}
else
        echo 'update filelist failed'
fi
