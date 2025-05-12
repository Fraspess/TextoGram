using Server;

internal class Program
{
    private static void Main(string[] args)
    {
        ChatServer server = new();
        server.StartClient();
    }
}