C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/console/filelist.fs

also nto definitions

: pepe
   s" <message timestamp='2008-03-19T22:23:20' device='Camera' message='Sending camtest.fts binned 1:1 compressed' />" ;

: popo
   s" <message timestamp='2008-03-19T22:23:20' device='Camera' />" ;

: pipo
   s" <message device='Camera' message='Sending camtest.fts binned 1:1 compressed' />" ;

: popi
   s" <message message='Sending camtest.fts binned 1:1 compressed' />" ;
