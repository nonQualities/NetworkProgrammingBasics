#include <stdio.h>
#include <sys/socket.h>
#include <netinet/in.h>

int main(){
    int sockfd;
    sockfd = socket(AF_INET, SOCK_STREAM,0);
    printf("Socket Server Created");
    return 0;
}