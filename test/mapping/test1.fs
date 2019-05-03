C" /usr/share/doc/VfxForth/Lib"    SetMacro LIB      \ VFX library base path
C" ../../Lib"                      SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs


: v1 s" 23" ;
: v2 s" 23.5" ;
: v3 s" 23:6" ;
: v4 s" -23:6" ;
: v5 s" 23:" ;
: v6 s" 23::" ;
: v7 s" 23:06.5" ;
: v8 s" 23:06:30" ;
: v9 s" 23:06:30.50" ;
: vA s" 23:::" ;
: vB s"   23:44" ;
: vC s"   +23:44" ;


: fk1  s" foo"  ;
: fk2  s" 23,4" ;
: fk3  s" 23:3,4" ;
: fk5  s" 23:3-4" ;
: fk6  s" 23:34:4/7" ;


23:45.6 R.A. -10:34:30.9 DEC.
lo-res equatorial coordinates
cr
fswap .RA space .DEC cr

23:45.6 R.A. -10:34:30.9 DEC.
hi-res equatorial coordinates
fswap .RA space .DEC cr


23:45.6 R.A. -10:34:30.9 DEC.
standard equatorial coordinates
fswap .RA space .DEC cr



