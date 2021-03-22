using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public DropdownSpawnManager spawnDropdown;
    public DropdownTargetManager targetDropdown;

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material selectionMaterial;

    private Transform newTransform;
    private Transform currentTransform;

    private Selectable newSelection;
    private Selectable currentSelection;

    public Text selectionText;

    private int fingerID = -1;

    private void Awake()
    {
    #if !UNITY_EDITOR
        fingerID = 0;
    #endif
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject(fingerID))
            return;
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                newTransform = hit.transform;

                if (newTransform != currentTransform)
                {
                    Selectable newSelect = newTransform.GetComponent<Selectable>();
                    if (newSelect != null)
                    {
                        if (newSelect.IsSelectable == true)
                            newSelect.SetMaterial(highlightMaterial);

                        ResetCurrentTransform();
                    }
                }
                currentTransform = newTransform;
            }
            else
            {
                ResetCurrentTransform();
                currentTransform = null;
                newTransform = null;
            }

            ClickSelection(newTransform);
        }
    }

    private void ResetCurrentTransform()
    {
        if (currentTransform != null)
        {
            Selectable currentSelect = currentTransform.GetComponent<Selectable>();
            if (currentSelect != null && currentSelect.IsSelectable == true && !currentSelect.IsSpawnOrTarget)
            {
                currentSelect.ResetMaterial();
            }
        }
    }

    private void ClickSelection(Transform selectedObject)
    {
        if(selectedObject != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                newSelection = selectedObject.GetComponent<Selectable>();
                if(newSelection != null)
                {
                    if(currentSelection != null)
                        currentSelection.Deselected();

                    newSelection.Selected(selectionMaterial);
                    currentSelection = newSelection;

                    selectionText.text = "Current Selection: \n" + newSelection.transform.gameObject.name;

                    newTransform = null;
                    currentTransform = null;
                }
            }
        }
    }

    public void AddSpawnAndTarget()
    {
        AddSpawn();
        AddTarget();
    }

    private void AddSpawn()
    {
        if (newSelection != null)
            spawnDropdown.AddNewOption(newSelection.transform);
        else
            Debug.LogWarning("No object currently selected.");
    }

    private void AddTarget()
    {
        if(newSelection != null)
            targetDropdown.AddNewOption(newSelection.transform);
        else
            Debug.LogWarning("No object currently selected.");
    }
}
