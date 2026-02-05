using System;
using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Transforms;
using RIEVES.GGJ2026.Core.Utilities;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Heat;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private ITransformSystem transformSystem;
        private HeatSystem heatSystem;

        private int initialBasePopulation = 25;
        private bool isSceneActive = false;

        private void Awake()
        {
            transformSystem = GameManager.GetSystem<ITransformSystem>();
            heatSystem = GameManager.GetSystem<HeatSystem>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            isSceneActive = false;

            points.Clear();
            spawnPoints.Clear();
            agents.Clear();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isSceneActive = true;

            var foundPoints = FindObjectsByType<PointOfInterest>(FindObjectsSortMode.None);
            var foundSpawns = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

            if (foundPoints.Length == 0 && foundSpawns.Length == 0)
                return;

            foreach (var poi in foundPoints)
            {
                if (!points.ContainsKey(poi.InterestType))
                    points[poi.InterestType] = new List<PointOfInterest>();
                points[poi.InterestType].Add(poi);
            }
            spawnPoints.AddRange(foundSpawns);

            var existingAgents = FindObjectsByType<CharacterActor>(FindObjectsSortMode.None);
            foreach (var agent in existingAgents)
                agents.Add(agent);

            foreach (CharacterState state in Enum.GetValues(typeof(CharacterState)))
                desiredProportions[state] = 0;

            desiredProportions[CharacterState.Idling] = 8;
            desiredProportions[CharacterState.Dancing] = 10;
            desiredProportions[CharacterState.Watching] = 13;

            int maxAttempts = 100;
            int attempts = 0;
            while (agents.Count < initialBasePopulation && attempts < maxAttempts)
            {
                SpawnNewAgent(true);
                attempts++;
            }

            if (attempts >= maxAttempts)
                Debug.LogWarning("AgentSystem: Population goal not met. Not enough Points of Interest available.");
        }

        public void OnUpdated(float deltaTime)
        {
            if (!isSceneActive)
                return;

            float heatDelta = Mathf.Max(0f, heatSystem.CurrentHeat - 1f);

            desiredProportions[CharacterState.Guarding] = 0 + Mathf.RoundToInt(3.5f * heatDelta);
            desiredProportions[CharacterState.Hunting] = 0 + Mathf.RoundToInt(3.5f * heatDelta);
            desiredProportions[CharacterState.Dancing] = 13 + Mathf.RoundToInt(2.5f * heatDelta);

            int currentGoal = initialBasePopulation + 1 + Mathf.RoundToInt(9.4f * heatDelta);
            if (agents.Count < currentGoal)
            {
                SpawnNewAgent(false);
            }
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
            CharacterState neededState = GetMostNeededState(allowIdle: init);
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
                var offset = Random.insideUnitSphere;
                offset.y = 0f;
                position = spawnPoint.transform.position + offset * spawnPoint.Radius;
            }

            CharacterActor instance = CreateCharacter(neededState, position, Quaternion.identity);
            if (instance != null)
            {
                agents.Add(instance);
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
            var instance = Instantiate(characterPrefab, pos, rot, parent: transformSystem.GetTransform("Runtime_Agents"));
            instance.Initialize(characterData);
            return instance;
        }

        private CharacterState GetMostNeededState(bool allowIdle)
        {
            var states = (CharacterState[])Enum.GetValues(typeof(CharacterState));
            int[] counts = GetCurrentPopulationCounts(states.Length);
            float[] weights = new float[states.Length];
            float totalWeight = 0;

            for (int i = 0; i < states.Length; i++)
            {
                CharacterState state = states[i];

                // If we are avoiding Idle and this is the Idle state, skip it
                if (!allowIdle && state == CharacterState.Idling)
                {
                    weights[i] = 0;
                    continue;
                }

                int desired = desiredProportions.GetValueOrDefault(state, 0);
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

        public void OnFixedUpdated(float deltaTime) { }
    }
}
