((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/item.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ========================
\ *N *\fo{INDI-ITEM} class
\ ========================

\ *P The *\fo{INDI-ITEM} class is the base class for all the properties
\ ** database levels. As such, it must *\b{not} be directly instantiated.

: INDI-VERSION				  \ -- ca u
\ *G Returns the current protocol version string supported by this client
\ ** library.
   s" 1.5"
;


\ ---------------
\ +H Design notes
\ ---------------

\ +P The *\fo{INDI-ITEM} class is the most generic class of the
\ +* *\i{Composite} pattern. The following implementation does not use
\ +* any additional collection classes, so a *\fo{m-next} pointer is
\ +* embedded in every object.

\ +P *\b{Public attributes vs getter/setter methods.}
\ +* In principle, using getter/setter methods have the advantage of using a
\ +* standard, consistent interface to these attributes.
\ +* However, they double the number of words to define.
\ +* Most attributes do not have an specific
\ +* behaviour when they are read or written and a public attribute would
\ +* be much simpler to use. There are some reasons why I use
\ +* getter/setter methods in this implementation:
\ +( a
\ +B The *\fo{state} atribute changes often and needs a lock to
\ +* serialize concurrent access and this can be embedded in the methods.
\ +* A similar thing occurs to *\fo{timeout} attribute.
\ +B The *\fo{LIGHT-VECTORS} do not have some attributes that all others have.
\ +* In this case, methods are overriden to produce exceptions.
\ +B Encapsulate the way "string attributes" are read or written.
\ +)

\ +P *\b{Overrinding the send selector}
\ +* Most *\fo{INDI-ITEM} objects have a favourite INDI message to send
\ +* to a server. For that reason, method *\fo{send} can be overwritten
\ +* at every level. Anyway, this class provides a default implementation
\ +* using helper methods *\fo{begin-xml}, *\fo{body-xml} and *\fo{end-xml}.
\ +* that sends the header, body and trailer of the XML message.

\ +P *\b{Socket I/O.}
\ +* XML I/O needs the socket GENIO *\i{sid}. A selector is provided for this.
\ +* The chosen policy is for each object to ask its parent for it until
\ +* it reaches to a *\fo{INDI-SERVER} object who really knows it.
\ +* Although this has some overhead, we make sure there is no problems with
\ +* out of date cached copies.

\ +P *\b{Semaphores and multitasking.}
\ +* Although semaphores are not strictly needed if using a cooperative
\ +* multitasker, we have included them and also *\fo{LOCK} and *\fo{UNLOCK}
\ +* selectors to allow using this library with the standard, pre-emptive
\ +* multitasker provided by MPE. Again, it uses the same
\ +* chain of responsability pattern, clumsy but effective.

\ ------------------
\ +H Class structure
\ ------------------

\ +[
object class   
   i-property implementation		\ implement get/set selectors
   i-iterator implementation		\ implement next-item
   
   cell% inst-var m-name		\ item name
   cell% inst-var m-label		\ item label for GUI usage
   cell% inst-var m-next		\ next indi-item pointer
   cell% inst-var m-parent		\ parent item
   
 protected 

   selector sid    ( this -- sid )	\ Get GENIO sid for XML I/O
   selector lock   ( this -- )		\ Lock database access for updates
   selector unlock ( this -- )		\ Unlock database access
   
   \ XML generation interface, redefined as convenient in each level
   
   selector begin-xml ( this -- )	\ write XML generation preamble
   selector body-xml  ( sid sid -- )	\ write XML generation body
   selector end-xml   ( this -- )	\ write XML generation end

 public

   selector send   ( this -- )		\ send XML message
   selector update ( i*x this -- )	\ to be used in property items
   selector copy   ( this -- )	        \ copy input value into hold value

end-class indi-item
\ +]



indi-item methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  m:   ( this -- sid )				\ overrides sid
\ +G Get the GENIO socket *\i{sid}. Actually, ask for it to its *\fo{parent}.
\ +* This chain of responsibility goes up to the *\fo{indi-server} level,
\ +* which knows what the proper value is.   
     m-parent @ sid
  ;m overrides sid

  
  m:   ( this -- sid )				\ overrides lock
\ +G Lock database for concurrent updates.
\ +* Actually, ask it to its *\fo{parent}.
\ +* This chain of responsibility goes up to the *\fo{indi-server} level,
\ +* which handles the semaphore.
     m-parent @ lock
  ;m overrides lock

  
  m:   ( this -- sid )				\ overrides unlock
\ +G Unlock database access. Actually, ask it to its *\fo{parent}.
\ +* This chain of responsibility goes up to the *\fo{indi-server} level,
\ +* which handles the semaphore.     
     m-parent @ unlock
  ;m overrides unlock


  :m parent!   ( obj this -- )
\ +G Set *\i{this} object's parent to *\i{obj}.    
     m-parent !
  ;m
  
  :m .name   ( this -- )			
\ +G Print the 'name' attribute to be used in XML format.
     ." name=" [char] ' emit m-name $@ type [char] ' emit
  ;m


  :m (.name)   ( this -- )
\ +G Print the name part for the *\fo{print} selector.   
     m-name $@ type space [char] = emit space
  ;m

  
  :m .label    ( this -- )
\ +G Print the 'label' attribute for the *\fo{print} selector.
     [char] [ emit m-label $@ type [char] ] emit
  ;m

  
  :m (send)   ( this -- )
\ +G Do the actual XML message transmission by invoking
\ +* *\fo{begin-xml} *\fo{body-xml} and *\fo{end-xml}.
     this dup begin-xml dup body-xml end-xml 
  ;m


\ -----------------
\ *H Public methods
\ -----------------

public
  
  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{indi-item} object with name given by *\i{ca1 u1}.
     0 dup m-name ! m-label ! m-name $!   0 dup m-next ! m-parent !
     m-name $@ m-label $!		\ initializes label to name
  ;m overrides construct


  :m name   ( this -- ca1 u1 )
\ *G Get the item's name string.   
     m-name $@
  ;m


  :m parent   ( this -- obj )
\ *G Get the item's parent object.
     m-parent @
  ;m


  :m label   ( this -- ca1 u1 )
\ *G Get the item's label string. Useful for GUIs.  
     m-label $@
  ;m

  
  :m label!   ( ca1 u1 this -- )
\ +G Set the item's label string. To be used by the *\fo{Listener} task only.
     m-label $!
  ;m
  
  m:  ( this -- addr )			\ overrides next-item
\ *G Get the address of the next *\fo{indi-item} object
\ ** for further *\fo{!} or *\fo{@}. Useful for getting siblings of a given
\ ** object in GUIs but not for scripts.
\ ** *\b{Warning:} This method is not thread-safe.
     m-next
  ;m overrides next-item
  
   
  m: ( this -- )			\ overrides send
\ *G Send the most characterisict INDI message to a server.
\ ** This is an abstract method with a default implementation.
\ ** in which subclasses overrides
\ ** *\fo{begin-xml} *\fo{body-xml} and *\fo{end-xml} with
\ ** the proper contents.
     this sid
     [io setIO this ['] (send) catch io]
     throw
  ;m overrides send				


end-methods persistant


\ ======
\ *> ###
\ ======
