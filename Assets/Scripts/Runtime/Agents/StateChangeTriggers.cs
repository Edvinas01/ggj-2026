using UnityEngine;
using CHARK.GameManagement;
using UnityEngine.Events;

namespace RIEVES.GGJ2026
{
    [System.Serializable]
    public class StateChangeTriggers
    {
        public CharacterState State;
        public UnityAction OnStateStarting;
        public float StartDelay;
        public UnityAction OnStateEnding;
        public float EndDelay;
    }
}
