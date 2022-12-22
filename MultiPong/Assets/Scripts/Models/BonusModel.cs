using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class BonusModel
    {
        public int Id;
        public string Name;
        public Material IconMaterial;
        public BonusType BonusType;
        public float EffectValue;
        public float EffectDuration;
    }
}