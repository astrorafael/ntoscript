((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/numberitem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==========================
\ *N *\fo{NUMBER-ITEM} class
\ ==========================

\ *P The *\fo{NUMBER-ITEM} class is the contained class of *\fo{NUMBER-VECTOR}
\ ** It can be directly instantiated.


\ *P The following public selectors may be invoked by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{MIN-VAL}
\ *B *\fo{MAX-VAL}
\ *B *\fo{STEP}
\ *B *\fo{DISPLAY-FORMAT}
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
\ +B *\fo{MIN-VAL!}
\ +B *\fo{MAX-VAL!}
\ +B *\fo{STEP!}
\ +B *\fo{DISPLAY-FORMAT!}
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

\ +P *\fo{NUMBER-ITEM}s have four more attributes besides the name and label:
\ +(
\ +B *\b{min}.    A float. Never modified (no need to lock).
\ +B *\b{max}.    A float. Never modified (no need to lock).
\ +B *\b{step}.   A float. Never modified (no need to lock).
\ +B *\b{format}. A string. Never modified (no need to lock).
\ +)

\ +P See the INDI specification for their descriptions.

\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-item class

   float% inst-var m-value		\ real value coming from remote side
   float% inst-var m-hold		\ hold area for XML output
   float% inst-var m-min		\ minimum allowed value
   float% inst-var m-max		\ maximum allowed value
   float% inst-var m-step		\ step value
   cell%  inst-var m-format		\ suggested display format

end-class number-item
\ +]


number-item methods

\ --------------------
\ +H Protected methods
\ --------------------
   
protected

  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<oneNumber ... >}.     
     2 spaces ." <oneNumber" space   this .name   [char] > emit
  ;m overrides begin-xml

  
  m:   ( this -- )			\ overrides body-xml
 \ +G Send the 'hold' value in scientific format as PCDATA.
     m-hold f@  f.
  ;m overrides body-xml

  
  m:   ( this -- )			\ overrides end-xml
\ +G Send *\f{</oneNumber>}.    
     ." </oneNumber>" cr
  ;m overrides end-xml


  :m .info    ( this -- )
     [char] { emit
     m-min  f@ f.
     m-max  f@ f.
     m-step f@ f.
     m-format $@ type
     [char] } emit
  ;m
  
\ -----------------
\ *H Public methods
\ -----------------
   
public

  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{NUMBER-ITEM} object with name given by *\i{ca1 u1}.     
     this [parent] construct
     %0 m-value f!  %0 m-hold f!
     %0 m-min   f!  %0 m-max  f!   %0 m-step f!
     0 m-format !
  ;m overrides construct

  
  m:   ( F: r1 ; S: this -- )		\ overrides set
\ *G Set the new value ('hold' value)  for this *\fo{NUMBER-ITEM}.     
     m-hold f!
  ;m overrides set

  
  m:  ( this -- F: r1 )			\ overrides get
\ *G Get the new 'live' value for this *\fo{NUMBER-ITEM}.     
     this lock  m-value f@  this unlock
  ;m overrides get
  
   
  m:   ( F: r1 ; S: this -- )		\ overrides update
\ +G Update the 'live' value with new datum. Done by the *\fo{LISTENER} task.
     m-value f!
  ;m overrides update			
  
   
  m:  ( this -- )			\ overrides print
\ *G Print *\fo{NUMBER-ITEM} name and value to the standard output.          
     this (.name) 
     m-value f@ f. 
     this .info space
     this .label cr
  ;m overrides print
  
  
  m:  ( this -- )			\ overrides copy
\ *G Copy 'live' value into 'hold' value.     
     this get m-hold f!
  ;m overrides copy
  

  :m min-val   ( this -- ; F -- r1 )
\ *G Get the item's minimum value. Useful for GUIs.            
     m-min f@
  ;m

  
  :m min-val!   ( this -- ; F r1 -- )
\ +G Set the item's minimum value. Used by the *\fo{Listener} task only.     
     m-min f!
  ;m

  
  :m max-val   ( this -- ; F -- r1 )
\ *G Get the item's maximum value. Useful for GUIs.       
     m-max f@
  ;m

  
  :m max-val!   ( this -- ; F -- r1 )
\ +G Set the item's maximum value. Used by the *\fo{Listener} task only.
     m-max f!
  ;m

  
  :m step   ( this -- ; F -- r1 )
\ *G Get the item's step value. Useful for GUIs.       
     m-step f@
  ;m

  
  :m step!   ( this -- ; F r1 -- )
\ +G Set the item's step value. Used by the *\fo{Listener} task only.
     m-step f!
  ;m

  
  :m display-format   ( this -- ca1 u1 )
\ *G Get the item's suggested print format string. Useful for GUIs.  
     m-format $@
  ;m

  
  :m display-format!   ( ca1 u1 this --  )
\ +G Set the item's suggested print format string. Useful for GUIs.  
     m-format $!
  ;m

  
end-methods persistant


\ ======
\ *> ###
\ ======
