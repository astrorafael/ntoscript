((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/cli/units.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ######
\ *> cli
\ ######

\ ==============
\ *N Angle units
\ ==============

\ *P Set of words dediacted to conversions from sexagesimal angles
\ ** to internal representation (floating point degrees in this case)
\ ** and to perform formatted output of such quantities.

\ *P The idea is to express these quantities in a compact notation and convert
\ ** to floating point for use *\fo{NUMBER-VECTOR}s by using unit suffixes
\ ** like:
\ *E    00:42:44 R.A.  +41:16:08 DEC.
\ **   +40:25:00 LONG. -03:42:00 LAT.         

\ *P The output format is flexible There are 3 coordinate systems
\ ** (*\fo{EQUATORIAL}, *\fo{ALTAZIMUTAL} and (*\fo{GEOGRAPHICAL}) and
\ ** 3 output possible display formats for each one.
\ ** Examples:

\ *E   standard equatorial   coordinates
\ **   lo-res   geographical coordinates
\ **   hi-res   altazimutal  coordinates

\ *P Of course, indiviual words permit complete control of display format in
\ ** every other situation.

\ -----------------------------
\ *H Floating point conversions
\ -----------------------------

: >fsexag	     \ S: n1 u2 -- ; F: -- r1
\ *G Convert a (numerator, denominator pair) *\i{n1 u2} into floating
\ ** point degrees by dividing n1/u2.   
   swap s>f s>f f/
;


: >ANGLE				 \ F: r1 -- r2
\ *G Convert a floating point angle *\i{r1} expressed in time units like
\ ** *\i{Right Ascension} in sexagesimal degrees.
   15.0e0 f*
;

: >TIME					  \  F: r1 -- r2
\ *G Convert a floating point angle *\i{r1} expressed in sexagesimal degrees
\ ** in time units. Used for \i{Right Ascension} angles.
  15.0e0 f/   
;


: R.A.					  \ n1 u2 -- ; F: -- r1
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Right Ascension}.
\ ** Valid range: *\f{00:00:00} to *\f{23:59:59}
   >fsexag
;   

: DEC.					  \ n1 u2 -- ; F: -- r1
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Declination}.
\ ** Valid range: *\f{-90:00:00} to *\f{+90:00:00}
   >fsexag
;   

: LONG.					 \ n1 u2 -- ; F: -- r1 
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Longitude}.
\ ** Valid range: *\f{-180:00:00} to *\f{+180:59:59}. East is positive.
   >fsexag
;   

: LAT.					  \ n1 u2 -- ; F: -- r1
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Latitude}.
\ ** Valid range: *\f{-90:00:00} to *\f{+90:00:00}
   >fsexag
;

: AZ.					  \ n1 u2 -- ; F: -- r1 
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Azimuth}.
\ ** Valid range: *\f{+180:00:00} to *\f{-180:00:00}.
\ ** East of meridian is positive
   >fsexag
;   

: ALT.					  \ n1 u2 -- ; F: -- r1
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Altitude}.
\ ** Valid range: *\f{-90:00:00} to *\f{+90:00:00}
   >fsexag
;

: DEG.					   \ n1 u2 -- ; F: -- r1
\ *G Convert a numerator, denominator pair *\i{n1 u2} into
\ ** floating point *\i{Degrees}.
   >fsexag
;

\ ----------------------------
\ *H Angles formatted printing
\ ----------------------------

: .DD:MM				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DD:MM}.   
   60.0e0 f* fr>s 2 <.D:MM>
;

: .DDD:MM				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DDD:MM}.   
   60.0e0 f* fr>s 3 <.D:MM>
;

: .DD:MM.M				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DD:MM.M}.   
   600.0e0 f* fr>s 2 <.D:MM.M>
;

: .DDD:MM.M				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DDD:MM.M}.   
   600.0e0 f* fr>s 3 <.D:MM.M>
;

: .DD:MM:SS				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DD:MM:SS}.   
   3600.0e0 f* fr>s 2 <.D:MM:SS>
;

: .DDD:MM:SS				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DDD:MM:SS}.   
   3600.0e0 f* fr>s 3 <.D:MM:SS>
;

: .DD:MM:SS.S				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DD:MM:SS.S}.
   36000.0e0 f* fr>s 2 <.D:MM:SS.S>
;

: .DDD:MM:SS.S				 \ F: r1 --
\ *G Print sexagesimal angle *\{r1} as *\fo{+DDD:MM:SS.S}.
   36000.0e0 f* fr>s 3 <.D:MM:SS.S>
;


Defer (.DEG)				\ F:  r1 --
\ *G Sexagesimal format Generic Degrees printing routine.

Defer (.RA)				\ F:  r1 --
\ *G Sexagesimal format Right Ascension printing routine.

Defer (.DEC)				\ F:  r1 --
\ *G Sexagesimal format Declination printing routine.

Defer (.AZ)				\ F:  r1 --
\ *G Sexagesimal format Azimuth printing routine.

Defer (.ALT)				\ F:  r1 --
\ *G Sexagesimal format Altitude printing routine.

Defer (.LONG)				\ F:  r1 --
\ *G Sexagesimal format Longitude printing routine.

Defer (.LAT)				\ F:  r1 --
\ *G Sexagesimal format Latitude printing routine.


: .DEG					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{R.A.} label.
   (.DEG) ." DEG."
;

: .RA					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{R.A.} label.
   (.RA) ." R.A."
;

: .DEC					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{DEC.} label.
   (.DEC) ." DEC."
;

: .LAT					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{LAT.} label.
   (.LAT) ." LAT."
;

: .LONG					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{LONG.} label
   (.LONG) ." LONG."
;

: .AZ					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{AZ.} label.
   (.AZ) ." AZ."
;

: .ALT					  \ F: r1 --
\ *G Print a floating point *\i{r1} as sexagesimal format plus *\f{ALT.} label.
   (.ALT) ." ALT."
;

\ --------------------------
\ *H Angles formatted output
\ --------------------------

: (hi-eq)
\ +G Set astronomical equatorial coordinates printing routines to high
\ +* resolution.
   Assign .DD:MM:SS.S to-do (.RA)
   Assign .DD:MM:SS   to-do (.DEC)
   Assign .DD:MM:SS   to-do (.DEG)
;


: (st-eq)
\ +G Set astronomical equatorial coordinates printing routines to standard
\ +* resolution.
   Assign .DD:MM:SS to-do (.RA)
   Assign .DD:MM:SS to-do (.DEC)
   Assign .DD:MM:SS to-do (.DEG)
; 


: (lo-eq)
\ +G Set astronomical equatorial coordinates printing routines to standard
\ +* resolution.
   Assign .DD:MM.M to-do (.RA)
   Assign .DD:MM   to-do (.DEC)
   Assign .DD:MM   to-do (.DEG)
; 


: (hi-altaz)
\ +G Set astronomical altazimutal coordinates printing routines to high
\ +* resolution.
   Assign .DDD:MM:SS.S   to-do (.AZ)
   Assign .DDD:MM:SS.S   to-do (.ALT)
   Assign .DDD:MM:SS.S   to-do (.DEG)
; 


: (st-altaz)
\ +G Set astronomical altazimutal coordinates printing routines to standard
\ +* resolution.
   Assign .DDD:MM:SS   to-do (.AZ)
   Assign .DDD:MM:SS   to-do (.ALT)
   Assign .DDD:MM:SS   to-do (.DEG)
; 


: (lo-altaz)
\ +G Set astronomical altazimutal coordinates printing routines to standard
\ +* resolution.
   Assign .DDD:MM   to-do (.AZ)
   Assign .DDD:MM   to-do (.ALT)
   Assign .DDD:MM   to-do (.DEG)
;


: (hi-geo)
\ +G Set geographical coordinates printing routines to high
\ +* resolution.
   Assign .DDD:MM:SS.S   to-do (.LONG)
   Assign .DDD:MM:SS.S   to-do (.LAT)
   Assign .DDD:MM:SS.S   to-do (.DEG)
; 


: (st-geo)
\ +G Set geographical coordinates printing routines to standard
\ +* resolution.
   Assign .DDD:MM:SS   to-do (.LONG)
   Assign .DDD:MM:SS   to-do (.LAT)
   Assign .DDD:MM:SS   to-do (.DEG)
; 


: (lo-geo)
\ +G Set geographical coordinates printing routines to standard
\ +* resolution.
   Assign .DDD:MM.M   to-do (.LONG)
   Assign .DDD:MM.M   to-do (.LAT)
   Assign .DDD:MM.M   to-do (.DEG)
;


Create coordinates-table
\ +G Jump table for coordinate selection.
' (hi-eq) ,
' (st-eq) ,
' (lo-eq) ,
' (hi-altaz) ,
' (st-altaz) ,
' (lo-altaz) ,
' (hi-geo) ,
' (st-geo) ,
' (lo-geo) ,



0 Constant Equatorial
\ *G Equatorial coordinates printing words *\fo{.RA} and *\fo{.DEC}

1 Constant Altazimutal
\ *G Altazimutal coordinates printing words *\fo{.AZ} and *\fo{.ALT}

2 Constant Geographical
\ *G Geographical coordinates printing words *\fo{.LAT} and *\fo{.LONG}


\ -------------------------------
\ *P Display resolution constants
\ -------------------------------

0 Constant hi-res
\ *G Select the highest resolution for the selected set of coordinates.

1 Constant standard
\ *G Select the most commonly used resolution for the selected set of
\ ** coordinates.

2 Constant lo-res
\ *G Select the lowest resolution for the selected set of coordinates.


: coordinates				  \ resol system --
\ *G Select the resolution for a given set of coordinates and resolution.
\ ** Example:   
\ *C    standard equatorial coordinates   
   3 * + cells coordinates-table + @ execute
;

standard equatorial coordinates
standard altazimutal coordinates
standard geographical coordinates

\ ====================================
\ *N Other Physical/Non-physical Units
\ ====================================

\ *P Unit names as suffixes to make scripts more readable when setting
\ ** property values.
\ ** These units are meant to be used when writting *\fo{NUMBER-VECTOR}s
\ ** whose values are *\fo{always} floating point.

\ -----------------------
\ *H Floating point Units
\ -----------------------

\ *P To use these units, preceeding numbers *\b{must} include a dot.

\ Therei s a problem with DPL when including words like 'secs' in definitions.
\ We should compile somehow DPL that was produced at comiple time.
\ so we need state smart words.
\ combined this with vectored execution and I don't now what the outcome will
\ be


(( NOT USED ANYMORE
: d>fp					  \ d1 -- ; F: -- r1
\ Converts double *\i{d1} into floating point taking *\fo{DPL} into account.
\ Unlike d>f, which always returns a floating point without decimals.   
\   23.34 d>fp gives  2.3340000e1
\  23.34 d>f  gives  +2334
    d>f dpl @ 10**n f/
;
))


: sec.					  \ d1 -- ; F: -- r1
\ *G Time seconds. Can express fractions of seconds.
; immediate


: e-/ADU				  \ d1 -- ; F: -- r1
\ *G For GAIN conversion word. Usage:
\ *C   2.5 e-/ADU
; immediate


: e-					  \ d1 -- ; F: -- r1
\ *G For RDNOISE conversion word. Also can express fractional electrons
\ ** coming from statistical estimation. Usage:
\ *C   34. e-
; immediate


: arcsec/pixel				 \ d1 -- ; F: -- r1 
\ *G SCALE conversion word. Usage:
\ *C   2.3 arcsec/pixel
   
; immediate
 

: volts					  \ d1 -- ; F: -- r1 
\ *G VPELT conversion word. Volts can be fractional. Usage:
\ *C   12. volts
   
; immediate


: degrees				  \ --
\ *G Syntactic sugar for the word below.
; immediate


: celsius			     \ d1 -- ; F: -- r1   
\ *G HOT,COLD temperature conversion word. Usage:
\ *C  -30. degrees celsius
   
; immediate


: cm.		                    \ d1 -- ; F: -- r1 
\ *G Convert *\i{r1} centimeters to *\i{r2} meters. Usage:
\ *C 100. cm.   
     1.0e-2 f*
; 


: mm.		                    \ d1 -- ; F: -- r1 
\ *G Convert *\i{r1} milimeters to *\i{r2} meters. Usage:
\ *C 3. mm.      
    1.0e-3 f*
; 


: um.					  \ d1 -- ; F: -- r1 
\ *G Convert *\i{r1} microns to *\i{r2} meters. Usage:
\ *C 9. um.   
    1.0e-6 f* 
;


: inch.					  \ d1 -- ; F: -- r1 
\ *G Convert *\i{r1} inches to *\i{r2} meters. Usage:
\ *C 9. inch.   
    2.54e0 cm. f* 
; 


\ ----------------
\ *H Integer Units
\ ----------------

\ *P To use these units, preceeding numbers *\b{must not} include a dot.

Alias: units s>f			\ n1 -- ; F: -- r1
\ *G Express generic integer quantities as floating point. Usage:
\ *C   12 units


Alias: images s>f			\ n1 -- ; F: -- r1
\ *G Number of images as floating point. Usage:
\ *C   4 images


Alias: pixels s>f			\ n1 -- ; F: -- r1
\ *G Express pixels as floating point. Usage:
\ *C   234 pixels


Alias: buffers s>f			\ n1 -- ; F: -- r1
\ *G Express pixels as floating point. Usage:
\ *C   234 pixels


Alias: #times s>f			\ n1 -- ; F: -- r1
\ *G Express number of times as floating point. Usage:
\ *C   234 #times


: x					  \ -- 
\ *G Syntactic sugar to express pair of things like dimensions. Usage:
\ *C   6.0 mm. x 4.0 mm.   
; immediate


\ ======
\ *> ###
\ ======
