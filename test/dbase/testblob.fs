
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/console/filelist.fs

also nto definitions

1 [if]

IndiServer: indiserver localhost 7624
indiserver connect
indiserver properties get

2000 ms

s" Camera" indiserver lookup drop constant ccd

ccd BLOBs enable

s" Exposure" ccd lookup drop constant exposure
s" ExpDur" exposure lookup drop constant duration
5.0 duration set

\ ' see-message IS parse-xml

exposure send&wait

s" Mata al indiserver ya" type cr
1000 ms
\ s" Hola amigos" type



[endif]



: photo1 ( -- )
  device Camera BLOBs enable
  2. secs property item   Camera Exposure ExpDur set
           property vector Camera Exposure send
;


definition: photo2 ( -- )
  device Camera BLOBs enable
  2. secs property item   Camera Exposure ExpDur set
           property vector Camera Exposure send
  begin
      pause
      property vector Camera Exposure state
  Busy <> until
end-definition
  