((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/switchvector.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ============================
\ *N *\fo{SWITCH-VECTOR} class
\ ============================

\ *P The *\fo{SWITCH-VECTOR} class is the container for all the
\ ** *\fo{SWITCH-ITEM} objects contained within.
\ ** It can be directly instantiated.

\ *P The following public selectors/methods may be invoked
\ ** by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{GROUP}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{RULE}
\ *B *\fo{STATE}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{TIMEOUT}(inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{GET} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{WAIT} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND&WAIT} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND&OK} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{SEND&IDLE} (inherited from *\fo{PROPERTY-VECTOR})
\ *B *\fo{PRINT} 
\ *B *\fo{COPY} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{SIZE} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{LOOKUP} (inherited from *\fo{INDI-COMPOSITE})
\ *)

\ +P The following public selectors/methods are invoked by the *\fo{LISTENER}
\ +* task:
\ +(
\ +B *\fo{CONSTRUCT}
\ +B *\fo{LABEL!} (inherited from *\fo{INDI-ITEM})
\ +B *\fo{GROUP!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{STATE!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{TIMEOUT!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{PERMISSION!} (inherited from *\fo{PROPERTY-VECTOR})
\ +B *\fo{RULE!}
\ +B *\fo{SEARCH-ITEM} (inherited from *\fo{INDI-COMPOSITE})
\ +B *\fo{PUSH-BACK} (inherited from *\fo{INDI-COMPOSITE})
\ +)


\ ---------------
\ +H Design notes
\ ---------------

\ +P When doing scripting, we *\fo{SET} and *\fo{SEND} one *\fo{SWITCH-ITEM} at
\ +* a time, since we do not enforce any rule policy at the client side.
\ 
\ +P When sending new values of switches, the property vector must know
\ +* who of its children *\fo{SWITCH-ITEM} has changed last. This has been
\ +* implemented as a pointer. When any of its children change, they update
\ +* this pointer.

\ +P Switch rules policies are enforced by the driver, not client.
\ +* They are offered as a hint for GUIs.

\ +P *\fo{LAST-SW} is implemented as a public field.
\ +* It is a reminder on other strategies apart from getter/setter methods and
\ +* also how to do it.


\ ---------------------------
\ *H Switch Vector attributes
\ ---------------------------

0 Constant OneOfMany
\ *G Switch rule value.

1 Constant AtMostOne
\ *G Switch rule value.

2 Constant AnyOfMany
\ *G Switch rule value.


: >$rule   ( n -- ca1 u1)
\ *G Given the permission number *\i{n}, gets its string literal.
   case
      OneOfMany  of s" OneOfMany" endof
      AtMostOne  of s" AtMostOne" endof
      AnyOfMany  of s" AnyOfMany" endof
   endcase
;


: >rule  ( ca u -- n True | False )
\ *G Translates *\f{"OneOfMany""}, *\f{"AtMostOne"} and *\f{"AnyOfMany""} str.
\ ** to integer constants *\fo{OneOfMany}, *\fo{AtMostOne} and *\fo{AnyOfMany}.
\ ** Return true upon succesful conversion.      
   2dup s" OneOfMany" str= if 2drop OneOfMany True exit then
   2dup s" AtMostOne" str= if 2drop AtMostOne True exit then
        s" AnyOfMany" str= if       AnyOfMany True exit then
   False
;

\ ------------------
\ +H Class structure
\ ------------------

\ +[
property-vector class
   
   cell% field% last-sw			\ last used switch object
   cell% inst-var m-rule		\ switch-vector update rule

end-class switch-vector
\ +]

\ -----------------
\ *H Public Fields
\ -----------------

(( For DocGen use.
cell% field% last-sw			\ this -- addr
\ *G This field holds the reference to the last switch object changed.
\ ** This is used by its children *\fo{SWITCH-ITEM}s.
\ ** Usage:
\ *C  <obj> last-sw @     or   <value> <obj> last-sw !
))

switch-vector methods

\ --------------------
\ +H Protected methods
\ --------------------

protected
   
  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<newSwitchVector ...>}.     
     ." <newSwitchVector" space  this [parent] begin-xml
  ;m overrides begin-xml

  
  m:   ( this -- )			\ overrides body-xml
\ +G Send only the last changed switch.
     this last-sw @ ?dup if send then
  ;m overrides body-xml

  
  m:   ( this -- )			\ overrides end-xml
 \ +G Send *\f{</newSwitchVector>}.    
     ." </newSwitchVector>"  cr
  ;m overrides end-xml

  :m .perm   ( this -- )
\ +G Print the property label and group to standard output.
     [char] { emit m-rule @ >$rule type [char] } emit
  ;m
  
\ -----------------
\ *H Public methods
\ ----------------
   
public
  
  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{SWITCH-VECTOR} object with name given by *\i{ca1 u1}.  
     this [parent] construct
     NULL this last-sw !
     AnyOfMany m-rule !
  ;m overrides construct

  
  :m rule   ( this -- n1 )
\ *G Get the *\fo{switch-vector}'s rule.
\ ** To be used by the *\fo{MAIN} task.
     m-rule @ 
  ;m

  
   :m rule!   ( ca u this -- )
\ +G Set the the *\fo{switch-vector}'s rule.
\ +* To be used by the *\fo{Listener} task.
     m-rule !
  ;m   
  
  m: ( this --  )			\ overrides print
\ *G Print to standard output the property name, state and individual
\ ** property item names and values.
     cr ." Property " m-name $@ type space
     this .writables space
     this .perm space
     this .label&group cr
     m-head @ begin dup while 2 spaces dup print next-item @ repeat drop
  ;m overrides print
  
end-methods persistant


\ ======
\ *> ###
\ ======
