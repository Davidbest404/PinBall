using UnityEngine;

public class InputDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Debug.Log("Space");
        if (Input.GetKeyDown(KeyCode.Z)) Debug.Log("Z");
        if (Input.GetKeyDown(KeyCode.X)) Debug.Log("X");
    }
}