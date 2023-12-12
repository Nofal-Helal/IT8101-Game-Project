using UnityEngine;
using UnityEngine.UI;

public class CircularBar : MonoBehaviour
{
    public Transform target;
    public float progress;
    public bool visible;
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        Material mat = Instantiate(rawImage.material);
        rawImage.material = mat;
    }

    void Update()
    {
        if (visible && target != null)
        {
            rawImage.enabled = true;
            transform.position = Camera.main.WorldToScreenPoint(target.position + new Vector3(0,1,0));
            rawImage.material.SetFloat("_Frac", progress);
        } else {
            rawImage.enabled = false;
        }
    }
}
