using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour
{
    private static Interaction _instance;
    public static Interaction Instance { get { return _instance; } }

    public delegate void DeselectionEvent();
    public event DeselectionEvent onDeselection;

    [SerializeField] private GameObject deselectCondition;

    private bool stop = false;

    void Awake()
    {
        _instance = this;
        eventQueue = new List<DeselectionEvent>();
    }

    void Update()
    {
        if(stop) { stop = false; return; }
        if(Input.GetMouseButtonDown(0))
        {
            if (deselectCondition != null) if (CheckDeselectCondition()) { Deselect(); return; }
            GameObject hit = ScreenMouseRay();
            if (Utility.IsPointerOverUIObject()) { return; }
            if (hit == null) { Deselect(); return; }

            try
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                interactable.OnInteraction();
            }
            catch (System.NullReferenceException) { }
        }
    }

    public void Stop() { stop = true; }

    public GameObject ScreenMouseRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f;

        Vector2 v = Camera.main.ScreenToWorldPoint(mousePos);

        Collider2D collider = Physics2D.OverlapPoint(v);
        if (collider == null) return null;
        return collider.gameObject;
    }

    /// <summary>
    /// If you press outside toDeselect, then Deselect() will be called
    /// </summary>
    /// <param name="toDeselect"></param>
    public void SetDeselectCondition(GameObject toDeselect)
    {
        deselectCondition = toDeselect;
    }
    /// <summary>
    /// Set the deselectCondition to null
    /// </summary>
    public void ClearDeselectCondition()
    {
        deselectCondition = null;
    }

    /// <summary>
    /// If you pressed outside deselectCondtition, it returns true. Else returns false.
    /// </summary>
    /// <returns></returns>
    private bool CheckDeselectCondition()
    {
        List<RaycastResult> results = Utility.GetPointerRaycastResults();
        for(int i = 0; i < results.Count; i++ )
        {
            if (results[i].gameObject == deselectCondition)
            {
                return false;
            }
            // if you didn't press outside, it doesnt deselect
        }
        return true;
    }

    private List<DeselectionEvent> eventQueue;

    private void Deselect()
    {
        ClearDeselectCondition();
        AddDeselectionQueue();
        onDeselection?.Invoke();
        ClearDeselectionEvents();
    }

    private void AddEventToDeselectionQueue(DeselectionEvent action)
    {
        eventQueue.Add(action);
    }
    
    public void RemoveEventFromDeselection(DeselectionEvent action)
    {
        if (eventQueue != null && eventQueue.Contains(action)) eventQueue.Remove(action);
        onDeselection -= action;
    }

    private void ClearDeselectionEvents()
    {
        onDeselection = null;
    }

    private void AddDeselectionQueue()
    {
        foreach (DeselectionEvent action in eventQueue)
        {
            onDeselection += action;
        }
        eventQueue.Clear();
    }
}