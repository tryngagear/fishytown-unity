using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Villager))]
public class VillagerDebug : MonoBehaviour
{
    Villager _villager;

    // Start is called before the first frame update
    void Start()
    {
        _villager = GetComponent<Villager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);

        GUILayout.BeginHorizontal();

        for(int i = 0; i < _villager.NeedsTracker.Needs.Count; i++)
        {
            Need need = _villager.NeedsTracker.Needs[i];

            var str = string.Format("{0}: {1}/{2}", need.Name, need.CurrentValue, need.MaxValue);

            GUI.color = Color.blue;
            GUI.Label(new Rect(screenPos.x, screenPos.y, 100, 50), str);
        }
        

        //GUI.backgroundColor = Color.green;
        //GUI.Box(new Rect(100, 25, 100, 30), "");

        GUILayout.EndHorizontal();

        
    }
}
