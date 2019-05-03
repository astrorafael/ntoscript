((
$Date: 2008-08-24 12:31:35 +0200 (dom 24 de ago de 2008) $
$Revision: 563 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/build/console/filelist.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))


\ ####################
\ *> build
\ *T Control File
\ ####################

\ *P Master control file which loads all other files.


\ ==========================
\ *N Configuration variables
\ ==========================

0 Constant Cooperative
\ *G Cooperative multitasker selected.

1 Constant Preemptive
\ *G Preemptive multitasker selected.

Cooperative Value Multitasker
\ *G Multitasker selection for turnkey generation.

\ Preemptive TO Multitasker

False Constant NTODEBUG
\ *G Enables debugging support

#8 Constant FPCELL
\ +G Set up the float size in the floating point library.
\ +* to properly interface external C shared libraries.


NTODEBUG [if]
  include %LIB%/XREF.FTH
  +xrefs
[then]
   


\ *[
include %LIB%/StringPk.fth		\ Counted string handling

include %LIB2%/Ndp387.fth
\ include %LIB%/Hfp387.fth

\ include %LIB%/Genio/FILE.FTH
include %LIB2%/Genio/FILE.FTH		\ I've customized the above file 


include %EXTERNALS%/extras/extras.fs
include %EXTERNALS%/extras/string.fs
include %EXTERNALS%/extras/sexag.fs
include %EXTERNALS%/extras/calendar.fs

include %EXTERNALS%/objects/struct.fs
include %EXTERNALS%/objects/objects.fs

Cooperative Multitasker = [if]
   include %EXTERNALS%/extras/lists.fs
   include %EXTERNALS%/multicop/multicop.fs
[else]
   include %LIB%/Lin32/MultiLin32.fth
   include %EXTERNALS%/extras/tasker.fs   
[then]

include %EXTERNALS%/yasol/sockets.fs
include %EXTERNALS%/yasol/Genio/sockets.fs

vocabulary nto
also nto definitions

include %SRC%/core/logger.fs
include %SRC%/core/dynbuffer.fs
include %SRC%/core/base64.fs
include %SRC%/core/zpipe.fs
include %SRC%/core/xml.fs
include %SRC%/core/task.fs
include %SRC%/core/edb.fs
include %SRC%/core/cli/datetime.fs

include %SRC%/core/interfaces.fs
include %SRC%/core/dbase/item.fs
include %SRC%/core/dbase/composite.fs
include %SRC%/core/dbase/server.fs
include %SRC%/core/dbase/device.fs
include %SRC%/core/dbase/propvector.fs
include %SRC%/core/dbase/textvector.fs
include %SRC%/core/dbase/textitem.fs
include %SRC%/core/dbase/numbervector.fs
include %SRC%/core/dbase/numberitem.fs
include %SRC%/core/dbase/lightvector.fs
include %SRC%/core/dbase/lightitem.fs
include %SRC%/core/dbase/switchvector.fs
include %SRC%/core/dbase/switchitem.fs
include %SRC%/core/dbase/blobvector.fs
include %SRC%/core/dbase/blobitem.fs   
include %SRC%/core/parser.fs

include %SRC%/core/cli/language.fs
include %SRC%/core/cli/dbase.fs
include %SRC%/core/cli/units.fs

include %SRC%/skymapping/misc.fs
include %SRC%/skymapping/interfaces.fs
include %SRC%/skymapping/coordinates.fs
include %SRC%/skymapping/grid.fs
include %SRC%/skymapping/xephem.fs
include %SRC%/skymapping/uniform1.fs
include %SRC%/skymapping/uniform2.fs
include %SRC%/skymapping/uniform3.fs
include %SRC%/skymapping/uniform4.fs
include %SRC%/skymapping/zigzag.fs
include %SRC%/skymapping/zigzag1.fs
include %SRC%/skymapping/zigzag2.fs
include %SRC%/skymapping/zigzag3.fs
include %SRC%/skymapping/zigzag4.fs

include %SRC%/skymapping/skymapper.fs

vocabulary ntoscript
also ntoscript definitions
\ *]

\ ======
\ *> ###
\ ======

