C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../..//Lib"                  SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs


ra+dec+uniform-ra heap-new constant skymapper1
00:00:00 R.A.   84:00:00 DEC. skymapper1 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper1 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper1 set-frame

ra-dec-uniform-ra heap-new constant skymapper2
00:45:22 R.A.   85:15:00 DEC. skymapper2 set-centre
01:00:00 DEG.   01:00:00 DEG. skymapper2 set-frame
5 x 5   skymapper2 set-frames




foreground logging on
XEphem-Tool: xephem

   
skymapper1 xephem set-mapper
xephem generate
S" Examine skymapper 1 and press key for skymapper 2" type cr
key drop


skymapper2 xephem set-mapper
xephem generate
S" Examine skymapper 2 and press key for skymapper 3" type cr
key drop


