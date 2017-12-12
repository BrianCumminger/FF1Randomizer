;;;;;;;;;General Purpose Long Jump
; Uses 6 bytes of zero-page temp ram, $1A-$1F
; Called from a table with the following structure:
;		JSR LongJump
;		.byte A, B, C
; where A B make up the address to jump to and C is the bank to jump to
; in that bank of the routine. Table entries need not be contiguous.
; Called function should write the bank to return to into $1F

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
LDA ($1C), Y ;get jump bank
JSR $FE03 ;SwapPRG_L
LDA #$BB ;write post-jump address to stack to masquerade as a JSR, will be start+$29 (or maybe start+$28)
PHA
LDA #$BB
PHA
LDA $1A ;restore registers
LDY $1B
JMP ($001D) ;actual function jump
; <----------  post-jump address mentioned above
STA $1E
LDA $1F ;get bank to jump back to
JSR $FE03 ;SwapPRG_L
LDA $1E
RTS
;total size: 57 bytes
