((
$Date: 2008-08-24 12:31:35 +0200 (dom 24 de ago de 2008) $
$Revision: 563 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/scripts/xephem.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ Library of utility words for XEphem simulated INDI devices.
\ In fact, this library encapsulates all properties handling and offers a 
\ high level API withwords like Photo, Focuser+, Focuser-, etc.


DEFINITION: discover   ( -- )
  indiserver connect
  indiserver properties get
  2 seconds waiting
END-DEFINITION

DEFINITION: My-Location  ( -- )
   40:25:00 LAT.   property item Mount GEOGRAPHIC_COORD LAT  set
  -03:42:00 LONG.  property item Mount GEOGRAPHIC_COORD LONG set
   property vector Mount GEOGRAPHIC_COORD send&idle
END-DEFINITION


\ An improved evrsion of the above, taking any gepgraphical location and
\ sending it to the Mount device
DEFINITION: Home-Location  ( geopoint -- )
   GeoPoint@
   property item   Mount GEOGRAPHIC_COORD LAT  set
   property item   Mount GEOGRAPHIC_COORD LONG set
   property vector Mount GEOGRAPHIC_COORD send&idle
END-DEFINITION


DEFINITION: Goto-M31  ( -- )
   00:42:44 R.A.  property item Mount EQUATORIAL_COORD RA  set
  +41:16:08 DEC.  property item Mount EQUATORIAL_COORD DEC set
   property vector Mount EQUATORIAL_COORD send&ok
END-DEFINITION


\ An improved version of the above to go anywhere in the sky.
DEFINITION: GoTo-RADEC		\ F: dec ra --
  property item   Mount EQUATORIAL_COORD RA  set
  property item   Mount EQUATORIAL_COORD DEC set
  property vector Mount EQUATORIAL_COORD send&ok
END-DEFINITION



\ -----------------
\ *H Camera Handling
\ ------------------

SEMAPHORE ccdsem
ccdsem    InitSem

1 CONSTANT 1x1
2 CONSTANT 2x2
3 CONSTANT 3x3
4 CONSTANT 4x4

DEFINITION: Photo ( binning -- ; F: exptime -- )
  device CCDCam BLOBs enable
  CASE
   1x1 OF True property item CCDCam Binning 1:1 set ENDOF  
   2x2 OF True property item CCDCam Binning 2:1 set ENDOF
   3x3 OF True property item CCDCam Binning 3:1 set ENDOF
   4x4 OF True property item CCDCam Binning 4:1 set ENDOF
  ENDCASE 
       property vector CCDCam Binning send&ok
       property item   CCDCam ExpValues ExpTime set
       property vector CCDCam ExpValues send&ok
END-DEFINITION


DEFINITION: Photos ( binning nphotos --  F: exptime -- )
  ccdsem request
  auto-blobs-dir property vector CCDCam Pixels directory set
  0 DO dup fdup Photo LOOP drop fdrop
  ccdsem signal
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

0 CONSTANT Blue
1 CONSTANT Visible
2 CONSTANT Red
3 CONSTANT IR
4 CONSTANT HII
5 CONSTANT Clear

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


True  CONSTANT switch-on
False CONSTANT switch-off

DEFINITION: Telescope ( flag -- )
  IF
    property item Mount Power On get 
    0= IF Telescope-on ENDIF
  else
    property item Mount Power Off get
    0= IF Telescope-off ENDIF
  ENDIF
END-DEFINITION


\ ---------------
\ *H Weather Task
\ ---------------
  
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



    
\ -----------------
\ *H Autofocus Task
\ -----------------

DEFINITION: VShape ( -- )
  16 0 DO
     2700 ms
     +Focuser
      s" taking a hi-res focus image" background logtask
     5.0 sec. 1x1 Photo
  LOOP
END-DEFINITION

  
DEFINITION: BestPosition ( -- )
    8 0 DO 2300 ms -Focuser LOOP
END-DEFINITION

    
DEFINITION: AutoFocus ( -- )
    ccdsem request
    VShape BestPosition
    ccdsem signal
END-DEFINITION    
    
    
DEFINITION: (Autofocuser) ( -- )
  default-io decimal
  background logging on
  s" started" background logtask
  BEGIN
     60 seconds waiting
     AutoFocus
  AGAIN     
END-DEFINITION


DEFINITION: Autofocuser ( -- )
  Assign (Autofocuser) catch
  ?dup IF background .#excp ccdsem initSem ENDIF
  s" finished" background logtask
END-DEFINITION

\ ----------------------------------  
\ +H Taking an LRGB exposuure series
\ ----------------------------------
  
DEFINITION: LRGB-Series  ( nphotos -- )
     s" Starting a  LRGB-Series" foreground log
     >R
     Red Filter
     Autofocuser
     s" Taking photos through Red Filter" foreground log
     30.0 sec. 2x2 R@ Photos
     Visible Filter
     Autofocuser
     s" Taking photos through Visible Filter" foreground log
     40.0 sec. 2x2 R@ Photos
     Blue Filter
     Autofocuser
     s" Taking photos through Blue Filter" foreground log
     60.0 sec. 2x2 R@ Photos
     Clear Filter
     Autofocuser
     s" Taking photos with no Filter" foreground log
     30.0 sec. 1x1 R> Photos
END-DEFINITION
    
  
  
