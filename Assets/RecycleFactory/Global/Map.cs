using System.Collections.Generic;
using RecycleFactory.Buildings;
using UnityEngine;

namespace RecycleFactory
{
    public class Map : MonoBehaviour
    {
        public static readonly Vector2Int mapSize = new Vector2Int(16, 10);
        public static readonly float cellScale = 1f;
        public static readonly float floorHeight = 0f;
        public static List<Building> buildingsUnordered = new List<Building>();
        public static Building[,] buildingsAt = new Building[mapSize.y, mapSize.x];

        public GameObject floor;

        public void Init()
        {
            floor.transform.localScale = mapSize.ConvertTo2D().ProjectTo3D() / 10f + Vector3.up;
            floor.transform.localPosition = (floor.transform.localScale * 10f / 2f).WithY(floorHeight) - new Vector3(0.5f, 0, 0.5f);
        }

        public static Building getBuildingAt(Vector2Int mapPos)
        {
            return buildingsAt[mapPos.y, mapPos.x];
        }

        public static bool isSpaceFree(Vector2Int pos, Vector2Int size)
        {
            for (int _x = 0; _x < size.x; _x++)
            {
                for (int _y = 0; _y < size.y; _y++)
                {
                    if (buildingsAt[pos.y, pos.x] != null) return false;
                }
            }
            return true;
        }

        public static Vector2Int ConvertToMapPosition(Vector3 position)
        {
            return new Vector2(Hexath.SnapNumberToStep(position.x, cellScale), Hexath.SnapNumberToStep(position.z, cellScale)).RoundToInt();
        }

        /// <summary>
        /// For each cell that the building takes it marks its corresponding position in the map matrix.
        /// </summary>
        public static void RegisterNewBuilding(Building building)
        {
            for (int _x = 0; _x < building.size.x; _x++)
            {
                for (int _y = 0; _y < building.size.y; _y++)
                {
                    Vector2Int pos = Utils.Rotate(new Vector2Int(_x, _y), building.rotation);
                    buildingsAt[building.mapPosition.y + pos.y, building.mapPosition.x + pos.x] = building;
                }
            }
        }

        /// <summary>
        /// For each cell that the building takes it clears its corresponding position in the map matrix.
        /// </summary>
        public static void RemoveBuilding(Building building)
        {
            for (int _x = 0; _x < building.size.x; _x++)
            {
                for (int _y = 0; _y < building.size.y; _y++)
                {
                    Vector2Int pos = Utils.Rotate(new Vector2Int(_x, _y), building.rotation);
                    buildingsAt[building.mapPosition.y + pos.y, building.mapPosition.x + pos.x] = null;
                }
            }
        }
    }
}
