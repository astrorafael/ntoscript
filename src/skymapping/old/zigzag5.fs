((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/old/zigzag5.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ ================================
\ *N *\fo{RA+DEC+ZIGZAG-DEC} class
\ ================================

\ --------------------------------------------------------
\ *P (+RA, +DEC) increments, DEC bands, zig-zag sky mapper
\ --------------------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------
   
\ ------------------
\ +H Class structure
\ ------------------

\ +[
zigzag-generator class
   
end-class ra+dec+zigzag-dec
\ +]

ra+dec+zigzag-dec methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  m:   ( this -- p1 True | False )	\ overrides (next-band+)
\ +G Passes to next DEC band from the initial DEC, incrementing RA
\ +* and returns updated sky point *\i{p1}.
     true m-zigzag !
     this dup (/dec) dup (ra+)
     (ra>?) if false exitm then
     m-current true
  ;m overrides (next-band+)

   
  m:   ( this -- p1 True | False )	\ overrides (next-band-)
\ +G Passes to next DEC band from the final DEC, incrementing RA
\ +* and returns updated sky point *\i{p1}.
     false m-zigzag !
     this dup (dec/) dup (ra+)
     (ra>?) if false exitm then
     m-current true
  ;m overrides (next-band-)

  
  m:   ( this -- p1 True | False )	\ overrides (next-point+)
\ +G Passes to next point when zig and returns updated sky point *\i{p1}.
     this dup (dec+) 
     dup (ra>?)  if drop false   exitm then
     dup (dec>?) if (next-band-) exitm then
     drop
     m-current true
  ;m overrides (next-point+)

  
  m:   ( this -- p1 True | False )	\ overrides (next-point-)
\ +G Passes to next point when zag and returns updated sky point *\i{p1}.
     this dup (dec-) 
     dup (ra>?)  if drop false   exitm then
     dup (dec<?) if (next-band+) exitm then
     drop
     m-current true
  ;m overrides (next-point-)
  
\ -----------------
\ *H Public methods
\ -----------------
   
public

  m:   ( this -- )			\ overrides reset
\ *G Reset internal state to start a new iteration.
     this dup [parent] reset
     dup (/ra) dup (/dec) (dec-)
     true m-zigzag !
  ;m overrides reset
   
end-methods
  
\ ======
\ *> ###
\ ======
 