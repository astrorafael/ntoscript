C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../Lib" SetMacro LIB2
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs


IndiServer: indiserver localhost 7624
indiserver connect
indiserver properties get
2 seconds waiting


definition: My-Location  ( -- )
   40:25:00 LAT.   property item Mount GEOGRAPHIC_COORD LAT  set
  -03:42:00 LONG.  property item Mount GEOGRAPHIC_COORD LONG set
   property vector Mount GEOGRAPHIC_COORD send&idle
end-definition


definition: Goto-M31  ( -- )
   00:42:44 R.A.  property item Mount EQUATORIAL_COORD RA  set
  +41:16:08 DEC.  property item Mount EQUATORIAL_COORD DEC set
   property vector Mount EQUATORIAL_COORD send&ok
end-definition


definition: Photo ( -- )
  device CCDCam BLOBs enable
  True property item   CCDCam Binning 1:1 set
       property vector CCDCam Binning send&ok
  20. sec. property item   CCDCam ExpValues ExpTime set
           property vector CCDCam ExpValues send&ok
end-definition


definition: +Focuser  ( -- )
      property item   OTA Focuser Focus get
      property item   OTA Focuser Focus step	
   f+ property item   OTA Focuser Focus set
      property vector OTA Focuser send&ok
end-definition


definition: -Focuser  ( -- )
      property item OTA Focuser Focus get
      property item OTA Focuser Focus step
   f- property item OTA Focuser Focus set
      property vector OTA Focuser send&ok
end-definition

0 Constant Blue
1 Constant Visible
2 Constant Red
3 Constant IR
4 Constant HII
5 Constant Clear

definition: Filter  ( n1 -- )
   case
     Blue    of True property item OTA Filter B set endof
     Visible of True property item OTA Filter V set endof
     Red     of True property item OTA Filter R set endof
     IR      of True property item OTA Filter I set endof
     HII     of True property item OTA Filter H set endof
	        True property item OTA Filter C set
   endcase
   property vector OTA Filter send&ok
end-definition


True  Constant switch-on
False Constant switch-off

definition: Telescope-on ( -- )
   s" switching on telescope mount" background logtask
   True property item   Mount Power On set
       property vector Mount Power send&ok  
end-definition

definition: Telescope-off ( -- )
   s" switching off telescope mount" background logtask
  True property item   Mount Power Off set 
       property vector Mount Power send&idle
end-definition

definition: Telescope ( flag -- )
  if
    property item Mount Power On get 
    0= if Telescope-on endif
  else
    property item Mount Power Off get
    0= if Telescope-off endif
  endif
end-definition

definition: (WeatherAction) ( -- )
  default-io decimal
  background logging on
  s" started" background logtask
  BEGIN
     2 seconds waiting
     property vector Weather WX state
     case
	Alert of switch-off telescope endof
	Ok    of switch-on  telescope endof
     endcase 
  AGAIN
end-definition

definition: WeatherAction ( -- )
  Assign (WeatherAction) catch
  background .#excp
  s" finished" background logtask
end-definition

Task Weather-Task
Assign WeatherAction To-Start Weather-Task

switch-on telescope
My-Location
Goto-M31
Red Filter
Photo


