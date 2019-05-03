((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/numbervector.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ============================
\ *N *\fo{NUMBER-VECTOR} class
\ ============================

\ *P The *\fo{NUMBER-VECTOR} class is the container for all the
\ ** *\fo{NUMBER-ITEM} objects contained within.
\ ** It can be directly instantiated.

\ *P The following public selectors/methods may be invoked
\ ** by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{GROUP}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{STATE}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{TIMEOUT}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{GET} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{WAIT} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND&WAIT} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND&OK} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND&IDLE} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{PRINT} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{COPY} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{SIZE} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{LOOKUP} (inherited from *\fo{INDI-COMPOSITE})
\ *)

\ +P The following public selectors/methods are invoked by the *\fo{LISTENER}
\ +* task:
\ +(
\ +B *\fo{CONSTRUCT} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{LABEL!} (inherited from *\fo{INDI-ITEM})
\ +B *\fo{GROUP!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{STATE!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{TIMEOUT!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{PERMISSION!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{SEARCH-ITEM} (inherited from *\fo{INDI-COMPOSITE})
\ +B *\fo{PUSH-BACK} (inherited from *\fo{INDI-COMPOSITE})
\ +)

\ ---------------
\ +H Design notes
\ ---------------

\ +P Nothing remarkable.


\ ------------------
\ +H Class structure
\ ------------------

\ +[
property-vector class

end-class number-vector
\ +]


number-vector methods

\ --------------------
\ +H Protected methods
\ --------------------

protected
   
  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<newNumberVector ...>}.    
     ." <newNumberVector" space  this [parent] begin-xml
  ;m overrides begin-xml

  
  m:   ( this -- )			\ overrides end-xml
\ +G Send *\f{</newNumberVector>}.     
     ." </newNumberVector>" cr
  ;m overrides end-xml

public					\ THIS IS IMPORTANT OR WON'T COMPILE  
  
end-methods persistant


\ ======
\ *> ###
\ ======
