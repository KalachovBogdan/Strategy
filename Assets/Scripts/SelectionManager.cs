using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class SelectionManager : MonoBehaviour
{
    public RectTransform SelectBox;
    public GameObject moveMarkerPrefab;

    public List<SelectableObject> AllSelectableObjects = new();
    public List<SelectableObject> CurrSelectedObjects = new();

    private bool isMouseDown = false, isDragging = false;
    private Vector3 MouseStartPos;
    private GameObject currentMoveMarker;

    void Update()
    {
        HandleSelection();
        HandleMoveCommand();
        CleanupDestroyedSelections();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            MouseStartPos = Input.mousePosition;

            foreach (var so in CurrSelectedObjects)
                so?.DeSelectMe();

            CurrSelectedObjects.Clear();
        }

        if (isMouseDown)
        {
            if (Vector3.Distance(Input.mousePosition, MouseStartPos) > 5f)
            {
                isDragging = true;
                SelectBox.gameObject.SetActive(true);

                UpdateSelectBox(Input.mousePosition);
                SelectUnits();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            isDragging = false;
            SelectBox.gameObject.SetActive(false);
        }
    }

    void UpdateSelectBox(Vector3 currentMouse)
    {
        float width = currentMouse.x - MouseStartPos.x;
        float height = currentMouse.y - MouseStartPos.y;

        SelectBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        SelectBox.anchoredPosition = (MouseStartPos + currentMouse) / 2;
    }

    void SelectUnits()
    {
        foreach (var so in AllSelectableObjects)
        {
            if (so == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(so.transform.position);
            Rect selectionRect = new Rect(
                SelectBox.anchoredPosition.x - SelectBox.sizeDelta.x / 2,
                SelectBox.anchoredPosition.y - SelectBox.sizeDelta.y / 2,
                SelectBox.sizeDelta.x,
                SelectBox.sizeDelta.y
            );

            bool inside = screenPos.x > selectionRect.x &&
                          screenPos.x < selectionRect.x + selectionRect.width &&
                          screenPos.y > selectionRect.y &&
                          screenPos.y < selectionRect.y + selectionRect.height;

            if (inside)
            {
                if (!CurrSelectedObjects.Contains(so))
                {
                    CurrSelectedObjects.Add(so);
                    so.SelectMe();
                }
            }
            else
            {
                if (CurrSelectedObjects.Contains(so))
                {
                    CurrSelectedObjects.Remove(so);
                    so.DeSelectMe();
                }
            }
        }
    }

    void HandleMoveCommand()
    {
        if (!Input.GetMouseButtonDown(1) || CurrSelectedObjects.Count == 0)
            return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
        {
            MoveUnitsWithFormation(hit.point);
            ShowMoveMarker(hit.point);
        }
    }

    void ShowMoveMarker(Vector3 position)
    {
        if (currentMoveMarker != null)
            Destroy(currentMoveMarker);

        currentMoveMarker = Instantiate(moveMarkerPrefab, position + Vector3.up * 0.05f, Quaternion.identity);
        StartCoroutine(RemoveMarkerAfterDelay(0.5f));
    }

    IEnumerator RemoveMarkerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentMoveMarker != null)
            Destroy(currentMoveMarker);
    }

    void MoveUnitsWithFormation(Vector3 target)
    {
        int count = CurrSelectedObjects.Count;
        float baseSpacing = 2f;

        float spacing = baseSpacing;
        NavMeshAgent agent = CurrSelectedObjects[0].GetComponent<NavMeshAgent>();
        if (agent != null)
            spacing += agent.radius * 2;

        int rowSize = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            if (CurrSelectedObjects[i] == null) continue;

            int row = i / rowSize;
            int col = i % rowSize;

            Vector3 offset = new Vector3(
                (col - rowSize / 2f) * spacing,
                0,
                (row - rowSize / 2f) * spacing
            );

            var controller = CurrSelectedObjects[i].GetComponent<UnitController>();
            if (controller != null)
                controller.MoveTo(target + offset);
        }
    }

    void CleanupDestroyedSelections()
    {
        CurrSelectedObjects.RemoveAll(obj => obj == null);
    }
}
