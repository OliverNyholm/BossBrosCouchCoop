using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealth : PoolableObject
{
    private TextMesh myTextMesh;
    public float myDuration;
    public float mySpeed;

    [SerializeField]
    private Vector2 myRandomXRange = new Vector2(-2.0f, 2.0f);
    [SerializeField]
    private Vector2 myRandomYRange = new Vector2(1.0f, 2.0f);
    private Vector3 myInitialScale;

    private Camera myMainCamera;

    private float myLifeTime;
    public AnimationCurve myScaleDownCurve = null;

    private void Awake()
    {
        myTextMesh = transform.GetComponent<TextMesh>();

        myInitialScale = transform.localScale;
        myMainCamera = Camera.main;
    }

    void Update()
    {
        myTextMesh.transform.rotation = myMainCamera.transform.rotation;

        myLifeTime -= Time.deltaTime;
        if (myLifeTime <= 0.0f)
            ReturnToPool();

        transform.position += Vector3.up * mySpeed * Time.deltaTime;
    }

    public void SetText(string aText, Color aColor, Vector3 aSpawnLocation)
    {
        Vector3 randomOffset = new Vector2(Random.Range(myRandomXRange.x, myRandomXRange.y), Random.Range(myRandomYRange.x, myRandomYRange.y));
        transform.position = aSpawnLocation + randomOffset;

        myTextMesh.text = aText;
        myTextMesh.color = aColor;

        LeanTween.scale(gameObject, Vector3.zero, myLifeTime).setEase(myScaleDownCurve);
    }

    public override void Reset()
    {
        myLifeTime = myDuration;
        transform.localScale = myInitialScale;
    }
}
