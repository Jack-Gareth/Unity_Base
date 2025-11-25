using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelMechanicsManager))]
public class LevelMechanicsVisualizer : Editor
{
    private const string VISUALIZER_CHILD_NAME = "_EditorVisualization";
    private const string SHOW_ALL_PREF_KEY = "LevelMechanicsVisualizer_ShowAll";
    
    private static bool showAllMechanics = false;
    private static Dictionary<LevelMechanicsManager, GameObject> allVisualizers = new Dictionary<LevelMechanicsManager, GameObject>();

    private void OnEnable()
    {
        showAllMechanics = EditorPrefs.GetBool(SHOW_ALL_PREF_KEY, false);
        SceneView.duringSceneGui += OnSceneGUIGlobal;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUIGlobal;
        CleanupVisualizer((LevelMechanicsManager)target);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Scene Visualization", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        showAllMechanics = EditorGUILayout.Toggle("Show All Level Mechanics", showAllMechanics);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool(SHOW_ALL_PREF_KEY, showAllMechanics);
            
            if (showAllMechanics)
            {
                UpdateAllVisualizations();
            }
            else
            {
                CleanupAllVisualizers();
            }
            
            SceneView.RepaintAll();
        }

        if (showAllMechanics)
        {
            EditorGUILayout.HelpBox("Showing all Level Mechanics in the scene. Uncheck to show only the selected one.", MessageType.Info);
        }
    }

    private void OnSceneGUIGlobal(SceneView sceneView)
    {
        if (showAllMechanics)
        {
            UpdateAllVisualizations();
        }
    }

    private void OnSceneGUI()
    {
        if (!showAllMechanics)
        {
            LevelMechanicsManager manager = (LevelMechanicsManager)target;
            UpdateSingleVisualization(manager);
        }
    }

    private void UpdateAllVisualizations()
    {
        LevelMechanicsManager[] allManagers = FindObjectsByType<LevelMechanicsManager>(FindObjectsSortMode.None);
        
        HashSet<LevelMechanicsManager> currentManagers = new HashSet<LevelMechanicsManager>(allManagers);
        List<LevelMechanicsManager> toRemove = new List<LevelMechanicsManager>();
        
        foreach (var kvp in allVisualizers)
        {
            if (kvp.Key == null || !currentManagers.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (var manager in toRemove)
        {
            CleanupVisualizer(manager);
        }

        foreach (LevelMechanicsManager manager in allManagers)
        {
            if (manager != null)
            {
                UpdateSingleVisualization(manager);
            }
        }
    }

    private void UpdateSingleVisualization(LevelMechanicsManager manager)
    {
        if (manager == null) return;

        GameObject visualizerObject;
        
        if (allVisualizers.ContainsKey(manager) && allVisualizers[manager] != null)
        {
            visualizerObject = allVisualizers[manager];
        }
        else
        {
            Transform visualizerTransform = manager.transform.Find(VISUALIZER_CHILD_NAME);
            
            if (visualizerTransform == null)
            {
                visualizerObject = new GameObject(VISUALIZER_CHILD_NAME);
                visualizerObject.transform.SetParent(manager.transform);
                visualizerObject.transform.localPosition = Vector3.zero;
                visualizerObject.transform.localRotation = Quaternion.identity;
                visualizerObject.transform.localScale = Vector3.one;
                visualizerObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            }
            else
            {
                visualizerObject = visualizerTransform.gameObject;
            }
            
            allVisualizers[manager] = visualizerObject;
        }

        SpriteRenderer spriteRenderer = visualizerObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = visualizerObject.AddComponent<SpriteRenderer>();
        }

        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = CreateSquareSprite();
        }

        BoxCollider2D collider = manager.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            visualizerObject.transform.localPosition = collider.offset;
            visualizerObject.transform.localScale = new Vector3(collider.size.x, collider.size.y, 1);
        }

        SerializedObject serializedManager = new SerializedObject(manager);
        
        bool blueMechanic = serializedManager.FindProperty("enableBlueMechanic").boolValue;
        bool redMechanic = serializedManager.FindProperty("enableRedMechanic").boolValue;
        bool greenMechanic = serializedManager.FindProperty("enableGreenMechanic").boolValue;
        bool yellowMechanic = serializedManager.FindProperty("enableYellowMechanic").boolValue;

        Color targetColor = serializedManager.FindProperty("disabledColor").colorValue;

        if (blueMechanic)
        {
            targetColor = serializedManager.FindProperty("blueMechanicColor").colorValue;
        }
        else if (redMechanic)
        {
            targetColor = serializedManager.FindProperty("redMechanicColor").colorValue;
        }
        else if (greenMechanic)
        {
            targetColor = serializedManager.FindProperty("greenMechanicColor").colorValue;
        }
        else if (yellowMechanic)
        {
            targetColor = serializedManager.FindProperty("yellowMechanicColor").colorValue;
        }

        spriteRenderer.color = targetColor;
        spriteRenderer.sortingOrder = -10;

        if (!Application.isPlaying)
        {
            spriteRenderer.enabled = true;
        }
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }

    private void CleanupVisualizer(LevelMechanicsManager manager)
    {
        if (manager != null)
        {
            Transform visualizerTransform = manager.transform.Find(VISUALIZER_CHILD_NAME);
            if (visualizerTransform != null)
            {
                DestroyImmediate(visualizerTransform.gameObject);
            }
        }
        
        if (allVisualizers.ContainsKey(manager))
        {
            if (allVisualizers[manager] != null)
            {
                DestroyImmediate(allVisualizers[manager]);
            }
            allVisualizers.Remove(manager);
        }
    }

    private void CleanupAllVisualizers()
    {
        List<LevelMechanicsManager> keys = new List<LevelMechanicsManager>(allVisualizers.Keys);
        foreach (var manager in keys)
        {
            CleanupVisualizer(manager);
        }
        allVisualizers.Clear();
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            LevelMechanicsManager[] allManagers = FindObjectsByType<LevelMechanicsManager>(FindObjectsSortMode.None);
            foreach (var manager in allManagers)
            {
                if (manager != null)
                {
                    Transform visualizerTransform = manager.transform.Find(VISUALIZER_CHILD_NAME);
                    if (visualizerTransform != null)
                    {
                        DestroyImmediate(visualizerTransform.gameObject);
                    }
                }
            }
        }
    }
}
