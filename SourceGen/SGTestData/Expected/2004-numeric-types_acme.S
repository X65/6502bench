;Project file was edited to get all big-endian data types, and to have an
;incorrect .junk alignment directive.
        !cpu    6502
*       =       $1000
        rts

        !byte   $11
        !word   $1122
        !24     $112233
        !32     $11223344
        !byte   $11
        !byte   $11,$22
        !byte   $11,$22,$33
        !byte   $11,$22,$33,$44
        !fill   2,$00
        !byte   $80
        !fill   3,$00
        !byte   $80
        !fill   4,$00
        !byte   $80
        !fill   5,$00
        !byte   $80
        !fill   256,$00
        !byte   $80
        !fill   257,$cc
        !hex    11
        !byte   $80
        !hex    11223344556677889900
        !byte   $80
LABEL   !hex    00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff ;comment
        !hex    00112233445566778899aabbccddeeffffeeddccbbaa99887766554433221100
        !byte   $80
        !align  255,0,$aa
        !byte   $81
        !align  63,0,$00
        !byte   $81
        !align  31,0,$ab
        !hex    0000000000000001
        !byte   $81
        !hex    1000000000000000
        !byte   $81
        !fill   2,$dd      ;incorrect alignment
        !align  255,0,$00
