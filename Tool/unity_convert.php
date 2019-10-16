<?php
/**
 * Copyright 2016 - 2019, Mage Education Technology Co. Ltd (https://mage.com.vn)
 * @category   Automation Tool
 * @package    Unity Converter
 * @version    1.0.00
 * @author     Original Author nghia@mage.com.vn
 * @copyright  Mage Education Technology Co. Ltd (https://mage.com.vn)
 *
 * Usage:
 * convert [-update][-add] [SourceFolderName] [SourcePackageName] [NewPackageName]
 */

/**
 ****************************************************************************************
 ** variables declaration
 **/
$excludeFolders = [
    "Demigiant"
];

$metaFileExtensions = [
    "mat",
    "anim",
    "prefab",
    "unity",
    "asset",
    "guiskin",
    "preset",
    "fontsettings",
    "controller",
    "pdf",
    "meta",
    "playable"
];

$pluginsTobeCleaned = [
    "Demigiant",
    "DOTween"
];

$updatedGuid = [];

$unityRunningNumber = 0;

$metaFilesTobeUpdated = [];


/**
 ****************************************************************************************
 ** parameter processing
 **/
if ('update' != $argv[1] && 'add' != $argv[1]) {
    echo "Invalid action (-update|-add). \r\nComand format: php -f unity_convert [-update][-add] [SourceFolderName] [SourcePackageName] [NewPackageName]";
    exit;
}

if ('add' == $argv[1]) {
    // check if all parameters are provided
    if (null == $argv[2] || null == $argv[3] || null == $argv[4]) {
        echo "Missing parameter for '-add' command.\r\nComand format: php -f unity_convert [-update][-add] [SourceFolderName] [SourcePackageName] [NewPackageName]";
        exit;
    }

    $scanFolder = $argv[2];
    $sourcePackageName = $argv[3];
    $newPackageName = $argv[4];

    if (!is_dir("$scanFolder")) {
        echo "$scanFolder is not valid directory.";
        exit;
    }

    if (!is_dir("{$scanFolder}\\{$sourcePackageName}")) {
        echo "{$scanFolder}\\{$sourcePackageName} is not valid directory.";
        exit;
    }

    echo "Adding new character name {$argv[4]}";

    shell_exec("attrib -H -S " . __DIR__. "\\$scanFolder\\*.* /D /S");
    echo "Converting all files to not hidden. \r\n";

    if (file_exists("{$scanFolder}\\{$newPackageName}") ) {
        echo "Folder {$scanFolder}\\{$newPackageName} exists. Please delete before process or use '-update' instead \r\n";
        exit;
    }

    // call add new package
    addNewPackage($scanFolder, $sourcePackageName, $newPackageName, $excludeFolders);

    exit;
}

if ('update' == $argv[1]) {
    // check if all parameters are provided
    if (null == $argv[2] || null == $argv[3] || null == $argv[4]) {
        echo "Missing parameter for 'update' command.\r\nComand format: php -f unity_convert [-update][-add] [SourceFolderName] [SourcePackageName] [NewPackageName]";
        exit;
    }

    $scanFolder = $argv[2];
    $sourcePackageName = $argv[3];
    $newPackageName = $argv[4];

    if (!is_dir("$scanFolder")) {
        echo "$scanFolder is not valid directory.";
        exit;
    }

    if (!is_dir("{$scanFolder}\\{$sourcePackageName}")) {
        echo "{$scanFolder}\\{$sourcePackageName} is not valid directory.";
        exit;
    }

    echo "Updating {$argv[4]}";

    shell_exec("attrib -H -S " . __DIR__. "\\$scanFolder\\*.* /D /S");
    echo "Converting all files to not hidden. \r\n";

    if (!file_exists("{$scanFolder}\\{$newPackageName}") ) {
        echo "Folder {$scanFolder}\\{$newPackageName} does not exists. Please run 'add' command instead \r\n";
        exit;
    }

    // call add new package
    updatePackage($scanFolder, $sourcePackageName, $newPackageName, $excludeFolders);

    exit;
}

exit;


/**
 *****************************************************************************************
 */
// utility functions to be used

function checkExclude($inputList, $check) {
    $found = false;

    foreach ($inputList as $i) {
        if (strcmp($check, $i) ==0) {
            $found = true;
            break;
        }
    }

    return $found;
}

function checkMetaFile($extension) {
    $found = false;
    global $metaFileExtensions;

    foreach ($metaFileExtensions as $i) {
        if (strcmp($extension, $i) ==0) {
            $found = true;
            break;
        }
    }

    return $found;
}

function checkTobeDeletedPlugins($check) {

    global $pluginsTobeCleaned;

    $found = false;

    foreach ($pluginsTobeCleaned as $i) {
        if (strcmp($check, $i) ==0) {
            $found = true;
            break;
        }
    }

    return $found;
}
/*****************************************************************************************/
function addNewPackage($scanFolder, $sourcePackageName, $newPackageName, $excludeFolders) {
    global $updatedGuid;
    global $unityRunningNumber;
    global $metaFilesTobeUpdated;
    $metaFilesTobeUpdated = [];

    // 1. copy scanfolder to new
    recursiveAddingPackage("{$scanFolder}\\{$sourcePackageName}", $sourcePackageName, "{$scanFolder}\\{$newPackageName}", $newPackageName);

    // 2. scan fame and update guid for meta files
    scanSourcesMeta("{$scanFolder}\\{$sourcePackageName}", $excludeFolders, $newPackageName);

    // 3. rescan and update guid for the remaining files
    scanAndUpdatedNewGuid("{$scanFolder}\\{$newPackageName}", $newPackageName);

}

function updatePackage($scanFolder, $sourcePackageName, $newPackageName, $excludeFolders) {
    global $updatedGuid;
    global $unityRunningNumber;
    global $metaFilesTobeUpdated;
    $metaFilesTobeUpdated = [];

    // 1. copy scanfolder to new
    recursiveUpdatingPackage("{$scanFolder}\\{$sourcePackageName}", $sourcePackageName, "{$scanFolder}\\{$newPackageName}", $newPackageName);

    // 2. scan fame and update guid for meta files
    scanSourcesMeta("{$scanFolder}\\{$sourcePackageName}", $excludeFolders, $newPackageName);

    // 3. rescan and update guid for the remaining files
    scanAndUpdatedNewGuid("{$scanFolder}\\{$newPackageName}", $newPackageName);

}

/**
 *****************************************************************************************
 */
 // file copy handling
 // recursive copy folder content
 function recursiveCopy($src, $dst) {
     $dir = opendir($src);
     @mkdir($dst);
     while(false !== ( $file = readdir($dir)) ) {
         if (( $file != '.' ) && ( $file != '..' )) {
             if ( is_dir($src . '/' . $file) ) {
                 recursiveCopy($src . '/' . $file,$dst . '/' . $file);
             }
             else {
                 copy($src . '/' . $file,$dst . '/' . $file);
             }
         }
     }
     closedir($dir);
 }

 // recursive adding package
 // replace the source package name by the new package name
 function recursiveAddingPackage($srcFolder, $sourcePackageName, $dstFolder, $newPackageName) {
     global $metaFilesTobeUpdated;

     $dir = opendir($srcFolder);
     @mkdir($dstFolder);
     while(false !== ( $file = readdir($dir)) ) {
         if (( $file != '.' ) && ( $file != '..' )) {
             if ( is_dir($srcFolder . '/' . $file) ) {
                 recursiveAddingPackage($srcFolder . '/' . $file, $sourcePackageName, $dstFolder . '/' . $file, $newPackageName);
             }
             else {

                 preg_match("/(^$sourcePackageName)\_(\S+)/", $file, $matches);
                 echo "Check filename: $file with $sourcePackageName. Found {$matches[0]} {$matches[1]} {$matches[2]}\r\n";

                 if (isset($matches[0]) && $matches[1] == $sourcePackageName) {
                     echo "Copy ". $srcFolder . '/' . $file . " to " . $dstFolder . '/' . "{$newPackageName}_{$matches[2]}".  "\r\n";
                     copy($srcFolder . '/' . $file, $dstFolder . '/' . "{$newPackageName}_{$matches[2]}");

                     $metaFilesTobeUpdated[$dstFolder . '/' . "{$newPackageName}_{$matches[2]}"] = true;
                 } else {
                     copy($srcFolder . '/' . $file, $dstFolder . '/' . $file);
                     $metaFilesTobeUpdated[$dstFolder . '/' . $file] = true;
                 }
             }
         }
     }
     closedir($dir);
 }

 // recursive updating package
 // 1. copy new files
 // 2. replace the source package name by the new package name
 function recursiveUpdatingPackage($srcFolder, $sourcePackageName, $dstFolder, $newPackageName) {
     global $metaFilesTobeUpdated;

     $dir = opendir($srcFolder);
     @mkdir($dstFolder);
     while(false !== ( $file = readdir($dir)) ) {
         if (( $file != '.' ) && ( $file != '..' )) {
             if ( is_dir($srcFolder . '/' . $file) ) {
                 recursiveUpdatingPackage($srcFolder . '/' . $file, $sourcePackageName, $dstFolder . '/' . $file, $newPackageName);
             }
             else {
                 preg_match("/(^$sourcePackageName)\_(\S+)/", $file, $matches);

                 if (isset($matches[0]) && $matches[1] == $sourcePackageName) {
                     if (!file_exists($dstFolder . '/' . "{$newPackageName}_{$matches[2]}") ) {
                         copy($srcFolder . '/' . $file, $dstFolder . '/' . "{$newPackageName}_{$matches[2]}");
                         $metaFilesTobeUpdated[$dstFolder . '/' . "{$newPackageName}_{$matches[2]}"] = true;
                     }
                 } else {
                     if (!file_exists($dstFolder . '/' . $file) ) {
                         copy($srcFolder . '/' . $file, $dstFolder . '/' . $file);
                         $metaFilesTobeUpdated[$dstFolder . '/' . $file] = true;
                     }

                 }
             }
         }
     }
     closedir($dir);
 }

 // recursive delete folder
 function removeDirectory($path) {
  	$files = glob($path . '/*');
 	foreach ($files as $file) {
 		is_dir($file) ? removeDirectory($file) : unlink($file);
 	}
 	rmdir($path);
  	return;
 }
/**
 **********************************************************************************
 **/
 // meta file and guid handling
 // scan sources metafile and map with new guid
function scanSourcesMeta($path, $excludes, $namespace)
{
    global $unityRunningNumber;
    global $metaFilesTobeUpdated;

    $ffs = scandir($path);

    unset($ffs[array_search('.', $ffs, true)]);
    unset($ffs[array_search('..', $ffs, true)]);

    // prevent empty ordered elements
    if (count($ffs) < 1)
        return;

    foreach($ffs as $ff){
        if ($ff[0] != ".") {
            if(is_dir($path.'/'.$ff)) {

                //exclude folder if in the exclude list
                if (!checkExclude($excludes, $ff)) {
                    scanSourcesMeta($path.'/'.$ff, $excludes, $namespace);
                } else {
                    echo "Exclude folder: $path/$ff \r\n";
                }

            } else {
                if (file_exists($path.'/'.$ff)) {
                    // get uploaded file extension
                    $fileExtension = pathinfo($path.'/'.$ff, PATHINFO_EXTENSION);

                    if ("meta" == $fileExtension) {
                        getMetaFileGuid($path.'/'.$ff, $namespace);
                    }
                }
            }
        }
    }
}

//scan meta file and update guid to new format using md5 with current namespace
function getMetaFileGuid($filename, $namespace) {
    global $updatedGuid;
    echo "Reading meta file: $filename\r\n";
    $file = fopen($filename, "r");

    $output = '';
    $has_content = true;

    $found_guid = false;

    //scan through file
    while (! feof($file)) {
        $line = fgets($file);

        //find for guid line
        preg_match('/(.*)(guid\s*\:\s*)([a-f0-9]{32})(.*)/', $line, $matches);

        if (isset($matches[0]) && $matches[0] != null) {

            // check against system guid mask

            preg_match('/(0000000000000000[1-f]000000000000000)/', $matches[3], $systemMatches);

            if ($systemMatches == null) {
                $found_guid = true;
                //$output .= $matches[1].$matches[2].md5($matches[3].$namespace).$matches[4]."\n";
                //$updatedGuid[$matches[3]] = true;
                $updatedGuid[$matches[3]] = md5($matches[3].$namespace);
                echo "Adding source guid: [{$matches[3]} => {$updatedGuid[$matches[3]]}]\r\n";
            }
        }
    }

    fclose($file);
}

// scan target file and match guid and source guid, if match then update to new guid
function scanAndUpdatedNewGuid($path, $namespace)
{
    global $metaFilesTobeUpdated;
    $ffs = scandir($path);

    unset($ffs[array_search('.', $ffs, true)]);
    unset($ffs[array_search('..', $ffs, true)]);

    // prevent empty ordered elements
    if (count($ffs) < 1)
        return;

    foreach($ffs as $ff){
        if ($ff[0] != ".") {
            if(is_dir($path.'/'.$ff)) {
                scanAndUpdatedNewGuid($path.'/'.$ff, $namespace);
            } else {
                if (file_exists($path.'/'.$ff)) {
                    // get uploaded file extension
                    $fileExtension = pathinfo($path.'/'.$ff, PATHINFO_EXTENSION);
                    if (checkMetaFile($fileExtension)) {
                        // try with update every file first
                        //if (isset($metaFilesTobeUpdated[$path.'/'.$ff]) && $metaFilesTobeUpdated[$path.'/'.$ff] == true) {
                            updatedNewGuid($path.'/'.$ff, $namespace);
                        //}
                    }
                }
            }
        }
    }
}

// scan target file and match guid and source guid, if match then update to new guid
function updatedNewGuid($filename, $namespace) {
    global $updatedGuid;
    echo "Updating meta file: $filename\r\n";
    $file = fopen($filename, "r");

    $output = '';
    $has_content = true;

    $found_guid = false;

    //scan through file
    while (! feof($file)) {
        $line = fgets($file);

        //find for guid line
        preg_match('/(.*)(guid\s*\:\s*)([a-f0-9]{32})(.*)/', $line, $matches);

        if (isset($matches[0]) && $matches[0] != null) {

            // check against system guid mask
            if (isset($updatedGuid[$matches[3]]) && $updatedGuid[$matches[3]] != null) {
                $found_guid = true;
                // replace the source guid by the new guid
                echo "Update guid: {$matches[3]} => {$updatedGuid[$matches[3]]} \r\n";
                $output .= $matches[1].$matches[2].$updatedGuid[$matches[3]].$matches[4]."\n";
                $updatedGuid[] = $matches[3];
            } else {
                $output .= "$line";
            }

        } else {
            $output .= "$line";
        }

    }

    fclose($file);

    // there is no guid, then exit
    if (!$found_guid) {
        return;
    }

    //save file
    $fh = fopen($filename, "w");
    fwrite($fh, $output);
    fclose($fh);
}

?>
