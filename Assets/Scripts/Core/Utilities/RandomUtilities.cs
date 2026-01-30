using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace InSun.JamOne.Core.Utilities
{
    public static class RandomUtilities
    {
        private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

        /// <returns>
        /// Random direction vector.
        /// </returns>
        public static Vector3 GetRandomDirection()
        {
            var vector = new Vector3(
                GetRandomFloat(min: -1f, max: +1f),
                GetRandomFloat(min: -1f, max: +1f),
                GetRandomFloat(min: -1f, max: +1f)
            );

            vector.Normalize();

            return vector;
        }

        /// <returns>
        /// Random  <see cref="float"/> value which is withing given <paramref name="range"/>.
        /// </returns>
        public static float GetRandomFloat(Vector2 range)
        {
            return GetRandomFloat(range.x, range.y);
        }

        /// <returns>
        /// Random <see cref="float"/> value between <paramref name="min"/> and <paramref name="max"/> range.
        /// </returns>
        public static float GetRandomFloat(float min = 0f, float max = 1f)
        {
            return (float)(Random.NextDouble() * (max - min) + min);
        }

        /// <returns>
        /// Random <see cref="int"/> value which is withing given <paramref name="range"/>.
        /// </returns>
        public static int GetRandomInt(Vector2Int range)
        {
            return GetRandomInt(range.x, range.y);
        }

        /// <returns>
        /// Random <see cref="int"/> value between <paramref name="min"/> and <paramref name="max"/> range.
        /// </returns>
        public static int GetRandomInt(int min = int.MinValue, int max = int.MaxValue)
        {
            return Random.Next(min, max);
        }

        /// <returns>
        /// Random element from <paramref name="enumerable"/>.
        /// </returns>
        public static TValue GetRandom<TValue>(this IEnumerable<TValue> enumerable)
        {
            if (TryGetRandom(enumerable, out var value))
            {
                return value;
            }

            throw new Exception("Could not find a random random element");
        }

        /// <returns>
        /// <c>true</c> if a random <paramref name="value"/> is retrieved from given
        /// <paramref name="enumerable"/> or <c>false</c> otherwise.
        /// </returns>
        public static bool TryGetRandom<TValue>(this IEnumerable<TValue> enumerable, out TValue value)
        {
            var list = enumerable.ToList();
            if (list.Count == 0)
            {
                value = default;
                return false;
            }

            var index = Random.Next(list.Count);
            value = list[index];
            return true;
        }

        /// <returns>
        /// <c>true</c> if a random <paramref name="index"/> is retrieved from given
        /// <paramref name="list"/> or <c>false</c> otherwise.
        /// </returns>
        public static bool TryGetRandomIndex<TValue>(this IReadOnlyList<TValue> list, out int index)
        {
            if (list.Count == 0)
            {
                index = 0;
                return false;
            }

            index = Random.Next(list.Count);
            return true;
        }

        /// <summary>
        /// Shuffle given list.
        /// </summary>
        public static void Shuffle<TValue>(this IList<TValue> list)
        {
            var remaining = list.Count;
            while (remaining > 1)
            {
                remaining--;

                var index = Random.Next(remaining + 1);
                (list[index], list[remaining]) = (list[remaining], list[index]);
            }
        }

        /// <returns>
        /// Random color from given <paramref name="gradient"/>.
        /// </returns>
        public static Color GetRandomColor(Gradient gradient)
        {
            return gradient.Evaluate(GetRandomFloat());
        }

        /// <returns>
        /// Random color.
        /// </returns>
        public static Color GetRandomColor(bool isRandomizeAlpha = true)
        {
            return new Color(
                GetRandomFloat(min: 0f, max: 1f),
                GetRandomFloat(min: 0f, max: 1f),
                GetRandomFloat(min: 0f, max: 1f),
                isRandomizeAlpha ? GetRandomFloat(min: 0f, max: 1f) : 1f
            );
        }
    }
}
