using UnityEngine;

public class KeepSceneView : MonoBehaviour
{
    public bool KeepSceneViewActive;

    void Start()
    {
#if UNITY_EDITOR
	    if (KeepSceneViewActive)
	    {
			UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));    
	    }
#endif
    }
}