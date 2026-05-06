#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>    
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>



int create_socketIPTCP(char* ip,  struct sockaddr_in *address, unsigned PORT){

    address->sin_family = AF_INET;
    address->sin_port = htons(PORT);

    //inet_pton returns -1 for failure, 0 for invalid input and 1 for success
    if(inet_pton(AF_INET,ip, &(address->sin_addr.s_addr))<=0){
        perror("Presentation to network failed!");
        return -1;
    }
    
    int SOCKET_FD = socket(AF_INET, SOCK_STREAM, 0);
    if(SOCKET_FD<0){
        perror("Socket Creation failed!!\n");
        return -1;
    }else{
        printf("Socket Creation Successful!\n");
        return SOCKET_FD;
    }

}

int create_socketIPUDP(char* ip,  struct sockaddr_in *address, unsigned PORT){

    address->sin_family = AF_INET;
    address->sin_port = htons(PORT);

    if(inet_pton(AF_INET,ip, &(address->sin_addr.s_addr))<=0){
        perror("Presentation to network failed!");
        return -1;
    }
    
    int SOCKET_FD = socket(AF_INET, SOCK_DGRAM, 0);
    if(SOCKET_FD<0){
        perror("Socket Creation failed!!\n");
        return -1;
    }else{
        printf("Socket Creation Successful!\n");
        return SOCKET_FD;
    }

}