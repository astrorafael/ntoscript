
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/filelist.fs

also nto definitions

Server: indi1 localhost 7624
indi1 s" indiserver" indi-server heap-new constant indiserver

s" AUDINE1" indi-device  heap-new constant audine1
audine1 indiserver push-back



s" ADC_SPEED" switch-vector heap-new constant psv 
psv audine1 push-back

s" 100KBS"    switch-item   heap-new constant psi1
psi1 psv push-back

s" 200KBS"    switch-item   heap-new constant psi2
psi2 psv push-back



s" STORAGE" text-vector heap-new constant ptv
ptv audine1 push-back

s" DIR"     text-item heap-new constant pti1
pti1 ptv push-back

s" PREFIX"  text-item heap-new constant pti2
pti2 ptv push-back



s" AREA_DIM" number-vector heap-new constant pnv
pnv audine1 push-back

s" DIMY"     number-item heap-new constant pni1
pni1 pnv push-back

s" DIMX"     number-item heap-new constant pni2
pni2 pnv push-back

s" PIXSZ"    number-item heap-new constant pni3
pni3 pnv push-back


s" IMAGE_BLOB" blob-vector heap-new constant pbv
pbv audine1 push-back

s" IMAGE" blob-item heap-new constant pbi1
pbi1 pbv push-back

s" bertie.jpg" pbi1 set
