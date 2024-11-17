using UnityEngine;
using UnityEditor;
using RecycleFactory.Buildings;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    [CustomEditor(typeof(ConveyorBelt_Building))]
    public class ConveyorBeltInspector : Editor
    {
        ConveyorBelt_Building building;
        private void OnEnable()
        {
            building = (ConveyorBelt_Building)target;
        }

        private void OnSceneGUI()
        {
            OnInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);
            SerializeConveyorBelt(building);
        }

        private static void SerializeConveyorBelt(ConveyorBelt_Building building)
        {
            GUILayout.Label("ConveyorBelt_Building name: " + building.name);
            GUILayout.Label("Lanes info");
            GUILayout.Label($"Lanes number: {ConveyorBelt_Driver.LANES_NUMBER}; {building.driver.allItemsReadonly.Count} items in total");
            GUILayout.BeginHorizontal();
            for (int l = 0; l < ConveyorBelt_Driver.LANES_NUMBER; l++)
            {
                GUILayout.BeginVertical();
                var node = building.driver.lanes[l].First;
                for (int i = 0; i < building.driver.lanes[l].Count; i++)
                {
                    GUILayout.Label($"{node.Value.name}, {node.Value.id}");
                    node = node.Next;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}
