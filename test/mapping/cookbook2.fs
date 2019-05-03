C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../LIB2"                     SetMacro LIB2
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC
C" ../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs


foreground logging on
IndiServer: indiserver localhost 7624
include xephem.fs
discover

Task AutoFocus-Task
Assign Autofocuser To-Start AutoFocus-Task

switch-on telescope
My-Location
Goto-M31
Red Filter
30.0 sec. 2x2 3 Photos
  
  