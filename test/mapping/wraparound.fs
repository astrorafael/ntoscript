C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../..//Lib"                  SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs

foreground logging on
XEphem-Tool: xephem

+RA +DEC Uniform SkyMapper: skymapper1
00:00:00 R.A.   84:00:00 DEC. skymapper1 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper1 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper1 set-frame

+RA -DEC Uniform SkyMapper: skymapper2
00:00:00 R.A.   85:00:00 DEC. skymapper2 set-centre
01:00:00 DEG.   01:00:00 DEG. skymapper2 set-frame
3 x 3   skymapper2 set-frames

+RA -DEC Uniform SkyMapper: skymapper3	\ Wraparound
22:00:00 R.A.   84:00:00 DEC. skymapper3 set-initial
02:00:00 R.A.   86:00:00 DEC. skymapper3 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper3 set-frame

-RA -DEC ZigZag SkyMapper: skymapper4	\ Wraparound
02:00:00 R.A.   84:00:00 DEC. skymapper4 set-initial
22:00:00 R.A.   86:00:00 DEC. skymapper4 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper4 set-frame


0 [if]
skymapper1 xephem set-mapper
xephem generate
S" Examine skymapper 1 and press key for skymapper 2" type cr
key drop

skymapper2 xephem set-mapper
xephem generate
S" Examine skymapper 2 and press key for skymapper 3" type cr
key drop

[then]

skymapper3 xephem set-mapper
xephem generate