#include <stdio.h>
#include <stdlib.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>



int main(){
char *ip = "127.0.0.1";

struct sockaddr_in addr_server;
addr_server.sin_family = AF_INET;  // IPv4
addr_server.sin_addr.s_addr = inet_addr(ip); 
addr_server.sin_port = htons(5100);

int socketFD = socket(AF_INET, SOCK_STREAM, 0);

bind(socketFD, (struct sockaddr *)&addr_server, sizeof(addr_server));
listen(socketFD, 10);
struct sockaddr_in client_addr;
size_t size_client = sizeof(client_addr);

int new_socket = accept(socketFD, 
                        (struct sockaddr *)&client_addr, 
                        (socklen_t *)&size_client);

char buffer[1024];

memset(buffer, 0, sizeof(buffer));

size_t valread = recv(new_socket, buffer, 1024, 0);
close(new_socket);
shutdown(socketFD, SHUT_RDWR);
perror("not recieving!!");

printf("%s \n", buffer);

return 0;
}

