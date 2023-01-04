using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using UnityEditor;

namespace Behaviour
{
    public class BehaviourGraphWindow : BaseGraphWindow
    {
        protected override void InitializeWindow(BaseGraph graph)
        {
            // Set the window title
            titleContent = new GUIContent("Behaviour Editor");

            graphView = new BehaviourGraphView(this);
            graphView.Add(new BehaviourToolbarView(graphView));

            rootView.Add(graphView);
        }

        [MenuItem("UnsungTools/Behaviour Editor")]
        public static BaseGraphWindow Open()
        {
            var graphWindow = GetWindow<BehaviourGraphWindow>();

            graphWindow.Show();

            return graphWindow;
        }

    }
}
