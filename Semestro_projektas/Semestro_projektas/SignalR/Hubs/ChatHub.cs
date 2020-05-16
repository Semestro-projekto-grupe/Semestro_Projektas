 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
 
using Microsoft.AspNetCore.SignalR;

namespace Semestro_projektas.SignalR.Hubs
{
    public class ChatHub : Hub
    {

         
        /*public void Send(string name, string message)
        {  
            Clients.All.AddNewMessageToPage(name, message);
        }*/

        public async Task Send(string name, string message) {
            await Clients.All.SendAsync("Send", name, message);
        }

        public async Task AddChannelUser(string newUser, int currentChannel)
        {
            await Clients.All.SendAsync("AddChannelUser", newUser, currentChannel);
        }

        public async Task AddChannelToUser(string userName)
        {
            await Clients.All.SendAsync("AddChannelToUser", userName);
        }
    }
}
