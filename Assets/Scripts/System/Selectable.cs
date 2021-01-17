using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    private Material originalMaterial;
    private Renderer rend;
    public bool IsSelectable { get => isSelectable; }
    private bool isSelectable = true;

    public bool IsSpawnOrTarget { get => isSpawnOrTarget; }
    private bool isSpawnOrTarget = false;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalMaterial = rend.material;
    }

    public void ResetMaterial()
    {
        if (rend != null)
            rend.material = originalMaterial;
    }

    public void SetMaterial(Material mat)
    {
        if (rend != null)
            rend.material = mat;
    }

    public void Selected(Material mat)
    {
        isSelectable = false;
        SetMaterial(mat);
    }

    public void Deselected()
    {
        if (!IsSpawnOrTarget)
        {
            isSelectable = true;
            ResetMaterial();
        }
    }

    public void SetAsSpawnOrTarget(Material mat)
    {
        Debug.LogWarning("Object set specifically as spawn or target through selection.");
        isSpawnOrTarget = true;
        isSelectable = false;
        SetMaterial(mat);
    }

    public void RemoveAsSpawnOrTarget()
    {
        Debug.LogWarning("Object removed specifically as spawn or target through selection.");
        isSpawnOrTarget = false;
        isSelectable = true;
        ResetMaterial();
    }
}
