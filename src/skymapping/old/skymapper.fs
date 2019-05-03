((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/old/skymapper.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############

\ =========================
\ *N *\fo{SKYMAPPER} module
\ =========================

\ *P This module enapsulate the creation of the appropiate sky mapper
\ ** object given the initial flags.
\ ** Three flags must be given:
\ *( 1
\ *B The way the R.A. coordinate should vary: *\fo{+RA}, *\fo{-RA}
\ *B The way the Dec. coordinate should vary: *\fo{+DEC}, *\fo{-DEC}
\ *B The mapping pattern: *\fo{UNIFORM}, *\fo{ZIGZAG}.
\ *)



\ ***********
\ *S Glossary
\ ***********

MODULE SkyMapper
\ *G

\ =================
\ *N Creation flags
\ =================

0 Constant Uniform
\ *G Select the uniform pattern.

1 Constant ZigZag
\ *G Select the zigzag pattern.

2 Constant Spiral
\ *G Select the spiral pattern

$00 Constant -RA
\ *G R.A. coordinate should decrease.

$01 Constant +RA
\ *G R.A. coordinate should increase.

$00 Constant -DEC
\ *G Dec. coordinate should decrease.

$02 Constant +DEC
\ *G Dec. coordinate should increase.

$00 Constant CW
\ *G Clockwise spiral.

$04 Constant CCW
\ *G Counterclockwise spiral.



\ =================
\ +N Internal Words
\ =================

: (UniformSkyMapper)		       \ raflag decflag -- obj
\ +G Create an uniform sky mapper object based on flags passed.   
   or case
      -ra -dec or of ra-dec-uniform-ra heap-new endof
      -ra +dec or of ra-dec+uniform-ra heap-new endof
      +ra -dec or of ra+dec-uniform-ra heap-new endof      
      +ra +dec or of ra+dec+uniform-ra heap-new endof
   endcase
;

: (ZigzagSkyMapper)		       \ raflag decflag -- obj
\ +G Create a zigzag sky mapper object based on flags passed.      
   or case
      -ra -dec or of ra-dec-zigzag-ra heap-new endof
      -ra +dec or of ra-dec+zigzag-ra heap-new endof
      +ra -dec or of ra+dec-zigzag-ra heap-new endof
      +ra +dec or of ra+dec+zigzag-ra heap-new endof
   endcase
;

: (SpiralSkyMapper)		       \ npoints raflag sense -- obj
\ +G Create a zigzag sky mapper object based on flags passed.      
   or case
      -ra  cw  or of ra-cw-spiral   heap-new endof
      -ra  ccw or of ra-ccw-spiral  heap-new endof
      +ra  cw  or of ra+cw-spiral   heap-new endof
      +ra  ccw or of ra+ccw-spiral  heap-new endof
      -dec cw  or of dec-cw-spiral  heap-new endof
      -dec ccw or of dec-ccw-spiral heap-new endof
      +dec cw  or of dec+cw-spiral  heap-new endof
      +dec ccw or of dec+ccw-spiral heap-new endof
   endcase
;

\ ======================
\ +N Sky Mapper creation
\ ======================

: (SkyMapper)				  \ raflag decflag pattern -- obj
\ *G Create an anonymous sky mapper object based on the *\i{pattern},
\ ** *\{rafflag} and *\i{decflag} passed.      
   case
      uniform of (UniformSkyMapper) endof
      zigzag  of (ZigzagSkyMapper)  endof
      spiral  of (SpiralSkyMapper)  endof
      NULL
   endcase
;


: SkyMapper:	      \ raflag decflag pattern "name" -- ; [child] obj
\ *G Create a named sky mapper object based on the *\i{pattern},
\ ** *\{rafflag} and *\i{decflag} passed.   
   (SkyMapper) Constant
;

EXPORT +RA
EXPORT -RA
EXPORT +DEC
EXPORT -DEC
EXPORT  CW
EXPORT  CCW

EXPORT UNIFORM
EXPORT ZIGZAG
EXPORT SPIRAL

EXPORT (SkyMapper)
EXPORT SkyMapper:

END-MODULE

\ ======
\ *> ###
\ ======
