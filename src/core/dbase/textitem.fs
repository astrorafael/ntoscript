((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/textitem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ========================
\ *N *\fo{TEXT-ITEM} class
\ ========================

\ *P The *\fo{TEXT-ITEM} class is the contained class of *\fo{TEXT-VECTOR}
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

\ +P This property managed dynamic allocated strings.

\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-item class

   cell% inst-var m-value		\ real value coming from remote side
   cell% inst-var m-hold		\ hold area for XML output

end-class text-item
\ +]


text-item methods

\ --------------------
\ +H Protected methods
\ --------------------
   
protected

  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<oneText ... >}.     
     2 spaces ." <oneText" space  this .name   [char] > emit
  ;m overrides begin-xml
   
   
  m:   ( this -- )			\ overrides body-xml
\ +G Send text hold value as PCDATA.    
     m-hold dup @ if $@ type else drop then
  ;m overrides body-xml
   
   
  m:   ( this -- )			\ overrides end-xml
\ +G Send *\f{</oneText>}.      
     ." </oneText>" cr
  ;m overrides end-xml
  
\ -----------------
\ *H Public methods
\ -----------------
   
public

  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{TEXT-ITEM} object with name given by *\i{ca1 u1}.
     this [parent] construct
     0 dup m-value ! m-hold !
  ;m overrides construct

  
  m:   ( ca1 u1 this -- )		\ overrides set
\ *G Set the new value ('hold' value)  for this *\fo{TEXT-ITEM}.    
     m-hold $!
  ;m overrides set

   
  m:   ( this -- ca1 u1 | x 0 )		\ overrides get
\ *G Get the new 'live' value for this *\fo{TEXT-ITEM}. If no value is yet
\ ** available, returns 0 as string length.     
     this lock
     m-value dup @ if $@ else 0 then
     this unlock
  ;m overrides get
  

  m:   ( ca1 u1 this -- )		\ overrides update
\ +G Update the 'live' value with new text. Done by the *\fo{LISTENER} task.  
     m-value $!
  ;m overrides update
  
  
  m:   ( this -- )			\ overrides print
\ *G Print *\fo{TEXT-ITEM} name and value to the standard output.     
     this (.name)
     this get dup if type else 2drop ." <empty>" then space
     this .label cr
  ;m overrides print
  

  m:    ( this -- )			\ overrides copy
\ *G Copy 'live' value into 'hold' value.
\ ** If no 'live' value, empty the 'hold' value.
     this get dup if m-hold $! else drop m-hold $off then 
  ;m overrides copy

   
end-methods persistant


\ ======
\ *> ###
\ ======
