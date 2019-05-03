((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/propvector.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==============================
\ *N *\fo{PROPERTY-VECTOR} class
\ ==============================

\ *P The *\fo{PROPERTY-VECTOR} class is another abstract class for the
\ ** different property vectors encapsulating common attributes.
\ ** As such, it should not be directly instantiated.


\ *P Common attributes to all property vectors:
\ *(
\ *B *\b{name}.    A string. Never modified (no need to lock).
\ *B *\b{label}.   A string. Never modified (no need to lock).
\ *B *\b{group}.   A string. Never modified (no need to lock).
\ *B *\b{permission}.  An integer. Never modified (no need to lock).
\ *B *\b{timeout}. An integer. Can be modified (needs a lock).
\ *B *\b{state}.   An integer. Can be modified (needs a lock).
\ *)

\ *P In addition, *\fo{SWITCH-VECTOR}s have:
\ *(
\ *B *\b{rule}.   An integer. Never modified (no need to lock).
\ *)

\ *P By exception, *\fo{LIGHT-VECTOR}s *\b{do not} have:
\ *(
\ *B *\b{permission}.
\ *B *\b{timeout}.
\ *)

\ ---------------
\ +H Design notes
\ ---------------

\ +P Initially, I had the idea of writting a general purpose
\ +* *\fo{PROPERTY-VECTO\ +* R} and not subclassing the different Text, Number
\ +* Switch, etc. property vectors.
\ +* Later on, some peculiarties of the Switch and Light vectors made me
\ +* go on with the idea of dedicating a separate subclass for each.
\ +* I have also chosen to include subclass light-vectors from the generic
\ +* property vector and override selctors to throw exception on access to
\ +* these attributes. It is quicker that dividing the property vectors
\ +* in two hierarchy branches.

\ +P Scripts are supposed to be synchronous. For that reason, I added a
\ +* *\fo{SEND&WAIT} method, for simple do-or-abort operation.
\ +* More sophisticated
\ +* behaviour is still possible using its factors *\fo{SEND} and *\fo{WAIT}.

\ +P The timeout attribute of a property vector is probably a floating
\ +* point number. In the INDI standard there is no slight hint about the units
\ +* (probably seconds). For that reason, I have casted this value as a single
\ +* cell integer value.

\ +P A default timeout value *\fo{PROPERTY-TIMEOUT} is used when the INDI
\ +* properties specify a zero or missing timeout, as per the INDI spec.
\ +* This means that a well behaved CCD device should specify somehow the
\ +* estimated upper limit timeout for the property which triggers the
\ +* exposure. Not doing so would cause the script to abort  when
\ +* *\fo{PROPERTY-TIMEOUT} seconds have passed.

\ +P Words *\fo{(propvector)} and *\fo{(propitem)} are factors for state-smart,
\ +* parsing words *\fo{propvector} and *\fo{propitem},
\ +* designed for scripts but can also be useful
\ +* by itself in GUIs. They take arguments in the reverse order of parsing.


\ ------------------
\ *H INDI Exceptions
\ ------------------

\ *[
ErrDef Ok-Excp    "Property in OK state"
ErrDef Busy-Excp  "Property in BUSY state"
ErrDef Alert-Excp "Property in ALERT state"
ErrDef Idle-Excp  "Property in IDLE state"
\ *]

\ ----------------------------------------
\ *H Property vector attributes and values
\ ----------------------------------------

60 Value PROPERTY-TIMEOUT
\ *G The default timeout in seconds to wait for a property's state change
\ ** from *\fo{BUSY} to some other value.


0 Constant IDLE
\ *G The IDLE or 'grey' property state.

1 Constant OK
\ *G The OK or 'green' property state.

2 Constant BUSY
\ *G The BUSY or 'yellow' property state.

3 Constant ALERT
\ *G The ALERT or 'red' property state.


: >$state   ( n -- ca1 u1)
\ *G Given the state number *\i{n}, gets its string literal.
   case
      Idle  of s" Idle"  endof
      Ok    of s" Ok"    endof
      Busy  of s" Busy"  endof
      Alert of s" Alert" endof
   endcase
;


: >state  ( ca u - n True | false )
\ *G Translates *\f{"Idle"}, *\f{"Ok"}, *\f{"Busy"}, *\f{"Alert"} strings
\ ** to integer constants
\ ** used for lights and property states.
\ ** *\i{flag} is true upon succesful conversion.   
\ We do not use *\fo{EVALUATE}.   
   2dup s" Idle"  str= if 2drop Idle  True exit then
   2dup s" Ok"    str= if 2drop Ok    True exit then
   2dup s" Busy"  str= if 2drop Busy  True exit then
        s" Alert" str= if       Alert True exit then
   False
;

     
: >$permission   ( n -- ca1 u1)
\ *G Given the permission constant *\i{n}, *\fo{r/o}, *\fo{w/o} and *\fo{r/w},
\ ** gets its string literal.
   case
      r/o  of s" ro" endof
      w/o  of s" wo" endof
      r/w  of s" rw" endof
   endcase
;


: >permission  ( ca u -- n True | False )
\ *G Translates *\f{"ro"}, *\f{"wo"} and *\f{"rw"} strings
\ ** to integer constants *\fo{r/o},  *\fo{w/o} and *\fo{r/w}.
\ ** Returned flag is true upon succesful conversion.      
   2dup s" ro" str= if 2drop r/o True exit then
   2dup s" wo" str= if 2drop w/o True exit then
        s" rw" str= if       r/w True exit then
   False
;


\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-composite class

   cell% inst-var m-state		\ property state
   cell% inst-var m-timeout		\ timeout for waits (in seconds)
   cell% inst-var m-group		\ GUI group
   cell% inst-var m-perm		\ permission
   
   selector timeout    ( this -- n1 )
   selector permission ( this -- n1 )
   selector wait       ( this -- )
   
end-class property-vector
\ +]

property-vector methods

\ --------------------
\ +H Protected methods
\ --------------------

protected
   
  :m .device   ( sid this -- )
\ +G Prints the *\f{device} attribute to the output given by *\i{sid}.
     m-parent @ ?dup if
	." device="  [char] ' emit name type [char] ' emit
     then
  ;m

  
  m:   ( this -- ) 			\ overrides begin-xml
\ +G Print only the *\f{name} and *\f{device} pv's attributes.
\ +* Derived classes should override and call this method.     
     this dup .name space .device [char] > emit cr
  ;m overrides begin-xml
  
  
  :m .writables   ( this -- )
\ +G Print the property's state, permissions & timeout to standard output.
     [char] ( emit
     this dup lock m-state @ swap unlock >$state type [char] , emit
     m-perm @ >$permission type  [char] , emit
     m-timeout ?
     [char] ) emit
  ;m

  
  :m .label&group   ( this -- )
\ +G Print the property label and group to standard output.
     this .label space
     [char] [ emit m-group $@ type [char] ] emit 
  ;m

  :m ?disconnect   ( this -- )
\ +G Ask the *\fo{indi-server} object to disconnect if there were I/O errors.
     this connected? 0= if
	this disconnect
     then
  ;m
  
  
  m:   ( this -- )			\ overrides getProperties
\ +G Do the actual sending of
\ +* *\f{<getProperties device='<device> name='<property>' ... />}.
     ." <getProperties version='" INDI-VERSION type
     ." ' device='" this parent name type
     ." ' name='" m-name $@ type ." '/>" cr
  ;m overrides getProperties

  
\ -----------------
\ *H Public methods
\ -----------------
   
public
   
  m:   ( ca1 u1 this -- )			\  overrides construct
\ *G Create a new *\fo{PROPERTY-VECTOR} object with name given by *\i{ca1 u1}.
     this [parent] construct
     IDLE m-state ! 
     PROPERTY-TIMEOUT m-timeout ! 
     r/w m-perm !
     0 m-group !  s" " m-group $!	\ empty string
  ;m overrides construct   

  
  :m state   ( this -- n1 )
\ *G Get the property vector's state.
\ ** To be used by the *\fo{MAIN} task.
     this dup lock m-state @ swap unlock
  ;m
  

  :m state!   ( n1 this -- )
\ +G Set the property vector's state.
\ +* To be used by the *\fo{Listener} task.
     m-state !        
  ;m   

  
  m:   ( this -- n1 )			        \ overrides timeout
\ *G Get the property vector's timeout.
\ ** To be used by the *\fo{MAIN} task.
     this dup lock  m-timeout @   swap unlock 
  ;m overrides timeout

  
   :m timeout!   ( n1 this --  )
\ +G Set the property vector's timeout.
\ +* If *\i{n1} is zero, the *\fo{DEFAULT-TIMEOUT} value is assigned instead.
\ +* To be used by the *\fo{Listener} task.
     ?dup if m-timeout ! then
  ;m   

  
  m:   ( this -- n1 )			        \ overrides permission
\ *G Get the property vector's permission.
\ ** To be used by the *\fo{MAIN} task.
     m-perm @ 
  ;m overrides permission

  
   :m permission!   ( n1 this -- )
\ +G Set the property vector's permission.
\ +* To be used by the *\fo{Listener} task.
     m-perm !
  ;m   

  
  :m group   ( this -- ca u )
\ *G Get the property vector's group.
\ ** To be used by the *\fo{MAIN} task.
     m-group $@ 
  ;m

  
   :m group!   ( ca u this -- )
\ +G Set the property vector's group.
\ +* To be used by the *\fo{Listener} task.
     m-group $!
  ;m   

  
  m:	( this -- )				\ overrides send
\ *G Send *\f{<newXXXVector...> ... </newXXXVector>} and then
\ ** mark the property vector's state as *\fo{BUSY}.
     this lock
     this sid [io setIO  this ['] (send) catch io]
     BUSY m-state !
     this unlock
     throw
  ;m overrides send

  
  m:   ( this -- )			\ overrides wait
\ *G Wait until property'state becomes non-busy or
\ ** until the property's *\fo{timeout} have elapsed. Calls *\fo{PAUSE}.
     this timeout 1000 * later			
     begin				\ -- n2
	delayed
	dup expired			\ -- n2 flag1
	this state BUSY <> or 		\ -- n2 flag1|flag2
	this connected? 0= or		\ -- n2 flag1|flag2|flag3
     until
     drop        
     this ?disconnect   
  ;m overrides wait
  
  
  :m send&wait   ( this -- )
\ *G Convenient method to invoke *\fo{send} and *\fo{wait} for the answer.
\ ** Guarantees that the @fo{STATE} is not *\fo{BUSY} or else it throws
\ ** *\fo{Busy-Excp}.     
     this dup send dup wait state  
     BUSY = if Busy-Excp throw then
  ;m

  
  :m send&ok   ( this -- )
\ *G Convenient method to invoke *\fo{send} and *\fo{wait} for the answer.
\ ** Guarantees that the property *\fo{STATE} is *\fo{OK} or else it throws
\ ** *\fo{Busy-Excp}, *\fo{Idle-Excp} or *\fo{Alert-Excp}.     
     this dup send&wait state dup
     ALERT = if Alert-Excp throw then
     IDLE  = if Idle-Excp  throw then
  ;m


  :m send&idle   ( this -- )
\ *G Convenient method to invoke *\fo{send} and *\fo{wait} for the answer.
\ ** Guarantees that the property *\fo{STATE} is *\fo{IDLE} or else it throws
\ ** *\fo{Busy-Excp}, *\fo{Ok-Excp} or *\fo{Alert-Excp}.
\ ** Useful when switching devices off.       
     this dup send&wait state dup
     ALERT = if Alert-Excp throw then
     OK    = if Ok-Excp    throw then
  ;m

  
  m: ( this --  )			\ overrides print
\ *G Print to standard output the property name, state and individual
\ ** property item names and values.
     cr ." Property " m-name $@ type space
     this .writables space
     this .label&group cr
     m-head @ begin dup while 2 spaces dup print next-item @ repeat drop
  ;m overrides print

    
end-methods persistant
  
\ ---------------------
\ *H Other helper words
\ ---------------------

ErrDef Prop-Vector-Excp "Property vector not found"
\ *G
  
  
: (vector)			  \ ca1 u1 ca2 u2 ---
\ *G Find a property vector whose name is given by *\i{ca1 u1} in device
\ ** whose name is given by *\i{ca2 u2}.
\ ** Can throw a *\fo{Dev-Excp} or *\fo{Prop-Vector-Excp} exceptions if
\ ** any object is not found.
\ ** Uses the global context given by the *\fo{CurrentServer}.
\ +* For that reason it is not suitable for the *\fo{LISTENER} task
\ +* (not thread-safe).      
   (device) Prop-Vector-Excp find-item
;


ErrDef Prop-Item-Excp   "Property item not found"
\ *G

: (item)			  \ ca1 u1 ca2 u2 ca3 u3 ---
\ +G Find a property item whose name is given by *\i{ca1 u1} inside a
\ +* property vector whose name is  *\i{ca2 u2} inside a device
\ +* whose name is given by *\i{ca3 u3}.
\ +* Can throw a *\fo{Dev-Excp}, *\fo{Prop-Vector-Excp} or
\ +* *\fo{Prop-Vector-Excp} exceptions if any object is not found.    
   (vector) Prop-Item-Excp find-item
;


\ ======
\ *> ###
\ ======
