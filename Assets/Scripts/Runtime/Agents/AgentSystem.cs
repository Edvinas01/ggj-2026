using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Utilities;
using RIEVES.GGJ2026.Runtime.Characters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RIEVES.GGJ2026
{
    internal sealed class AgentSystem : MonoSystem, IUpdateListener, IFixedUpdateListener
    {
        [SerializeField]
        private CharacterActor characterPrefab;

        [SerializeField]
        private List<CharacterData> characterSpawnPool = new();

        readonly Dictionary<CharacterState, int> desiredProportions = new();
        readonly Dictionary<InterestType, List<PointOfInterest>> points = new();
        readonly HashSet<CharacterActor> agents = new();

        int desiredPopulation = 20;

        private void InitializeProportions()
        {
            foreach (CharacterState state in Enum.GetValues(typeof(CharacterState)))
            {
                desiredProportions[state] = 0;
            }

            desiredProportions[CharacterState.Idling] = 6;
            desiredProportions[CharacterState.Dancing] = 7;
            desiredProportions[CharacterState.Watching] = 10;
        }

        private void Start()
        {
            InitializeProportions();

            foreach (var agent in agents)
            {
                CharacterState newState = GetRandomState(agent);
                agent.SetState(newState);
                agent.SetPatienceDuration(newState);
            }

            int currentPop = agents.Count;
            for (int i = currentPop; i < desiredPopulation; i++)
            {
                CharacterState neededState = GetMostNeededState();
                var desiredInterest = neededState switch
                {
                    CharacterState.Watching => InterestType.Watch,
                    CharacterState.Dancing => InterestType.Dancing,
                    CharacterState.Guarding => InterestType.Guard,
                    CharacterState.Hunting => InterestType.Patrol,
                    _ => InterestType.Watch
                };

                var spawnPoint = PickRandomWaypoint(desiredInterest);
                if (spawnPoint == null)
                {
                    i--;
                    continue;
                }

                Quaternion spawnRotation = spawnPoint.Facing
                    ? spawnPoint.transform.rotation
                    : Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                CharacterActor newAgent = CreateCharacter(neededState, spawnPoint.transform.position, spawnRotation);
                if (newAgent != null)
                {
                    newAgent.SetState(neededState);
                    newAgent.SetPatienceDuration(neededState);
                    agents.Add(newAgent);
                }
                else
                {
                    i--;
                }
            }
        }

        private CharacterState GetMostNeededState()
        {
            var states = (CharacterState[])Enum.GetValues(typeof(CharacterState));
            int[] currentCounts = new int[states.Length];
            foreach (var agent in agents)
                currentCounts[(int)agent.CurrentState]++;

            CharacterState bestState = CharacterState.Idling;
            int maxDemand = -1;

            foreach (var state in states)
            {
                int desired = desiredProportions.ContainsKey(state) ? desiredProportions[state] : 0;
                int demand = desired - currentCounts[(int)state];

                if (demand > maxDemand)
                {
                    maxDemand = demand;
                    bestState = state;
                }
            }

            return bestState;
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

        public CharacterActor CreateCharacter(CharacterState state, Vector3 position, Quaternion rotation)
        {
            var candidates = characterSpawnPool.Where(data =>
                data.ActivityPatience == null || data.ActivityPatience.Count == 0 ||
                data.ActivityPatience.Any(p => p.activity == state)
            ).ToList();

            if (candidates.Count > 0)
            {
                var characterData = candidates[Random.Range(0, candidates.Count)];
                var instance = Instantiate(characterPrefab, position, rotation);
                instance.Initialize(characterData);
                return instance;
            }

            Debug.LogError($"Could not find any CharacterData for {state} or any Generalists.");
            return null;
        }

        public CharacterState GetRandomState(CharacterActor agent)
        {
            var states = (CharacterState[])Enum.GetValues(typeof(CharacterState));
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
            if (agentPrefs == null || agentPrefs.Count == 0)
            {
                return CharacterState.Idling;
            }

            if (agentPrefs.Exists(p => p.activity == CharacterState.Idling))
            {
                return CharacterState.Idling;
            }

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
