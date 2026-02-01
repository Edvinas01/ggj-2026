using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Utilities;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Heat;
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

        private readonly Dictionary<CharacterState, int> desiredProportions = new();
        private readonly Dictionary<InterestType, List<PointOfInterest>> points = new();
        private readonly List<SpawnPoint> spawnPoints = new();
        public readonly HashSet<CharacterActor> agents = new HashSet<CharacterActor>();

        private HeatSystem heatSystem;
        private int initialBasePopulation = 20;

        private void Awake()
        {
            heatSystem = GameManager.GetSystem<HeatSystem>();
        }

        private void Start()
        {
            foreach (CharacterState state in Enum.GetValues(typeof(CharacterState)))
                desiredProportions[state] = 0;

            desiredProportions[CharacterState.Idling] = 6;
            desiredProportions[CharacterState.Dancing] = 7;
            desiredProportions[CharacterState.Watching] = 10;

            var oldActors = agents.ToList();
            var currentCount = agents.Count;
            for (; agents.Count < initialBasePopulation;)
                SpawnNewAgent(true);

            // TODO: We should set the target permenantyly instead.
            foreach (var actor in agents)
            {
                if (!oldActors.Contains(actor))
                    continue;

                actor.CurrentTarget = null;
            }

        }

        public void OnUpdated(float deltaTime)
        {
            float heatProgress = Mathf.InverseLerp(1f, 2.2f, heatSystem.CurrentHeat);
            desiredProportions[CharacterState.Guarding] = Mathf.RoundToInt(Mathf.Lerp(0, 6, heatProgress));
            desiredProportions[CharacterState.Hunting] = Mathf.RoundToInt(Mathf.Lerp(0, 4, heatProgress));

            int currentGoal = initialBasePopulation + Mathf.RoundToInt(Mathf.Lerp(0, 5, heatProgress));
            if (agents.Count < currentGoal)
                SpawnNewAgent(false);
        }

        private InterestType MapStateToInterest(CharacterState state)
        {
            return state switch
            {
                CharacterState.Watching => InterestType.Watch,
                CharacterState.Dancing => InterestType.Dancing,
                CharacterState.Guarding => InterestType.Guard,
                CharacterState.Hunting => InterestType.Patrol,
                _ => InterestType.Watch
            };
        }

        private void SpawnNewAgent(bool init)
        {
            CharacterState neededState = GetMostNeededState();
            InterestType interest = MapStateToInterest(neededState);

            Vector3 position;
            PointOfInterest poi = null;
            if (init)
            {
                poi = PickRandomWaypoint(interest);
                if (poi == null)
                    return;

                position = poi.transform.position;
            }
            else
            {
                if (spawnPoints.Count == 0)
                    return;

                var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                position = spawnPoint.transform.position;
            }

            CharacterActor instance = CreateCharacter(neededState, position, Quaternion.identity);
            if (instance != null)
            {
                AddAgent(instance);
                if (init)
                {
                    instance.SetState(neededState);
                    instance.SetPatienceDuration(neededState);
                    instance.CurrentTarget = poi;
                }
            }
        }

        public CharacterActor CreateCharacter(CharacterState state, Vector3 pos, Quaternion rot)
        {
            var candidates = characterSpawnPool.Where(data =>
                data.ActivityPatience == null || data.ActivityPatience.Count == 0 ||
                data.ActivityPatience.Any(p => p.activity == state)
            ).ToList();

            if (candidates.Count == 0)
                return null;

            var characterData = candidates[Random.Range(0, candidates.Count)];
            var instance = Instantiate(characterPrefab, pos, rot);
            instance.Initialize(characterData);
            return instance;
        }

        private CharacterState GetMostNeededState()
        {
            var states = (CharacterState[])Enum.GetValues(typeof(CharacterState));
            int[] counts = GetCurrentPopulationCounts(states.Length);
            float[] weights = new float[states.Length];
            float totalWeight = 0;

            for (int i = 0; i < states.Length; i++)
            {
                int desired = desiredProportions.GetValueOrDefault(states[i], 0);
                int demand = Mathf.Max(0, desired - counts[i]);
                float weight = demand * demand;

                weights[i] = weight;
                totalWeight += weight;
            }

            return RollWeightedState(states, weights, totalWeight);
        }

        public CharacterState GetRandomState(CharacterActor agent)
        {
            var states = (CharacterState[])Enum.GetValues(typeof(CharacterState));
            int[] counts = GetCurrentPopulationCounts(states.Length);
            var prefs = agent.CharacterData.ActivityPatience;

            float[] weights = new float[states.Length];
            float totalWeight = 0;
            bool isGeneralist = prefs == null || prefs.Count == 0;

            for (int i = 0; i < states.Length; i++)
            {
                int demand = Mathf.Max(0, desiredProportions.GetValueOrDefault(states[i], 0) - counts[i]);

                if (isGeneralist)
                {
                    weights[i] = demand;
                }
                else
                {
                    var p = prefs.Find(x => x.activity == states[i]);
                    weights[i] = (p.maxTime > 0) ? demand * p.maxTime : 0;
                }
                totalWeight += weights[i];
            }

            if (totalWeight <= 0)
                return HandleDefaultState(agent);

            return RollWeightedState(states, weights, totalWeight);
        }

        private int[] GetCurrentPopulationCounts(int size)
        {
            int[] counts = new int[size];
            foreach (var agent in agents)
                counts[(int)agent.CurrentState]++;

            return counts;
        }

        private CharacterState RollWeightedState(CharacterState[] states, float[] weights, float total)
        {
            if (total <= 0)
                return CharacterState.Idling;

            float roll = Random.Range(0, total);
            float cursor = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                cursor += weights[i];
                if (roll <= cursor)
                    return states[i];
            }

            return CharacterState.Idling;
        }

        private CharacterState HandleDefaultState(CharacterActor agent)
        {
            var prefs = agent.CharacterData.ActivityPatience;
            if (prefs == null || prefs.Count == 0 || prefs.Exists(p => p.activity == CharacterState.Idling))
                return CharacterState.Idling;

            return prefs.GetRandom().activity;
        }

        public PointOfInterest PickRandomWaypoint(InterestType type)
        {
            if (!points.TryGetValue(type, out var list) || list.Count == 0)
                return null;

            var available = list.Where(p => !agents.Any(a => a.CurrentTarget == p)).ToList();
            return available.Count == 0 ? null : available[Random.Range(0, available.Count)];
        }

        public void AddPointOfInterest(PointOfInterest poi)
        {
            if (!points.ContainsKey(poi.InterestType))
                points[poi.InterestType] = new List<PointOfInterest>();

            points[poi.InterestType].Add(poi);
        }

        public void RemovePointOfInterest(PointOfInterest poi)
        {
            if (points.ContainsKey(poi.InterestType))
                points[poi.InterestType].Remove(poi);
        }

        public void AddSpawnPoint(SpawnPoint sp) => spawnPoints.Add(sp);
        public void RemoveSpawnPoint(SpawnPoint sp) => spawnPoints.Remove(sp);
        public void AddAgent(CharacterActor agent) => agents.Add(agent);
        public void RemoveAgent(CharacterActor agent) => agents.Remove(agent);
        public void OnFixedUpdated(float deltaTime) { }
    }
}
