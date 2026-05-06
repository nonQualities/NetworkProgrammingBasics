#include<stdio.h>
#include <string.h>
#include <stdbool.h>
#include "socketUtil.h"


int main(){
    unsigned PORT = 2000;
    char *ip = "49.37.100.158";
    struct sockaddr_in address;
    int socket_client = create_socketIPTCP(ip, &address, PORT);
    int connection_stts = connect(socket_client, (struct sockaddr *)&address, sizeof(address));
    if(connection_stts<0){
        perror("Connection Failed!!\n");
        return -1;
    }else{
        printf("Connection Successful!\n"); }
    
    char* message = NULL;
    size_t message_size = 0;
    printf("type 'exit' to quit OR Enter the message to send:\n");

    while (true)
    {
        size_t read = getline(&message, &message_size, stdin);
        if (read == -1 || strcmp(message, "exit\n") == 0) {
            perror("Error reading input");
            free(message);
            break;
        }
        send(socket_client, message, strlen(message), 0);
    }

    return 0;

}