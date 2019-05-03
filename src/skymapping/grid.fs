((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/grid.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############
      
\ =============================
\ *N *\fo{GRID-GENERATOR} class
\ =============================

\ *P Abstract class to factor common code and additional interface for
\ ** all grid-style sky point generators
\ ** These  grid-style sky point generators sweep the sky in bands, either
\ ** in RA or DEC order, ascending or descending, uniformingly or
\ ** performing zig-zags.

\ *P All RA coordinates in this class are stored in degrees, for
\ ** convenience.

\ ------------------
\ +H Auxiliary words
\ ------------------


: %over>float				\ S: n1 -- ; F: -- r1
\ +G Convert overlap percentage to suitable fraction using formulae
\ +* *\f{1/(1-n1/100)}. The reciprocal is taken because corrections are divided
\ +* instead of multiplied in the forthcoming code.  
   s>f 100.0e0 f/ fnegate 1.0e0 f+ 1/f
;


\ ------------
\ +H Structure
\ ------------
\ +[

object class

   i-property  implementation		\ get/set selectors for objects.
   i-reset     implementation		\ reset selector

   float%           inst-var m-ra-overlap       \ user defined
   float%           inst-var m-dec-overlap      \ user defined
   cell%            inst-var m-seq#		\ current sequence number
   cell%            inst-var m-method	\ Flag T =initial/final F=center/frames
                    inst-value m-nra	\ number of frames for RA
                    inst-value m-ndec	\ number of frames for DEC
   char% /EqPoint * inst-var m-initial		\ Initial Sky Point 
   char% /EqPoint * inst-var m-final		\ Final Sky Point
   char% /EqPoint * inst-var m-centre		\ Centre Sky Point
   char% /EqPoint * inst-var m-frame		\ Sky Point Frame
   char% /EqPoint * inst-var m-current		\ Current sky point
   char% /EqPoint * inst-var m-buffered		\ Buffered Current sky point

   
   selector (next-band)   ( this -- False | p1 True )
   selector wraparound    ( this --  )
   
 public

   selector next-point	   ( this -- False | p1 True )
   
end-class grid-generator
\ +]



grid-generator methods

protected

\ --------------------
\ +H Protected methods
\ --------------------

  :m ra-correction   ( this -- ; F: r1 -- r2 )
\ +G Calculates the RA correction factor due to cos(dec) plus the desired
\ +* user overlapping factor. Declination to be taken into account comes from
\ +* the CCD frame edge, not the centre.     
     fabs				\ fabs for negative Dec !
     m-frame   sky.dec f@ f2/ f-
     deg>rad fcos			\ frame edge cos(dec)
     m-ra-overlap f@ f*			\ plus user RA correction
  ;m


  :m dec-correction   ( this -- ; F: -- r1 )
\ +G Calculates the DEC correction factor due the desired
\ +* user overlapping factor.
     m-dec-overlap f@
  ;m
  
  
  :m (/ra)   ( this -- )
\ +G Reset current RA to its initial value.      
     m-initial sky.ra f@   m-current sky.ra  f!
  ;m


  :m (ra/) ( this -- )
\ +G Reset current RA to its final value.           
     m-final sky.ra f@   m-current sky.ra f!
  ;m
  

  :m (ra+)   ( this -- )
\ +G Increment current RA by a frame/correction factor.     
     m-frame sky.ra f@
     m-current sky.dec f@ this ra-correction f/
     m-current sky.ra dup f@  f+   f!
  ;m
  
   
  :m (ra-)   ( this -- )
\ +G Decrements current RA by its *\f{frame/correction} value.     
     m-frame sky.ra f@
     m-current sky.dec f@ this ra-correction f/
     m-current sky.ra dup f@  fswap f-   f!
  ;m
  

  :m (ra>?)   ( this -- f )
\ +G Test for current RA passing past the final point.     
     m-current sky.ra f@   m-final sky.ra f@  f>
  ;m

  
   :m  (ra<?)  ( this -- f )
\ +G test for current RA passing past the initial point in back. iteration.
      m-current sky.ra f@   m-initial sky.ra f@ f<
   ;m


   :m  (/dec)   ( this -- )
\ +G  Resets current DEC to its initial value.     
      m-initial sky.dec f@ m-current sky.dec f!
  ;m
   

  :m (dec/)   ( this -- )
\ +G Resets current DEC to its final value., in back. iterations.     
     m-final sky.dec f@ m-current sky.dec f!
  ;m
  
   
  :m (dec+)   ( this -- )
\ +G Increments the current DEC by its *\i{frame/correction} value.
     m-frame   sky.dec f@ 
     this dec-correction f/
     m-current sky.dec dup f@  f+   f!
  ;m


  :m (dec-)   ( this -- )
\ +G Decrements the current DEC by its *\i{frame/correction} value.     
     m-frame   sky.dec f@ 
     this dec-correction f/
     m-current sky.dec dup f@  fswap f-  f!
  ;m

   
  :m (dec>?)   ( this -- f )
\ +G Test for current DEC passing past the final point.   
     m-current sky.dec f@  m-final sky.dec f@ f>
  ;m


  :m (dec<?)   ( this -- f )
\ +G Test for current DEC passing past the initial point in back. iterations.  
     m-current sky.dec f@  m-initial sky.dec f@ f<
  ;m

  
  :m (sort-ra)   ( this -- )
\ +G Sort RA component in initial & final points.
     m-initial sky.ra f@   m-final sky.ra f@
     f2dup fmin frot- fmax
     m-final sky.ra f! m-initial sky.ra f!
  ;m


  :m (sort-dec)   ( this -- )
\ +G Sort DEC component in initial & final points by ascending mode.      
     m-initial sky.dec f@ m-final sky.dec f@
     f2dup fmin frot- fmax
     m-final sky.dec f!  m-initial sky.dec f!
  ;m


  :m (sort)   ( this -- )
\ +G Sort Sky Region corners, so that initial point has the lowest RA and DEC
\ +* and the final point has the highest RA and DEC.      
     this dup (sort-ra) (sort-dec)
  ;m

  
  :m centre(dec)>initial(dec)   ( n2 this -- )
\ +G Calculate the initial DEC coordinate based on the centre DEC, the *\i{n2}
\ +* number of frames and the overlap correction.     
     2/ s>f m-frame sky.dec f@ f*	\ delta value
     this dec-correction f/		\ applies correction
     m-centre sky.dec f@ fswap f-	\ add centre
     m-initial sky.dec f!
  ;m

  
  :m centre(dec)>final(dec)   ( n2 this -- )
\ +G Calculate the final DEC coordinate based on the centre DEC, the *\i{n2}
\ +* number of frames and the overlap correction.          
     2/ s>f m-frame sky.dec f@ f*
     this dec-correction f/
     m-centre sky.dec f@ f+
     m-final sky.dec f!
  ;m

  
  :m centre(ra)>initial(ra)  ( n1 this -- )
\ +G Calculate the initial RA coordinate based on the centre RA, the *\i{n1}
\ +* number of frames and the declination+overlap correction.         
      2/ s>f m-frame sky.ra f@ f* 	\ delta value
      m-centre sky.dec f@ this ra-correction f/	\ apply correction
      m-centre sky.ra f@ fswap f- 	\ add centre
      m-initial sky.ra f!
  ;m

  
  :m centre(ra)>final(ra)  ( n1 this -- )
\ +G Calculate the final RA coordinate based on the centre RA, the *\i{n1}
\ +* number of frames and the declination+overlap correction.     
      2/ s>f m-frame sky.ra f@ f* 
      m-centre sky.dec f@ this ra-correction f/
      m-centre sky.ra f@ f+ 
      m-final sky.ra f!
  ;m

  :m centre>corners    ( this -- )
\ +G Calculate the initial and final sky points from the field center and
\ +* number of frames for each axis (RA,DEC).
     m-ndec dup this centre(dec)>initial(dec)   this centre(dec)>final(dec)
     m-nra  dup this centre(ra)>initial(ra)     this centre(ra)>final(ra)
  ;m

  :m reduce-ra				\ this --
\ +G Copies current sky point into the buffered sky point output, reducing the
\ +* RA range from negative to positive and from degrees to hours.
     m-current  dup sky.ra  f@ -freduce-sg >TIME sky.dec f@
     m-buffered dup sky.dec f! sky.ra f!
  ;m     

  :m -initial-ra
\ +G Makes initial RA negative if a wraparound situation is detected for +RA  
\ +* skymappers, that is: RA(initial) > 12 and RA(final) < 12
     m-initial sky.ra f@ 180.0e0 f>
     m-final   sky.ra f@ 180.0e0 f< and
     if m-initial sky.ra dup f@ 360.0e0 f- f! then
  ;m
  
  :m -final-ra
\ +G Makes final RA negative if a wraparound situation is detected for -RA  
\ +* skymappers, that is: RA(initial) < 12 and RA(final) > 12
     m-initial sky.ra f@ 180.0e0 f<
     m-final   sky.ra f@ 180.0e0 f> and
     if m-final sky.ra dup f@ 360.0e0 f- f! then
  ;m
  
\ -----------------
\ *H Public methods
\ -----------------
   
public
    
  m:   ( this -- )			\ overrides construct
\ *G Default constructor.    
     %0 %0 m-initial initEqPoint
     %0 %0 m-final   initEqPoint
     %0 %0 m-centre  initEqPoint
     %0 %0 m-frame   initEqPoint
     %0 %0 m-current initEqPoint
     m-seq# off
     %1 m-ra-overlap  f!
     %1 m-dec-overlap f!
     0 dup [to-inst] m-nra   [to-inst] m-ndec
     True m-method !
  ;m overrides construct


  :m set-initial   ( F: ra dec -- ; S:  this --  )
\ *G Set the initial Sky Point. Convert *\i{ra} to degrees.
     fswap >ANGLE m-initial EqPoint!
     True m-method !
  ;m

   
  :m set-final   ( F: ra dec -- ; S:  this --  )
\ *G Set the final Sky Point. Convert *\i{ra} to degrees.
     fswap >ANGLE  m-final  EqPoint!
     True m-method !
  ;m

  
  :m set-centre   ( F: ra dec -- ; S:  this --  )
\ *G Set the center Sky Point in the centre + frames approach.
\ ** Convert *\i{ra} to degrees.
     fswap >ANGLE  m-centre  EqPoint!
     False m-method !
  ;m

  
  :m set-frame   ( F: ra dec -- ; S:  this --  )
\ *G Set the increment both in *\i{ra} and *\i{dec}.
\ ** Both magnitudes must be previously in degrees.
     fswap m-frame  EqPoint!
  ;m

  
  :m set-frames   ( n1 n2 this --  )
\ *G Set the number of frames in RA (*\i{n1}) and DEC (*\i{n2}) to take.
\ ** *\i{n1} and *\i{n2} must be odd numbers. Used in the centre + frames
\ ** approach, must be called *\b{after} *\fo{set-frame} and *\fo{set-centre}.
     [to-inst] m-ndec
     [to-inst] m-nra
     False m-method !
  ;m

  
  m:   ( this -- )			\ overrides reset
\ +G Reset generator.
\ +* Reset the current sequence number to 1.     
\ +* Must be overriden and invoked by children by children.     
     m-seq# off
     m-method @ 0= if this centre>corners then
     this wraparound 
     this (sort)
  ;m overrides reset

  
  :m set-overlap   ( n1 n2 this -- )
\ *G Set the user defined overlap factors that will be applied to both frame
\ ** axis in RA and Dec respectively. *\i{n1} is the *\fo{%RA} overlap and
\ ** *\i{n2} is the *\fo{%DEC} overlap. Range for all: 0 <= |n| < 100 .
\ ** Negative values in fact leave holes.     
\ +* Use the formulae  *\f{1/(1-%/100)}
     %over>float m-dec-overlap f!
     %over>float m-ra-overlap  f!
  ;m

    
  m:   ( this -- )			\ overrides print
\ *G Iterates through the initial until final sky points, printing to stdout
\ ** the (RA, Dec) coordinates.
     this reset cr
     begin this next-point while .EqPoint repeat
  ;m overrides print
    
  
  :m seq#  ( this -- n )
\ *G Obtains the sequence number for the current point.
\ ** Must be called after*\fo{next-point}. To be used by auxiliar tools.
     m-seq# @
  ;m
  
  
  :m frame   ( this -- addr )
\ *G Get the frame size *fo{/SkyPoint} structure. To be used by auxiliar tools.
     m-frame 
  ;m

  
   :m initial   ( this -- addr )
\ *G Get the initial *fo{/SkyPoint} structure. To be used by auxiliar tools.
     m-initial
  ;m

  
  :m final   ( this -- addr )
\ *G Get the final *fo{/SkyPoint} structure. To be used by auxiliar tools.
     m-final
  ;m

end-methods persistant
   



