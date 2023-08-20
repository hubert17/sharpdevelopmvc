using Microsoft.AspNet.SignalR;

public class ChatHub : Hub
{
	public void Send(string name, string message)
	{
		// Broadcast the message to all clients
		Clients.All.broadcastMessage(name, message);
	}
}