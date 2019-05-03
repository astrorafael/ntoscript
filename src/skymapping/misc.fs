((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/misc.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> skymapping
\ #############

\ ======================
\ *N FP Stack operations
\ ======================

2.0e0 FConstant %2			\ -- f#(2.0)
\ *G Floating Point 2.0


: f2dup				 \  F: r1 r2 -- r1 r2 r1 r2
\ *G Floating point equivalent of *\fo{2DUP}   
   fover fover
;


: frot-				 \ F: r1 r2 r3 -- r3 r1 r2
\ *G Floating point equivalent of *\fo{-ROT} 
   frot frot
;


: -freduce-sg		     \ F: r1 -- r2
\ *G Reduce negative sexagesimal floating point value *\i{ri} to 0..360.0
\ ** range.
   begin fdup f0< while 360.0e0 f+ repeat
;

: +freduce-sg		     \ F: r1 -- r2
\ *G Reduce positive sexagesimal floating point value *\i{ri} to 0..360.0
\ ** range.
   360.0e0 fmod	
;


: freduce-sg				  \ F: r1 -- r2
\ *G Reduce positive or negative sexagesimal floating point value *\i{ri}
\ ** to 0..360.0 range.   
   -freduce-sg +freduce-sg
;


\ ===================
\ *N Conversion Units
\ ===================

: %RA					  \ n1 -- n1
\ *G Overlap Percentage of RA. Range: 0 <= u1 < 100
; immediate


: %DEC					  \ n1 -- n1
\ *G Overlap Percentage of DEC. Range: 0 <= u1 < 100
; immediate

: frames				  \ --
\ *G Expreses number for frames. Syntactic sugar. Intended for use with
\ ** *\fo{set-frames} method.   
\ *C   3 x 4 frames  skymapper2 set-frames
; immediate


\ ====================================
\ *N CCD and OTA Instrument parameters
\ ====================================

\ *P Structures to hold instrimental data for various tools and purposes like
\ ** calculating the FOV of a given CCD Chip through a given OTA.

struct /CCDChip
\ *G CCD Chip structure
\ *[   
   1 floats field ccd.width		\ whole imaging width in meters
   1 floats field ccd.height		\ whole imaging area  in meters
   1 floats field ccd.wpix		\ pixel phyisical width in meters
   1 floats field ccd.hpix		\ pixel phyisical height in meters
   1 cells  field ccd.wres		\ width resolution
   1 cells  field ccd.hres		\ height resolution
end-struct
\ *]   


: imaging-area				  \ ccd --
\ +G Calculates the CCD imaging area given its pixel physical dimensions
\ +* and CCD resolution. Store it into the *\i{ccd} structure. Assume these
\ +* otehr fields properly initialized.  
   dup ccd.wres @ s>f dup ccd.wpix f@ f* dup ccd.width  f!
   dup ccd.hres @ s>f dup ccd.hpix f@ f*     ccd.height f!
;

   
: initCCDChip		      \ S: wres hres addr -- ; F: wpix hpix --
\ +G Initializes a *\fo{/CCDChip} with given values.
   dup  ccd.hpix f! dup ccd.wpix f! 	\ store pixel pyhisical size
   tuck ccd.hres ! tuck ccd.wres !	\ store CCD resolution
   imaging-area
;   


: CCDChip:	\ S: resw resh "name" -- ; F: wpix hpix -- ; [child] -- addr
\ *G Create a *\fo{/CCDChip} structure with given resolution *\i{resw}
\ ** and  *\i{resw} in pixels and pixel phisical width *\i{wpix} and *\i{hpix}.
\ *C     1024 x 768  9.0 um. x 9.0 um. CCDChip: KAF-400
   Create here /CCDChip allot initCCDChip
;


struct /OTA
\ *G Optical Tube Assembly structure.
\ *[   
   1 floats field ota.focal		\ in meters
   1 floats field ota.diam		\ in meters
end-struct
\ *]   


: initOTA				\ S: addr -- ; F: fl diam --
\ +G Initializes a *\fo{/OTA} with given values of focal length *\i{fl} and
\ +* diameter.
   dup ota.diam f! ota.focal f!
;   


: OTA:			\ "name" ; F: focal diam -- ; [child] -- addr
\ *G Create an *\fo{/OTA} structure with a given focal length and diameter.
\ *C    80.0 inch. x 8.0 inch. OTA: LX200-8"@f/10  
   Create here /OTA allot initOTA
;


: 1D-FOV					  \ w fl -- fov
\ +G Calculates the field of view, in floating point degrees, of a
\ +* focal plane width *\i{w} respect to a focal length *\i{fl}.
   %2 f* fatan2 %2 f* rad>deg
;


: FOV					  \ ota ccd -- width height
\ *G Calculates the FOV in floating point degrees of a *\i{ccd} chip
\ ** through a given *\i{ota}. The resulting *\i{width} angular dimension
\ ** is usually assigned to RA and the *\i{height} to Dec.
   2dup ccd.width  f@ ota.focal f@ 1D-FOV
        ccd.height f@ ota.focal f@ 1D-FOV
;


\ ======
\ *> ###
\ ======
