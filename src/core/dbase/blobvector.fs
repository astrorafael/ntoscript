((
$Date: 2008-08-24 12:31:35 +0200 (dom 24 de ago de 2008) $
$Revision: 563 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/blobvector.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ==========================
\ *N *\fo{BLOB-VECTOR} class
\ ==========================

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
\ *B *\fo{SET} 
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
\ +B *\fo{DIRECTORY@}
\ +B *\fo{SEARCH-ITEM} (inherited from *\fo{INDI-COMPOSITE})
\ +B *\fo{PUSH-BACK} (inherited from *\fo{INDI-COMPOSITE})
\ +)

\ ---------------
\ +H Design notes
\ ---------------

\ +P The *\fo{BLOB-VECTOR} also holds a directory to save the incoming
\ +* BLOBs. Selector *\fo{SET} (unusued so far) is used to set this directory.
\ +* We lock the database to avoid concurrent write by the main task and read
\ +* by the listener task.
\ +* We have also attached to the *\fo{SET} method the responsibility for
\ +* directory creation. 

\ ------------------
\ +H Class structure
\ ------------------

\ *[
property-vector class

   cell% inst-var m-directory		\ directory to save incoming BLOBs
   
end-class blob-vector
\ *]


blob-vector methods

\ --------------------
\ +H Protected methods
\ --------------------

protected
   
  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<newBLOBVector ...>}.     
     ." <newBLOBVector" space   this [parent] begin-xml
  ;m overrides begin-xml

  
  m:   ( sid this -- )			\ overrides end-xml
  \ +G Send *\f{</newBLOBVector>}.     
     ." </newBLOBVector>" cr
  ;m overrides end-xml

public
  m:   ( ca1 u1 this -- )			\  overrides construct
\ *G Create a new *\fo{BLOB-VECTOR} object with name given by *\i{ca1 u1}.
     this [parent] construct
     0 m-directory !   s" " m-directory $! \ empty string
  ;m overrides construct   

  
  m:   ( ca u this -- )			\ overrides set
\ *G Set the directory path *\i{ca u} for incoming BLOBs. Create a new
\ ** directory if does not exists.
\ ** If no directory is ever set, use the user's current directory.
\ ** To be used by the *\fo{MAIN} task.     
     this lock
     m-directory $!
     s" mkdir -p " PAD place m-directory $@ PAD append
     PAD count ShellCmd
     this unlock
  ;m overrides set

  
  :m directory@   ( this -- ca u )
\ *G Get the directory path *\i{ca u} where incoming BLOBs are stored.
\ ** If *\i{u] is zero, use the current directory. To be used by the
\ ** *\fo{LISTENER} task.
     m-directory $@
  ;m
  
  
end-methods persistant

\ ---------------------
\ *H Other helper words
\ ---------------------

  : auto-blobs-dir			  \ -- ca u
\ *G Generate an automatic directory string in the *\fo{PAD} with format
\ ** *\f{$HOME/ccd/<julian day>} to store incoming BLOBs like CCD images.
\ ** *\f{$HOME} is the user's HOME environment variable.
\ ** *\b{Note:} This word does *\b{not} create the directory.
   [ also SUBSTITUTIONS ] $HOME [ previous ] PAD $move
   s" /ccd/" PAD append
   gmtime&date >r 2>r 3drop 2r> r>	\ only interested in dd mm yyy
   >JD12UT 0 <# #s #> PAD append	\ julian day as string
   PAD count
;

\ ======
\ *> ###
\ ======
