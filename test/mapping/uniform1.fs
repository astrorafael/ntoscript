C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../..//Lib"                  SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs


ra+dec+uniform-ra heap-new constant skymapper1
00:50:00 R.A.  +40:00:00 DEC. skymapper1 set-initial
00:35:00 R.A.  +43:00:00 DEC. skymapper1 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper1 set-frame

foreground logging on
XEphem-Tool: xephem
skymapper1 xephem set-mapper
\ xephem capture


   

