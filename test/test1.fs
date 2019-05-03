((

$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/test/test1.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))


include ../src/externals/multicop/lists.fs
include ../src/externals/multicop/multicop.fs
include ../src/externals/yasol/sockets.fs
include ../src/externals/yasol/Genio/sockets.fs

0 Constant myflags

s" localhost" 7624 EndPoint: remote known-ip \ the "remote" end point

SockDev: mysock

256 Constant BufSize
BufSize +User RxBuffer			\ task reception buffer

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
      RxBuffer BufSize ReadEx-Gen throw
      RxBuffer over
      [io
      default-io
      s" RECEIVED BYTES: " type . cr
      swap type cr
      io]
   again
; 


: Action
   ['] (Action) is-action ;

