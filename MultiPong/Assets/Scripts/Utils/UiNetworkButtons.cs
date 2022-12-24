using Unity.Netcode;
using UnityEngine;

public class UiNetworkButtons : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20,20, 200,200));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start HOST")) 
                NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Start CLIENT")) 
                NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Start SERVER")) 
                NetworkManager.Singleton.StartServer();
        }
        GUILayout.EndArea();
    }
}
