((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/device.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==========================
\ *N *\fo{INDI-DEVICE} class
\ ==========================

\ *P An *\fo{INDI-DEVICE} object is the second level iin the hierarchy of
\ ** *\fo{INDI-ITEM}s. These objects are instantiated on demand by
\ ** the background *\fo{LISTENER} task when new properties are discovered.

\ *P Two interesting usages of a given *\fo{INDI-DEVICE} are:
\ *(
\ *B To enable/disable BLOBs from this device.
\ *B To discover properties for this device only.
\ *)


\ *P The following public selectors/methods may be invoked
\ ** by the *\fo{MAIN} task:
\ *(
\ *B *\fo{ENABLE}
\ *B *\fo{DISABLE}
\ *B *\fo{PRINT}
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{GET} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{SEND} (inherited fro *\fo{INDI-ITEM})
\ *B *\fo{COPY} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{SIZE} (inherited from *\fo{INDI-COMPOSITE})
\ *B *\fo{LOOKUP} (inherited from *\fo{INDI-COMPOSITE})
\ *)


\ +P The following public selectors/methods are invoked by the *\fo{LISTENER}
\ +* task:
\ +(
\ +B *\fo{CONSTRUCT}
\ +B *\fo{LABEL!} (inherited from *\fo{INDI-ITEM})
\ +B *\fo{SEARCH-ITEM} (inherited from *\fo{INDI-COMPOSITE})
\ +B *\fo{PUSH-BACK} (inherited from *\fo{INDI-COMPOSITE})
\ +)


\ ---------------
\ +H Design notes
\ ---------------

\ +P We use the *\fo{SEND} selector to enable/disable the sending
\ +* of BLOBs from this device to this client.
\ +* We use the *\fo{GET} selector to send an INDI <getPrioperties>
\ +* for this device only.  
\ +* Apart from these two methods,
\ +* an *\fo{INDI-DEVICE} object is no more than a container 
\ +* for propery vectors.

\ +P Word *\fo{(device)} is a factor for state-smart, parsing word
\ +* *\fo{device}, designed for scripts but can also be useful
\ +* by itself in GUIs. It takes arguments in the reverse order of parsing.


\ ------------------
\ +H Class structure
\ ------------------

\ +[
indi-composite class

   cell% inst-var m-blobs		\ flag: enable blobs for this device

end-class indi-device
\ +]


indi-device methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  :m (enableBLOBs)   ( this -- )
\ +G Send a full *\f{<enableBLOB device='<device>' ...></enableBLOB>} message.
     ." <enableBLOB device='"  m-name $@ type  ." '>"
     m-blobs @  if ." Also" else ." Never" then
     ." </enableBLOB>" cr
  ;m

  
  :m enableBLOBs   ( this -- )
 \ +G Redirect I/O to socket and inbvoke *\fo{(enableBLOBs)},
 \ +* catching its exceptions.
     this lock
     this sid [io SetIO this ['] (enableBLOBs) catch io]
     this unlock
     throw
  ;m
  

  m:   ( this -- )			\ overrides getProperties
\ +G Do the actual sending of *\f{<getProperties device='<device>' ... />}.   
     ." <getProperties version='" INDI-VERSION type
     ." ' device='" m-name $@ type  ." '/>" cr
  ;m overrides getProperties
  
\ -----------------
\ *H Public methods
\ -----------------
   
public

  m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{INDI-DEVICE} object with name given by *\i{ca1 u1}.    
     this [parent] construct  false m-blobs !
  ;m overrides construct


  :m enable   ( addr -- )		
\ *G  Enable BLOBs for this device. Use as follows:
\ *C   <a device object> BLOBs enable            
     true m-blobs !    this enableBLOBs
  ;m

  
  :m disable   ( addr -- )		
\ *G  Enable BLOBs for this device. Use as follows:
\ *C   <a device object> BLOBs disable            
     false m-blobs !   this enableBLOBs
  ;m

  
  m:   ( this --  )			\ overrides print
\ *G Print all device properties.      
     cr
     ." ===================================================================" cr
     ." Device "  m-name $@ type cr
     m-head @ begin dup while 2 spaces dup print next-item @ repeat drop
  ;m overrides print


end-methods persistant

\ ---------------------
\ *H Other helper words
\ ---------------------

ErrDef Dev-Excp         "INDI Device not found"
\ *G
    
: (device)				  \ ca1 u1 -- obj
\ *G Find an *\fo{INDI-DEVICE} whose name is given by *\i{ca1 u1}.
\ ** Throw a *\fo{Dev-Excp} exception if not found.
\ ** Uses the global context given by the *\fo{CurrentServer}.
\ +* For that reason it is not suitable for the *\fo{LISTENER} task
\ +* (not thread-safe).   
   CurrentServer Dev-Excp find-item
;


\ ======
\ *> ###
\ ======
