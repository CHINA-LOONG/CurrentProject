import os
import sys
import json
import shutil
import plistlib as Plist
from sys import argv
from mod_pbxproj import XcodeProject

path = argv[1]
native_sdk_path = argv[2] + '/../fun_sdk_ios'
jsonFilePath = argv[2]  + '/FunSdk/Plugins/iOS/FunSdk.json'

def addUrlScheme(plist, sns, sns_short, id):
    scheme_value = sns_short+id
    new_dict = {'CFBundleTypeRole': 'Editor', 'CFBundleURLName': sns, 'CFBundleURLSchemes': [scheme_value]}
    if "CFBundleURLTypes" in plist:
        plist["CFBundleURLTypes"].append(new_dict.copy())
    else:
        plist["CFBundleURLTypes"] = [new_dict]

def copytree(src, dst, symlinks=False, ignore=None):
    if not os.path.exists(dst):
        os.makedirs(dst)
    for item in os.listdir(src):
        s = os.path.join(src, item)
        d = os.path.join(dst, item)
        if os.path.isdir(s):
            copytree(s, d, symlinks, ignore)
        else:
            if not os.path.exists(d) or os.stat(src).st_mtime - os.stat(dst).st_mtime > 1:
                shutil.copy2(s, d)

print('---------Funplus Unity SDK, Prepare for excuting our magic scripts------------')
print('Funplus Unity SDK, post_process.py xcode build path --> ' + path)
print('Funplus Unity SDK, post_process.py native ios sdk path --> ' + native_sdk_path)
print('Funplus Unity SDK, post_process.py json file path  --> ' + jsonFilePath)

print('Funplus Unity SDK, Step 1: Copy Native SDK to {path}'.format(path=native_sdk_path))
copytree(native_sdk_path, path  + '/fun_sdk_ios')

print('Funplus Unity SDK, Step 2: rewrite info.plist ')
#read json to check which config item should write to info.plist
with open(jsonFilePath) as config_file:
    config_items = json.load(config_file)

if config_items['enablefb'] == 'True':
    print 'enabled fb login'
    fb_appid = config_items['fbappid']
    fb_app_disname = config_items['fbappdisname']
    if fb_appid and fb_app_disname:
        pl = Plist.readPlist(path+'/Info.plist')
        pl["FacebookAppID"] = fb_appid
        pl["FacebookDisplayName"] = fb_app_disname
        # add url scheme
        addUrlScheme(pl, 'facebook', 'fb', fb_appid)
        Plist.writePlist(pl, path+'/Info.plist')
    else:
        print 'wrong with fb config'

if config_items['enablevk'] == 'True':
    print 'enabled vk login'
    vk_appid = config_items['vkappid']
    if vk_appid :
        pl = Plist.readPlist(path+'/Info.plist')
        pl["VKAppID"] = vk_appid
        # add url scheme
        addUrlScheme(pl, 'vk', 'vk', vk_appid)
        Plist.writePlist(pl, path+'/Info.plist')
    else:
        print 'wrong with vk config'

if config_items['enablewx'] == 'True':
    print 'enabled wechat login'
    wx_appid = config_items['wxappid']
    wx_appkey = config_items['wxappkey']
    wx_msdk_url = config_items['wxmsdkurl']
    wx_msdk_offerid = config_items['wxmsdkofferid']
    if wx_appid and wx_appkey and wx_msdk_url:
        pl = Plist.readPlist(path+'/Info.plist')
        pl["WXAppID"] = wx_appid
        pl["WXAppKey"] = wx_appkey
        pl["MSDK_URL"] = wx_msdk_url
        pl["MSDK_OfferId"] = wx_msdk_offerid
        # add url scheme
        addUrlScheme(pl, 'wechat', 'wx', wx_appid)
        Plist.writePlist(pl, path+'/Info.plist')
    else:
        print 'wrong with wechat config'

fileShare = True
if config_items['iTunesfilesharing'] == 'YES':
    fileShare = True
else:
    fileShare = False


pl = Plist.readPlist(path+'/Info.plist')
pl["UIFileSharingEnabled"] = fileShare
Plist.writePlist(pl,path+'/Info.plist')

print('Funplus Unity SDK, Step 3: change build setting')
project = XcodeProject.Load(path +'/Unity-iPhone.xcodeproj/project.pbxproj')

project.add_other_ldflags('-all_load')
project.add_other_ldflags('-Objc')
#project.add_other_buildsetting('CLANG_CXX_LIBRARY', 'libstdc++')

if project.modified:
    project.backup()
    project.save()

print('Funplus Unity SDK, Step 4: run install.py in SDK')
installScriptPath = path  + '/fun_sdk_ios/install/mod_pbxproj/install.py'

os.system('chmod 755 {path}'.format(path=installScriptPath))
os.system('python {script_path} {pro_path}'.format(script_path=installScriptPath, pro_path=path +'/Unity-iPhone.xcodeproj'))

print('----------------------------------end for excuting our magic scripts to tweak our xcode ----------------------------------')

