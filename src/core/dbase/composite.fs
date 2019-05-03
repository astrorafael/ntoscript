((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/composite.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ =============================
\ *N *\fo{INDI-COMPOSITE} class
\ =============================

\ *P *\fo{INDI-COMPOSITE} class is the base class for all container classes 
\ ** that have other objects embedded. As such, it should not be directly
\ ** instatiated. Usable descendants of this class are:
\ *(
\ *B *\fo{INDI-SERVER}.
\ *B *\fo{INDI-DEVICE}.
\ *B The different property vectors (*\fo{TEXT-VECTOR}, *\fo{NUMBER-VECTOR},
\ ** etc.)
\ *)


\ ---------------
\ +H Design notes
\ ---------------

\ +P This class maintains an internal linked list of *\fo{INDI-DEVICE} objects.
\ +* Efficient insertion at the end is done using a *\fo{m-last} pointer.
\ +* There is no provision yet to delete children objects. Thus,
\ +* *\f{<delProperty>} INDI message cannot yet be implemented.

\ +P Besides insertion, the other important operation is *\fo{SEARCH-ITEM}.
\ +* The search is case-sensitive.

\ +P The *\fo{SIZE} selector was implemented by sweeping the list to see how
\ +* it worked.
\ +* This method is secondary and execution speed is not so important. 

\ +P *\b{Warning:} It seems that VFX local variables do not
\ +* work within methods.



\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-item class
   i-list       implementation		\ only implement push-back
   i-connection implementation		\ connect/connected?/disconnect

   cell% inst-var m-head		\ list head
   cell% inst-var m-last		\ list tail

 protected
   
   selector getProperties ( this -- )	\ variations of this XML message

end-class indi-composite
\ +]


indi-composite methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

   m:   ( this -- ) 			\ overrides begin-xml
\ +G Do nothing. What's really important is *\fo{body-xml}.
\ +* Derived classes should override this method.     
  ;m overrides begin-xml

  
   m:   ( this -- ) 			\ overrides body-xml
\ +G Invoke *\fo{send} for all children property items.     
     m-head @ begin dup while dup send next-item @ repeat drop
  ;m overrides body-xml

  
   m:   ( this -- ) 			\ overrides end-xml
\ +G Do nothing. What's really important is *\fo{body-xml}.
\ +* Derived classes should override this method.     
  ;m overrides end-xml

  
  :m (size)   ( this -- u )			
\ +G Count children *\fo{INDI-ITEM}s by sweeping the list.
     m-head @ 0			\ -- addr counter
     begin over while 1+ swap next-item @ swap repeat nip
  ;m

  
\ -----------------
\ *H Public methods
\ -----------------

public
  
  m:   ( ca1 u1 this -- )		\ overrides construct
\ *G Creates a new *\fo{indi-composite} object with name given by *\i{ca1 u1}.
     this [parent] construct  0 dup m-head !  m-last !
  ;m overrides construct

  
  m:   ( item this -- )			\ overrides push-back
\ +G Add a child *\fo{indi-item} to the internal collection.
\ +* Not thread-safe version, it is intended for the *\fo{Listener} task.     
     this over parent!			\ set other node parent to 'this'
     m-head @ 0= if dup m-head ! then	\ first time 
     m-last @ ?dup if >r dup r> next-item ! then \ updates last item pointer
     m-last !				\ set the last element
  ;m overrides push-back

  
  m:   ( this -- )			\ overrides get
\ *G Get properties for this server/device/property.
     this lock
     this sid [io SetIO this ['] getProperties catch io]
     this unlock
     throw
  ;m overrides get

  
  :m size   ( this -- u )			
\ *G Get children objects' count.
     this dup lock dup (size) swap unlock
  ;m

  
  m:   ( this --  )			\ overrides print
\ *G Recursively print children *\fo{INDI-ITEM}s.
\ +* *\b{Warning:} Potential race conditions with concurrent item deletes.     
     m-head @ begin dup while dup print cr next-item @ repeat drop
  ;m overrides print


  m:   ( this --  )			\ overrides latch
\ *G Recursively copy ive values onto hold values in children objects.
\ +* *\b{Warning:} Potential race conditions with concurrent item deletes.     
     m-head @ begin dup while dup copy next-item @ repeat drop     
  ;m overrides copy

  m:   ( this -- flag )			\ overrides connected?
     this parent connected?
  ;m overrides connected?

  
  m:   ( this -- )			\ overrides disconnect
     this parent disconnect
  ;m overrides disconnect

  
  :m search-item   ( ca1 u1 this -- obj true | false )
\ *G Search for a given *\fo{INDI-ITEM} by name in its collection.
\ ** The search is case sensitive.
\ ** Not thread-safe version, it is intended for the *\fo{Listener} task.
\   { addr len } VFX LOCAL VARIABLES DON'T WORK !!!!
     m-head @ true			\ -- ca1 u1 obj flag
     begin over 0<> and while		\ -- ca1 u1 obj
	   >r r@ name			\ -- ca1 u1 ca2 u2 
	   2over str= if		\ -- ca1 u1 
	      r> false			\ --  ca1 u1 obj false
	   else
	      r> next-item @ true	\ -- ca1 u1 obj2 true
	   then 
     repeat
     -rot 2drop				\ -- obj | NULL
     dup if True then
  ;m

  
  :m lookup  ( ca1 u1 this -- obj true | false ) 
\ *G Search for a given *\fo{INDI-ITEM} by name in its collection.
\ ** The search is case sensitive. Thread-safe version, uses a lock.
\ ** and it is intended for the *\f{MAIN} task.     
     this dup lock   search-item   this unlock
  ;m
  
end-methods persistant

\ ---------------------
\ *H Other helper words
\ ---------------------

  
: find-item				  \ ca1 u1 obj1 n1 -- obj2
\ *G Find a children object *\i{obj2} whose name is *\i{ca1 u1}
\ ** in parent object *\i{obj1}. Throw *\i{n1} exception code if not found.
   >r search-item 0= if r> throw then r> drop
;

\ ======
\ *> ###
\ ======
