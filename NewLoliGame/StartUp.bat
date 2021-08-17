@echo off
rem chcp 65001

rem 这里是一些路径的常量设置 使用set来处理 
set UNITY_PATH="C:\Program Files\Unity2019.2.5\Editor\Unity.exe"

set POWER_SHELL_PATH="C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe"

set EXPORT_PROJECT_PATH="C:\Users\EDZ\Desktop\New Info\LianCha";

set UNITY_METHOD_NAME=MyEditorScript.Build

echo 清理文件夹
if exist %EXPORT_PROJECT_PATH% ( rd /s /q %EXPORT_PROJECT_PATH% ) 
md %EXPORT_PROJECT_PATH%

echo 1.开始编译(调用unity中打包方法来打包)

rem %UNITY_PATH% -quit -batchmode -logFile %UNITY_LOG_PATH% -projectPath %UNITY_PROJECT_PATH% -executeMethod %UNITY_METHOD_NAME% 

%UNITY_PATH% -quit -batchmode -logFile %1 -projectPath %2 -executeMethod %UNITY_METHOD_NAME% %3

if not %errorlevel%==0 ( goto fail ) else ( goto success )

:success
echo 2.打包成功
%POWER_SHELL_PATH% -file success.ps1
goto end
 
:fail
echo 3.打包失败
%POWER_SHELL_PATH% -file fail.ps1
goto end

:end

echo 4.批处理结束

pause

