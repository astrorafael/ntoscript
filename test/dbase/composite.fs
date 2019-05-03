((

$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/test/dbase/composite.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/filelist.fs

also indiscript

cr
s" Father" indi-composite heap-new constant father
s" Child1" indi-item heap-new father push-back
father size . cr


s" Child1" father search-item [if] name type cr [else] .( not found ) cr [then]


s" Child2" indi-item heap-new father push-back
father size . cr
s" Child2" father search-item [if] name type cr [else] .( not found ) cr [then]

s" Child3" indi-item heap-new father push-back
father size . cr
s" Child3" father search-item [if] name type cr [else] .( not found ) cr [then]

s" Child4" indi-item heap-new father push-back
father size . cr
s" Child4" father search-item [if] name type cr [else] .( not found ) cr [then]

s" Child1" father search-item [if] name type cr [else] .( not found ) cr [then]
s" Child2" father search-item [if] name type cr [else] .( not found ) cr [then]
s" Child3" father search-item [if] name type cr [else] .( not found ) cr [then]
s" Child4" father search-item [if] name type cr [else] .( not found ) cr [then]
father print

