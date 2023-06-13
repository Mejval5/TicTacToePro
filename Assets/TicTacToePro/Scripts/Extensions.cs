using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TicTacToePro
{
    public static class Extensions
    {
        static System.Random rnd = new System.Random();

        public static T GetRandom<T>(this IList<T> source, T fallback)
        {
            if (source.Count == 0)
                return fallback;

            int randIndex = rnd.Next(source.Count);
            return source[randIndex];
        }

        public static IEnumerable<T> PickRandom<T>(this IList<T> source, int count)
        {
            return source.OrderBy(x => rnd.Next()).Take(count);
        }

        public static void EnsureComponent<T>(this Component component, ref T output) where T : Component
        {
            if (!output && !component.TryGetComponent<T>(out output))
            {
                output = component.gameObject.AddComponent<T>();
            }
        }

        public static void EnsureComponent<T>(this Component component) where T : Component
        {
            if (!component.TryGetComponent<T>(out T _))
            {
                component.gameObject.AddComponent<T>();
            }
        }
    }
}