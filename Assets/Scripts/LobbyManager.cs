using System;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;
using Zenject;

public class LobbyManager : MonoBehaviour
{
        [SerializeField] private GameObject roomButtonPrefab;
        private UnityClient _client;

        [Inject]
        private void Construct(UnityClient client)
        {
            _client = client;
        }

        private void Start()
        {
            _client.Client.MessageReceived += MessageReceiveHandler;
        }

        private void MessageReceiveHandler(object sender, MessageReceivedEventArgs e)
        {
            using var message = e.GetMessage();
            switch (message.Tag)
            {
                case Tags.EnterRoomLobby:
                    EnterRoomLobby(e);
                    break;
            }
        }

        private void EnterRoomLobby(MessageReceivedEventArgs e)
        {
            
        }
}