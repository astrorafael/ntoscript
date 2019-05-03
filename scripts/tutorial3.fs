((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/scripts/tutorial3.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ example for the sky mapping tutorial.

foreground logging on

IndiServer: indiserver localhost 7624
include xephem.fs

-03:42 LONG.  40:25 LAT. GeoPoint: Madrid

4904 x 3280  7.4 um. x 7.4 um. CCDChip: SXVF-H36

80.0 inch. x 8.0 inch. OTA: LX200-8"

-RA +DEC Uniform SkyMapper: skymapper1
00:50 R.A.   43:00 DEC.     skymapper1 set-initial
00:35 R.A.   39:30 DEC.     skymapper1 set-final
LX200-8"     SXVF-H36 FOV   skymapper1 set-frame
   10 %RA      10 %DEC      skymapper1 set-overlap


XEphem-Tool: xephem
skymapper1 xephem set-mapper
xephem generate


DEFINITION: LRGB-Mosaic ( -- )
  skymapper1 reset
  BEGIN
     skymapper1 next-point
  WHILE
	EqPoint@ GoTo-RADEC
	10 seconds waiting
	5 LRGB-Series
  REPEAT
END-DEFINITION

  
discover
switch-on telescope  
Madrid Home-Location
LRGB-Mosaic

   


