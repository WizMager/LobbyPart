using System;
using System.Collections.Generic;
using DarkRift;
using DarkRift.Server;

namespace LobbyPlugin
{
    public class LobbyManager : Plugin
    {
        public override Version Version => new Version(1, 0, 0);
        public override bool ThreadSafe => false;
        private readonly Dictionary<ushort, IClient> _clients = new Dictionary<ushort, IClient>();

        public LobbyManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += ClientConnectedHandler;
            ClientManager.ClientDisconnected += ClientDisconnectedHandler;
        }

        private void ClientConnectedHandler(object sender, ClientConnectedEventArgs e)
        {
            var client = e.Client;
            _clients.Add(client.ID, client);
            client.MessageReceived += MessageReceivedHandler;
        }

        private void MessageReceivedHandler(object sender, MessageReceivedEventArgs e)
        {
            using(var message = e.GetMessage())
            {
                switch (message.Tag)
                {
                    case Tags.PlayerLoginTag:
                        CheckPlayerNickname(e);
                        break;
                }
            }
        }

        private void CheckPlayerNickname(MessageReceivedEventArgs e)
        {
            using (var message = e.GetMessage())
            {
                var client = e.Client;
                using (var reader = message.GetReader())
                {
                    var nickname = reader.ReadString();
                    bool isSuccess;
                    if (nickname.Length >= 3 && nickname.Length <= 12)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                    }
                    using (var writer = DarkRiftWriter.Create())
                    {
                        writer.Write(isSuccess);
                        using (var successMessage = Message.Create(Tags.PlayerLoginSuccessTag, writer))
                        {
                            client.SendMessage(successMessage, SendMode.Reliable);
                        }
                    }
                } 
            }
        }

        private void ClientDisconnectedHandler(object sender, ClientDisconnectedEventArgs e)
        {
            var client = e.Client;
            _clients.Remove(e.Client.ID);
            client.MessageReceived -= MessageReceivedHandler;
        }
    }
}