
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/console/filelist.fs

also nto definitions


Server: indi1 localhost 7624
indi1 s" indiserver1" indi-server heap-new constant indiserver1
indiserver1 connect
indiserver1 properties get

Server: indi2 localhost 7625
indi2 s" indiserver2" indi-server heap-new constant indiserver2
indiserver2 connect
indiserver2 properties get









