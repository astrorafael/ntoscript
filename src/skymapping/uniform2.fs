((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/uniform2.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ ================================
\ *N *\fo{RA-DEC-UNIFORM-RA} class
\ ================================

\ -------------------------------------------------------
\ *P (-RA, -DEC) increments, RA bands, uniform sky mapper
\ -------------------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------
   
\ ------------------
\ +H Class structure
\ ------------------

\ +[
grid-generator class
      
end-class ra-dec-uniform-ra   
\ +]

ra-dec-uniform-ra methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

  m: ( this -- p1 true | false )
\ +G Passes to next RA band and returns updated sky point *\i{p1}.       
     this dup (ra/) dup (dec-)
     (dec<?) if false exitm then
     this reduce-ra m-buffered true
  ;m overrides (next-band)

   m:   ( this -- )			\ overrides wraparound
\ +G Handle wraparound situation by making final RA negative.  
      this -final-ra
   ;m overrides wraparound
  
\ -----------------
\ *H Public methods
\ -----------------

public
   
  m:   ( this -- )			\ overrides reset
\ *G Reset internal state to start a new iteration.     
     this dup [parent] reset
     dup (ra/) dup (dec/) (ra+)
  ;m overrides reset

  
  m:   ( this -- p1 True | False)	\ overrides next-point
\ *G Get next sky point *\i{p1} or a false flag when finished.      
     1 m-seq# +!
     this dup (ra-) 
     dup (dec<?) if drop false  exitm then
     dup (ra<?)  if (next-band) exitm then
     drop
     this reduce-ra m-buffered true
  ;m overrides next-point
   
end-methods persistant

\ ======
\ *> ###
\ ======
