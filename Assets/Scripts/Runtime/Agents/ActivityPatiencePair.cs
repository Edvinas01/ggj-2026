using UnityEngine;
using CHARK.GameManagement;
using UnityEngine.Events;
using System.Collections.Generic;

namespace RIEVES.GGJ2026
{
    [System.Serializable]
    public struct ActivityPatiencePair
    {
        public CharacterState activity;
        public int minTime;
        public int maxTime;
    }
}
