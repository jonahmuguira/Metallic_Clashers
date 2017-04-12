using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Library;

using UnityEngine;

using Random = System.Random;

public class RandomManager : MonoSingleton<RandomManager>
{
    private class RandomGenerator
    {
        public Random randomizer;
        public int seed;
    }

    [Serializable]
    private class RandomGeneratorPair
    {
        public string key;
        public int value;
    }

    [SerializeField]
    private class RandomGeneratorPairList
    {
        public List<RandomGeneratorPair> randomGeneratorPairs;
    }

    [SerializeField]
    private bool m_StaticSeed;

    private string m_Path = "/RandomManager.json";

    private Dictionary<string, RandomGenerator> m_RandomGenerators =
        new Dictionary<string, RandomGenerator>();

    protected override void OnAwake()
    {
        if (!m_StaticSeed || !File.Exists(Application.persistentDataPath + m_Path))
            return;

        var jsonData = File.ReadAllText(Application.persistentDataPath + m_Path);
        var seedList = JsonUtility.FromJson<RandomGeneratorPairList>(jsonData);

        m_RandomGenerators =
            seedList.randomGeneratorPairs.ToDictionary(
                pair => pair.key,
                pair =>
                    new RandomGenerator
                    {
                        randomizer = new Random(pair.value),
                        seed = pair.value
                    });

        DontDestroyOnLoad(this);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        var seedList =
            new RandomGeneratorPairList
            {
                randomGeneratorPairs =
                    m_RandomGenerators.Select(
                        pair =>
                            new RandomGeneratorPair
                            {
                                key = pair.Key,
                                value = pair.Value.seed,
                            }).ToList()
            };

        var jsonData = JsonUtility.ToJson(seedList);
        File.WriteAllText(Application.persistentDataPath + m_Path, jsonData);
    }

    public int Range(string key, int min, int max)
    {
        var value = TryGetValueOrDefault(key);

        return value.randomizer.Next(min, max);
    }

    public float Range(string key, float min, float max)
    {
        var value = TryGetValueOrDefault(key);

        return (float)value.randomizer.NextDouble() * Mathf.Abs(max - min) + min;
    }

    public int Range<T>(string key) where T : struct, IConvertible
    {
        var value = TryGetValueOrDefault(key);

        var enumCount = Enum.GetNames(typeof(T)).Length;
        return value.randomizer.Next(0, enumCount);
    }

    private RandomGenerator TryGetValueOrDefault(string key)
    {
        RandomGenerator value;
        if (!m_RandomGenerators.TryGetValue(key, out value))
            value = AddNewValue(key);

        return value;
    }

    private RandomGenerator AddNewValue(string key)
    {
        var seed = (int)DateTime.Now.Ticks;
        var newValue =
            new RandomGenerator
            {
                randomizer = new Random(seed),
                seed = seed,
            };
        m_RandomGenerators.Add(key, newValue);

        return newValue;
    }
}
