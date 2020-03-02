using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PreviewCard : MonoBehaviour
{
    public delegate void OpenHandDelegate();
    public event OpenHandDelegate onHandOpen;
    void OnMouseDown()
    {
        onHandOpen?.Invoke();
        Interaction.Instance.Stop();
    }
}
