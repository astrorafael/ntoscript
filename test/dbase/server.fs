
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/filelist.fs

also nto definitions

Server: indi1 localhost 7624
indi1 s" indiserver" indi-server heap-new constant indiserver




