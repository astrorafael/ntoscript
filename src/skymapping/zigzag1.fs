((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/zigzag1.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ ================================
\ *N *\fo{RA+DEC+ZIGZAG-RA} class
\ ================================

\ --------------------------------------------------------
\ *P (+RA, +DEC) increments, RA bands, zig-zag sky mapper.
\ --------------------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------
   
\ ------------------
\ +H Class structure
\ ------------------

\ +[
zigzag-generator class

end-class ra+dec+zigzag-ra  
\ +]

ra+dec+zigzag-ra methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  m:   ( this -- p1 True | False )	\ overrides (next-band+)
\ +G Passes to next RA band from the initial RA, incrementing DEC
\ +* and returns updated sky point *\i{p1}.
     true m-zigzag !
     this dup (/ra) dup (dec+)
     (dec>?) if false exitm then
     this reduce-ra  m-buffered true
  ;m overrides (next-band+)

  
  m:   ( this -- p1 True | False )	\ overrides (next-band-)
\ +G Passes to next RA band from the final RA, incrementing DEC
\ +* and returns updated sky point *\i{p1}.
     false m-zigzag !
     this dup (ra/) dup (dec+)
     (dec>?) if false exitm then
     this reduce-ra  m-buffered true
  ;m overrides (next-band-)

  
  m:   ( this -- p1 True | False )	\ overrides (next-point+)
\ +G Passes to next point when zig and returns updated sky point *\i{p1}.
      this dup (ra+) 
      dup (dec>?) if drop false   exitm then
      dup (ra>?)  if (next-band-) exitm then
      drop
      this reduce-ra  m-buffered true
   ;m overrides (next-point+)

   
  m:   ( this -- p1 True | False )	\ overrides (next-point-)
\ +G Passes to next point when zag and returns updated sky point *\i{p1}.
      this dup (ra-) 
      dup (dec>?) if drop false   exitm then
      dup (ra<?)  if (next-band+) exitm then
      drop
      this reduce-ra  m-buffered true
   ;m overrides (next-point-)

   
   m:   ( this -- ) 			\ overrides wraparound
\ +G Handle wraparound situation by making initial RA negative.      
      this -initial-ra 
   ;m overrides wraparound

   
\ -----------------
\ *H Public methods
\ -----------------
   
public
   
  m:   ( this -- )			\ overrides reset
\ *G Reset internal state to start a new iteration.
     this dup [parent] reset
     dup (/ra) dup (/dec) (ra-)
     true m-zigzag !
  ;m overrides reset

end-methods persistant
  
\ ======
\ *> ###
\ ======
