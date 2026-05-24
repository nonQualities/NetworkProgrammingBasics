#include <stdio.h>
#include <stdlib.h>
#include <stdlib.h>
#include <unistd.h>
#include <time.h>


enum STATE{
IDLE,
TRANSMITTING,
BACKING_OFF,
};


int main(){

    int ticks =0, iterations = 0;
    enum STATE state = IDLE;

    printf("Enter the number of Ticks: ");
    scanf( "%d", &ticks);

    int total_ticks = 0;
    int ticks_to_send = ticks;

    while(ticks != 0){
        printf("%d", iterations++);
        if(state == IDLE){
           printf("\033[32m[INFO] : Transmitting: TICK %d \033[0m \n", ticks);
           int random1 = rand()%5;
           int random2 =rand()%10;
        //    printf("r1 = %d,\t r2 = %d\n", random1, random2);
           if(random1 < random2) state = TRANSMITTING;
           else state = IDLE;
            // state = TRANSMITTING;
            ticks --; total_ticks++;
        }
        else if (state == TRANSMITTING)
        {
           printf("\033[31m[INFO] : COLLUSION!!: BACKING_OFF: TICK %d.\033[0m\n",ticks);
           sleep(1);
           int random1 = rand()%5;
           int random2 =rand()%10;
        //    printf("r1 = %d,\t r2 = %d\n", random1, random2);
           if(random1 < random2) state = BACKING_OFF;
           ticks--;
        }
        else
        {
            state = IDLE;
        }
    }

    printf("DETAILS: \n\n");
    printf(" Total Number of Packets: %d,\n Total Packets sent: %d", ticks_to_send, total_ticks);
    return 0;
}