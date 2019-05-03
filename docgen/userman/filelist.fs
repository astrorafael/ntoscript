((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/docgen/userman/filelist.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../src"                   SetMacro SRC
C" ../../externals"             SetMacro EXTERNALS

DocOnly titlepg.man			\ only for TeX
DocOnly intro.man			\ only for TeX


\ The Properties database

DocOnly %SRC%/core/dbase/dbase.man
DocOnly %SRC%/core/dbase/item.fs
DocOnly %SRC%/core/dbase/composite.fs
DocOnly %SRC%/core/dbase/server.fs
DocOnly %SRC%/core/dbase/device.fs
DocOnly %SRC%/core/dbase/propvector.fs
DocOnly %SRC%/core/dbase/textvector.fs
DocOnly %SRC%/core/dbase/textitem.fs
DocOnly %SRC%/core/dbase/numbervector.fs
DocOnly %SRC%/core/dbase/numberitem.fs
DocOnly %SRC%/core/dbase/lightvector.fs
DocOnly %SRC%/core/dbase/lightitem.fs
DocOnly %SRC%/core/dbase/switchvector.fs
DocOnly %SRC%/core/dbase/switchitem.fs
DocOnly %SRC%/core/dbase/blobvector.fs
DocOnly %SRC%/core/dbase/blobitem.fs

\ Interfaces

DocOnly %SRC%/core/interfaces.man
DocOnly %SRC%/core/interfaces.fs

\ Command Line Interface

DocOnly %SRC%/core/cli/datetime.fs
DocOnly %SRC%/core/cli/language.fs
DocOnly %SRC%/core/cli/dbase.fs
DocOnly %SRC%/core/cli/units.fs

\ Sky Mapping file

DocOnly %SRC%/skymapping/skymapping.man
DocOnly %SRC%/skymapping/skymapper.fs
DocOnly %SRC%/skymapping/coordinates.fs
DocOnly %SRC%/skymapping/grid.fs
DocOnly %SRC%/skymapping/uniform1.fs
DocOnly %SRC%/skymapping/uniform2.fs
DocOnly %SRC%/skymapping/uniform3.fs
DocOnly %SRC%/skymapping/uniform4.fs
DocOnly %SRC%/skymapping/uniform4.fs
DocOnly %SRC%/skymapping/zigzag.fs
DocOnly %SRC%/skymapping/zigzag1.fs
DocOnly %SRC%/skymapping/zigzag2.fs
DocOnly %SRC%/skymapping/zigzag3.fs
DocOnly %SRC%/skymapping/zigzag4.fs
DocOnly %SRC%/skymapping/misc.fs
DocOnly %SRC%/skymapping/xephem.fs


\ Miscellany

DocOnly %EXTERNALS%/extras/sexag.fs
DocOnly %EXTERNALS%/objects/objects.fs
DocOnly %EXTERNALS%/objects/struct.fs
