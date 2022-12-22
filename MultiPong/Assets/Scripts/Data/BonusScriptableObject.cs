using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = nameof(BonusScriptableObject), menuName = "Data/Create SO/Bonuses settings", order = 2)]
    public class BonusScriptableObject : ScriptableObject
    {
        public List<BonusModel> BonusModels;
    }
}
