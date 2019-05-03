((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/lightitem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ =========================
\ *N *\fo{LIGHT-ITEM} class
\ =========================

\ *P The *\fo{LIGHT-ITEM} class is the contained class of *\fo{LIGHT-VECTOR}
\ ** It can be directly instantiated.

\ *P The following public selectors may be invoked by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{GET}
\ *B *\fo{PRINT}
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

\ +P Light values do not support output XML generation
\ +* so we leave *\fo{begin-xml}, *\fo{body-xml} and *\fo{end-xml} undefined.

\ +P Lights do not have a *\fo{set} method, thus generating an exception
\ +* by default.

\ +P As the 'live' value can change anytime, we must use a lock to get access
\ +* to it.

\ ---------------
\ *H Helper words
\ ---------------

Alias: >$light >$state   ( n -- ca1 u1 )
\ *G The same set of strings and constants are used for property state
\ ** and light values.

Alias: >light >state   ( ca1 u1 -- n flag )
\ *G Idem.

\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-item class

   cell% inst-var m-value		\ real value coming from remote side

end-class light-item
\ +]


light-item methods

\ -----------------
\ *H public methods
\ ----------------
   
public

  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{LIGHT-ITEM} object with name given by *\i{ca1 u1}.     
     this [parent] construct
     IDLE m-value ! 
  ;m overrides construct

  
  m:   ( this -- n1 )			\ overrides get
\ *G Get the new 'live' value for this *\fo{LIGHT-ITEM}.     
     this lock  m-value @  this unlock
  ;m overrides get

  
  m:  ( n1 this -- )			\ overrides update
 \ +G Update the 'live' value with new datum. Done by the *\fo{LISTENER} task.
     m-value !
  ;m overrides update

  
  m:  ( this -- )			\ overrides print
\ *G Print *\fo{LIGHT-ITEM} name and value to the standard output.      
     this (.name)
     this get >$light type space
     this .label cr
  ;m overrides print

  
end-methods persistant


\ ======
\ *> ###
\ ======
