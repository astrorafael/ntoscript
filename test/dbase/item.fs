((

$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/test/dbase/item.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/filelist.fs

also indiscript

s" Item1" indi-item heap-new constant myitem
cr

myitem name type cr
myitem parent @ . cr
myitem .name cr				\ this is protected !
myitem myitem parent !
myitem . cr 
myitem parent @ . cr

