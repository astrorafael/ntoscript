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
00:00:00 R.A.   84:00:00 DEC. skymapper2 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper2 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper2 set-frame

ra-dec+uniform-ra heap-new constant skymapper3
00:00:00 R.A.   84:00:00 DEC. skymapper3 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper3 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper3 set-frame

ra+dec-uniform-ra heap-new constant skymapper4
00:00:00 R.A.   84:00:00 DEC. skymapper4 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper4 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper4 set-frame

0 [if]
ra+dec+uniform-dec heap-new constant skymapper5
00:00:00 R.A.   84:00:00 DEC. skymapper5 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper5 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper5 set-frame

ra-dec-uniform-dec heap-new constant skymapper6
00:00:00 R.A.   84:00:00 DEC. skymapper6 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper6 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper6 set-frame

ra-dec+uniform-dec heap-new constant skymapper7
00:00:00 R.A.   84:00:00 DEC. skymapper7 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper7 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper7 set-frame

ra+dec-uniform-dec heap-new constant skymapper8
00:00:00 R.A.   84:00:00 DEC. skymapper8 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper8 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper8 set-frame

[then]

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


skymapper3 xephem set-mapper
xephem generate
S" Examine skymapper 3 and press key for skymapper 4" type cr
key drop

skymapper4 xephem set-mapper
xephem generate
S" Examine skymapper 4 and press key for skymapper 5" type cr
key drop

0 [if]
   
skymapper5 xephem set-mapper
xephem generate
S" Examine skymapper 5 and press key for skymapper 6" type cr
key drop

skymapper6 xephem set-mapper
xephem generate
S" Examine skymapper 6 and press key for skymapper 7" type cr
key drop

skymapper7 xephem set-mapper
xephem generate
S" Examine skymapper 7 and press key for skymapper 8" type cr
key drop

skymapper8 xephem set-mapper
xephem generate
S" Examine skymapper 8" type cr

[then]

