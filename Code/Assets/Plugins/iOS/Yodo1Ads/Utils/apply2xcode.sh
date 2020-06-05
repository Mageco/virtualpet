#!/bin/sh

arg1=$1

if [ ! -n "$arg1" ]; then {
	echo "input xcode .xcodeproj  path "
} else {
  PROJECT_NAME=${arg1}
  python ./apply2xcode.py  ${PROJECT_NAME}
} 
fi

workpath=`dirname $1`
infoplistPath="Info.plist"
paths=`find ${workpath} -name ${infoplistPath}`

for filePath in $paths  
do  
  always=`/usr/libexec/PlistBuddy -c 'Print :NSLocationAlwaysUsageDescription' ${filePath}`
  if [ ! -n "$always" ];then {
    /usr/libexec/PlistBuddy -c 'Add :NSLocationAlwaysUsageDescription string "Some ad content may require access to the location for an interactive ad experience."' ${filePath}
    # echo "添加 [位置访问权限] NSLocationAlwaysUsageDescription "
  }
  fi

  when=`/usr/libexec/PlistBuddy -c 'Print :NSLocationWhenInUseUsageDescription' ${filePath}`
  if [ ! -n "$when" ];then {
    /usr/libexec/PlistBuddy -c 'Add :NSLocationWhenInUseUsageDescription string "Some ad content may require access to the location for an interactive ad experience."' ${filePath}
    # echo "添加 [位置访问权限] NSLocationWhenInUseUsageDescription "
  }
  fi 
  applovin_Key=`/usr/libexec/PlistBuddy -c 'Print :AppLovinSdkKey' ${filePath}`
  if [ ! -n "$applovin_Key" ];then {
    /usr/libexec/PlistBuddy -c 'Add :AppLovinSdkKey string "xcGD2fy-GdmiZQapx_kUSy5SMKyLoXBk8RyB5u9MVv34KetGdbl4XrXvAUFy0Qg9scKyVTI0NM4i_yzdXih4XE"' ${filePath}
  }
  fi 

done




