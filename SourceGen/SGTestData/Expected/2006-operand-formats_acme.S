;Project file was edited for some ASCII operands.
        !cpu    65816
*       =       $1000
        !as
        !rs
        clc
        xce
        sep     #$30
        lda     $01
        lda     $0102
        lda+3   $010203
        lda     1
        lda     258
        lda+3   66051
        lda     %00000001
        lda     %0000000100000010
        lda+3   %000000010000001000000011
        bra     skipdata

        !byte   $01
        !word   $0201
        !24     $030201
        !32     $04030201
        !byte   1
        !word   513
        !24     197121
        !32     67305985
        !byte   %00000001
        !word   %0000001000000001
        !24     %000000110000001000000001
        !32     %00000100000000110000001000000001

skipdata lda    #'h'
        lda     'h'
        lda+2   'h'
        lda+3   'h'
        lda     #$1f
        lda     #' '
        lda     #'"'
        lda     #$27
        lda     #'~'
        lda     #$7f
        lda     #$80
        lda     #$9f
        lda     #' ' | $80
        lda     #'"' | $80
        lda     #$a7
        lda     #'~' | $80
        lda     #$ff
        rep     #'0'
        !al
        !rl
        lda     #'h'
        lda     #'H' | $80
        lda     #$6868
        rts

more_ascii !byte 'h'
        !byte   $80
        !word   $6868
        !byte   $80
        !word   skipdata
        !24     skipdata
        !byte   $10,$3f
        !byte   <more_ascii
        !byte   >more_ascii
        !word   more_ascii
        !24     more_ascii
        !byte   $10,$70
