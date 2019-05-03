
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/console/filelist.fs

also nto definitions


Server: indi1 localhost 7624
indi1 s" indiserver" indi-server heap-new constant indiserver
indiserver connect
indiserver properties get

2000 ms


s" Camera" indiserver lookup drop constant ccd

ccd BLOBs enable

s" Exposure" ccd lookup drop constant exposure
\ s" ExpDur" exposure lookup drop constant duration
\ 5.0 duration set

\ s" Binning"  ccd lookup drop constant binning


\ exposure send
\ cr








