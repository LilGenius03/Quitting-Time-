using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light pointLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * speed, 1));
    }
}
