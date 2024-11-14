using RecycleFactory.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecycleFactory
{
    [CustomEditor(typeof(RecycleFactory.UI.UIController))]
    public class UIInputMaskInspector : Editor
    {
        private void OnSceneGUI()
        {
            OnInspectorGUI();
            Repaint();
        }
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();
            GUILayout.Space(15);
            for (int i = 0; i < UIInputMask.masks.Count; i++)
            {
                GUILayout.Space(3);
                GUILayout.Label($"[{i}] mask ({UIInputMask.masks[i].name}) is " + (UIInputMask.masks[i].isPointerInside ? "HOVERED" : "not hovered"));
            }
        }
    }
}
