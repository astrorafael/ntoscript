
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/filelist.fs

also indiscript definitions

Server: indi1 localhost 7624
indi1 s" indiserver" indi-server heap-new constant indiserver

s" Camera" indi-device heap-new constant ccd
ccd indiserver push-back

s" Binning" property-vector heap-new constant pv
pv ccd push-back






