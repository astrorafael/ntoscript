
definition: discover   ( -- )
  indiserver connect
  indiserver properties get
  2 seconds waiting
end-definition

definition: My-Location  ( -- )
   40 deg 25 mm 00 ss property item Mount GEOGRAPHIC_COORD LAT  set
  -03 deg 42 mm 00 ss property item Mount GEOGRAPHIC_COORD LONG set
   property vector Mount GEOGRAPHIC_COORD send&idle
end-definition

definition: Goto-M31  ( -- )
   00 hh  42 mm 44 ss property item Mount EQUATORIAL_COORD RA  set
  +41 deg 16 mm 08 ss property item Mount EQUATORIAL_COORD DEC set
   property vector Mount EQUATORIAL_COORD send&ok
end-definition

\ -----------------
\ *H Camera Handling
\ ------------------

semaphore ccdsem
ccdsem    InitSem

1 Constant 1x1
2 Constant 2x2
3 Constant 3x3
4 Constant 4x4

definition: Photo ( binning -- ; F: exptime -- )
  device CCDCam BLOBs enable
  case
   1x1 of True property item CCDCam Binning 1:1 set endof  
   2x2 of True property item CCDCam Binning 2:1 set endof
   3x3 of True property item CCDCam Binning 3:1 set endof
   4x4 of True property item CCDCam Binning 4:1 set endof
  endcase 
       property vector CCDCam Binning send&ok
       property item   CCDCam ExpValues ExpTime set
       property vector CCDCam ExpValues send&ok
end-definition


definition: Photos ( binning nphotos --  F: exptime -- )
  ccdsem request
\  auto-blobs-dir property vector CCDCam Pixels directory set
  0 DO dup fdup Photo LOOP drop fdrop
  ccdsem signal
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


definition: Telescope-on ( -- )
   s" switching on telescope mount" foreground .logtask
   True property item   Mount Power On set
       property vector Mount Power send&ok  
end-definition


definition: Telescope-off ( -- )
   s" switching off telescope mount" foreground .logtask
  True property item   Mount Power Off set 
       property vector Mount Power send&idle
end-definition


True  Constant switch-on
False Constant switch-off

definition: Telescope ( flag -- )
  if
    property item Mount Power On get 
    0= if Telescope-on endif
  else
    property item Mount Power Off get
    0= if Telescope-off endif
  endif
end-definition


\ ---------------
\ *H Weather Task
\ ---------------
  
definition: (WeatherAction) ( -- )
  default-io decimal
  foreground logging on
  s" started" foreground .logtask
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
  foreground .#excp
  s" finished" foreground .logtask
end-definition

\ -----------------
\ *H Autofocus Task
\ -----------------

definition: VShape ( -- )
  16 0 DO
     2700 ms
     +Focuser
      s" taking a hi-res focus image" foreground .logtask
     5.0 secs 1x1 Photo
  LOOP
end-definition

  
definition: BestPosition ( -- )
    8 0 DO 2300 ms -Focuser LOOP
end-definition

definition: AutoFocus ( -- )
    ccdsem request
    VShape BestPosition
    ccdsem signal
end-definition    
    
    
definition: (Autofocuser) ( -- )
  default-io decimal
  foreground logging on
  s" started" foreground .logtask
  BEGIN
     60 seconds waiting
     AutoFocus
  AGAIN     
end-definition


definition: Autofocuser ( -- )
  Assign (Autofocuser) catch
  ?dup if foreground .#excp ccdsem initSem then
  s" finished" foreground .logtask
end-definition