C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../..//Lib"                  SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs



\ ra+dec+uniform-ra heap-new constant skymapper1
+RA +DEC Uniform SkyMapper: skymapper1
00:00:00 R.A.   84:00:00 DEC. skymapper1 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper1 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper1 set-frame


foreground logging on
XEphem-Tool: xephem

0 [if]
   
skymapper1 xephem set-mapper
xephem generate
S" Examine skymapper 1 and press key for skymapper 2" type cr
key drop


[then]

