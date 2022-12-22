using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = nameof(GameSettingsScriptableObject), menuName = "Data/Create SO/Game settings", order = 1)]
    public class GameSettingsScriptableObject : ScriptableObject
    {
        public GameSettings GameSettings;
    }
}
