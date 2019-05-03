((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/interfaces.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############
\ *> interfaces
\ #############

\ ==================
\ *N Core interfaces
\ ==================

\ *P Interfaces defined for the *\f{INDIScript} core.

\ *[
interface
   selector connect			\ i*x this -- j*x
   selector connected?			\ i*x this -- j*x
   selector disconnect			\ i*x this -- j*x
end-interface i-connection


interface
   selector lock			\ i*x this -- j*x
   selector unlock			\ i*x this -- j*x
end-interface i-latch

interface
   selector set				\ i*x this -- j*x
   selector get				\ i*x this -- j*x
end-interface i-property

interface
   selector push-front			\ i*x this -- j*x
   selector push-back			\ i*x this -- j*x
   selector pop-front			\ i*x this -- j*x
   selector pop-back			\ i*x this -- j*x
end-interface i-list

interface
   selector next-item			\ i*x this -- j*x
end-interface i-iterator
\ *]

\ ======
\ *> ###
\ ======
