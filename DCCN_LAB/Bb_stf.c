#include <stdio.h>
#include <stdint.h>

void transmit_bit_stuffed(const int *payload, int length) {
    int consecutive_ones = 0;

    printf("Transmitted Bit Stream: ");
    
    for (int i = 0; i < length; i++) {
        int current_bit = payload[i];
        printf("%d", current_bit);

        if (current_bit == 1) {
            consecutive_ones++;
            // Stuff a '0' after five consecutive '1's
            if (consecutive_ones == 5) {
                printf("\033[1;31m0\033[0m"); // Red '0' to highlight the stuffed bit
                consecutive_ones = 0; 
            }
        } else {
            consecutive_ones = 0;
        }
    }
    printf("\n");
}


// Standard Point-to-Point Protocol (PPP) control characters
#define FLAG_BYTE 0x7E
#define ESC_BYTE  0x7D

void transmit_byte_stuffed(const uint8_t *payload, int length) {
    printf("Transmitted Byte Stream (Hex): ");
    
    for (int i = 0; i < length; i++) {
        uint8_t current_byte = payload[i];

        // If the payload byte is a control character, escape it
        if (current_byte == FLAG_BYTE || current_byte == ESC_BYTE) {
            printf("\033[1;31m%02X\033[0m ", ESC_BYTE); // Red ESC to highlight stuffed byte
        }
        
        printf("%02X ", current_byte);
    }
    printf("\n");
}