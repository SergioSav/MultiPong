using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Assets.Scripts.NetworkControllers
{
    public class NetworkService : MonoBehaviour
    {
        public void StartMatchAsHost()
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "127.0.0.1",
                1234,
                "0.0.0.0");
            NetworkManager.Singleton.StartHost();
        }

        public void StartMatchAsClient(string ipAddress)
        {
            Debug.Log($"connect to {ipAddress}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                ipAddress,
                1234);
            NetworkManager.Singleton.StartClient();
        }
    }
}
