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

        public async Task Send(string name, string message, int channel) {
            await Clients.All.SendAsync("Send", name, message, channel);
        }

        public async Task AddChannelUser(int currentChannel)
        {
            await Clients.All.SendAsync("AddChannelUser", currentChannel);
        }

        public async Task AddChannelToUser(string userName)
        {
            await Clients.All.SendAsync("AddChannelToUser", userName);
        }

        public async Task KickChannelUser(string userName, int channel)
        {
            await Clients.All.SendAsync("KickChannelUser", userName, channel);
        }


        public async Task EditMessage(string message, string contId, int channel)
        {
            await Clients.All.SendAsync("EditMessage", message, contId, channel);
        }

        public async Task DeleteMessage(int id, int channel)
        {
            await Clients.All.SendAsync("DeleteMessage", id, channel);
        }

    }
}
