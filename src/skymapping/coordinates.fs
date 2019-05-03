((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/skymapping/coordinates.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #############
\ *> skymapping
\ #############

\ ============================
\ *N *\fo{/EQPOINT} structure
\ ============================

\ +P Things like sky coordinate pairs do not really exhibit any visible
\ +* behaviour that could be coded in methods. So I have decided to turn
\ +* it into a structure whose fields can be freely accesed.

\ +P Since this is a frely accesible structure, it is up to the programmer
\ +* to store/retrive whatever he wants to. He is also responsible to
\ +* maintain the proper units (radians, degrees, hours)

\ +P Use words *\fo{f@} or *\fo{f!} (or synonyms *\fo{fpfetch}
\ +* and *\fo{fpstore}) to get/set *\fo{sky.ra} or *\fo{sky.y}.
\ +* Use words  *\fo{$@} or *\fo{$!} ((or synonyms *\fo{$fetch} and
\ +* and *\fo{$store}) to read/write the label string.

struct /EqPoint
\ *G Equatorial coordinates structure.
\ *[   
   1 floats field sky.ra		\ Right Ascension
   1 floats field sky.dec		\ Declination
end-struct
\ *]   


: EqPoint!   ( F: dec ra -- ; S: p1 --  )
\ *G Write coodinate pairs on the float stack to the *\fo{/EqPoint} structure.
\ ** Reverse ordering of coordinates ensures easy access to *\i{ra} and
\ ** peform conversions from time to degrees.
   dup sky.ra f! sky.dec f!
;

: EqPoint@   ( F: -- dec ra ; S: p1 --  )
\ *G Read coodinate pairs from the *\fo{/EqPoint} structure to the float stack.
\ ** Reverse ordering of coordinates ensures easy access to *\i{ra} and
\ ** peform conversions from degrees to time.
   dup sky.dec f@ sky.ra f@
;

: initEqPoint				  \ p1 -- ; F: ra dec --
\ *G Initialize a *\fo{/EqPoint} structure *\i{p1},
\ ** with *\i{ra} and *\i{dec} initial values.
   fswap EqPoint!
;

: EqPoint:			\ "name" ; F: ra dec -- ; [child] addr
\ *G Create a *\fo{/EqPoint} named structure in the dictionary.      
   Create here /EqPoint allot initEqPoint
;


: .2xDEG				  \ F: ra dec --
\ *G Print a pair of floating point coordinates both in degrees
\ ** where *\i{ra} can be Right ascension or azimith, and *\i{dec}
\ ** may be Declination or Altitude.   
    fswap .DEG 2 spaces .DEG cr
;

: .RA.DEC				  \ F: ra dec --
\ *G Print to standard output the RA & DEC float coordinates.
\ ** There is no conversion for RA and will print whatever angle value
\ ** is passed.   
   fswap .RA 2 spaces .DEC cr
;

   
: .EqPoint				  \ p1 --
\ *G Print to standard output the RA and DEC coordinates 
\ ** of a *\i{p1} *\fo{/EqPoint} structure.
   dup sky.ra f@ sky.dec f@ .RA.DEC
;

\ ============================
\ *N *\fo{/GEOPOINT} structure
\ ============================

\ +P Geographical coordinates. same comments as *\fo{/EqPoint} apply.

struct /GeoPoint
\ *G Geographical coordinates structure.
\ *[   
   1 floats field geo.long		\ Longitude in degrees
   1 floats field geo.lat		\ Latitude  in degrees
end-struct
\ *]   


: GeoPoint!   ( F: long lat -- ; S: p1 --  )
\ *G Write coodinate pairs on the float stack to the *\fo{/GeoPoint} structure.
   dup geo.lat f! geo.long f! 
;

: GeoPoint@   ( F: -- long lat ; S: p1 --  )
\ *G Read coodinate pairs from the *\fo{/GeoPoint} structure
\ ** to the float stack.
   dup geo.long f@  geo.lat f@
;

: initGeoPoint				  \ p1 -- ; F: long lat --
\ *G Initialize a *\fo{/GeoPoint} structure *\i{p1},
\ ** with *\i{long} and *\i{lat} initial values.
   GeoPoint!
;

: GeoPoint:			\ "name" ; F: long lat -- ; [child] addr
\ *G Create a *\fo{/GeoPoint} named structure in the dictionary.   
   Create here /GeoPoint allot initGeoPoint
;


: .LONG.LAT				  \ F: long lat --
\ *G Print to standard output the long and lat coordinates.  
   fswap .LONG 2 spaces .LAT cr
;

   
: .GeoPoint				  \ p1 --
\ *G Print to standard output the RA and DEC coordinates 
\ ** of a *\i{p1} *\fo{/GeoPoint} structure.
   dup geo.long f@ geo.lat f@ .LONG.LAT
;
