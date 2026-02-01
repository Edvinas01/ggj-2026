using UnityEngine;
using CHARK.GameManagement;

namespace RIEVES.GGJ2026
{
    public class PointOfInterest : MonoBehaviour
    {
        public InterestType InterestType = InterestType.Watch;
        public bool Facing = false;
        public float StayWithinRange = 3f;
        public float MoveWithinRange = 0.1f;
    }
}
