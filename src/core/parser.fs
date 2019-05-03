((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/parser.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############################
\ *! parser
\ *R @node{INDI message parser}
\ *T INDI message parser
\ #############################

\ ***************
\ *S Introduction
\ ***************

\ *P A custom XML parser for INDI messages through a TCP socket.
\ ** We should properly talk about a set of parsers, one for each root
\ ** element that we may find in the TCP input stream.

\ *P Currently, the following root elements are handled:
\ *(
\ *B *\f{<defTextVector>}
\ *B *\f{<defNumberVector>}
\ *B *\f{<defSwitchVector>}
\ *B *\f{<defLightVector>}
\ *B *\f{<defBLOBVector>}
\ *B *\f{<setSwitchVector>}
\ *B *\f{<setTextVector>}
\ *B *\f{<setNumberVector>}
\ *B *\f{<setLightVector>}
\ *B *\f{<setBLOBVector>}
\ *B *\f{<message>}
\ *)

\ *P The following root elements are silently ignored:
\ *(
\ *B *\f{<delProperty>}
\ *B *\f{<newTextVector>}
\ *B *\f{<newNumberVector>}
\ *B *\f{<newSwitchVector>}
\ *B *\f{<newBLOBVector>}
\ *)

\ ***************
\ +S Design notes
\ ***************

\ +P When designing how to parse the XML string sent by INDI servers
\ +* I had two options:
\ +(
\ +B Either get an external XML parser like *\i{Lib/XML.fth}.
\ +B Or write a generic parser myself.
\ +B Or study the problem and produce a custom solution.
\ +)

\ +P In the end, the third option was the way to go for several reasons:
\ +(
\ +B No *\f{<?xml ..?>} string is ever transmitted in the INDI protocol.
\ +* This discards *\i{Lib/XML.fth}.
\ +B INDI messages are very simple. Writting a generic parser seems overkill.
\ +B I wanted to avoid memory copying as much as possible. In-place decoding
\ +* in the receiving buffer was the most economic option in terms of memory
\ +* above all, thinking of receiving those large image BLOBs.
\ +)

\ +P This custom solution was already implemented in *\i{indistcript v1.0}
\ +* but I hope this version shows better performance. Indeed,
\ +* the former message reception was done character by character and this
\ +* caused a heavy CPU usage doing system calls.

\ +P The input buffer has been completely overhauled. It is no longer a
\ +* class (it doesn't need to be) and allows requests of several hundred
\ +* or thousand bytes in a single system call.

\ ***********
\ *S Glossary
\ ***********


\ =================
\ *N User Variables
\ =================

\ *P These user variables keep track of current objects per listener in the
\ ** muti-stage parsing process.

cell +User theDevice		
\ *G Current device object.

cell +User thePropertyVector		
\ *G Current property object.

cell +User thePropertyItem		
\ *G Current property item object.


\ ====================
\ *N Parser Exceptions
\ ====================

\ *[
ErrDef State-Excp  "Bad decoded State"
ErrDef Switch-Excp "Bad decoded Switch"
ErrDef Number-Excp "Bad Decoded Number"
ErrDef Light-Excp  "Bad decoded Light"
ErrDef Perm-Excp   "Bad decoded Permission"
ErrDef Rule-Excp   "Bad decoded Rule"
ErrDef Attr-Excp   "Mandatory attribute not found"
\ *]

\ ============
\ *N Utilities
\ ============

: $>s					  \ ca u -- n1
\ *G Converts a string to a single cell integer, rounding floats and double
\ ** cells to single cells integers. Throw *\fo{Number-Excp} on failure. 
  pad place pad number?
   case
      2 of d>s  endof			\ double->single
     -2 of fr>s endof 			\ float->single      
      0 of Number-Excp throw endof 	\ not a number
      				        \ single cell
   endcase      
;


: create-item				  \ ca1 u1 class parent -- obj
\ *G Create an *\fo{INDI-ITEM} object *\i{obj} with a given name *\i{ca1 u1},
\ ** *\i{class} and *\i{parent} object.  Append it to *\i{parent}. Example:
\ *C      s" TELESCOPE_POSITION" NUMBER-VECTOR theDevice @ CREATE-ITEM    
   >r heap-new dup r> push-back			\ -- obj
;

\ ======================
\ +N INDI Debugging aids
\ ======================

$00000001 Constant defTextVector
\ +G Mask for incoming INDI *\f{<defNumberVector>} messages.

$00000002 Constant defNumberVector 
\ +G Mask for incoming INDI *\f{<defNumberVector>} messages.

$00000004 Constant defSwitchVector 
\ +G Mask for incoming INDI *\f{<defSwitchVector>} messages.

$00000008 Constant defLightVector  
\ +G Mask for incoming INDI *\f{<defLightVector>} messages.

$00000010 Constant defBLOBVector   
\ +G Mask for incoming INDI *\f{<defBLOBVector>} messages.

$00000020 Constant setSwitchVector 
\ +G Mask for incoming INDI *\f{<setSwitchVector>} messages.

$00000040 Constant setNumberVector 
\ +G Mask for incoming INDI *\f{<setNumberVector>} messages.

$00000080 Constant setTextVector   
\ +G Mask for incoming INDI *\f{<setTextVector>} messages.

$00000100 Constant setLightVector  
\ +G Mask for incoming INDI *\f{<setBLOBVector>} messages.

$00000200 Constant setBLOBVector   
\ +G Mask for incoming INDI *\f{<setBLOBVector>} messages.

$00000400 Constant indi-message    
\ +G Mask for incoming INDI *\f{<message>} messages.

: ?indi-debug			  \ ca1 u1 flag -- ca1 u1
\ +G Conditionally prints the INDI message contained in ca1 u1 depending
\ +* on the mask set in *\fo{USER} variable *\fo{dbgTask}.
   dbgTask @ and if 2dup indi log1 then
;


\ ====================
\ +N Attribute parsing
\ ====================

\ +P XML parser words for common attributes in the different
\ +* <setXVector>. <defXVector> or <oneX> messages.

: parse-device   ( ca u -- obj True | ca2 u2 False )
\ +G Extract the device name in the *\i{ca u} INDI message, looks for the
\ +* proper object. Return either a True flag and the object or .
\ +* a False flag and the non-exitent device name *\i{ca2 u2}.
   s" device=" attr-value 0= if Attr-Excp throw then
   2dup theServer @ ( not-found!) search-item dup
   if 2swap 2drop then			\ drops ca2 u2 if found
;

   
: parse-child-name   ( ca1 u1 addr -- obj True | ca2 u2 False )
\ +G Extract the name attribute's value in the *\i{c1a u1} INDI message,
\ +* looks for the proper object.  Return either a True flag and the object or
\ +* a False flag and the non existing child  name *\i{ca2 u2}.
\ +* *\i{addr} is the address of a context variable to search for children:
\ +* either *\fo{theDevice} for property vectors or *\fo{thePropertyVector}
\ +* for property items.   
   -rot s" name=" attr-value		\ addr ca3 u3 flag
   0= if Attr-Excp throw then
   rot >r 2dup r> ( not-found!) search-item    
   dup if 2swap 2drop then 			\ drops ca2 u2 if found
;


: parse-label   ( ca u -- )
\ +G Extract and update *\fo{thePropertyVector} optional label with
\ +* the new value set in the *\i{ca u} INDI message.
   s" label=" attr-value 0= if 2drop exit then
   thePropertyVector @ label!   
;


: parse-item-label   ( ca u -- )
\ +G Extract and update *\fo{thePropertyItem} optional label with
\ +* the new value set in the *\i{ca u} INDI message.
   s" label=" attr-value 0= if 2drop exit then
   thePropertyItem @ label!   
;


: parse-group   ( ca u -- )
\ +G Extract and update *\fo{thePropertyVector} optional group with
\ +* the new value set in the *\i{ca u} INDI message.
   s" group=" attr-value 0= if 2drop exit then
   thePropertyVector @ group!   
;

   
: parse-state   ( ca u -- )
\ +G Extract and update *\fo{thePropertyVector} state with the new value set in
\ +* the *\i{ca u} INDI message,
   s" state=" attr-value 0= if Attr-Excp throw then
   >state 0= if State-Excp throw then
   thePropertyVector @ state!
;


: parse-timeout   ( ca u -- )
\ +G Extract and update *\fo{thePropertyVector} optional timeout  with
\ +* the new value set in the *\i{ca u} INDI message,
   s" timeout=" attr-value 0= if 2drop exit then
   $>s thePropertyVector @ timeout!
;


: parse-perm   ( ca u -- )
\ +G Extract and update *\fo{thePropertyVector} mandatory permission for
\ +* non light-vectors with
\ +* the new value set in the *\i{ca u} INDI message.
   s" perm=" attr-value 0= if Attr-Excp throw then
   >permission 0= if Perm-Excp throw then
   thePropertyVector @ permission!   
;


: parse-rule   ( ca u -- )
\ +G Extract and update *\fo{thePropertyVector} mandatory rule for
\ +* switch-vectors with
\ +* the new value set in the *\i{ca u} INDI message.
   s" rule=" attr-value 0= if Attr-Excp throw then
   >rule 0= if Rule-Excp throw then
   thePropertyVector @ rule!   
;


: parse-message ( ca u -- )
\ +G Extract optional timestamp and message and print it
\ +* to the *\fo{default-io}. No info is stored in the database.
\ +* If no timestamp is present, generates a local ISO 8601 timestamp.   
   2dup  s" message="   attr-value 0= if 4drop exit then
   2swap s" timestamp=" attr-value 0= if
      2drop gmtime&date $iso-8601-time
   then					\ cam um caT uT
   thePropertyVector @ name 2swap	\ cam um caP uP caT uT
   theDevice @ name 2swap		\ cam um caP uP caD uD caT uT
   indi log4
;


: parse-format				    \ ca u --
\ +G Parse the format='<format>' attribute of <defNumber> message *\i{ca u}
\ +* and updates the *\fo{NUMBER-ITEM} with this value.
   s" format=" attr-value 0= if Attr-Excp throw then
   thePropertyItem @ display-format!
;


: parse-min				    \ ca u --
\ +G Parse the min='<min>' attribute of <defNumber> message *\i{ca u}
\ +* and updates the *\fo{NUMBER-ITEM} with this value.
   s" min=" attr-value 0= if Attr-Excp throw then
   >float 0= if Number-Excp throw then
   thePropertyItem @ min-val!
;


: parse-max				    \ ca u --
\ +G Parse the min='<min>' attribute of <defNumber> message *\i{ca u}
\ +* and updates the *\fo{NUMBER-ITEM} with this value.
   s" max=" attr-value 0= if Attr-Excp throw then
   >float 0= if Number-Excp throw then
   thePropertyItem @ max-val!
;


: parse-step				    \ ca u --
\ +G Parse the min='<min>' attribute of <defNumber> message *\i{ca u}
\ +* and updates the *\fo{NUMBER-ITEM} with this value.
   s" step=" attr-value 0= if Attr-Excp throw then
   >float 0= if Number-Excp throw then
   thePropertyItem @ step!
;

: parse-size				    \ ca u --
\ +G Parse the size='<number>' attribute of <oneBLOB> message *\i{ca u}
\ +* and updates the *\fo{BLOB-ITEM} with this value.
   s" size=" attr-value 0= if Attr-Excp throw then
   pad place pad number? 1 <> if  Number-Excp throw then
   thePropertyItem @ size!
;


: parse-file-format				    \ ca u --
\ +G Parse the format='<format>' attribute of <oneBLOB> message *\i{ca u}
\ +* and updates the *\fo{BLOB-ITEM} with this value.
   s" format=" attr-value 0= if Attr-Excp throw then
   thePropertyItem @ file-format!
;


\ ==========================
\ +N Individual INDI parsers
\ ==========================

\ +P As said in the introduction section, there is one parser per root XML
\ +* element to be found.

\ -----------------
\ +H <defX> helpers
\ -----------------

: new-item				  \ ca u class --
\ +G Create a new property item of a given *\i{class}.
\ +* Extract the object name from the INDI message  *\i{ca u}.
\ +* Store the created property item in *\fo{thePropertyItem}.   
   -rot thePropertyVector @ parse-child-name
   0= if
      rot thePropertyVector @ create-item
   else
      nip				\ get rid of class
   then
   thePropertyItem !
;


: switch-update				 \ ca u --
\ +G Extract the PCDATA from <defSwitch> or <oneSwitch> and update the
\ +* properties database.   
   #pcdata >switch
   0= if Switch-Excp throw then
   thePropertyItem @ update
;


: light-update				 \ ca u --
\ +G Extract the PCDATA from <defLight> or <oneLight> and update the
\ +* properties database.   
   #pcdata >light
   0= if Light-Excp throw then
   thePropertyItem @ update
;


: number-update				 \ ca u --
\ +G Extract the PCDATA from <defNumber> or <oneNumber> and update the
\ +* properties database.
   #pcdata
   >float 0= if Number-Excp throw then
   ThePropertyItem @ update
;


: text-update				 \ ca u --
\ +G Extract the PCDATA from <defSwitch> or <oneSwitch> and update the
\ +* properties database.   
   #pcdata 
   thePropertyItem @ update
;

\ ------------------
\ +H <defX> handlers
\ ------------------

: <defSwitch>   ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<defSwitch>...</defSwitch>} subelement in the input
\ +* *\i{ca1 u1} string. Update database if found.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.
   s" </defSwitch>"  parse-element
   dup if
      2dup switch-item new-item
      2dup parse-item-label
      switch-update
      true exit then
   2drop false
;


: <defLight> ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<defLight>...</defLight>} subelement in the input
\ +* *\i{ca1 u1} string. Update database if found.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </defLight>"  parse-element
   dup if
      2dup light-item new-item
      2dup parse-item-label
      light-update
      true exit
   then
   2drop false
;


: <defNumber> ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<defNumber>...</defNumber>} subelement in the input
\ +* *\i{ca1 u1} string. Update database if found.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </defNumber>"  parse-element
   dup if
      2dup number-item new-item
      2dup parse-item-label
      2dup parse-format
      2dup parse-min
      2dup parse-max
      2dup parse-step
      number-update
      true exit
   then
   2drop false
;


: <defBLOB> ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<defBLOB .../>} subelement in the input
\ +* *\i{ca1 u1} string. Update database if found.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" />"  parse-element
   dup if
      2dup blob-item new-item
      parse-item-label
      true  exit
   then
   2drop false
;


: <defText1> ( ca u -- ca1 u1 flag )
\ +G Find a *\f{<defText>...</defText>} subelement in the input
\ +* *\i{ca1 u1} string. Update database if found.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </defText>"  parse-element
   dup if
      2dup text-item new-item
      2dup parse-item-label
      text-update
      true exit
   then
   2drop false
;


: <defText/> ( ca u -- ca1 u1 flag )
\ +G Find a *\f{<defText ../>} subelement in the input
\ +* *\i{ca1 u1} string. Update database if found.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" />"  parse-element
   dup if
      2dup text-item new-item
      parse-item-label
      true exit
   then
   2drop false
;


:  <defText> ( ca u -- ca1 u1 flag )
\ +G Find a *\f{<defText>...</defText>} or *\f{<defText ../>}
\ +* subelement in the input *\i{ca1 u1} string.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   <defText1> if true exit then <defText/>
;


\ --------------------------
\ +H <defXVector> helpers
\ -------------------------

: new-device				  \ ca u --
\ +G Extract the device name in the *\i{ca u} INDI message, looks for the
\ +* proper object (using *\fo{parse-device}) and set *\fo{TheDevice}
\ +* context variable for other words. Create a new device if not found.   
   parse-device
   0= if indi-device theServer @ create-item then
   theDevice !
;


: new-vector				  \ ca u class --
\ +G Create a new property vector of a given *\i{class}.
\ +* Extract the object name from the INDI message  *\i{ca u}.
\ +* Store the created property in *\fo{thePropertyVector}.   
   -rot theDevice @ parse-child-name
   0= if
      rot theDevice @ create-item
   else
      nip				\ get rid of class
   then
   thePropertyVector !
;


\ --------------------------
\ +H <defXVector> handlers
\ --------------------------

:  (defLightVector)	                 \ ca1 u1 --
\ +G Parser action for *\fo{<defLightVector>}
\ +* on full element string given by *\i{ca1 u1}.      
   attr-list				\ -- ca2 u2 ca3 u3
   2dup new-device			\ -- ca2 u2 ca3 u3
   2dup light-vector new-vector		\ -- ca2 u2 ca3 u3
   2dup parse-label
   2dup parse-group
   2dup parse-state
   parse-message
   begin <defLight> 0= until
   2drop 
;


:  (defTextVector)			  \ ca1 u1 --
\ +G Parser action for *\fo{<defTextVector>}
\ +* on full element string given by *\i{ca1 u1}.      
   attr-list				\ -- ca2 u2 ca3 u3
   2dup new-device			\ -- ca2 u2 ca3 u3
   2dup text-vector new-vector		\ -- ca2 u2 ca3 u3
   2dup parse-label
   2dup parse-group
   2dup parse-state
   2dup parse-perm
   2dup parse-timeout
   parse-message
   begin <defText> 0= until
   2drop 
;


:  (defNumberVector)			  \ ca1 u1 --
\ +G Parser action for *\fo{<defNumberVector>}
\ +* on full element string given by *\i{ca1 u1}.
   attr-list				\ -- ca2 u2 ca3 u3
   2dup new-device			\ -- ca2 u2 ca3 u3
   2dup number-vector new-vector	\ -- ca2 u2 ca3 u3
   2dup parse-label
   2dup parse-group
   2dup parse-state
   2dup parse-perm
   2dup parse-timeout
   parse-message
   begin <defNumber> 0= until
   2drop 
;


:  (defBLOBVector)			  \ ca1 u1 --
\ +G Parser action for *\fo{<defBLOBVector>}
\ +* on full element string given by *\i{ca1 u1}.   
   attr-list				\ -- ca2 u2 ca3 u3
   2dup new-device			\ -- ca2 u2 ca3 u3
   2dup blob-vector new-vector		\ -- ca2 u2 ca3 u3
   2dup parse-label
   2dup parse-group
   2dup parse-state
   2dup parse-perm
   2dup parse-timeout
   parse-message
   begin <defBLOB> 0= until
   2drop 
;


:  (defSwitchVector)			  \ ca1 u1 --
\ +G Parser action for *\fo{<defSwitchVector>}
\ +* on full element string given by *\i{ca1 u1}.   
   attr-list				\ -- ca2 u2 ca3 u3
   2dup new-device			\ -- ca2 u2 ca3 u3
   2dup switch-vector new-vector 	\ -- ca2 u2 ca3 u3
   2dup parse-label
   2dup parse-group
   2dup parse-state
   2dup parse-perm
   2dup parse-rule
   2dup parse-timeout
   parse-message
   begin <defSwitch> 0= until
   2drop 
;



\ -------------------
\ +H <oneX> helpers
\ -------------------

: get-item				    \ ca u --
\ +G Searches the properties database starting at *\fo{thePropertyVector} level
\ +* for a property item whose name is contained in the attributes list
\ +* string *\i{ca u}.
\ +* Set *\fo{ThePropertyItem} context variable for other words to use it.
\ +* Throws a *\fo{Prop-Item-Excp} if object not found.   
   thePropertyVector @ parse-child-name
   0= if Prop-Item-Excp throw then
   thePropertyItem !
;



: blob-update				   \ ca u --
\ +G Extract the size, format and PCDATA from <oneBlob> and update the
\ +* properties database.   
   2dup parse-size 2dup parse-file-format #pcdata 
   thePropertyItem @ update
;


\ -----------------
\ +H <oneX> parsers
\ -----------------

: <oneNumber> ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<oneNumber>...</oneNumber>} subelement in the input
\ +* *\i{ca1 u1} string.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </oneNumber>"  parse-element
   dup if 2dup get-item number-update true exit then
   2drop false
;

: <oneLight> ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<oneLight>...</oneLight>} subelement in the input
\ +* *\i{ca1 u1} string.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </oneLight>"  parse-element
   dup if 2dup get-item light-update true exit then
   2drop false
;


: <oneSwitch>   ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<oneSwitch>...</oneSwitch>} subelement in the input
\ +* *\i{ca1 u1} string.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </oneSwitch>"  parse-element
   dup if 2dup get-item switch-update true exit then
   2drop false
;


: <oneText> ( ca u -- ca1 u1 flag )
\ +G Find a *\f{<oneText>...</oneText>} subelement in the input
\ +* *\i{ca1 u1} string.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </oneText>"  parse-element
   dup if 2dup get-item text-update true exit then
   2drop false
;

: <oneBLOB> ( ca1 u1 -- ca2 u2 True | False )
\ +G Find a *\f{<oneBLOB>...</oneBLOB>} subelement in the input
\ +* *\i{ca1 u1} string.
\ +* Leave remaining string (of parent element) at *\i{ca2 u2}.   
   s" </oneBLOB>"  parse-element
   dup if 2dup get-item blob-update true  exit then
   2drop false 
;


\ -----------------------
\ +H <setXVector> helpers
\ -----------------------

: get-device			         \ ca u --
\ +G Very similar to *\fo{new-device} but this one throws a
\ +* *\fo{Dev-Excp} if device not found. See *\fo{new-device}.
   parse-device
   0= if Dev-Excp throw then theDevice !
;

: get-vector				  \ ca u --
\ +G Searches the properties database starting at *\fo{theDevice} level
\ +* for a property vector whose name is contained in the attributes list
\ +* string *\i{ca u}.
\ +* Set *\fo{ThePropertyVector} context variable for other words to use it.
\ +* Throws a *\fo{Prop-Vector-Excp} if object not found.
   theDevice @ parse-child-name 0= if Prop-Vector-Excp throw then
   thePropertyVector !
;


\ -----------------------
\ +H <setXVector> parsers
\ -----------------------

:  (setLightVector)			  \ ca1 u1
\ +G Parser action for *\fo{<setLightVector>}
\ +* on full element string given by *\i{ca1 u1}.   
   attr-list				\ -- ca2 u2 ca3 u3
   2dup get-device			\ -- ca2 u2 ca3 u3
   2dup get-vector			\ -- ca2 u2 ca3 u3
   2dup parse-state
   parse-message
   begin <oneLight> 0= until
   2drop
;


:  (setTextVector)			  \ ca1 u1
\ +G Parser action for *\fo{<setTextVector>}
\ +* on full element string given by *\i{ca1 u1}.     
   attr-list				\ -- ca2 u2 ca3 u3
   2dup get-device			\ -- ca2 u2 ca3 u3
   2dup get-vector			\ -- ca2 u2 ca3 u3
   2dup parse-state
   2dup parse-timeout
   parse-message
   begin <oneLight> 0= until
   2drop
;


:  (setSwitchVector)			  \ ca1 u1
\ +G Parser action for *\fo{<setSwitchVector>}
\ +* on full element string given by *\i{ca1 u1}.   
   attr-list				\ -- ca2 u2 ca3 u3
   2dup get-device			\ -- ca2 u2 ca3 u3
   2dup get-vector			\ -- ca2 u2 ca3 u3
   2dup parse-state
   2dup parse-timeout
   parse-message
   begin <oneSwitch> 0= until
   2drop
;


:  (setNumberVector)			  \ ca1 u1
\ +G Parser action for *\fo{<setNumberVector>}
\ +* on full element string given by *\i{ca1 u1}.     
   attr-list				\ -- ca2 u2 ca3 u3
   2dup get-device			\ -- ca2 u2 ca3 u3
   2dup get-vector			\ -- ca2 u2 ca3 u3
   2dup parse-state
   2dup parse-timeout
   parse-message
   begin <oneNumber> 0= until
   2drop
;


:  (setBLOBVector)			  \ ca1 u1
\ +G Parser action for *\fo{<setBLOBVector>}
\ +* on full element string given by *\i{ca1 u1}.  
   attr-list				\ -- ca2 u2 ca3 u3
   2dup get-device			\ -- ca2 u2 ca3 u3
   2dup get-vector			\ -- ca2 u2 ca3 u3
   2dup parse-state
   2dup parse-timeout
   parse-message
   begin <oneBLOB> 0= until
   2drop
;


\ ------------------------
\ +H Miscelaneous handlers
\ ------------------------

: (indi-message)			  \ ca1 u1 --
\ +G Parser action for *\fo{<message>}
\ +* on full element string given by *\i{ca1 u1}.     
   attr-list
   2dup  s" message="   attr-value 0= if 4drop 2drop exit then
   2swap
   2dup  s" device="    attr-value 0= if 2drop s" " then
   2swap s" timestamp=" attr-value 0= if 2drop gmtime&date $iso-8601-time then
   indi log3
   2drop
;

\ ==================
\ *N INDI XML Parser
\ ==================

\ *P The INDI XML parser is really a collection of individual root element
\ +* parsers combined into one package.

: /tag	      \ ca1 u1 ca2 u2 -- ca3 u3  ;  skip tag
\ +G Adjust INDI XML message *\i{ca1 u1} by skipping the "<tag" substring
\ +* given by *\i{ca2 u2}, yielding the trimmed string *\i{ca3 u3}.
   nip 1+ /string
;

: indi-parser:		   \ xt mask "name" -- ; [child] ca1 u1 ca2 u2 addr -- 
\ *G Definer word to create a parser per incoming XML message type.
\ ** Also wraps each parser with exception handler code.
\ ** User exceptions are trapped, system exeptions are re-thrown.   
\ ** The actual parser action *\i{xt} is provided and then executed
\ ** at runtime. *\i{ca1 u1} denotes the XML INDI message to parse and
\ ** *\i{ca2 u2} is the tag string.
\ ** If the proper *\i{mask} is set, message will be dumped to standard
\ ** output.   
   create , ,		
 does>
   dup @ swap cell+ @ >r >r		\  -- ca1 u1 ca2 u2 ; R: -- xt mask
   2swap r> ?indi-debug 2swap		\  -- ca1 u1 ca2 u2 ; R: -- xt
   /tag r> catch
   dup 500 >= if indi .#excp exit then  \ handle user defined exceptions
   throw				\ re-throws the rest
;

' (defTextVector)   defTextVector   indi-parser: <defTextVector>
' (defNumberVector) defNumberVector indi-parser: <defNumberVector>
' (defSwitchVector) defSwitchVector indi-parser: <defSwitchVector>
' (defLightVector)  defLightVector  indi-parser: <defLightVector>
' (defBLOBVector)   defBLOBVector   indi-parser: <defBLOBVector>
' (setSwitchVector) setSwitchVector indi-parser: <setSwitchVector>
' (setNumberVector) setNumberVector indi-parser: <setNumberVector>
' (setTextVector)   setTextVector   indi-parser: <setTextVector>
' (setLightVector)  setLightVector  indi-parser: <setLightVector>
' (setBLOBVector)   setBLOBVector   indi-parser: <setBLOBVector>
' (indi-message)    indi-message    indi-parser: <message>


: parse-indi				  \ ca1 u1 ca2 u2  --
\ *G The INDI Custom XML parser that recogizes all needed root element
\ ** tags and perform actions on their contents. It is the action for
\ *\fo{PARSE-XML} when this file is loaded.
   2dup s" message"         str= if <message>         exit then
   2dup s" setTextVector"   str= if <setTextVector>   exit then
   2dup s" setSwitchVector" str= if <setSwitchVector> exit then
   2dup s" setNumberVector" str= if <setNumberVector> exit then
   2dup s" setLightVector"  str= if <setLightVector>  exit then
   2dup s" setBLOBVector"   str= if <setBLOBVector>   exit then
   2dup s" defTextVector"   str= if <defTextVector>   exit then
   2dup s" defSwitchVector" str= if <defSwitchVector> exit then
   2dup s" defNumberVector" str= if <defNumberVector> exit then
   2dup s" defLightVector"  str= if <defLightVector>  exit then
   2dup s" defBLOBVector"   str= if <defBLOBVector>   exit then
   2dup s" delProperty"     str= if 4drop           exit then
   2dup s" newTextVector"   str= if 4drop           exit then
   2dup s" newSwitchVector" str= if 4drop           exit then
   2dup s" newNumberVector" str= if 4drop           exit then
   2dup s" newBLOBVector"   str= if 4drop           exit then
   2drop
;


\ ----------------------------------------------------------------------

' parse-indi is parse-xml




\ ======
\ *> ###
\ ======
