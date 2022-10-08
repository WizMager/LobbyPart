using DarkRift.Client.Unity;
using UnityEngine;
using Zenject;

public class ClientInstaller : MonoInstaller
{
    [SerializeField] private GameObject unityClientPrefab;
    public override void InstallBindings()
    {
        var clientInstance = Container.InstantiatePrefabForComponent<UnityClient>(unityClientPrefab);
        
        Container.BindInstance(clientInstance).AsSingle().NonLazy();
    }
}