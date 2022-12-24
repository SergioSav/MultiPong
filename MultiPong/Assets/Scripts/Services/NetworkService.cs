using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Assets.Scripts.NetworkControllers
{
    public class NetworkService : NetworkBehaviour
    {
        private const string _BROADCAST_LISTEN_ADDRESS = "0.0.0.0";

        private GameSettings _settings;
        private GameModel _gameModel;

        public void Setup(GameSettings settings, GameModel gameModel)
        {
            _settings = settings;
            _gameModel = gameModel;
        }

        public void StartMatchAsHost()
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                NativeNetTools.LocalHost,
                _settings.ConnectionPort,
                _BROADCAST_LISTEN_ADDRESS);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnection;
            NetworkManager.Singleton.StartHost();
        }

        private void OnClientConnection(ulong clientId)
        {
            _gameModel.RegisterPlayer(clientId);
            if (_gameModel.RegisteredPlayerCount == _settings.PlayersCount)
                MatchmakingDoneServerRpc();
        }

        public void StartMatchAsClient(string ipAddress)
        {
            Debug.Log($"connect to {ipAddress}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                ipAddress,
                _settings.ConnectionPort);
            NetworkManager.Singleton.StartClient();
        }

        public void StopMatch()
        {
            NetworkManager.Singleton.Shutdown();
        }

#region RPCs
        [ServerRpc]
        private void MatchmakingDoneServerRpc()
        {
            MatchmakingDoneClientRpc();
        }

        [ClientRpc]
        private void MatchmakingDoneClientRpc()
        {
            _gameModel.AllPlayersReady();
        }

        [ServerRpc]
        public void MatchEndServerRpc(bool isHostWinner)
        {
            MatchEndClientRpc(isHostWinner);
        }

        [ClientRpc]
        private void MatchEndClientRpc(bool isHostWinner)
        {
            _gameModel.EndMatch(IsHost ? isHostWinner : !isHostWinner);
        }

        [ServerRpc]
        public void UpdateScoreServerRpc(int playerOneScore, int playerTwoScore)
        {
            UpdateScoreClientRpc(playerOneScore, playerTwoScore);
        }

        [ClientRpc]
        private void UpdateScoreClientRpc(int playerOneScore, int playerTwoScore)
        {
            if (!IsHost)
                _gameModel.UpdateScores(playerOneScore, playerTwoScore);
        }
#endregion

        public void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 300, 200));
            GUI.Label(new Rect(0, 0, 300, 200), NativeNetTools.GetLocalIPAddress(), 
                new GUIStyle { fontSize = 40, fontStyle = FontStyle.Bold });
            GUILayout.EndArea();
        }
    }
}
