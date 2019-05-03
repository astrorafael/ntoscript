((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/cli/dbase.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ######
\ *> cli
\ ######

\ ===========================
\ *N Database scripting words
\ ===========================

\ *P Database creation starts by creating the top level *\fo{INDI-SERVER}
\ ** object. A convenient parsing word *\fo{INDIServer:} is provided.
\ ** Then, you must connect to the remote server and let the INDI protocol
\ ** populate the lower levels of the hierarchy. 
\ ** Later on, use state-smart parsing words *\fo{DEVICE}, *\fo{VECTOR} or
\ ** *\fo{ITEM} to perform database object lookup.

\ *P *\b{Smart words are evil}.
\ ** Remember not to tick, *\fo{[COMPILE]} or *\fo{POSTPONE} these words
\ ** or any word containing them.
\ ** A paper on the subject may be found at
\ ** *\f{http://www.complang.tuwien.ac.at/papers/} as ertl98.ps.gz.

\ *P Usage of these words assume that a *\fo{CurrentServer} is already set.
\ ** *\fo{TheCurrentServer} is set when invoking a *\fo{CONNECT} on a given
\ ** *\fo{INDI-SERVER} object. It can also be manually changed by issuing:
\ *C    <an indi-server object> To TheCurrentServer


\ *P Example of usage:
\ *E
\ ** INDIServer: indiserver localhost 7624
\ ** indiserver connect
\ ** indiserver properties get
\ ** 2 seconds waiting
\ **
\ ** device CCDCam print
\ **
\ ** property vector CCDCam Exposure timeout .
\ **
\ ** True property item CCDCam Binning 1:1 set
\ **
\ ** : photo   ( -- )
\ **    5.0 property item   CCDCam ExpValues ExpTime set
\ **        property vector CCDCam ExpValues send&wait
\ ** ;


\ ===============
\ +N Design Notes
\ ===============

\ +P All three *\fo{(device)}, *\fo{(vector)} and *\fo{(item)}
\ +* take strings in reverse order to aid definition of compile-time factors
\ +* *\fo{[device]}, *\fo{[vector]} and *\fo{[item]},
\ +* although they complicate life for the interpret-time
\ +* behaviour.

\ ------------------
\ +H Syntactic sugar
\ ------------------

: property				  \ --
\ *G Syntactic sugar for *\fo{vector} or *\fo{item}. Usage:
\ *C   property vector Camera Exposure ...
\ *C   property item Camera Exposure ExpDur ....   
; immediate


: properties			  \ --
\ *G Syntactic sugar for *\fo{get}. Usage:
\ *C   indiserver properties get
\ *C   device Camera properties get
; immediate


: BLOBs					  \ --
\ *G Syntactic sugar for usages below:   
\ *C   device Camera BLOBs enable
\ *C   device camera BLOBs disable        
; immediate

: directory				  \ --
\ *G Syntactic sugar for usage below:   
\ *C   s" /tmp" property vector Camera Pixels directory set
; immediate


\ -----------------------
\ +H Compile-only factors
\ -----------------------


: [device]	     \ Comp: "device" -- ; Exec: -- obj
\ +G Immediate, compile-time only factor for *\fo{device}.
   ?comp
   parse-name postpone sliteral
   postpone (device)
; immediate


: [vector]	     \ Comp: "device" "propvector" -- ; Exec: -- obj
\ +G Immediate, compile-time only factor for *\fo{vector}.   
   ?comp
   parse-name parse-name
   postpone sliteral postpone sliteral
   postpone (vector)
; immediate


: [item] \ Comp: "device" "propvector" "propitem" -- ; Exec: -- obj
\ +G Immediate, compile-time only factor for *\fo{item}.   
   ?comp
   parse-name parse-name parse-name
   postpone sliteral postpone sliteral postpone sliteral
   postpone (item)
; immediate


\ --------------------
\ +H State smart words
\ --------------------

: INDIServer:			   \ "name" "host" "port" -- ; [child] -- obj
\ *G Create a named *\fo{INDI-SERVER} object that will connect to host 
\ ** and port given in the input stream.
\ ** Invoking the name will return the object handle *\i{obj}.
\ *C     INDIServer: indiserver1 localhost 7624
   ?exec
   parse-name parse-name parse-name (INDIServer)
; immediate


: device		    \ "device" -- obj
\ *G Find an *\fo{INDI-DEVICE} whose name is given in the input stream.
\ ** Throw a *\fo{Dev-Excp} exception if not found.   
   [ also forth ] state @ [ previous ] if
      postpone [device] exit
   then
   parse-name (device)
; immediate


: vector		    \ "device" "propvector" -- obj
\ *G Find a *\fo{PROPERTY-VECTOR} whose names are given in the input stream.
\ ** Can throw a *\fo{Dev-Excp} or *\fo{Prop-Vector-Excp} exceptions if
\ ** any object is not found.      
   [ also forth ] state @ [ previous ] if
      postpone [vector] exit
   then
   parse-name parse-name 2swap (vector)
; immediate


: item		    \ "device" "propvector" "propitem" -- obj
\ *G Find a *\fo{TEXT-ITEM, SWITCH_ITEM, ...} whose names are given in the
\ ** input stream.
\ ** Can throw a *\fo{Dev-Excp}, *\fo{Prop-Vector-Excp} or
\ ** *\fo{Prop-Item-Excp} exceptions if any object is not found.      
   [ also forth ] state @ [ previous ] if
      postpone [item] exit
   then
   parse-name 2>R parse-name parse-name 2swap 2R> (item)
; immediate


\ ======= 
\ *> ####
\ =======
