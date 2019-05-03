((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/old/uniform8.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ =================================
\ *N *\fo{RA+DEC-UNIFORM-DEC} class
\ =================================

\ --------------------------------------------------------
\ *P (+RA, -DEC) increments, DEC bands, uniform sky mapper
\ --------------------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------
   
\ ------------------
\ +H Class structure
\ ------------------

\ +[
grid-generator class

end-class ra+dec-uniform-dec
\ +]

 ra+dec-uniform-dec methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  m:   ( this -- p1 True | False )	\ overrides (next-band)
\ +G Passes to next DEC band and returns updated sky point *\i{p1}.
     this dup (dec/) dup (ra+)
     (ra>?) if false exitm then
     m-current true
  ;m overrides (next-band)
   
\ -----------------
\ *H Public methods
\ -----------------

public  
  
  m:   ( this -- )			\ overrides reset
\ *G Reset internal state to start a new iteration.
     this dup [parent] reset 
     dup (/ra) dup (dec/) (dec+)
  ;m overrides reset

  
  m:   ( this -- p1 True | False)	\ overrides next-point
\ *G Get next sky point *\i{p1} or a false flag when finished.  
     1 m-seq# +!
     this dup (dec-) 
     dup (ra>?)  if drop false  exitm then
     dup (dec<?) if (next-band) exitm then
     drop
     m-current true
  ;m overrides next-point
   
end-methods

\ ======
\ *> ###
\ ======
