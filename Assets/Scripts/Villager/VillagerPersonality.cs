using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Species{}
public enum Humor{Sanguine, Phelgmatic, Melancholic, Choleric}
public enum Quirks{}

public class VillagerPersonality : MonoBehaviour
{
    private Species species;
    private Humor humor;
    private Quirks q1, q2, q3;
    private string unique;
}
