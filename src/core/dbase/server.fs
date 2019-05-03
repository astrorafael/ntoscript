((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/server.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==========================
\ *N *\fo{INDI-SERVER} class
\ ==========================


\ *P The *\fo{INDI-SERVER} class is the root class in the hierarchy of
\ ** *\fo{INDI-ITEM}s.
\ ** The user must first create an *\fo{INDI-SERVER} object for further
\ ** interactions. The user permanently binds an *\fo{INDI-SERVER} object to a
\ ** remote host and port by passing the proper parameters in the constructor.
\ ** However, the connection is not made until a *\fo{CONNECT} is attempted.
\ ** At this moment, the socket is open and the background *\fo{LISTENER}
\ ** is started.
\ ** When the user invokes the *\fo{DISCONNECT} method, the socket is closed
\ ** and the background task is *\fo{HALT}ed.

\ *P When the  *\fo{INDI-SERVER} object is disconnected, any *\fo{SEND} method
\ ** will send the XML stream to *\fo{XConsole}. In the current implementation,
\ ** any I/O error in the background *\fo{Listener} task causes this task to
\ ** *\fo{STOP} and the socket is closed. No inter-task communication exist
\ ** so the *\fo{MAIN} task does not notice anything until doing an I/O itself,
\ ** which will throw an exception.


\ *P The following public selectors/methods may be invoked
\ ** by the *\fo{MAIN} task:
\ *(
\ *B *\fo{CONSTRUCT}
\ *B *\fo{CONNECTED?}
\ *B *\fo{CONNECT}
\ *B *\fo{DISCONNECT}
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{SEND} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{GET} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{LATCH} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{SIZE} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{PRINT} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{LOOKUP} (inherited from *\fo{INDI-COMPOSITE})
\ *)

\ +P The following public selectors/methods are invoked by the *\fo{LISTENER}
\ +* task:
\ +(
\ +B *\fo{SEARCH-ITEM} (inherited from *\fo{INDI-COMPOSITE})
\ +B *\fo{PUSH-BACK} (inherited from *\fo{INDI-COMPOSITE})
\ +)


\ ---------------
\ +H Design notes
\ ---------------

\ +P The way *\fo{INDI-SERVER} creates and initializes its linked
\ +* *\fo{LISTENER} task is determined by when the user area is set up.
\ +* The user area is not set up until the task is
\ +* *\fo{INITIATE}d. Before going to its main loop, the slave task stops
\ +* itself to let the server object in the *\fo{MAIN} task initialize
\ +* some of his *\fo{USER} variables.

\ +P There is not an easy way to get from the GENIO socket device
\ +* about the connection status. When the remote server is disconnected
\ +* the background listener is stopped as soon as it attempts I/O. So the way
\ +* to know about the socket connection is to get the task status.

\ +P Since we have a "forest" of property trees, console commands and scripts
\ +* need to operate on a given context.
\ +* The global value *\fo{theCurrentServer} fits in that purpose.
\ +* This global value is assigned to the last server object which performs
\ +* a *\fo{CONNECT}, although the user or script can change it manually:
\ +C    <an indi-server object>  To TheCurrentServer .

\ +P Defining word *\fo{(INDIServer)} is a factor for parsing word
\ +* *\fo{INDIServer:}, designed for scripts but can also be useful
\ +* by itself in GUIs.


NULL Value TheCurrentServer
\ *G Current server being handled by the scripts or other console operations.

ErrDef Server-Excp         "INDI Server not found"
\ *G Exception thrown on *\fo{NULL} value for *\fo{TheCurrentServer}.


\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-composite class   

   cell% inst-var m-tcb			\ Linked task Id
   cell% inst-var m-sid			\ Current XML I/O device 
   char% /Semaphore * inst-var m-sem 	\ per-task semaphore
   char% /SockDev   * inst-var m-sock	\ per-task client socket
   char% /EndPoint  * inst-var m-endpoint	\ host+port to connect to
   
end-class indi-server
\ +]

indi-server methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  m:   ( this -- sid )			\ overrides sid
\ +G Get the *\fo{/SockDev} SID asociated with the server task.     
     m-sid @ 
  ;m overrides sid

  
  m:   ( this -- )			\ overrides lock
\ +G Lock the associated server *\fo{/Semaphore} to prevent concurrent
\ +* updates to the properites tree.     
     m-sem request 
  ;m overrides lock

  
  m:   ( this -- )			\ overrides unlock
\ +G Unlock the associated server *\fo{/Semaphore} to allow
\ +* access to the properites tree.
\ Note: Multitasker *\fo{signal} is in conflict with *\fo{i-sync} interface
\ and so this trick must be done.     
     m-sem signal
  ;m overrides unlock

  
  m: ( this -- )	               \ overrides getProperties
\ +G Do the actual sending of *\f{<getProperties version='...'/>}.
     ." <getProperties version='" INDI-VERSION type ." '/>" cr
  ;m overrides getProperties

  
  :m create-task   ( this -- )
\ +G Create the listening background task for this server.
     new-task  m-tcb !
  ;m

  
  :m init-task   ( this -- )
\ +G Launches the *\fo{Listener} task asnd initialize some specific *\fo{USER}
\ +* variables known to this object.
     ['] Listener m-tcb @ initiate     
     this   m-tcb @ theServer   his !
     m-sem  m-tcb @ RxSemaphore his !
     m-sock m-tcb @ RxSocket    his !
            m-tcb @ dbgTask     his off
     m-sock m-sid !
     pause				\ let the other task run
  ;m

  
  :m open-socket ( this -- )
\ +G Create a socket bound to this *\fo{INDI-SERVER} class instance.
     m-endpoint -1 SOCKDEV_NONBLOCK m-sock open-gio throw drop
  ;m
  
public

\ -----------------
\ *H Public methods
\ -----------------
  
  m:   ( ca1 u1 u3 ca2 u2 this -- )		\ overrides construct
\ *G Create and *\fo{INDI-SERVER} object and associated background task.
\ ** Object name is given by *\i{ca2 u2}.     
\ ** Host is given in *\i{ca1 u1} string.
\ ** TCP port is given in *\i{u3}.  
     this [parent] construct 	 
     m-endpoint known-ip		\ initializes the endpoint with values
     m-sem  initSem
     m-sock initSockDev
     xconsole m-sid !
     this create-task
  ;m overrides construct


  m:   ( this -- flag )			\ overrides connected?
\ *G Tell connection status. True if connected.
     m-tcb @ running? 0<>
  ;m overrides connected?  

  
  m:   ( this --  )			\ overrides connect
\ *G Open a socket to host/port given in the *\fo{INDI-SERVER} constructor.
\ ** Also creates a background task to listen to incoming data.
     this connected? 0= if
	this dup init-task open-socket 
	m-tcb @ restart			\ let the task continue
	this to TheCurrentServer
     then
  ;m overrides connect

  
  m:   ( this --  )			\ overrides disconnect
\ *G Disconnect from server. Do nothing if already disconnected.
     xconsole m-sid !			\ do it before throw in close-gio
     this connected? if
	m-tcb @ halt 
	m-sock close-gio throw
     then
  ;m overrides disconnect
  
  :m +trace   ( mask this -- )
\ +G Enable trace to standard output of incoming XML message whose type is
\ +* given by *\i{mask}. Used for debugging purposes. Must be used *\b{after}
\ +* *\fo{CONNECT} has been issued.
\ +C   defNumberVector <an indi-server object> +trace
    this [current] lock
    m-tcb @ dbgTask his			\ -- mask addr
    dup @ rot or 			\ -- addr val|mask
    swap !
    this [current] unlock
  ;m  

  :m -trace   ( mask this -- )
\ +G Disable trace to standard output of incoming XML message whose type is
\ +* given by *\i{mask}. Used for debugging purposes. Must be used *\b{after}
\ +* *\fo{CONNECT} has been issued.
\ +C   defNumberVector <an indi-server object> -trace  
    this [current] lock
    m-tcb @ dbgTask his			\ -- mask addr
    dup @ rot invert and 		\ -- addr ~mask&val
    swap !
    this [current] unlock
  ;m

end-methods persistant

\ ---------------------
\ *H Other helper words
\ ---------------------

: (INDIServer)		       \ ca1 u1 ca2 u2 ca3 u3 -- ;  [child] -- obj
\ *G Create a named *\fo{INDI-SERVER} object whose name is *\i{ca1 u1}
\ ** that will connect to host given
\ ** by *\i{ca2 u2} and TCP port string *\i{ca3 u3}.
\ ** Invoking the object name name will return the object handle *\i{obj}.  
  PAD place PAD number? 1 <> if	\ move number to PAD and performs conversion
     INVALID_NUMERIC_ARG throw
  then 
  >R 2>R 2dup				\ S: name name ; R: -- port host
  2R> 2swap R> -rot			\ reorder args for constructor
  indi-server  heap-new			\ create object
  -rot ($CREATE) ,			\ and constant in dictionary
DOES>
  @  					\ retrieves object handle
;

: CurrentServer			  \ -- obj
\ *G Return *\fo{theCurrentServer} value or
\ ** throw a *\fo{Server-Excp} exception if *\fo{NULL}.
   theCurrentServer dup 0= if Server-Excp throw then
;



\ ======
\ *> ###
\ ======
