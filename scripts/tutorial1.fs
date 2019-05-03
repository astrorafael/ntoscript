((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/scripts/tutorial1.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ This file covers examples 1-10 in the tutorial

IndiServer: indiserver localhost 7624
indiserver connect
indiserver properties get
2 seconds waiting


DEFINITION: My-Location  ( -- )
   40:25:00 LAT.   property item Mount GEOGRAPHIC_COORD LAT  set
  -03:42:00 LONG.  property item Mount GEOGRAPHIC_COORD LONG set
   property vector Mount GEOGRAPHIC_COORD send&idle
END-DEFINITION


DEFINITION: Goto-M31  ( -- )
   00:42:44 R.A.  property item Mount EQUATORIAL_COORD RA  set
  +41:16:08 DEC.  property item Mount EQUATORIAL_COORD DEC set
   property vector Mount EQUATORIAL_COORD send&ok
END-DEFINITION


DEFINITION: Photo ( -- )
  device CCDCam BLOBs enable
  True property item   CCDCam Binning 1:1 set
       property vector CCDCam Binning send&ok
  20. sec. property item   CCDCam ExpValues ExpTime set
           property vector CCDCam ExpValues send&ok
END-DEFINITION


DEFINITION: +Focuser  ( -- )
      property item   OTA Focuser Focus get
      property item   OTA Focuser Focus step	
   f+ property item   OTA Focuser Focus set
      property vector OTA Focuser send&ok
END-DEFINITION


DEFINITION: -Focuser  ( -- )
      property item OTA Focuser Focus get
      property item OTA Focuser Focus step
   f- property item OTA Focuser Focus set
      property vector OTA Focuser send&ok
END-DEFINITION

0 Constant Blue
1 Constant Visible
2 Constant Red
3 Constant IR
4 Constant HII
5 Constant Clear

DEFINITION: Filter  ( n1 -- )
   CASE
     Blue    OF True property item OTA Filter B set ENDOF
     Visible OF True property item OTA Filter V set ENDOF
     Red     OF True property item OTA Filter R set ENDOF
     IR      OF True property item OTA Filter I set ENDOF
     HII     OF True property item OTA Filter H set ENDOF
	        True property item OTA Filter C set
   ENDCASE
   property vector OTA Filter send&ok
END-DEFINITION


True  Constant switch-on
False Constant switch-off

DEFINITION: Telescope-on ( -- )
   s" switching on telescope mount" background logtask
   True property item   Mount Power On set
       property vector Mount Power send&ok  
END-DEFINITION

DEFINITION: Telescope-off ( -- )
   s" switching off telescope mount" background logtask
  True property item   Mount Power Off set 
       property vector Mount Power send&idle
END-DEFINITION

DEFINITION: Telescope ( flag -- )
  IF
    property item Mount Power On get 
    0= IF Telescope-on ENDIF
  else
    property item Mount Power Off get
    0= IF Telescope-off ENDIF
  ENDIF
END-DEFINITION

DEFINITION: (WeatherAction) ( -- )
  default-io decimal
  background logging on
  s" started" background logtask
  BEGIN
     2 seconds waiting
     property vector Weather WX state
     CASE
	Alert OF switch-off telescope ENDOF
	Ok    OF switch-on  telescope ENDOF
     ENDCASE 
  AGAIN
END-DEFINITION

DEFINITION: WeatherAction ( -- )
  Assign (WeatherAction) catch
  background .#excp
  s" finished" background logtask
END-DEFINITION

Task Weather-Task
Assign WeatherAction To-Start Weather-Task

switch-on telescope
My-Location
Goto-M31
Red Filter
Photo


