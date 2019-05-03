expose-module objects
cr
.( INTERFACE I-TEST ) cr
i-test .s drop
i-test interface-map 2@ .s 2drop
i-test interface-map-offset @ .s drop
i-test interface-offset @ .s drop
.( CLASS PER1 ) cr
per1 .s drop
per1 interface-map 2@ .s 2drop
per1 interface-map-offset @ .s drop
per1 interface-offset @ .s drop
.( COMPROBANDO ) cr
per1 class->map 4 - @ . i-test . i-test2 .