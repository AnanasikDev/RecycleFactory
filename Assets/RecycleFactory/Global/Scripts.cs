using UnityEngine;
using RecycleFactory.UI;
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
        [SerializeField] private PlayerCamera _PlayerCamera;
        public static PlayerCamera PlayerCamera;

        [SerializeField] private PlayerBuilder _PlayerBuilder;
        public static PlayerBuilder PlayerBuilder;

        [SerializeField] private UIController _UIController;
        public static UIController UIController;

        [SerializeField] private BalanceManager _BalanceManager;
        public static BalanceManager BalanceManager;

        [SerializeField] private Map _Map;
        public static Map Map;

        [SerializeField] private Budget _Budget;
        public static Budget Budget;

        private void Start()
        {
            if (Application.isPlaying)
            {
                PlayerCamera = _PlayerCamera;
                PlayerBuilder = _PlayerBuilder;
                Map = _Map;
                UIController = _UIController;
                BalanceManager = _BalanceManager;
                Budget = _Budget;

                Budget.Init();
                UIController.Init();
                BalanceManager.Init();
                Map.Init();
                PlayerCamera.Init();
                PlayerBuilder.Init();
            }
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