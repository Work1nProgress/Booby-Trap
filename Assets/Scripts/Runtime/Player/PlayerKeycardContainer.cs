using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeycardContainer : MonoBehaviour
{
    [SerializeField] List<string> keyCards = new List<string>();

    public bool Contains(string key) => keyCards.Contains(key);
    public void Add(string key) => keyCards.Add(key);
    public void Remove(string key) => keyCards.Remove(key);
}
