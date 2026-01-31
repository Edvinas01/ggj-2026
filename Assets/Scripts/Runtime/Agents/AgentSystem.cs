using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Utilities;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RIEVES.GGJ2026
{
    internal sealed class AgentSystem : MonoSystem, IUpdateListener, IFixedUpdateListener
    {
        readonly Dictionary<CharacterState, int> desiredProportions = new();
        readonly Dictionary<InterestType, List<PointOfInterest>> points = new();
        readonly List<CharacterActor> agents = new();

        private void Start()
        {
            var states = System.Enum.GetValues(typeof(CharacterState));
            foreach (CharacterState state in states)
            {
                desiredProportions[state] = 0;
            }

            desiredProportions[CharacterState.Idle] = 1;
            desiredProportions[CharacterState.Dancing] = 1;
            desiredProportions[CharacterState.GuardingPointOfInterest] = 3;
        }

        public PointOfInterest PickRandomWaypoint(InterestType interestType)
        {
            if (points.Count == 0)
            {
                Debug.LogWarning("No points of interest available.");
                return null;
            }

            if (!points.ContainsKey(interestType) || points[interestType].Count == 0)
            {
                Debug.LogWarning($"No points of interest available for interest type: {interestType}");
                return null;
            }

            var unoccupiedPoints = points[interestType]
                .Where(p => !agents.Any(a => a.CurrentTarget == p))
                .ToList();

            if (unoccupiedPoints.Count == 0)
            {
                Debug.LogWarning($"No unoccupied points of interest available for interest type: {interestType}");
                return null;
            }

            return unoccupiedPoints[Random.Range(0, unoccupiedPoints.Count)];
        }

        public CharacterState GetRandomState(CharacterActor agent)
        {
            var states = System.Enum.GetValues(typeof(CharacterState));
            int[] stateCounts = new int[states.Length];
            foreach (var a in agents)
            {
                stateCounts[(int)a.CurrentState]++;
            }

            int[] desiredCounts = new int[stateCounts.Length];
            for (int i = 0; i < desiredCounts.Length; i++)
            {
                desiredCounts[i] = desiredProportions.ContainsKey((CharacterState)i)
                    ? desiredProportions[(CharacterState)i]
                    : 0;
            }

            float[] weights = new float[desiredCounts.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                var count = desiredCounts[i] - stateCounts[i];
                weights[i] = count > 0 ? count * 1.5f : 0;
            }

            float totalWeight = weights.Sum();
            if (totalWeight <= 0)
                return CharacterState.Idle;

            weights.Shuffle();
            float randomValue = Random.Range(0, totalWeight);
            for (int i = 0; i < weights.Length; i++)
            {
                if (randomValue < weights[i])
                {
                    return (CharacterState)i;
                }
                randomValue -= weights[i];
            }

            return CharacterState.Idle;
        }

        public void AddPointOfInterest(PointOfInterest poi)
        {
            if (!points.ContainsKey(poi.InterestType))
            {
                points[poi.InterestType] = new List<PointOfInterest> { poi };
            }
            else
            {
                points[poi.InterestType].Add(poi);
            }
        }

        public void RemovePointOfInterest(PointOfInterest poi)
        {
            if (points.ContainsKey(poi.InterestType))
                points[poi.InterestType].Remove(poi);
        }

        public void AddAgent(CharacterActor agent)
        {
            agents.Add(agent);
        }

        public void RemoveAgent(CharacterActor agent)
        {
            agents.Remove(agent);
        }

        public void OnUpdated(float deltaTime)
        {
        }

        public void OnFixedUpdated(float deltaTime)
        {
        }
    }
}
