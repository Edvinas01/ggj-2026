using UnityEngine;
using CHARK.GameManagement;
using UnityEngine.Events;

namespace RIEVES.GGJ2026
{
    [System.Serializable]
    public class StateChangeTriggers
    {
        public CharacterState State;
        public UnityEvent OnStateStarting;
        public float StartDelay;
        public UnityEvent OnStateEnding;
        public float EndDelay;
    }
}
