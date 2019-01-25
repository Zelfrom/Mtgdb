@echo off

set configuration=release
set origin=F:\Repo\Git\mtgDb
set output=%origin%\out
set pub=D:\Distrib\games\mtg\Mega\Mtgdb.Gui

for /f "delims=" %%x in (%origin%\shared\SolutionInfo.cs) do @set version=%%x
rem [assembly: AssemblyVersion("1.3.5.20")]
set version=%version:~28,-3%

set targetRoot=D:\Distrib\games\mtg\Packaged\%version%
set packageName=Mtgdb.Gui.v%version%
set target=%targetRoot%\%packageName%
set targetBin=%target%\bin\v%version%

set utilexe=%output%\bin\%configuration%\Mtgdb.Util.exe
set msbuildexe="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
set nunitconsoleexe=%origin%\tools\NUnit.Console-3.7.0\nunit3-console.exe
set git="C:\Program Files\Git\bin\git.exe"

set googledriveexe=%origin%\tools\gdrive-windows-x64.exe
set signid=1IUp6u10KW4tv9AumeddhrL9UtQBejYEg
set fileid=0B_zQYOTucmnUOVE1eDU0STJZeE0
set testsignid=13kTrLvgeyIF2ZMOzJMzGfhcx3M0-63_B
set testfileid=18-gJb7NpBxSgjDgqDkuyKfX42pQUtdRt

rem goto upload
rem goto sign
rem goto git

%msbuildexe% %origin%\Mtgdb.sln /verbosity:m

if errorlevel 1 exit /b

%utilexe% -update_help

if errorlevel 1 exit /b

rmdir /q /s %targetRoot%
mkdir %targetRoot%
mkdir %target%

xcopy /q /r /i /e %output%\data %target%\data
xcopy /q /r /i /e %output%\bin\%configuration% %targetBin%
xcopy /q /r /i /e %output%\etc %target%\etc
xcopy /q /r /i /e %output%\images %target%\images
xcopy /q /r /i /e %output%\update %target%\update
xcopy /q /r /i /e %output%\help %target%\help
xcopy /q /r /i /e %output%\color-schemes %target%\color-schemes
xcopy /q /r /i /e %output%\charts %target%\charts
xcopy /q %output%\..\LICENSE %target%

del /q /s %target%\update\app\*
del /q /s %target%\update\*.bak
del /q /s %target%\update\*.zip
del /q /s %target%\update\notifications\*.zip
del /q /s %target%\update\notifications\new\*
del /q /s %target%\update\notifications\read\*
del /q /s %target%\update\notifications\archive\*
del /q %target%\update\filelist.txt
rmdir /q /s %target%\update\megatools-1.9.98-win32

del /q /s %target%\color-schemes\current.colors
del /q /s %target%\images\*.jpg
del /q /s %target%\images\*.txt
del /q /s %targetBin%\*.xml
del /q /s %targetBin%\*.pdb
rmdir /q /s %target%\data\index\keywords-test
rmdir /q /s %target%\data\index\search-test
rmdir /q /s %target%\data\index\suggest-test
rmdir /q /s %target%\data\index\deck\search
rmdir /q /s %target%\data\index\deck\suggest

del /q %target%\data\allSets-x.json
del /q %target%\data\AllSets.v42.json
rmdir /q /s %target%\data\index\keywords\0.47
rmdir /q /s %target%\data\index\search\0.47
rmdir /q /s %target%\data\index\suggest\0.47

cscript shortcut.vbs %target%\Mtgdb.Gui.lnk %version% %output%\bin\%configuration%\mtg64.ico

del /q /s %target%\*.vshost.*
%utilexe% -sign %target%\bin\v%version% -output %targetBin%\filelist.txt

"%output%\update\7z\7za.exe" a %target%.zip -tzip -ir!%target%\* -mmt=on -mm=LZMA -md=64m -mfb=64 -mlc=8

:sign
%utilexe% -sign %target%.zip -output %targetRoot%\filelist.txt

rem publish to old mega directory
del /q /s %pub%\Archive\*.zip
del /q /s %pub%\FileList\*.txt
xcopy /q %target%.zip %pub%\Archive
xcopy /q %targetRoot%\filelist.txt %pub%\FileList

rem upload to test directory so update can be tested without affecting actual users
%googledriveexe% update %testfileid% %target%.zip
%googledriveexe% update %testsignid% %targetRoot%\filelist.txt

start D:\Games\Mtgdb.Gui\Mtgdb.Gui.lnk
%nunitconsoleexe% %output%\bin\release-test\Mtgdb.Test.dll
if errorlevel 1 exit /b %errorlevel%

echo Ready to create update Notification
pause

:git
%git% -C f:/Repo/Git/Mtgdb.wiki pull
%utilexe% -notify
%git% -C f:/Repo/Git/Mtgdb.Notifications add -A
%git% -C f:/Repo/Git/Mtgdb.Notifications commit -m auto
%git% -C f:/Repo/Git/Mtgdb.Notifications push

:upload
%googledriveexe% update %fileid% %target%.zip
%googledriveexe% update %signid% %targetRoot%\filelist.txt

exit /b