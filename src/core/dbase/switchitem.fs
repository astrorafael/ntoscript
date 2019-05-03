((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/switchitem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==========================
\ *N *\fo{SWITCH-ITEM} class
\ ==========================

\ *P The *\fo{SWITCH-ITEM} class is the contained class of *\fo{SWITCH-VECTOR}
\ ** It can be directly instantiated.

\ *P The following public selectors may be invoked by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{SET}
\ *B *\fo{GET}
\ *B *\fo{COPY}
\ *B *\fo{PRINT}
\ *B *\fo{SEND}
\ *)

\ +P The following public selectors are invoked by the *\fo{LISTENER} task:
\ +(
\ +B *\fo{CONSTRUCT}
\ +B *\fo{LABEL!}  (inherited from *\fo{INDI-ITEM})
\ +B *\fo{UPDATE}
\ +)


\ ---------------
\ +H Design notes
\ ---------------

\ +P This class encapsulated two values:
\ +(
\ +B The 'hold' value that we *\{SET} in the script and will be sent to the
\ +* remote device as the new proposed value.
\ +B The real 'live' value that we *\fo{GET} from the device on return or any
\ +* other asynchronous message. This value may be different from the one sent.
\ +)

\ +P Most of the time we can ignore this distinction.
\ +* However, there are times when we may want to synchronize these two using
\ +* the method *\fo{COPY}, which copies the 'live' value onto the
\ +* 'hold' value.

\ +P As the 'live' value can change anytime, we must use a lock to get access
\ +* to it.

\ ---------------
\ *H Helper words
\ ---------------

: >switch ( ca1 u1 - flag1 True | False )
\ *G Translates *\f{"On"} and *\f{"Off"} INDI strings to boolean flags.
\ ** *\i{flag1} is the converted value and flag on TOS is the 'found' flag.
\ +* We can't use *\fo{EVALUATE} because these words are already defined.   
   2dup
   s" On"  str= if 2drop True  dup exit then
   s" Off" str= if       False True exit then
   False 
;

: >$switch   ( flag -- ca1 u1)
\ *G Given the switch *\i{flag}, gets its string literal.
   case
      True  of s" On"  endof
      False of s" Off" endof
   endcase
;


\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-item class

   cell% inst-var m-value		\ real value coming from remote side
   cell% inst-var m-hold		\ hold area for XML output

end-class switch-item
\ +]

switch-item methods

\ --------------------
\ +H Protected methods
\ --------------------

protected
  
  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<oneSwitch ... >}.     
     2 spaces ." <oneSwitch" space   this .name   [char] > emit
  ;m overrides begin-xml

  
  m:   ( this -- )			\ overrides body-xml
\ +G Send *\f{On} or *\f{Off} PCDATA.          
     m-hold @ >$switch type
  ;m overrides body-xml

  
  m:   ( this -- )			\ overrides end-xml
\ +G Send *\f{</oneSwitch>}.          
     ." </oneSwitch>" cr
  ;m overrides end-xml

  
\ -----------------
\ *H public methods
\ ----------------
   
public

  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{SWITCH-ITEM} object with name given by *\i{ca1 u1}.  
     this [parent] construct
     false dup m-value ! m-hold !
  ;m overrides construct


  m:   ( flag this -- )		\ overrides set
\ *G Set the new value ('hold' value)  for this *\fo{SWITCH-ITEM}.
     m-hold !   this m-parent @ last-sw !
  ;m overrides set

   
  m:   ( this -- flag )		\ overrides get
\ *G Get the new 'live' value for this *\fo{SWITCH-ITEM}.
     this lock  m-value @  this unlock
  ;m overrides get

   
  m:   ( flag this -- )			\ overrides update
\ +G Update the 'live' value with new datum. Done by the *\fo{LISTENER} task.
     m-value !
  ;m overrides update

  
  m:  ( this -- )			\ overrides print
\ *G Print *\fo{SWITCH-ITEM} name and value to the standard output.     
     this (.name) space
     this get >$switch type space
     this .label
     cr
  ;m overrides print

  
  m:  ( this -- )			\ overrides copy
\ *G Copy 'live' value into 'hold' value.
     this get m-hold !
  ;m overrides copy

  
end-methods persistant


\ ======
\ *> ###
\ ======
