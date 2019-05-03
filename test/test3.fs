((

$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/test/test3.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))


include ../src/externals/multicop/lists.fs
include ../src/externals/multicop/multicop.fs
include ../src/externals/yasol/sockets.fs
include ../src/externals/yasol/Genio/sockets.fs

include ../src/parser/dynbuffer.fs


0 Constant myflags

s" localhost" 7624 EndPoint: remote known-ip \ the "remote" end point

SockDev: mysock
/XMLBuf +USER RxBuffer
RxBuffer xb-init

\ ------------------------------------------------------------
\ COMMON WORDS
\ ------------------------------------------------------------

: default-io          \ The default Forth console I/O
   xconsole SetIO ;

: is-action \ xt -- ; wrapper around real action code given by xt
   catch
   default-io dup errno-base <= if
      .errno-excp
   else
      drop
   then
;

: .connect
   cr ."  Connected to server " cr
;

: Task1-init
   mysock SetIO
   remote -1 myflags open-gen throw drop
   [io default-io .connect io]   
;

: (Action)
   task1-init
   s" <getProperties version='1.5'/>" type cr
   begin
      RxBuffer xb-available
      ReadEx-Gen throw
      RxBuffer xb-wp+!
      RxBuffer xb-used tuck
      [io
          default-io
          s" ===================================" type cr
          type cr
      io]
      RxBuffer xb-rp+!
      RxBuffer xb-realloc
   again
; 


: Action
   ['] (Action) is-action ;



