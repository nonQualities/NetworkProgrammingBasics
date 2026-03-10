#include <stdio.h>

int main(){
    int N, count = 0;
    printf("Enter a number:");
    scanf("%d", &N);
    while(N){
        count += N & 1;
        N = N >> 1;
    }
    printf("The bit count for the number is : %d", count);
    return 0;
}