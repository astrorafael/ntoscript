((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/old/spiral2.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############

\ ===============================
\ *N *\fo{ra+ccw-spiral} class
\ ===============================  

\ --------------------------------------------------------------------------
\ *P (+RA, CCW) spiral generator
\ --------------------------------------------------------------------------

\ ------------
\ +H Structure
\ ------------

\ +[
spiral-generator class
   
end-class ra+ccw-spiral
\ +]

ra+ccw-spiral methods

protected

\ --------------------
\ +H Protected methods
\ --------------------

  m:   ( this -- )   		\ overrides (next-point)
     m-seq# @ 1- Seq0 []@  this (ra*+)
     m-seq# @ 1- Seq2 []@  this (dec*+)
  ;m overrides (next-point)
  
public
  
end-methods

\ ======
\ *> ###
\ ======
