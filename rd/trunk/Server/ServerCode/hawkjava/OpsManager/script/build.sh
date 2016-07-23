#!/bin/bash

foreachd(){
	for file in $1/*
	do
		if [ -d $file -a $file != '.' -a $file != '..' -a $file != '.svn' ]
		then
			foreachd $file
		elif [ -f $file ]
		then
			fileName=`echo ${file} | sed 's/.\///'`
			if [ ${fileName} != 'build.sh' -a ${fileName} != 'build_xml.sh' -a ${fileName} != 'filelist.txt' -a ${fileName} != 'svnup.sh' ]
			then
				echo ${fileName} `md5sum $(pwd)/${fileName} | cut -d ' ' -f1` >> filelist.txt
			fi
		fi
	done
}

>filelist.txt
echo "svn up ..."
svn up
if [[ "x$1" == 'x' ]]
then
    foreachd "."
else
    foreachd "$1"
fi

cat filelist.txt
echo 'build success'
