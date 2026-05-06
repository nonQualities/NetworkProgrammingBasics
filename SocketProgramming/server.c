#include <stdbool.h>
#include<stdio.h>
#include <string.h>
#include "socketUtil.h"


int main(){
    unsigned PORT = 49153;
    char *ip = "0.0.0.0";
    struct sockaddr_in address;
    int socket_client = create_socketIPTCP(ip, &address, PORT);
    if(bind(socket_client, (struct sockaddr *)&address, sizeof(address))<0){
        perror("Binding Failed!!\n");
        return -1;} else{ printf("Binding Successful!\n"); }

    if(listen(socket_client, 10)<0){
        perror("Listening Failed!!\n");
        return -1;} else{ printf("Listening Successful!\n"); }

    struct sockaddr_in client_address;
    int addrlen = sizeof(client_address);

    int new_socket = 
    accept(socket_client, (struct sockaddr *)&client_address, (socklen_t*)&addrlen);

    if(new_socket<0){
        perror("Accepting Failed!!\n");
        return -1;
    }else{
        printf("Accepting Successful!\n");
    }


    char buffer[1024];
    while(true){
        memset(buffer, 0, sizeof(buffer));
        size_t valread = recv(new_socket, buffer, 1024, 0);
        if(valread<=0){
            close(new_socket);
            shutdown(socket_client, SHUT_RDWR);
            perror("Not Recieving!! Socket Close\n");
            break;
        }
        printf("%s\n", buffer);
    }
    return 0;

    }