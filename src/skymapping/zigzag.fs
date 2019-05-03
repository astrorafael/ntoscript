((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/zigzag.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
   
\ ================================
\ *N *\fo{ZIGZAG_GENERATOR} class
\ ================================

\ --------------------------------------------
\ *P Abstract class for all zigzag sky mappers
\ --------------------------------------------

\ ---------------
\ +H Design notes
\ ---------------
   
\ ------------------
\ +H Class structure
\ ------------------

\ +[
grid-generator class

   cell% inst-var m-zigzag		\ zig-zag flag. true is +, false -

 protected
   
   selector (next-band+)
   selector (next-band-)
   selector (next-point+)
   selector (next-point-)

  public
   
end-class zigzag-generator   
\ +]

zigzag-generator methods

\ -----------------
\ *H Public methods
\ -----------------

public

  m:   ( this -- p1 True | False)	\ overrides next-point
\ *G Get next sky point *\i{p1} or a false flag when finished.   
     1 m-seq# +!
     this m-zigzag @ if (next-point+) else (next-point-) then
  ;m overrides next-point
  
end-methods persistant

\ ======
\ *> ###
\ ======
     