((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/old/zigzag8.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ ================================
\ *N *\fo{RA+DEC-ZIGZAG-DEC} class
\ ================================

\ --------------------------------------------------------
\ *P (+RA, -DEC) increments, DEC bands, zig-zag sky mapper
\ --------------------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------
   
\ ------------------
\ +H Class structure
\ ------------------

\ +[
ra+dec+zigzag-dec class

end-class ra+dec-zigzag-dec
\ +]

ra+dec-zigzag-dec methods

\ -----------------
\ *H Public methods
\ -----------------
   
public

  m:   ( this -- )			\ overrides reset
\ *G Reset internal state to start a new iteration.
     this dup [parent] reset
     dup (/ra) dup (dec/) dup (dec+)
     false m-zigzag !      
  ;m overrides reset
  
end-methods
  
\ ======
\ *> ###
\ ======
 