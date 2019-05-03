
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/filelist.fs

also nto definitions


Variable m-tcb
Variable m-sid
Create m-sock /SockDev  allot&erase
Create m-sem /Semaphore allot&erase
Create m-endpoint /EndPoint allot&erase

Server: RemoteServer localhost 7624

: (getProperties)   ( this -- )	
\ +G Do the actual sending of *\f{<getProperties version='1.5'}.
     ." <getProperties version='1.5'"  ." />" cr
; 

: create-task   ( this -- )
\ +G Create the listening background task for this server.
     new-task  m-tcb !
;

: init-task   ( this -- )
\ +G Launches the *\fo{Listener} task asnd initialize some specific *\fo{USER}
\ +* variables known to this object.
   ['] Listener m-tcb @ initiate     
   NULL   m-tcb @ ServerObj   his !
   m-sem  m-tcb @ RxSemaphore his !
   m-sock m-tcb @ RxSocket    his !
   m-sock m-sid !
   pause				\ let the other task run
;

: open-socket ( this -- )
\ +G Create a socket bound to this *\fo{INDI-SERVER} class instance.
   RemoteServer -1 SOCKDEV_NONBLOCK m-sock open-gio throw drop
;

: myconstruct   ( this -- )		\ overrides construct
\ *G Create and *\fo{INDI-SERVER} object and associated background task.
\ ** Object name is given by *\i{ca1 u1}.     
\ ** Host/port given by the *\fo{/EndPoint} *\i{addr} parameter. 
     m-sem initSem
     m-sock  initSockDev
     xconsole m-sid !
     create-task           
;

: connected?   ( this -- flag )
\ *G Tell connection status. True if connected.
     m-tcb @ running?
;

: connect   ( addr this --  )		\ overrides connect
\ *G Open a socket to host/port given by the *\fo{/EndPoint} *\i{addr}.
\ ** Also creates a background task to listen to incoming data.
   connected? not if
      init-task open-socket 
      m-tcb @ restart			\ let the task continue
   then
;

: disconnect   ( this --  )			\ overrides disconnect
\ *G Disconnect from server. Do nothing if already disconnected.
   connected? if
      m-tcb @ halt 
      m-sock close-gio throw
      xconsole m-sid !
   then	
;
  
: getProperties   ( this -- )			\ overrides get
\ *G Send an INDI <getProperties> message.
\ I SHOULD CATCH EXCEPTIONS HERE BUT DONT KNOW YET HOW TO DO IT !!!!
    m-sid @
   [io SetIO (getProperties) io]
;

