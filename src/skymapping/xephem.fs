((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/xephem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############

\ ============================
\ *N *\fo{XEPHEM-TOOL} class
\ ============================

\ *P This class is reponsible to interface XEphem to interactively define
\ ** the initial and final sky mapping points and generate the
\ ** *\i{'mapping.epl'} and  *\i{'mapping.ano'} files for the user to
\ ** graphically display the mapped fields and sequence order.

\ ------------------
\ *H Auxiliary words
\ ------------------




\ ------------
\ *H Structure
\ ------------

\ +[
object class

 protected

   : ANNOTATIONS-FILE   S" /.xephem/mapping.ano" ;
   : EYEPIECES-FILE     S" /.xephem/mapping.epl";
   : IN-FIFO            S" /usr/local/xephem/fifos/xephem_in_fifo" ;
   : LOC-FIFO           S" /usr/local/xephem/fifos/xephem_loc_fifo" ;
      
      
   #256 Constant BufSz			        \ buffer size in bytes
   EDB-Table: micro-handler		      \ vector of EDB subfield handlers

   cell% inst-var m-mapper			\ pointer to bound sky mapper
   cell% inst-var m-point1		        \ flag: point1/point2
   char% /FileDev * inst-var m-file		\ File SID for XML output
   char% BufSz    * inst-var m-xebuf	        \ XEphem input buffer

 public
   
end-class xephem-tool
\ +]

xephem-tool methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

   :m mid-ra   ( this -- ; F: -- r1 )
      m-mapper @ dup initial sky.ra f@
                     final   sky.ra f@ f+ f2/
   ;m
   
   :m mid-dec   ( this -- ; F: -- r1 )
      m-mapper @ dup initial sky.dec f@
                     final   sky.dec f@ f+ f2/
   ;m

  :m <eyepieces>   ( this -- )
\ +G Print the eyepiece positions to standard output.     
      m-mapper @ reset
     ." <EyepiecePositions>" cr
     begin
	 m-mapper @ next-point
     while
	2 spaces
	." <position ra='" dup sky.ra  f@ >ANGLE  DEG>RAD f. [char] ' emit
	."  dec='"             sky.dec f@         DEG>RAD f. [char] ' emit
	."  width='"  m-mapper @ frame sky.ra  f@ DEG>RAD f. [char] ' emit 
	."  height='" m-mapper @ frame sky.dec f@ DEG>RAD f. [char] ' emit
	."  alt='0' az='0' pa='0' isRound='0' isSolid='0'"	   
	."  />" cr   
     repeat
     ." </EyepiecePositions>" cr
  ;m
  
  :m (eyepieces)   ( this -- )
\ +G Redirect standard output to *\fo{EYEPIECES-FILE}
\ +* and invoke *\fo{<eyepieces>}  .      
     [ also SUBSTITUTIONS ] $HOME [ previous ] PAD $move
     EYEPIECES-FILE PAD append
     PAD count
     2dup delete-file DELETE-FILE-EXCP ?throw
     w/o  open-gen OPEN-FILE-EXCP ?throw drop
     this <eyepieces>
     close-gen CLOSE-FILE-EXCP ?throw
  ;m

  
  :m eyepieces	 ( this -- )
\ +G Wrapper around *\fo{(eyepieces)} that catches exceptions.  
    [io m-file setIO this ['] (eyepieces) catch io] throw
  ;m

  
  :m <annotations>   ( this -- )
\ +G Print to standard output the sequence order as XEphem annotations.
    m-mapper @ reset
    ." <Annotations>" cr 
    begin
       m-mapper @ next-point
    while
	  2 spaces 
	  ." <annotation  hide='0' window='SkyMap' textdx='0' textdy='0' "
	  ." linedx='0' linedy='0' clientarg='0'"
	  ."  worldx='" dup   sky.dec f@        DEG>RAD f. [char] ' emit
	  ."  worldy='"       sky.ra  f@ >ANGLE DEG>RAD f. [char] ' emit
	  ." >" 
	  m-mapper @ seq#  (u.) type
	  ." </annotation>" cr 
    repeat
    ." </Annotations>" cr 
  ;m

  
  :m (annotations)   ( this -- )
\ +G Redirect standard output to *\fo{ANNOTATIONS-FILE} and invoke
\ +* *\fo{<annotations>} .
     [ also SUBSTITUTIONS ] $HOME [ previous ] PAD $move
     ANNOTATIONS-FILE PAD append  
     PAD count
     2dup delete-file DELETE-FILE-EXCP ?throw
     w/o OPEN-GEN OPEN-FILE-EXCP ?throw drop
     this <annotations>
     CLOSE-GEN CLOSE-FILE-EXCP ?throw
  ;m

   
  :m annotations    ( this -- )			
\ +G Wrapper around *\fo{(annotations)} that catches exceptions.    
    [io m-file setIO this ['] (annotations) catch io] throw
  ;m

  
  :m <xephem-recenter>   ( this -- )
\ +G Print to standard output the XEphem GoTo message to display
\ +* the mapped approximatd position in the SKyMap View.
    ." RA:"   this mid-ra  DEG>RAD f.
    ." Dec:"  this mid-dec DEG>RAD f.
  ;m

  
  :m (xephem-recenter)   ( this -- )
\ +G Redirect the XEphem  GoTo message to
\ +* *\fo{IN-FIFO} named pipe and invoke *\fo{<xephem-recenter>}.
     IN-FIFO w/o OPEN-GEN OPEN-FILE-EXCP ?throw drop
     this <xephem-recenter>
     CLOSE-GEN CLOSE-FILE-EXCP ?throw
  ;m

  
  :m xephem-recenter   ( this -- )
\ +G Wrapper around *\fo{(xephem-recenter)} that catches exceptions.  
    [io m-file setIO this ['] (xephem-recenter) catch io] throw
  ;m


  m: skip-field	 ( ca1 u1 this -- ca2 u2 )
\ *G Skip one field altogether *\i{ca1 u1} and get next field  *\i{ca1 u1}.
     edb-field 2drop 
  ;m

  
  m:   ( ca1 u1 this -- )			\ Subfield3 handler
\ +G Convert input string into fp Right Ascension and store it into the
\ +* initial or final point of the sky mapper object.
      -leading ($>sexag) drop R.A. >ANGLE 
      m-point1 @ if
        m-mapper @ initial sky.ra f!
      else
	 m-mapper @ final sky.ra f!
     then
  ;m Subfield3 edb-handler-for micro-handler

  
  m:   ( ca1 u1 this -- )	\ Subfield4 handler
\ +G Convert input string into fp Declination and store it into the
\ +* initial or final point of the sky mapper object.  
     -leading ($>sexag) drop DEC.
     m-point1 @ if
       m-mapper @ initial sky.dec f!
       m-point1 off
     else
	m-mapper @ final sky.dec f!
    then
  ;m Subfield4 edb-handler-for micro-handler


  :m micro-parser-edb  ( ca1 u1 this -- )
\ +G Micro EDB parser to parse Right Ascension and Declination only.
    edb-field 2drop   edb-field 2drop \ skip name & type
    edb-field edb-subfield this Subfield3 micro-handler 2drop \ parse RA
    edb-field edb-subfield this Subfield4 micro-handler 2drop \ parse DEC
    2drop
  ;m

  
  :m <xephem-receive>
\ +G Receive two Sky Point from XEphem in edb line format,parse them and
\ +* set them as the initial and final sky mappr points.  
    [io xconsole SetIO cr
      S" Waiting for initial sky point from XEphem" foreground log
    io]
    m-xebuf BufSz READEX-GEN READ-FILE-EXCP ?throw
    m-xebuf swap this micro-parser-edb
    [io xconsole SetIO
      S" Waiting for final sky point from XEphem" foreground log
    io]
    m-xebuf BufSz READEX-GEN READ-FILE-EXCP ?throw
    m-xebuf swap this micro-parser-edb
  ;m

  :m (xephem-receive)
\ +G Open the XEphem recepcion FIFO, receives 2 sky points from xephem
\ +* arocess the information (using *\fo{<xephem-receive>} ) and closes
\ +* the FIFO.  
     LOC-FIFO r/o OPEN-GEN OPEN-FILE-EXCP ?throw drop
     this <xephem-receive>
     CLOSE-GEN CLOSE-FILE-EXCP ?throw
  ;m
  
  :m xephem-receive
\ +G Wrapper around *\fo{(xephem-receive)} that catches exceptions.   
    [io m-file setIO this ['] (xephem-receive) catch io] throw
  ;m

\ -----------------
\ *H Public methods
\ -----------------
  
public
  
  m:   ( this -- )			\ overrides construct
\ *G Class constructor,
    m-file    initFileDev
    m-point1 on
  ;m overrides construct

  
  :m set-mapper    ( mapper this -- )
\ *G Set the proper *\fo{GRID-GENERATOR} sky mapper object instance.    
      m-mapper !
  ;m

  :m recenter				\ --
  \ *G Recenter mapped field on XEphem's Sky View.
     this xephem-recenter
  ;m

  
  :m generate				\ --
\ *G Generate *\i{'mapping.epl'}
\ ** and  *\i{'mapping.ano'} files for the user to display the mapped zones
\ ** and order.
\ Reuse m-xebuf for building up counted string .....  
    this eyepieces
    this annotations
    m-mapper @ seq# 1- (u.) m-xebuf place
    S"  samples for eyepieces & annotations." m-xebuf append
    m-xebuf count foreground log
  ;m
  

  :m capture				\ --
\ *G Recenter graphical field on XEphem's Sky View.
\ ** Receive the initial and final points by graphically clicking on XEphem's
\ ** Receive 2 points from XEphem SkyView and generate *\i{'mapping.epl'}
\ ** and  *\i{'mapping.ano'} files for the user to display the mapped zones
\ ** and order.  
    this xephem-recenter
    this xephem-receive
    this generate
  ;m
  
end-methods persistant


: XEphem-Tool:				  \ "name"
\ *G Create a named object of class *\fo{XEPHEM-TOOL}  
  xephem-tool heap-new constant
;
