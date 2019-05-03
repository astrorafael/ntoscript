((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/task.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #########################
\ *! task
\ *R @node{Background Task}
\ *T Background Task
\ #########################

\ ***************
\ *S Introduction
\ ***************

\ *P The architecture of *\f{NTOScript} is a main task who deals with
\ ** user commands and a background task that listens to an indiserver
\ ** in a given host and port.

\ *P The task duties are:
\ *( 1
\ *B Listen to an incoming stream of INDI messages and store chunks
\ ** in a buffer.
\ *B Identify and isolate whole INDI messages from this stream.
\ *B Invoke the XML INDI decoding procedures on each isolated message.
\ *B Perform buffer clean up activities.
\ *)

\ *P Outside the scope of this task words is not only the XML decoding and
\ ** but also to specify the remote server endpoint and to send INDI messages
\ ** to the indiserver, for instance *\f{<getProperties version='1.5'/>}.

\ ***************
\ +S Design notes
\ ***************

\ +P As the pattern is a simple master/slave task relationship,
\ +* I have avoided as much as much as possible using thread synchronization
\ +* mechanisms other than *\fo{STOP} and *\fo{RESTART}.

\ +P I could have chosen a finer grain in lockin/unlocking database access but
\ +* Controlling sempahore release on exceptions could be a nightmare.
\ +* Doing a whole lock and release cycle in one place avoids this problem.

\ ***********
\ *S Glossary
\ ***********

MODULE INDI-TASK

\ =============
\ *N Task Words
\ =============

\ -----------------
\ *H Task variables
\ -----------------


/XMLBuf +USER RxBuffer
\ +G An XML Buffer per task.

2 cells +User RootTag
\ +G Current tag string (reference) in XML input buffer.

cell +User TheServer
\ *G The *\fo{indi-server} class instance that created this task.

cell +User RxSocket
\ *G Reference to a per-server client socket (using GENIO interface).

cell +User RxSemaphore
\ *G Reference to a per-server semaphore to serialize access to the shared
\ ** data tree from this task task and the *\fo{MAIN} task.

cell +User dbgTask
\ *G Bitfield containing flags for incoming XML selective printing
\ ** to standard output. Used for debugging purposes.


\ ----------------------
\ *H XML Buffer handling
\ ----------------------

Defer parse-xml			\ ca1 u1 ca2 u2 --
\ *G Decode a full XML INDI message. *\i{ca1 u1} is the full INDI
\ ** message and *\i{ca2 u2}is the tag string.



: see-message				  \ ca1 u1 ca2 u2 --
\ +G The default implementation of *\fo{decode-message} that
\ +* displays the message name and buffer statistics.
\ +* Useful for debuggin purposes. Assigned by default until the true
\ +* INDI parser installs.   
   base @ >r decimal
   cr
   type space
   [char] ( emit . drop [char] ) emit space cr
   RxBuffer .xb-buffer
   \ ." Stack = " depth .
   r> base !
;

' see-message IS parse-xml

: stop-listener				  \ n1 --
\ +G Stop the listener task. *\{n1} is the throw code.   
   dup indi .#excp
   s"  (a Listener) closes socket and stops" indi logtask
   RxSocket @ close-gio drop
   stop
   throw				\ just in case ...
;


: store-into-buffer			  \ --
\ +G Read as much data as possible from the socket and
\ +* store it into the XML input buffer. Stops listener on I/O errors.
   RxBuffer xb-available
   RxSocket @ ReadEx-gio       
   ?dup if
      stop-listener			\ and never come back ...
   then		
   RxBuffer xb-wp+!      
;


: find-tag			  \ -- ca1 u1 ca2 u2
\ +G Returns the remaining string *\i{ca2 u2} after the tag *\i{ca1 u1}.   
   RxBuffer xb-used open-tag 2dup RootTag 2!
;


: (parse-xml)		 \ ca1 u1 ca2 u2 --
\ +G Wraps an exception handler around *\fo{PARSE-XML} and makes sure
\ +* that the *\fo{RxSemaphore} semaphore is released both on normal
\ +* and abnormal termination.
\ +* Also re-throws the exception to perform further actions such as printing
\ +* and error message.   
   RxSemaphore @ request 
   RootTag 2@ ['] parse-xml catch
   RxSemaphore @ signal throw 
;


: analyze-buffer				  \ --
\ +G Do as much XML parsing as possible with the data we have in the
\ +* XML input buffer. Stops when there is not a full XML element to be
\ +* parsed. Also exits when a tag is not yet found.
  begin
     find-tag				\ -- ca1 u1 ca2 u2
     dup 0= if
	4drop exit			\ no tag found, bail out
     then
     full-element?			\ -- ca3 u3 True | False
  while					\ -- ca3 u3 ; if true, XML message
	2dup 
	(parse-xml)		\ may throw exceptions.
	+ RxBuffer xb.rp !		\ updates read pointer
  repeat
;


\ -------------------------
\ *H Task action words (II)
\ -------------------------


: is-action \ xt -- ;
\ +G wrapper around real task action code given by *\i{xt}.
\ +* Catches, prints exceptions and stops the *\fo{Listener} task.
   catch stop-listener
;


: listener-init				  \ --
\ +G Initializes the listener task, mainly the internal reception buffer.   
   decimal default-io 
   RxBuffer xb-init
;


: (Listener)
\ +G The actual Listener loop that initialzes, reads data into buffer
\ +* searches for complete XML messages and calls the parser and perform
\ +* buffer reallocation and growing if necessary.   
   stop					\ wait until master server allows.
   listener-init
   begin
      store-into-buffer
      analyze-buffer
      RxBuffer xb-realloc&grow
   again
; 


: Listener				  \ --
\ *G The task action being *\fo{INITIATE}d.
\ +* Only exist from *\i{is-action} if an exception has been catched.   
   ['] (Listener) is-action 
;


Export Listener
Export TheServer
Export RxSemaphore
Export RxSocket
Export dbgTask
Export parse-xml


END-MODULE

\ ======
\ *> ###
\ ======

