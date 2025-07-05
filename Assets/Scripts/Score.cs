using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score instance;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }
}
