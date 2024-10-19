using System.Collections.Generic;
using RecycleFactory.Buildings;
using UnityEngine;

namespace RecycleFactory
{
    public static class Map
    {
        public static readonly Vector2Int mapSize = new Vector2Int(16, 10);
        public static readonly float cellScale = 1f;
        public static readonly float floorHeight = 0f;
        public static List<Building> buildingsUnordered = new List<Building>();
        public static Building[,] buildingsAt = new Building[mapSize.y, mapSize.x];

        public static Vector2Int ConvertToMapPosition(Vector3 position)
        {
            return new Vector2(Hexath.SnapNumberToStep(position.x, cellScale), Hexath.SnapNumberToStep(position.z, cellScale)).RoundToInt();
        }

        public static void RegisterNewBuilding(Building building)
        {
            buildingsAt[building.mapPosition.y, building.mapPosition.x] = building;
        }

        public static void RemoveBuilding(Building building)
        {
            buildingsAt[building.mapPosition.y, building.mapPosition.x] = null;
        }
    }
}
