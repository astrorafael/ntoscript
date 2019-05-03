
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/filelist.fs

also nto definitions

s" bertie.jpg" base64-encode
