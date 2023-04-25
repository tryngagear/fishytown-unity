using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Need", menuName = "ScriptableObjects/Life/Need", order = 1)]
public class NeedDef : ScriptableObject
{
    [SerializeField] public string needName;
    [SerializeField] public string needDescription;
    [SerializeField] public int maxValue = 100;
    public AnimationCurve weight = AnimationCurve.Linear(0, 0.5f, 1, 0.5f);
    [SerializeField] public float decayRate = 0.01f;
    public Vector2 balancedThreshold = new Vector2(0.3f, 0.6f);
}