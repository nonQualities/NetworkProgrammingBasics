#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>

#define PORT 5100

int main(){

    char* ip = "127.0.0.1";
    struct sockaddr_in addr;
    addr.sin_addr.s_addr = htonl(ip);
    addr.sin_family = AF_INET;
    addr.sin_port = htons(PORT);

    int SERVER_FD = socket(AF_INET, SOCK_STREAM, 0);
    int conn_stts = connect(SERVER_FD, (struct sockaddr*) &addr, sizeof(addr));

    if(conn_stts < 0){
        perror("Connection Failed");
        exit(EXIT_FAILURE);
    }

    char *msg = NULL;
    size_t msg_size = 0;
    printf("Enter a string to be sent: ");
    size_t read = getline(&msg,&msg_size, stdin);
    send(SERVER_FD, msg, msg_size, 0);


    return 0;

}