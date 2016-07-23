# !/usr/bin/env python
# Filename:install.py
# Copyright 2010-present Funplus.
#
# This script install the Funplus iOS Sdk  package
# Support Xcode 4, 5 & 6 projects.
# ---------------------------------------------------------------------------


def main():
	import os
	import sys
	from mod_pbxproj import XcodeProject
	import argparse
	import json

	arg_parser = argparse.ArgumentParser(description="Modify an xcode project file using a single command at a time.")
	arg_parser.add_argument('project', help="Project path")
	args = arg_parser.parse_args()

	py_path = os.path.abspath(os.path.dirname(sys.argv[0]))
	Funplus_sdk_path = os.path.join(py_path, os.pardir, os.pardir)

	#Load the project file
	if os.path.isdir(args.project) :
		pbxproj = os.path.join(args.project, 'project.pbxproj')

	if not os.path.isfile(pbxproj) :
		raise Exception("Project File not found")

	project = XcodeProject.Load(pbxproj)
	
	#Mods file
	mods_path = os.path.join(py_path, os.pardir, "Mods")
	for root, dirs, files in os.walk(mods_path):
		for name in files:
			if os.path.splitext(name)[1] == ".projmods":
					jsonFile = file(os.path.join(root, name))
					try:
						jsonString = json.load(jsonFile)
					except ValueError as e:
						print(".projmods is broken", json.last_error_position)
					project.apply_mods(mod_dict=jsonString, default_path=Funplus_sdk_path)
					
	#Project save
	project.save()

if __name__ == "__main__":
	main()
