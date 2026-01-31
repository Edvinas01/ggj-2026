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

            desiredProportions[CharacterState.Idling] = 1;
            desiredProportions[CharacterState.Dancing] = 1;
            desiredProportions[CharacterState.Watching] = 2;
            desiredProportions[CharacterState.Guarding] = 3;
            desiredProportions[CharacterState.Hunting] = 1;
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
            var states = (CharacterState[])System.Enum.GetValues(typeof(CharacterState));
            int stateCount = states.Length;

            // 1. Calculate current population counts
            int[] populationCounts = new int[stateCount];
            foreach (var a in agents)
            {
                populationCounts[(int)a.CurrentState]++;
            }

            var agentPrefs = agent.CharacterData.ActivityPatience;
            float[] finalWeights = new float[stateCount];
            float totalWeight = 0;

            // A "Generalist" has no specific activity definitions and can do anything
            bool isGeneralist = agentPrefs == null || agentPrefs.Count == 0;

            for (int i = 0; i < stateCount; i++)
            {
                CharacterState state = states[i];

                // Calculate Global Demand: (Desired - Current)
                int desired = desiredProportions.ContainsKey(state) ? desiredProportions[state] : 0;
                int demand = Mathf.Max(0, desired - populationCounts[i]);

                if (isGeneralist)
                {
                    // Generalists follow pure demand
                    finalWeights[i] = demand;
                }
                else
                {
                    // Specialists only weigh activities they have defined
                    // Using Find matches the CharacterState directly now
                    int prefIndex = agentPrefs.FindIndex(p => p.activity == state);

                    if (prefIndex != -1)
                    {
                        // Weight = Demand * Max Patience
                        finalWeights[i] = demand * agentPrefs[prefIndex].maxTime;
                    }
                    else
                    {
                        finalWeights[i] = 0;
                    }
                }

                totalWeight += finalWeights[i];
            }

            // 2. Selection & Fallback Logic
            if (totalWeight <= 0)
            {
                return HandleDefaultState(agent);
            }

            // Weighted Random Roll
            float randomValue = Random.Range(0, totalWeight);
            float cursor = 0;
            for (int i = 0; i < stateCount; i++)
            {
                cursor += finalWeights[i];
                if (randomValue <= cursor) return states[i];
            }

            return HandleDefaultState(agent);
        }

        private CharacterState HandleDefaultState(CharacterActor agent)
        {
            var agentPrefs = agent.CharacterData.ActivityPatience;

            // Rule: If they can Idle, they should default to Idle.
            // Generalists (empty list) can always idle.
            if (agentPrefs.Count == 0)
            {
                return CharacterState.Idling;
            }

            // Specialists can only idle if they have it in their list.
            bool canIdle = agentPrefs.Exists(p => p.activity == CharacterState.Idling);
            if (canIdle)
            {
                return CharacterState.Idling;
            }

            // If they can't idle, pick their first defined activity so they aren't stuck.
            return agentPrefs[0].activity;
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
