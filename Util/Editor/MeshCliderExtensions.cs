using UnityEngine;
using UnityEditor;

public static class MeshColliderExtensions
{
    [MenuItem("CONTEXT/MeshRenderer/Adjust to MeshRenderer Bounds")]
    private static void AdjustBounds(MenuCommand command)
    {
        MeshRenderer meshRenderer = (MeshRenderer)command.context;
        BoxCollider boxCollider = meshRenderer.GetComponent<BoxCollider>();
        
        if (boxCollider == null)
            boxCollider = meshRenderer.gameObject.AddComponent<BoxCollider>();
        
        boxCollider.center = meshRenderer.bounds.center - meshRenderer.transform.position;
        boxCollider.size = meshRenderer.bounds.size;
    }
}
