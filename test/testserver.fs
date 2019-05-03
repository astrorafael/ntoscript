C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/console/filelist.fs

also nto definitions


\ s" localhost" 7624 IndiServer: indiserver
\ s" localhost" 7624 indi-server heap-new constant indiserver

IndiServer: indiserver localhost 7624

: foo
    s" indiserver1"  s" localhost"  s" 7624"
;



INDIServer: indiserver2 localhost 7624