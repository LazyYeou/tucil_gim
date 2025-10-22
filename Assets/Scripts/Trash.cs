using UnityEngine;


public class Trash : MonoBehaviour
{
    public bool IsCollected { get; private set; }


    public void Collect()
    {
        if (IsCollected) return;
        IsCollected = true;
        GameManager.Instance.OnTrashCollected(this);
        Destroy(gameObject);
    }
}