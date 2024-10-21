using System.Collections.Generic;
using RecycleFactory.Buildings;
using UnityEngine;

namespace RecycleFactory
{
    public class Map : MonoBehaviour
    {
        public static readonly Vector2Int mapSize = new Vector2Int(16, 10);
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
            for (int _x = 0; _x < Mathf.Abs(size.x); _x++)
            {
                for (int _y = 0; _y < Mathf.Abs(size.y); _y++)
                {
                    int y = pos.y + _y * (int)Mathf.Sign(size.y);
                    int x = pos.x + _x * (int)Mathf.Sign(size.x);
                    if (x < 0 || y < 0 || x > mapSize.x - 1 || y > mapSize.y - 1)
                        return false;

                    if (buildingsAt[y, x] != null) return false;
                }
            }
            return true;
        }

        public static Vector2Int ConvertToMapPosition(Vector3 position)
        {
            return new Vector2(Hexath.SnapNumberToStep(position.x, 1), Hexath.SnapNumberToStep(position.z, 1)).RoundToInt();
        }

        /// <summary>
        /// For each cell that the building takes it marks its corresponding position in the map matrix.
        /// </summary>
        public static void RegisterNewBuilding(Building building)
        {
            var pos = building.mapPosition;
            var size = building.size;
            for (int _x = 0; _x < Mathf.Abs(building.size.x); _x++)
            {
                for (int _y = 0; _y < Mathf.Abs(building.size.y); _y++)
                {
                    buildingsAt[pos.y + _y * (int)Mathf.Sign(size.y), pos.x + _x * (int)Mathf.Sign(size.x)] = building;
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
                    Vector2Int pos = Utils.RotateXY(new Vector2Int(_x, _y), building.rotation);
                    buildingsAt[building.mapPosition.y + pos.y, building.mapPosition.x + pos.x] = null;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    var building = getBuildingAt(new Vector2Int(x, y));
                    Gizmos.color = building == null ? Color.black : Color.white;
                    Gizmos.DrawWireCube(new Vector3(x, 1, y), Vector3.one * 0.4f);
                }
            }
        }
    }
}
