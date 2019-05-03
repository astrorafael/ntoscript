
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC

include %SRC%/filelist.fs

also nto definitions

Server: indi1 localhost 7624
indi1 s" indiserver" indi-server heap-new constant indiserver

s" AUDINE1" indi-device  heap-new dup constant audine1


s" ADC_SPEED" switch-vector heap-new 2dup swap push-back
s" 100KBS"    switch-item heap-new over push-back
s" 200KBS"    switch-item heap-new over push-back
drop
s" AREA_DIM" number-vector heap-new 2dup swap push-back
s" DIMY"     number-item heap-new over push-back
s" DIMX"     number-item heap-new over push-back
s" PIXSZ"    number-item heap-new over push-back
drop
s" AREA_DIM_RECT" number-vector heap-new 2dup swap push-back
s" DIMY"          number-item heap-new over push-back
s" DIMX"          number-item heap-new over push-back
s" ORIGX"         number-item heap-new over push-back
s" ORIGY"         number-item heap-new over push-back
drop
s" AREA_PRESETS"  switch-vector heap-new 2dup swap push-back
s" CENTER"        switch-item heap-new over push-back
s" CORNER1"       switch-item heap-new over push-back
s" CORNER2"       switch-item heap-new over push-back
s" CORNER3"       switch-item heap-new over push-back
s" CORNER4"       switch-item heap-new over push-back
drop
s" AREA_PRESET_SIZE" number-vector heap-new 2dup swap push-back
s" SIZE"             number-item heap-new over push-back
drop
s" AREA_SELECTION" switch-vector heap-new 2dup swap push-back
s" FULL_FRAME"     switch-item heap-new over push-back
s" FULL_FRAME_OV"  switch-item heap-new over push-back
s" PRESETS"        switch-item heap-new over push-back
s" USER_DEFINED"   switch-item heap-new over push-back
drop
s" BINNING" switch-vector heap-new 2dup swap push-back
s" 1X1"     switch-item heap-new over push-back
s" 2X2"     switch-item heap-new over push-back
s" 3X3"     switch-item heap-new over push-back
s" 4X4"     switch-item heap-new over push-back
drop
s" CCD_STATUS" light-vector heap-new 2dup swap push-back
s" ALERT"      light-item heap-new over push-back
s" EXP"        light-item heap-new over push-back
s" IDLE"       light-item heap-new over push-back
s" OK"         light-item heap-new over push-back
s" READ"       light-item heap-new over push-back
s" WAIT"       light-item heap-new over push-back
drop
s" CHIP"    switch-vector heap-new 2dup swap push-back
s" KAF400"  switch-item heap-new over push-back
s" KAF1600" switch-item heap-new over push-back
s" KAF3000" switch-item heap-new over push-back
s" KAF3200" switch-item heap-new over push-back
drop
s" CCD_TEMP"  number-vector heap-new 2dup swap push-back
s" COLD"      number-item heap-new over push-back
s" HOT"       number-item heap-new over push-back
s" VPELT"     number-item heap-new over push-back
drop
s" EXPOSURE" switch-vector heap-new 2dup swap push-back
s" START"    switch-item heap-new over push-back
s" STOP"     switch-item heap-new over push-back
drop
s" EXP_COUNTERS"  number-vector heap-new 2dup swap push-back
s" COUNT"         number-item heap-new over push-back
s" DELAY"         number-item heap-new over push-back
s" EXPTIME"       number-item heap-new over push-back
s" PROGRESS"      number-item heap-new over push-back
drop
s" EXP_LIMITS" number-vector heap-new 2dup swap push-back
s" COUNT"      number-item heap-new over push-back
s" DELAY"      number-item heap-new over push-back
s" EXPTIME"    number-item heap-new over push-back
drop
s" FITS_TEXT_DATA" text-vector heap-new 2dup swap push-back
s" COMMENT"        text-item heap-new over push-back
drop
s" IMAGETYP" switch-vector heap-new 2dup swap push-back
s" BIAS"     switch-item heap-new over push-back
s" DARK"     switch-item heap-new over push-back
s" FLAT"     switch-item heap-new over push-back
s" FOCUS"    switch-item heap-new over push-back
s" OBJECT"   switch-item heap-new over push-back
drop
s" CCD_CLEAN"  number-vector heap-new 2dup swap push-back
s" NUMBER"     number-item  heap-new over push-back
drop
s" PATTERN" switch-vector heap-new 2dup swap push-back
s" ON"      switch-item heap-new over push-back
s" OFF"     switch-item heap-new over push-back
drop
s" PHOT_PARS" number-vector heap-new 2dup swap push-back
s" GAIN"      number-item heap-new over push-back
s" RDNOISE"   number-item heap-new over push-back
drop
s" SHUTTER_DELAY"  number-vector heap-new 2dup swap push-back
s" DELAY"          number-item heap-new over push-back
drop
s" SHUTTER_LOGIC" switch-vector heap-new 2dup swap push-back
s" NEGATIVE"      switch-item heap-new over push-back
s" POSITIVE"      switch-item heap-new over push-back
drop
s" STORAGE" text-vector heap-new 2dup swap push-back
s" DIR"     text-item heap-new over push-back
s" PREFIX"  text-item heap-new over push-back
drop
s" STORAGE_FLIP"    switch-vector  heap-new 2dup swap push-back
s" FLIP_LEFT_RIGHT" switch-item heap-new over push-back
s" FLIP_UP_DOWN"    switch-item heap-new over push-back
drop
s" STORAGE_SERIES" switch-vector heap-new 2dup swap push-back
s" ALWAYS"         switch-item heap-new over push-back
s" NEVER"          switch-item heap-new over push-back
s" TWO_OR_MORE"    switch-item heap-new over push-back
drop
s" WCS_SEED" number-vector heap-new 2dup swap push-back
s" SCALE"    number-item heap-new over push-back
s" ROTA"      number-item heap-new over push-back
drop
s" EVENT_FIFO" text-vector heap-new 2dup swap push-back
s" DIR"         text-item heap-new over push-back
s" NAME"        text-item heap-new over push-back
drop
s" FOCUS_BUFFER" number-vector heap-new 2dup swap push-back
s" SIZE"         number-item heap-new over push-back
drop
s" CONFIGURATION" switch-vector heap-new 2dup swap push-back
s" SAVE"     switch-item heap-new over push-back

drop
indiserver push-back

\ s" CONFIGURATION" audine1 search-item drop print
\ audine1 send
