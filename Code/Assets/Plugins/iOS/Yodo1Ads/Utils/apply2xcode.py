#!/usr/bin/python

# argv[1] Your xcode project path
# argv[2] Your project Name  like test.xcodeproj
# argv[3] Kryptanium SDK path. We will copy the SDK to the project directory.
#


import os
import sys
import shutil

from mod_pbxproj import XcodeProject

##### Common Settings.
projectPath = sys.argv[1]

frameworkPath = 'System/Library/Frameworks/';
dylibPath = '/usr/lib/';
tbdPath = '/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk/usr/lib/';


#### Kryptannium SDK Settings.
# Dependent Frameworks
frameworks = [
{'name':'CoreFoundation.framework','weak':False},
{'name':'AdSupport.framework','weak':False},
{'name':'EventKit.framework','weak':False},
{'name':'EventKitUI.framework','weak':False},
{'name':'CoreData.framework','weak':False},
{'name':'Photos.framework','weak':False},
{'name':'OpenGLES.framework','weak':False},
{'name':'UIKit.framework','weak':False},
{'name':'SystemConfiguration.framework','weak':False},
{'name':'QuartzCore.framework','weak':False},
{'name':'MobileCoreServices.framework','weak':False},
{'name':'ImageIO.framework','weak':False},
{'name':'Foundation.framework','weak':False},
{'name':'CoreGraphics.framework','weak':False},
{'name':'CFNetwork.framework','weak':False},
{'name':'AudioToolbox.framework','weak':False},
{'name':'AssetsLibrary.framework','weak':False},
{'name':'StoreKit.framework','weak':True},
{'name':'CoreTelephony.framework','weak':False},
{'name':'CoreText.framework','weak':False},
{'name':'MessageUI.framework','weak':False},
{'name':'CoreLocation.framework','weak':False},
{'name':'AddressBook.framework','weak':False},
{'name':'Accounts.framework','weak':False},
{'name':'Social.framework','weak':False},
{'name':'MediaPlayer.framework','weak':False},
{'name':'AVFoundation.framework','weak':False},
{'name':'CoreMedia.framework','weak':False},
{'name':'CoreMotion.framework','weak':False},
{'name':'WebKit.framework','weak':False},
{'name':'GameController.framework','weak':False},
{'name':'WatchConnectivity.framework','weak':True},
{'name':'GLKit.framework','weak':True},
]

# Dependent dylibs.
dylibs = [
'libsqlite3.dylib',
'libz.dylib',
'libz.1.2.5.dylib',
'libresolv.dylib',
'libicucore.dylib',
'libresolv.9.dylib',
'libc++.dylib',
]

tbds = [
'libsqlite3.tbd',
'libz.tbd',
'libz.1.2.5.tbd',
'libresolv.tbd',
'libicucore.tbd',
'libresolv.9.tbd',
'libc++.tbd',
]

#Other linker Flags.
other_ldflags = [
	'-ObjC',
	'-lxml2'
]

#XCODE Build Settings. For Unity 
build_settings = [
#{'key':'GCC_ENABLE_OBJC_EXCEPTIONS','value':'YES'},
]




#### Methods
def flog(log):
	print (log),
	

def add_frameworks(project):
	frameworksGroup = project.get_or_create_group('Frameworks')
	flog('\n======== Add Dependent Frameworks ========\n')
	for f in frameworks:
		frame_name = f['name']
		project.add_file_if_doesnt_exist(frameworkPath + frame_name, parent=frameworksGroup,tree='SDKROOT', weak=f['weak'])
		flog( frame_name + ' \n')


def add_dylibs(project):
	flog('\n======== Add Dependent dylibs ========\n')
	frameworksGroup = project.get_or_create_group('Frameworks')
	for d in dylibs:
		project.add_file_if_doesnt_exist(dylibPath + d, parent=frameworksGroup, tree='SDKROOT')
		flog(d +'  \n')

def add_tbds(project):
	flog('\n======== Add Dependent tbds ========\n')
	frameworksGroup = project.get_or_create_group('Frameworks')
	for d in tbds:
		project.add_file_if_doesnt_exist(tbdPath + d, parent=frameworksGroup, tree='SDKROOT')
		flog(d +'  \n')

def add_other_ldflags(project):
	flog('\n======== Add Other Linker Flags ========\n')
	for f in other_ldflags:
		project.add_other_ldflags(f)
		flog( f + ' \n')


def change_buildsettings(project):
	flog('\n======== Change Build Settings ========\n')
	for s in build_settings:
		project.mod_buildSettings(s['key'],s['value'])
		flog(s['key'] + '=' + s['value'] + '\n')

def save_project(project):
	project.save()


def disableArc(filePath):
    fileId = project.get_file_id_by_path(filePath)
    files = project.get_build_files(fileId)
    for f in files:
        f.add_compiler_flag('-fno-objc-arc')

# if sys.version_info > (3, 0):
#      raise RuntimeError('Below Python 3.0 is required')

#### Script Start.
#flog('Start of python script\n')
flog('project path:\n' + projectPath + '\n')

project = XcodeProject.Load(projectPath + '/project.pbxproj')

add_frameworks(project)

if os.path.isdir(tbdPath):
	add_tbds(project)
else:
	add_dylibs(project)

add_other_ldflags(project)
change_buildsettings(project)
save_project(project)
flog('\nAll done!\n')
#### Script End.
