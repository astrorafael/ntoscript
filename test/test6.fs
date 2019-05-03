
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/filelist.fs


: no-process
   2drop 
;


: .connect				  \ --
   [io default-io cr ."  Connected to server " cr io]
;

: set-remote-server			  \ --
   s" localhost" 7624 RemoteServer known-ip
;


: get-properties				  \ --
   ." <getProperties version='1.5'/>" cr
;   

' get-properties    is Hook2
' set-remote-server is Hook1

Task Task1

: go
   ['] Action Task1 initiate ;


