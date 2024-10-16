using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

[ExecuteInEditMode]
public class Scripts : MonoBehaviour
{
    [SerializeField] private PlayerCamera _playerCamera;
    public static PlayerCamera playerCamera;

    [SerializeField] private PlayerBuilder _playerBuilder;
    public static PlayerBuilder playerBuilder;

    private void Start()
    {
        playerCamera = _playerCamera;
        playerBuilder = _playerBuilder;

        playerCamera.Init();
        playerBuilder.Init();
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