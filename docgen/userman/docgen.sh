#!/bin/sh
# 
# $Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
# $Revision: 522 $
# $Author: astrorafael $
# $HeadURL: file:///home/desa/repos/ntoscript/trunk/docgen/userman/docgen.sh $
#
# 2008 By Rafael Gonzalez Fuentetaja
# This file is placed in the public domain. NO WARRANTY.



userman="userman"
refman="refman"
doc="../../doc"

if test $# -ne 1
then
    echo "Usage: $0 ( $userman | $refman )"
    exit 0
fi

if test $1 != "$userman" -a $1 != "$refman" 
then
    echo "Incorrect argument: $1"
    echo "Usage: $0 ( $userman | $refman )"
    exit 0
fi

mode=$1

if test ! -d $doc
then
    mkdir $doc || exit
fi


# generate .tex and html files by VFX's DocGen
vfxlin create $mode include docgen.dgs

# Rerun twice to generate proper TOC

# To generate an index texify -p manual.tex must be run instead of pdftex
# texify -p manual.tex
# texify -p manual.tex


pdftex userman.tex 
pdftex userman.tex

echo "copying manuals to the doc folder"
mv userman.pdf $doc
mv userman.log log.txt


echo "cleaning again temporary files"
rm userman.*
rm *.tex


