using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtensions
{
    public static TKey GetWeightedRandomElement<TKey>(this Dictionary<TKey, int> weightDictionary)
    {
        int probabilitySum = weightDictionary.Sum(x => x.Value);
        int rng = Random.Range(0, probabilitySum);
        int tmpSum = 0;
        foreach (var kvp in weightDictionary)
        {
            tmpSum += kvp.Value;
            if (rng < tmpSum)
                return kvp.Key;
        }
        throw new System.Exception("No element selected. Check the dictionary for valid weights.");
    }

    public static TKey GetWeightedRandomElement<TKey>(this Dictionary<TKey, float> weightDictionary)
    {
        float probabilitySum = weightDictionary.Sum(x => x.Value);
        float rng = Random.Range(0, probabilitySum);
        float tmpSum = 0;
        foreach (var kvp in weightDictionary)
        {
            tmpSum += kvp.Value;
            if (rng < tmpSum)
                return kvp.Key;
        }
        throw new System.Exception("No element selected. Check the dictionary for valid weights.");
    }

    /// <summary>
    /// Increases the value of the given key by an amount. If the key doesn't exist yet, it creates the key-value-pair with that value.
    /// </summary>
    public static void Increment<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int amount = 1)
    {
        if (dictionary.ContainsKey(key)) dictionary[key] += amount;
        else dictionary.Add(key, amount);
    }

    /// <summary>
    /// Decreases the value of the given key by 1. If the key doesn't exist or is <= 0, it throws an error. If they value is at 1, it removes the key.
    /// </summary>
    public static void Decrement<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int amount = 1)
    {
        if (dictionary.ContainsKey(key))
        {
            if (dictionary[key] < amount) throw new System.Exception($"Key {key} is not allowed to be <{amount} when trying to decrement {amount}, but is {dictionary[key]}");
            else dictionary[key] -= amount;
        }
        else throw new System.Exception($"Key {key} doesn't exist.");
    }

    /// <summary>
    /// Adds a value to a dictionary that stores values grouped by a key.
    /// </summary>
    public static void AddToValueList<TKey, T>(this Dictionary<TKey, List<T>> dictionary, TKey key, T value)
    {
        if (dictionary.ContainsKey(key)) dictionary[key].Add(value);
        else dictionary.Add(key, new List<T>() { value });
    }
}