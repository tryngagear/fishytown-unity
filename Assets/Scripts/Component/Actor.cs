using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class Actor : Entity, IPathing, IMessaging
{
    Seeker _seeker;
    ComponentController _compController;

    //Properties
    //BehaviourTree Tree => _behaviourRunner.tree;

    protected override void Start()
    {
        base.Start();

        _compController = new ComponentController(this);

        _seeker = GetComponent<Seeker>();
        _seeker.pathCallback += OnPathCompleted;
    }

    public override void Tick(int tick)
    {
        base.Tick(tick);

        _compController.Tick(tick);

        //if (Tree)
        //    Tree.Update();
    }

    public void RequestPath(Vector3 target)
    {
        _seeker.StartPath(transform.position, target);
    }

    public void DestroyPathing()
    {
        _seeker.pathCallback -= OnPathCompleted;
    }

    public void OnPathCompleted(Path path)
    {
        if(!path.error)
        {
            Debug.Log("Error finding path: " + path.errorLog);
            return;
        }

        OnNonIntrusiveMessage(new PathFoundMessage(path));
    }

    public void OnNonIntrusiveMessage(NonIntrusiveMessage message)
    {
        throw new NotImplementedException();
    }

    public object OnIntrusiveMessage(IntrusiveMessage message)
    {
        throw new NotImplementedException();
    }
}
