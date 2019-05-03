((

$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/test/test5.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))




\ : .s [io xconsole setIO .s io]  ;

include ../src/externals/multicop/lists.fs
include ../src/externals/multicop/multicop.fs
include ../src/externals/yasol/sockets.fs
include ../src/externals/yasol/Genio/sockets.fs

include ../src/parser/dynbuffer.fs
include ../src/parser/xml.fs




\ ------------------------------------------------------------
\ USER (PER TASK) VARIABLES
\ ------------------------------------------------------------

SOCKDEV_NONBLOCK Constant SockFlags
\ *G Client Socket Flags

/SockDev +User RxSocket
\ *G A client socket per task.

/XMLBuf +USER RxBuffer
\ *G An XML Buffer per task.

2 cells +User CurTag
\ *G Current tag string in XML input buffer

/endPoint +User RemoteServer

RemoteServer unknown-ip

\ ------------------------------------------------------------
\ COMMON WORDS
\ ------------------------------------------------------------

: default-io				  \ --
\ *G The default Forth console I/O.
   xconsole SetIO
;

: task-io				  \ --
\ *G The task operating mode I/O.
   RxSocket op-handle ! 
   RxSocket ip-handle !
;


Defer decode-message			\ ca1 u1 --
\ *G Decodes a full XML INDI message.

: dump-message			  \ ca1 u1 --
[io 
default-io cr ." *** stack depth = " depth .
." *** buf depth = " Rxbuffer xb-wp-offset .  ." ***" 
cr type
io]
;

: see-message				  \ ca1 u1 --
   [io
   default-io 
   base @ >r
   cr
   CurTag 2@ type space 
   [char] ( emit . drop [char] ) emit space 
   ." Buffer Used = " Rxbuffer xb-used nip .
   ." Depth = " Rxbuffer xb-wp-offset .
   ." Stack = " depth .
   r> base !
   io]
;


' see-message IS decode-message


: is-action \ xt -- ;
\ *G wrapper around real action code given by *\i{xt}.   
   catch
   default-io
   dup errno-base <= if
      .errno-excp
   else
      ." Exception#: " . cr
   then
;

: .connect				  \ --
   [io default-io cr ."  Connected to server " cr io]
;



: store-into-buffer			  \ --
\ *G Read as much data as possible from the socket and
\ ** store it into the XML input buffer. Can throw exceptions.
   RxBuffer xb-available ReadEx-Gen throw RxBuffer xb-wp+!
;


: find-tag			  \ -- ca1 u1 ca2 u2
\ *G Returns the remaining string *\i{ca2 u2} after the tag *\i{ca1 u1}   
   RxBuffer xb-used open-tag 2dup CurTag 2!
;


: analyze-buffer				  \ --
\ *G Do as much XML parsing as possible with the data we have in the
\ ** XML input buffer. Stops when there is not a full XML element to be
\ ** parsed.
  begin
     find-tag	
     full-element?
  while					\ -- ca1 u1 ; if true, XML message
	2dup
	decode-message			\ may throw user exceptions.
	+ RxBuffer xb.rp !		\ updates read pointer
  repeat
;
   
: get-properties				  \ --
   ." <getProperties version='1.5'/>" cr
;   
   

: set-remote-server			  \ --
   s" localhost" 7624 RemoteServer known-ip
;

: Task1-init				  \ --
   decimal
   RxSocket initSockDev
   task-io
   RxBuffer xb-init
   set-remote-server
   RemoteServer -1 SockFlags open-gen throw drop
   .connect
   get-properties
;

: (Action)
  (( task1-init
  begin
      store-into-buffer
      analyze-buffer
      RxBuffer xb-realloc&grow
   again ))
; 


: Action
   ['] (Action) is-action ;

Task Task1
' Action Task1 initiate

: KACTION     \ n -- 
   xconsole SetIO decimal
   BEGIN DUP EMIT 2000 ms [CHAR] * EMIT AGAIN DROP ;

: ACTION1     \ -- 
   [CHAR] 1 KACTION ;

TASK TASK2
' ACTION1 TASK2 INITIATE

: pepe s" <oneNumber name='Pres'>     995.45231972711678736   </oneNumber>" ;
