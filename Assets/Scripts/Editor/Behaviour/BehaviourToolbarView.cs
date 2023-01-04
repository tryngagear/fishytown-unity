using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

namespace Behaviour
{
    public class BehaviourToolbarView : ToolbarView
    {
        public BehaviourToolbarView(BaseGraphView graphView) : base(graphView) { }

        protected override void AddButtons()
        {
            // Add the hello world button on the left of the toolbar
            AddButton("Hello !", () => Debug.Log("Hello World"), left: false);

            // add the default buttons (center, show processor and show in project)
            //base.AddButtons();
        }
    }
}
