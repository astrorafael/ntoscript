((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dbase/blobitem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> properties
\ #############

\ ========================
\ *N *\fo{BLOB-ITEM} class
\ ========================

\ *P The *\fo{BLOB-ITEM} class is the contained class of *\fo{BLOB-VECTOR}
\ ** It can be directly instantiated.

\ *P The following public selectors may be invoked by the *\fo{MAIN} task:
\ *(
\ *B *\fo{NAME} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{PARENT} (inherited from *\fo{INDI-ITEM})
\ *B *\fo{LABEL} (inherited from *\fo{INDI-ITEM}
\ *B *\fo{SET}
\ *B *\fo{GET}
\ *B *\fo{PRINT}
\ *B *\fo{SEND}
\ *)

\ +P The following public selectors are invoked by the *\fo{LISTENER} task:
\ +(
\ +B *\fo{CONSTRUCT}
\ +B *\fo{UPDATE}
\ +B *\fo{FILE-FORMAT!}
\ +B *\fo{SIZE!}
\ +)


\ ---------------
\ +H Design notes
\ ---------------

\ +P As the name says, BLOBs are for arbitraty binary large objects. We
\ +* have chosen to make an economic use of RAM, so BLOBs really live
\ +* on the filesystem. The 'hold' and 'live' values are really file
\ +* paths where we can locate the BLOB.

\ +P The current implementation does not compress the 'hold' value to be sent.
\ +* Sending BLOBs to the remote device is intended for very rare ocassions
\ +* like firmware updates for instance. There is no need yet to solve the
\ +* problem of how to specify a good interface for the script, recognize
\ +* the .z extension and perform the compression.

\ ------------------
\ +H Class structure
\ ------------------
\ +[
indi-item class

   cell% inst-var m-fname		\ input file name without directory
   cell% inst-var m-value		\ output file name
   cell% inst-var m-valfmt		\ output file format as a file suffix
   cell% inst-var m-valsz		\ output file size in bytes
   cell% inst-var m-hold		\ input file name
   cell% inst-var m-holdfmt		\ input file format as a file suffix
   cell% inst-var m-holdsz		\ input file size in bytes

end-class blob-item
\ +]



blob-item methods

\ --------------------
\ +H protected methods
\ --------------------
   
protected

  m:   ( this -- )			\ overrides begin-xml
\ +G Send *\f{<oneBLOB ... >}.       
     2 spaces s" <oneBLOB" type space  this .name
     ."  size='" m-holdsz @ 1 .r ." ' format='" m-holdfmt $@ type ." '>" 
  ;m overrides begin-xml

  
  m:   ( this -- )			\ overrides body-xml
\ +G Read the input file, encode it using Base64 and send it to indiserver.
     cr m-hold $@ base64-encode		\ b64 encoding includes TYPE
  ;m overrides body-xml

  
  m:   ( this -- )			\ overrrides end-xml
\ +G Send *\f{</oneBLOB>}.        
      s" </oneBLOB>" type cr
  ;m overrides end-xml

   
  :m (file-path)   ( this -- )
\ +G Composes the output file name from the parent's property directory,
\ +* the property name, date and format suffix.
     this name m-fname $! s" _" m-fname $+!  \ name and underscore
     time&date $iso-8601-time m-fname $+!    \ append iso 8601 date
     m-valfmt $@ m-fname $+!		     \ append format suffix
     this parent directory@ dup >r m-value $!
     r> if
	s" /" m-value $+!		\ append / if dir non-empty
     then
     m-fname $@ m-value $+!
  ;m

  
  :m (.message)   ( ca1 u1 ca2 u2 this -- )
\ +G Print an informative, timestamped message concatenating *\i{ca1 u1} and
\ +* *\i{ca2 u2} strings.
\ +* *\i{ca1 u1} is printed first followed by semicolon and space.
     this parent parent name 
     gmtime&date $iso-8601-time
     indi log4
  ;m
  

  :m (holdsz!)   ( ca u this -- )
\ +G Set the holding file size for the file given by *\i{ca u} string.    
      r/o bin open-file OPEN-FILE-EXCP ?throw
      dup file-size FILE-SIZE-EXCP ?throw
      drop m-holdsz !			\ only gets the LSB cell
      close-file CLOSE-FILE-EXCP ?throw
   ;m

   

   :m (holdfmt!)    ( ca u this -- )
\ +G Set format for the file given by *\i{ca u} string for the "hold" value.   
      s" ." search 0= if 2drop s" " then m-holdfmt $!
   ;m


   :m (uncompress)   ( this -- )
\ +G Uncompress the received file if a suffix *\f{.z} was received.
         m-valfmt $@ s" .z" search if
	 m-value $@
	 2dup zInflate		\ may throw exceptions
	 2dup delete-file DELETE-FILE-EXCP ?throw
	 2 chars - s" uncompressed " this (.message)
      then
      2drop
   ;m
    
\ -----------------
\ *H Public methods
\ -----------------

 public
   
   m:   ( ca1 u1 this -- )		\ overrides construct
\ +G Create a new *\fo{BLOB-ITEM} object with name given by *\i{ca1 u1}.     
      this [parent] construct  0 dup m-value ! dup m-valfmt ! dup m-valsz !
      dup m-hold ! dup m-holdfmt ! dup m-holdsz ! m-fname !
   ;m overrides construct

   
   m:   ( ca u this -- )		\ overrides set
\ *G Set the string with the file name *\i{ca u} for a BLOB to send.
\ ** Also find out the input file prefix (either normal or .z compressed)
\ ** and size.      
      2dup m-hold $! 2dup this (holdsz!) this (holdfmt!)
   ;m overrides set

   
   m:  ( this -- ca u | x 0 )			\ overrides get
\ *G Get the file path *\i{ca u} where the received BLOB is stored.
      this lock
      m-value dup @ if $@ else 0 then
      this unlock
   ;m overrides get

   
   m:  ( ca u this -- )			\ overrides update
\ +G Decode data *\i{ca u} using base64 and save it into a file.
\ +* File name is generated from property name and a timestamp.
\ +* Format is:
\ +C    <Property_name>_YYYYMMDDHHMMSS<.suffix>     
      this (file-path)
      m-value $@ base64-decode
      m-fname $@ s" Received file" this (.message)
      this parent directory@ dup 0= if
	 2drop s" (current execution directory)"
      then
       s" Stored in" this (.message)
      this (uncompress)
   ;m overrides update

   
   m: ( this -- )			\ overrides send
\ *G Send BLOB to indiserver and log timestamps.       
      s" Sending file" m-hold $@ this (.message) 
      this [parent] send
      s" Sent file" m-hold $@ this (.message) 
   ;m overrides send

   
   m:  ( this -- )			\ overrides print
\ *G Print received *\fo{BLOB-ITEM} file name to standard output.
      this (.name)
      this get dup if type else 2drop ." <empty>" then space
      this .label cr
   ;m overrides print

   
   :m file-format!   ( ca u this -- )
\ +G Used by the *\fo{Listener} task to store the format attribute for
\ +* the input file.
      m-valfmt $!
   ;m


   :m size!   ( u this -- )
\ +G Used by the *\fo{Listener} task to store the size attribute for
\ +* the output file.
      m-valsz !
   ;m
   
   
end-methods persistant


\ ======
\ *> ###
\ ======
