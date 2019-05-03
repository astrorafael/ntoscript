C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../..//Lib"                  SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs

: C1
   S" Annonymous,f, 0:58:10.4, 44:01:35,0.00,2000,0"
;

: C2
   S" And Phi-42A,f|D|B7, 1:09:30.1|5.55, 47:14:30|-13,4.26,2000,0"
;

: C3
   S" M32,f|H|E2, 0:42:41.8, 40:51:57,8.10,2000,510|390|179.294"
;

: C4
   S" M32,f|H|E2, 0:42:41.8, 40:51:57,8.10,2000,510|390|179.294"
;


80.0 inch. x 8.0 inch. OTA: LX200-8"@f/10
1024 x 768  9.0 um. x 9.0 um. CCDChip: KAF-400


 LX200-8"@f/10 KAF-400 FOV .2xdeg

ra+dec+uniform-ra heap-new constant skymapper1
00:00:00 R.A.   84:00:00 DEC. skymapper1 set-initial
02:00:00 R.A.   86:30:01 DEC. skymapper1 set-final
01:00:00 DEG.   01:00:00 DEG. skymapper1 set-frame
                0 %RA  0 %DEC skymapper1 set-overlap     \ optional 


: test
   skymapper1 reset
   begin
      skymapper1 next-point
   while
	 .eqpoint cr
   repeat
   
;

foreground logging on
XEphem-Tool: xephem
skymapper1 xephem set-mapper
xephem generate




   

