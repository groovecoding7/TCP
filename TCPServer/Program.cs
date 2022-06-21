// See https://aka.ms/new-console-template for more information

using TCPServer;

TcpServerHandler tcpServer = new TcpServerHandler(null, 0);

tcpServer.Run();
