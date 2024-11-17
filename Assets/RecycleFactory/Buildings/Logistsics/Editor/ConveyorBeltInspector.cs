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

            if (!Application.isPlaying)
            {
                GUILayout.Label("Info about conveyor will appear here in playmode");
                return;
            }

            if (building.driver == null)
            {
                GUILayout.Label("Conveyor driver is unitialized");
                return;
            }

            GUILayout.Space(15);
            SerializeConveyorBelt(building);
            GUILayout.Space(15);
            GUILayout.Label("Connection info");
            if (building.driver.nextDriver == null)
            {
                GUILayout.Label("No next conveyor found");
            }
            else
            {
                GUILayout.Label("Next conveyor building name: " + building.driver.nextDriver.conveyorBuilding.name);
                GUILayout.Label("Connection type: " + (building.driver.IsOrthogonalTo(building.driver.nextDriver) ? "orthogonal" : "straight"));
            }
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
