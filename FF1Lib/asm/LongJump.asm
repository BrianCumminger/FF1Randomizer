;;;;;;;;;General Purpose Long Jump with modified SwapPRG routine
; Uses 7 bytes of zero-page temp ram, $19-$1F
; Called from a table with the following structure:
;		JSR LongJump
;		.byte A, B, C
; where A B make up the address to jump to and C is the bank to jump to
; in that bank of the routine. Table entries need not be contiguous.
; Combined with modifications to SwapPRG, we automatically swap
; back to the previously used bank.
; The previous bank to swap back to is stored at $60FC, and is
; saved before any swapping takes place

STA $1A ;save A and Y
TYA
STA $1B
PLA ;pop return addr from stack, put in zero page
STA $1C
PLA
STA $1D
LDY #$1
LDA ($1C), Y ;get jump address, write to temp+$E
STA $1E
INY
LDA ($1C), Y
STA $1F
INY
LDA $60FC ;get bank currently loaded and save it
STA $19
LDA ($1C), Y ;get jump bank
JSR $FE03 ;SwapPRG_L
LDA #$D7 ;write post-jump address to stack to masquerade as a JSR,  D7F5
PHA
LDA #$F5
PHA
LDA $1A ;restore registers
LDY $1B
JMP ($001E) ;actual function jump
; <----------  post-jump address mentioned above
STA $1E
LDA $19 ;get bank to jump back to
JSR $FE03 ;SwapPRG_L
LDA $1E
RTS
;total size: 56 bytes
