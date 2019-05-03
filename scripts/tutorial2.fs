((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/scripts/tutorial2.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ This file covers example 11 in the tutorial

foreground logging on
IndiServer: indiserver localhost 7624
include xephem.fs       \ file where the above definitions live
discover

Task AutoFocus-Task
Assign Autofocuser To-Start AutoFocus-Task

switch-on telescope
My-Location
Goto-M31

Red Filter
30.0 sec. 2x2 3 Photos
  
Visible Filter
30.0 sec. 2x2 3 Photos

Blue Filter
60.0 sec. 2x2 3 Photos

Clear Filter
60.0 sec. 1x1 3 Photos
