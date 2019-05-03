((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/lightvector.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==============================
\ *N *\fo{LIGHT-VECTOR} class
\ ==============================

\ *P The *\fo{LIGHT-VECTOR} class is the container for all the
\ ** *\fo{LIGHT-ITEM} objects contained within.
\ ** It can be directly instantiated.


\ *P The following public selectors/methods may be invoked
\ ** by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{GROUP}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{STATE}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{GET} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{PRINT} (inherited from *\fo{PROPERTY-VECTOR})
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

\ +P By definition, *\fo{LIGHT-VECTOR} are no modified by the client
\ +* software, thus they do no support output XML generation nor timeouts
\ +* nor writte permissions. To enforce this, we have redefined the proper
\ +* selectors to throw an exception.


\ ------------------
\ *H INDI Exceptions
\ ------------------

\ *[
ErrDef LightVector-Excp "Light vectors can only receive data"
\ *]

\ ------------------
\ +H Class structure
\ ------------------

\ +[
property-vector class

end-class light-vector
\ +]


light-vector methods

\ --------------------
\ *H Public methods
\ --------------------

public
   
  m:   ( this -- )			\ overrides send
\ +G Throw a *\fo{LightVector-Excp} exception.
     LightVector-Excp throw
  ;m overrides send

  
  m:   ( this -- )			\ overrides wait
\ +G Throw a *\fo{LightVector-Excp} exception.
     LightVector-Excp throw
  ;m overrides wait

  
  m:   ( this -- )			\ overrides copy
\ +G Throw a *\fo{LightVector-Excp} exception.
     LightVector-Excp throw
  ;m overrides copy

  
  m:   ( this -- )			\ overrides timeout
\ +G Throw a *\fo{Method-Excp} exception.
     Method-Excp throw
  ;m overrides timeout

  
  m:   ( this -- )			\ overrides permission
\ +G Throw a *\fo{Method-Excp} exception.
     Method-Excp throw
  ;m overrides permission


  m: ( this --  )			\ overrides print
\ *G Print to standard output the property name, state and individual
\ ** property item names and values.
     cr ." Property " m-name $@ type space
      [char] ( emit this state >$state type [char] ) emit space
     this .label&group cr
     m-head @ begin dup while 2 spaces dup print next-item @ repeat drop
  ;m overrides print
    
  
end-methods persistant
  

\ ======
\ *> ###
\ ======
