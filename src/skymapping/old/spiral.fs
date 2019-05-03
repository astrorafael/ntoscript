((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/old/spiral.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############

\ ===============================
\ *N *\fo{SPIRAL-GENERATOR} class
\ ===============================

\ ------------
\ +H Structure
\ ------------
\ +[
grid-generator class

   cell% inst-var m-points		\ number of points to produce

 protected

   selector (next-point)		\ select next point in spiral

 public
   
end-class spiral-generator
\ +]

spiral-generator methods

protected

\ --------------------
\ +H Protected methods
\ --------------------
  
  :m (ra*+)   ( n this -- )
\ +G Increments the current RA by a *\b{signed} frame value.
\ ** *\i{n} is the spiral sign, either *\f{1,0,-1}.     
     s>f  m-frame sky.ra f@ f* 
     this ra-correction f/ 
     m-current sky.ra dup f@  f+   f!
  ;m


  :m (dec*+)   ( n this -- )
\ +G Increments the current RA by a *\b{signed} frame value.
\ ** *\i{n} is the spiral sign, either *\f{1,0,-1}.     
     s>f  m-frame sky.dec f@ f*
     this dec-correction f/
     m-current sky.dec dup f@ f+ f!
  ;m
   

  :m (end-spiral?)    ( this -- flag )
\ +G Test for end of spiralling pattern.     
     m-seq# @ 1- m-points @ < 0=
  ;m
 
\ -----------------
\ *H Public methods
\ -----------------
   
public
   
  m:   ( this -- )			\ overrides construct
\ *G     
     this [parent] construct
      0 m-points !
  ;m overrides construct


  :m set-points ( n1 this -- )
\ *G Set the number *\i{n1} of points to generate     
     m-points !
  ;m

   
  m:   ( this -- )			\ overrides reset
 \ *G Reset internal state to start a new iteration.
 \ Can't use *\fo{[parent] reset} because it sorts inital/final points.    
     this dup (/ra) dup (/dec)  m-seq# off
  ;m overrides reset

  
  m:   ( this -- p1 true | false)	\ overrides next-point
\ *G Get next sky point *\i{p1} or a false flag when finished.     
     1 m-seq# +!
     this (end-spiral?) if false exitm then
     this (next-point)
     this reduce-ra m-buffered true
   ;m overrides next-point

end-methods

\ ===================
\ +N Spiral sequences
\ ===================

\ +P Sign sequences for spiralling.
\ +* These are combined as multipliers with frame values
\ +* and increment RA and DEC as appropiate.
\ +* Up to 73 points can be generated.

create Seq0
\ +G Sequence table 0.
 0 ,				
 1 ,  0 ,
-1 , -1 ,  0 ,  0 ,
 1 ,  1 ,  1 ,  0 ,  0 ,  0 ,
-1 , -1 , -1 , -1 ,  0 ,  0 ,  0 ,  0 ,
 1 ,  1 ,  1 ,  1 ,  1 ,  0 ,  0 ,  0 , 0 , 0 ,
-1 , -1 , -1 , -1 , -1 , -1 ,  0 ,  0 , 0 , 0 , 0 , 0 ,
 1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  0 , 0 , 0 , 0 , 0 , 0 , 0 ,
-1 , -1 , -1 , -1 , -1 , -1 , -1 , -1 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 ,

create Seq1
\ +G Sequence table 1.
0 ,
0 , -1 ,
0 ,  0 , 1 ,  1 ,
0 ,  0 , 0 , -1 , -1 , -1 ,
0 ,  0 , 0 ,  0 ,  1 ,  1 ,  1 ,  1 ,
0 ,  0 , 0 ,  0 ,  0 , -1 , -1 , -1 , -1 , -1 ,
0 ,  0 , 0 ,  0 ,  0 ,  0 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,
0 ,  0 , 0 ,  0 ,  0 ,  0 ,  0 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,
0 ,  0 , 0 ,  0 ,  0 ,  0 ,  0 ,  0 , -1 , -1 , -1 , -1 , -1 , -1 , -1 , -1 ,

create Seq2
\ +G Sequence table 2.
0 ,
0 ,  1 ,
0 ,  0 , -1 , -1 ,
0 ,  0 ,  0 ,  1 ,  1 ,  1 ,
0 ,  0 ,  0 ,  0 , -1 , -1 , -1 , -1 ,
0 ,  0 ,  0 ,  0 ,  0 ,  1 ,  1 ,  1 ,  1 ,  1 ,
0 ,  0 ,  0 ,  0 ,  0 ,  0 , -1 , -1 , -1 , -1 , -1 , -1 ,
0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,
0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 ,  0 , -1 , -1 , -1 , -1 , -1 , -1 , -1 , -1 , 

create Seq3
\ +G Sequence table 3.
 0 ,
-1 ,  0 ,
 1 ,  1 ,  0 ,  0 ,
-1 , -1 , -1 ,  0 ,  0 ,  0 ,
 1 ,  1 ,  1 ,  1 ,  0 ,  0 ,  0 , 0 ,
-1 , -1 , -1 , -1 , -1 ,  0 ,  0 , 0 , 0 ,
 1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  0 , 0 , 0 , 0 , 0 , 0 ,
-1 , -1 , -1 , -1 , -1 , -1 , -1 , 0 , 0 , 0 , 0 , 0 , 0 , 0 ,
 1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 , 1 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 ,


: []@ ( n1 addr -- n2 )
\ +G Seqyence table fetch. Index *\i{n1} from 0.   
   swap cells + @
;

\ ======
\ *> ###
\ ======
