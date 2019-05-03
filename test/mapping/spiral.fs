C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../..//Lib"                  SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs

+RA CW Spiral SkyMapper:      skymapper1
                           15 skymapper1 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper1 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper1 set-frame

+RA CCW Spiral SkyMapper:     skymapper2
                           15 skymapper2 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper2 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper2 set-frame

-RA CW Spiral SkyMapper:      skymapper3
                           15 skymapper3 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper3 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper3 set-frame

-RA CCW Spiral SkyMapper:     skymapper4
                           15 skymapper4 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper4 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper4 set-frame

+DEC CW Spiral SkyMapper:     skymapper5
                           15 skymapper5 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper5 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper5 set-frame

+DEC CCW Spiral SkyMapper:    skymapper6
                           15 skymapper6 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper6 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper6 set-frame

-DEC CCW Spiral SkyMapper:    skymapper7
                           15 skymapper7 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper7 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper7 set-frame

-DEC CCW Spiral SkyMapper:    skymapper8
                           15 skymapper8 set-points
00:00:00 R.A.   84:00:00 DEC. skymapper8 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper8 set-frame

+RA CW Spiral SkyMapper:      skymapper9
                           11 skymapper9 set-points
00:00:00 R.A.  -84:00:00 DEC. skymapper9 set-initial
01:00:00 DEG.   01:00:00 DEG. skymapper9 set-frame

ra-dec+uniform-ra heap-new constant skymapper10
00:00:00 R.A.   84:00:00 DEC. skymapper10 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper10 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper10 set-frame

foreground logging on
XEphem-Tool: xephem


   
skymapper1 xephem set-mapper
xephem generate
S" Examine skymapper 1 and press key for skymapper 2" type cr
key drop
1 [if]
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

