using System.Collections.Generic;
using RecycleFactory.Buildings;
using UnityEngine;

namespace RecycleFactory
{
    public class Map : MonoBehaviour
    {
        public static Vector2Int mapSize = new Vector2Int(16, 10);
        /// <summary>
        /// Shift of the matrix (0,0) point relatively to the world (0,0) position. Updated on map extension
        /// </summary>
        public static Vector2Int mapShift = new Vector2Int(0, 0);

        public static readonly float floorHeight = 0f;
        public static List<Building> buildingsUnordered = new List<Building>();
        public static Building[,] buildingsAt = new Building[mapSize.y, mapSize.x];

        public static Vector2Int invalidLocation = new Vector2Int(-100, -100);

        public GameObject floor;

        public void Init()
        {
            floor.transform.localScale = mapSize.ConvertTo2D().ProjectTo3D() / 10f + Vector3.up;
            floor.transform.localPosition = (floor.transform.localScale * 10f / 2f).WithY(floorHeight) - new Vector3(0.5f, 0, 0.5f);
        }

        public static Building getBuildingAt(Vector2Int mapPos)
        {
            if (isMapPosValid(mapPos))
                return buildingsAt[mapPos.y, mapPos.x];
            return null;
        }

        public static bool isMapPosValid(Vector2Int mapPos)
        {
            return mapPos.x >= 0 && mapPos.x < mapSize.x && mapPos.y >= 0 && mapPos.y < mapSize.y;
        }

        public static bool isMapPosValid(int x, int y)
        {
            return isMapPosValid(new Vector2Int(x, y));
        }

        public static bool isSpaceFree(Vector2Int pos, Vector2Int shift, Vector2Int size)
        {
            for (int _x = 0; _x < Mathf.Abs(size.x); _x++)
            {
                for (int _y = 0; _y < Mathf.Abs(size.y); _y++)
                {
                    int y = pos.y + _y * (int)Mathf.Sign(size.y) + shift.y;
                    int x = pos.x + _x * (int)Mathf.Sign(size.x) + shift.x;
                    if (!isMapPosValid(x, y))
                        return false;

                    if (buildingsAt[y, x] != null) return false;
                }
            }
            return true;
        }

        public static Vector2Int world2map(Vector2 position)
        {
            return new Vector2(
                position.x,
                position.y
                ).RoundToInt() + mapShift;
        }

        public static Vector2Int world2map(Vector3 position)
        {
            return new Vector2(
                position.x, 
                position.z
                ).RoundToInt() + mapShift;
        }

        public static Vector3 map2world(Vector2Int position)
        {
            return (position - mapShift).ConvertTo2D().ProjectTo3D();
        }

        /// <summary>
        /// For each cell that the building takes it marks its corresponding position in the map matrix.
        /// </summary>
        public static void RegisterNewBuilding(Building building)
        {
            var pos = building.mapPosition + building.shift;
            for (int _x = 0; _x < Mathf.Abs(building.size.x); _x++)
            {
                for (int _y = 0; _y < Mathf.Abs(building.size.y); _y++)
                {
                    buildingsAt[pos.y + _y * (int)Mathf.Sign(building.size.y), pos.x + _x * (int)Mathf.Sign(building.size.x)] = building;
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
                    Gizmos.DrawWireCube(new Vector3(x, 1, y) - mapShift.ConvertTo2D().ProjectTo3D(), Vector3.one * 0.4f);
                }
            }
        }

        public void Extend(int xpos, int ypos, int xneg, int yneg)
        {
            Vector2Int prevMapSize = mapSize;
            Vector2Int delta = new Vector2Int(xpos + xneg, ypos + yneg);
            mapSize += delta;
            mapShift += new Vector2Int(xneg, yneg);

            Building[,] result = new Building[mapSize.y, mapSize.x];
            for (int y = 0; y < prevMapSize.y; y++)
            {
                for (int x = 0; x < prevMapSize.x; x++)
                {
                    result[y + yneg, x + xneg] = buildingsAt[y, x];
                }
            }

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    Vector2Int newMatrixPos = new Vector2Int(x + xneg, y + yneg);
                    result[y, x]?.Rebase(newMatrixPos);
                }
            }

            buildingsAt = result;

            // rescale floor plane
            floor.transform.localScale += delta.ConvertTo2D().ProjectTo3D() / 10f;
        }
    }
}
