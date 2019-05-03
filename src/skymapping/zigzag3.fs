((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/zigzag3.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ ================================
\ *N *\fo{RA-DEC+ZIGZAG-RA} class
\ ================================

\ --------------------------------------------------------
\ *P (-RA, +DEC) increments, RA bands, zig-zag sky mapper.
\ --------------------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------

\ +* P This class inherits from *\fo{RA+DEC+ZIGZAG-RA}.

\ ------------------
\ +H Class structure
\ ------------------

\ +[
ra+dec+zigzag-ra class

end-class ra-dec+zigzag-ra
\ +]

ra-dec+zigzag-ra methods

\ --------------------
\ +H Protected methods
\ --------------------

protected

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
     dup (ra/) dup (/dec) (ra+)
     false m-zigzag ! 
  ;m overrides reset

end-methods persistant
  
\ ======
\ *> ###
\ ======
