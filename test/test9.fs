C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/filelist.fs

also nto definitions

16 kB Constant Filesize

Create mempad Filesize allot

: myread				  \ -- ca1 u1
   s" bertie.txt" r/o bin open-file  OPEN-FILE-EXCP ?throw
   >r mempad Filesize r@ read-file READ-FILE-EXCP ?throw
   r> close-file  CLOSE-FILE-EXCP ?throw
   mempad swap
;

      
myread s" decoded-result.jpg" base64-decode  