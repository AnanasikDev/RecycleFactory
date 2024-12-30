using UnityEngine;
using RecycleFactory.UI;
using RecycleFactory.Buildings;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using RecycleFactory.Player;

namespace RecycleFactory
{

    [ExecuteInEditMode]
    public class Scripts : MonoBehaviour
    {
        [SerializeField] private PlayerController _PlayerController;
        public static PlayerController PlayerController;

        [SerializeField] private PlayerCamera _PlayerCamera;
        public static PlayerCamera PlayerCamera;

        [SerializeField] private PlayerBuilder _PlayerBuilder;
        public static PlayerBuilder PlayerBuilder;

        [SerializeField] private PlayerDemolisher _PlayerDemolisher;
        public static PlayerDemolisher PlayerDemolisher;

        [SerializeField] private UIController _UIController;
        public static UIController UIController;

        [SerializeField] private PlayerModeSwitch _PlayerModeSwitch;
        public static PlayerModeSwitch PlayerModeSwitch;

        [SerializeField] private Map _Map;
        public static Map Map;

        [SerializeField] private Budget _Budget;
        public static Budget Budget;

        [SerializeField] private BuildingArrowPreviewController _BuildingArrowPreviewController;
        public static BuildingArrowPreviewController BuildingArrowPreviewController;

        [SerializeField] private AllBuildings _AllBuildings;
        public static AllBuildings AllBuildings;

        [SerializeField] private LevelController _LevelController;
        public static LevelController LevelController;

        [SerializeField] private AllItems _AllItems;
        public static AllItems AllItems;

        private void Start()
        {
            #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Init();
                }
            #else
                Init();
            #endif
        }

        private void Init()
        {
            PlayerController = _PlayerController;
            PlayerCamera = _PlayerCamera;
            PlayerBuilder = _PlayerBuilder;
            PlayerDemolisher = _PlayerDemolisher;
            Map = _Map;
            UIController = _UIController;
            Budget = _Budget;
            BuildingArrowPreviewController = _BuildingArrowPreviewController;
            AllBuildings = _AllBuildings;
            LevelController = _LevelController;
            AllItems = _AllItems;
            PlayerModeSwitch = _PlayerModeSwitch;

            PlayerModeSwitch.Init();
            AllBuildings.Init();
            AllItems.Init();
            LevelController.Init();
            BuildingArrowPreviewController.Init();
            Budget.Init();
            UIController.Init();
            Map.Init();
            PlayerController.Init();
            StatisticsManager.Init();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (HasNullSerializedFields())
                {
                    Debug.LogError("Play mode prevented due to null fields in Scripts.");
                    EditorApplication.isPlaying = false;
                }
            }
#endif
        }

        private bool HasNullSerializedFields()
        {
#if UNITY_EDITOR
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                // Check only serialized fields (public or with [SerializeField])
                bool isSerializable = field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;
                if (isSerializable && field.GetValue(this) == null)
                {
                    Debug.LogError($"Null field detected: {field.Name}");
                    return true;
                }
            }
#endif
            return false;
        }
    }
}