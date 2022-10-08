using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LoginLobby : MonoBehaviour
{
        [SerializeField] private TMP_InputField nicknameInputField;
        [SerializeField] private Button loginButton;
        private UnityClient _client;
        private bool _receiveCheckNickname;
        private bool _nicknameCheckResult;

        [Inject]
        private void Construct(UnityClient client)
        {
                _client = client;
        }

        private void Start()
        {
                loginButton.onClick.AddListener(OnLoginClickHandler);
                _client.Client.MessageReceived += MessageReceiveHandler;    
        }

        private void Update()
        {
                if (!_receiveCheckNickname) return;
                if (_nicknameCheckResult)
                {
                        SceneManager.LoadScene(1);
                        _receiveCheckNickname = false;
                }
                else
                {
                        loginButton.interactable = true;
                        nicknameInputField.interactable = true;
                        nicknameInputField.text = "Login is not success!";
                        _receiveCheckNickname = false;
                } 
        }

        private void MessageReceiveHandler(object sender, MessageReceivedEventArgs e)
        {
                using var message = e.GetMessage();
                switch (message.Tag)
                {
                     case   Tags.PlayerLoginSuccessTag:
                             CheckSuccessLoginHandler(message);
                             break;
                }
        }

        private void CheckSuccessLoginHandler(Message message)
        {
                using var reader = message.GetReader();
                var loginResult = reader.ReadBoolean();
                _nicknameCheckResult = loginResult;
                _receiveCheckNickname = true;
        }

        private void OnLoginClickHandler()
        {
                if (string.IsNullOrWhiteSpace(nicknameInputField.text)) return;
                var nickname = nicknameInputField.text;
                using var writer = DarkRiftWriter.Create();
                writer.Write(nickname);
                using var message = Message.Create(Tags.PlayerLoginTag, writer);
                _client.SendMessage(message, SendMode.Reliable);
                loginButton.interactable = false;
                nicknameInputField.text = "";
                nicknameInputField.interactable = false;
        }

        private void OnDestroy()
        {
                loginButton.onClick.RemoveListener(OnLoginClickHandler); 
                _client.Client.MessageReceived -= MessageReceiveHandler;
        }
}