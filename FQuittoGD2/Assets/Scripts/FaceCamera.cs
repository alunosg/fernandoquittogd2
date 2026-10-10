using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public float baseScale = 0.1f;
    private Camera MainCamera;
    private Vector3 startScale;
    void Start()
    {
        MainCamera = Camera.main;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = MainCamera.transform.rotation;
        transform.localScale = startScale * (baseScale  *
            Vector3.Distance(transform.position, MainCamera.transform.position));
    }
}
