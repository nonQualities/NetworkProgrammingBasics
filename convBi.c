

#include <stdio.h>


void printBinary(unsigned int n) {
    int i;
    int bits = sizeof(unsigned int) * 8; 
    
    for (i = bits - 1; i >= 0; i--) {
        
        if ((n >> i) & 1) {
            printf("1");
        } else {
            printf("0");
        }
    }
    printf(" ");
}

int main() {
    unsigned int number;

    printf("Enter a decimal number: ");
    if (scanf("%u", &number) != 1) {
        printf("Invalid input!");
        return 1;
    }

    printf("Binary representation: ");
    printBinary(number);

    return 0;
}