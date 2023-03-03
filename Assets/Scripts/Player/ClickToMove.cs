using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class ClickToMove : MonoBehaviour, IPathing
{
    Camera mainCamera;
    Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                RequestPath(hit.point);
            }
        }
    }

    public void RequestPath(Vector3 target)
    {
        seeker.StartPath(transform.position, target);
    }

    public void OnPathCompleted(Path path)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + path.error);
    }

    public void DestroyPathing()
    {
        seeker.pathCallback -= OnPathCompleted;
    }
}
