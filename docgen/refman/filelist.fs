((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/docgen/refman/filelist.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../src"                   SetMacro SRC
C" ../../externals"             SetMacro EXTERNALS
C" ../../build"                 SetMacro BUILD

DocOnly titlepg.man			\ only for TeX
DocOnly intro.man			\ only for TeX
DocOnly architecture.man		\ only for TeX


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

\ The core files

DocOnly %SRC%/core/parser.fs
DocOnly %SRC%/core/task.fs
DocOnly %SRC%/core/xml.fs
DocOnly %SRC%/core/dynbuffer.fs
DocOnly %SRC%/core/logger.fs
DocOnly %SRC%/core/base64.fs
DocOnly %SRC%/core/zpipe.fs
DocOnly %SRC%/core/edb.fs

\ Interfaces

DocOnly %SRC%/core/interfaces.man
DocOnly %SRC%/core/interfaces.fs

\ Command Line Interface

DocOnly %SRC%/core/cli/datetime.fs
DocOnly %SRC%/core/cli/language.fs
DocOnly %SRC%/core/cli/dbase.fs
DocOnly %SRC%/core/cli/units.fs

\ Sky Mapping

DocOnly %SRC%/skymapping/skymapping.man
DocOnly %SRC%/skymapping/skymapper.fs
DocOnly %SRC%/skymapping/interfaces.fs
DocOnly %SRC%/skymapping/coordinates.fs
DocOnly %SRC%/skymapping/grid.fs
DocOnly %SRC%/skymapping/uniform1.fs
DocOnly %SRC%/skymapping/uniform2.fs
DocOnly %SRC%/skymapping/uniform3.fs
DocOnly %SRC%/skymapping/uniform4.fs
DocOnly %SRC%/skymapping/zigzag.fs
DocOnly %SRC%/skymapping/zigzag1.fs
DocOnly %SRC%/skymapping/zigzag2.fs
DocOnly %SRC%/skymapping/zigzag3.fs
DocOnly %SRC%/skymapping/zigzag4.fs
DocOnly %SRC%/skymapping/xephem.fs
DocOnly %SRC%/skymapping/misc.fs

\ External modules

DocOnly %EXTERNALS%/yasol/sockets.fs
DocOnly %EXTERNALS%/yasol/Genio/sockets.fs

DocOnly %EXTERNALS%/extras/lists.fs
DocOnly %EXTERNALS%/multicop/multicop.fs

DocOnly %EXTERNALS%/extras/extras.fs
DocOnly %EXTERNALS%/extras/sexag.fs
DocOnly %EXTERNALS%/extras/string.fs
DocOnly %EXTERNALS%/extras/tasker.fs

DocOnly %EXTERNALS%/objects/objects.fs
DocOnly %EXTERNALS%/objects/struct.fs

\ Console generation application

DocOnly %BUILD%/console/turnkey.fs
DocOnly %BUILD%/console/filelist.fs
DocOnly %BUILD%/console/ntoscript.fs

