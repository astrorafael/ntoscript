C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC
C" ../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs

also nto definitions


