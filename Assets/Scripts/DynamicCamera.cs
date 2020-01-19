using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public float myDampTime = 0.2f;
    public float myScreenEdgeBuffer = 4f;
    public List<Transform> myPlayerTransforms = new List<Transform>();

    public Vector3 myOffset = new Vector3(0.0f, 28.0f, -20.0f);
    public float myDistanceOffset = 1.0f;

    private Camera myCamera;
    private Vector3 myMoveVelocity;
    private Vector3 myZoomVelocity;

    private TargetHandler myTargetHandler;
    private Subscriber mySubscriber;

    private void Awake()
    {
        myCamera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();

        ReplacePlayersToTarget(myTargetHandler);

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        //PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageType.PlayerResucitated);
    }

    private void OnDestroy()
    {
        //PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageType.PlayerResucitated);
    }

    public void ReplacePlayersToTarget(TargetHandler aTargetHandler)
    {
        myPlayerTransforms.Clear();

        List<GameObject> players = myTargetHandler.GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            myPlayerTransforms.Add(players[index].transform);
        }
    }

    private void FixedUpdate()
    {
        CalculateCameraFactors(out Vector3 centerPoint);
        transform.position = Vector3.SmoothDamp(transform.position, centerPoint, ref myMoveVelocity, myDampTime);

        //CalculateZoom(centerPoint);

        for (int index = 0; index < myPlayerTransforms.Count; index++)
        {
            Debug.DrawLine(myPlayerTransforms[index].position, centerPoint);
        }
    }

    private void CalculateCameraFactors(out Vector3 aCenterPoint)
    {
        aCenterPoint = Vector3.zero;
        for (int index = 0; index < myPlayerTransforms.Count; index++)
        {
            aCenterPoint += myPlayerTransforms[index].position;
        }

        if (myPlayerTransforms.Count > 0)
            aCenterPoint /= myPlayerTransforms.Count;
        else
            aCenterPoint = transform.position;
    }

    private void CalculateZoom(Vector3 aCenterPoint)
    {
        Bounds bounds = FindBounds();

        // Calculate distance to keep bounds visible. Calculations from:
        //     "The Size of the Frustum at a Given Distance from the Camera": https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
        //     note: Camera.fieldOfView is the *vertical* field of view: https://docs.unity3d.com/ScriptReference/Camera-fieldOfView.html

        Vector3 boundsLocal = myCamera.transform.InverseTransformVector(bounds.size);

        float desiredFrustumWidth = boundsLocal.x + 2 * myScreenEdgeBuffer;
        float desiredFrustumHeight = boundsLocal.y + 2 * myScreenEdgeBuffer;

        float distanceToFitHeight = desiredFrustumHeight * 0.5f / Mathf.Tan(myCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float distanceToFitWidth = desiredFrustumWidth * 0.5f / Mathf.Tan(myCamera.fieldOfView * myCamera.aspect * 0.5f * Mathf.Deg2Rad);

        float resultDistance = Mathf.Max(distanceToFitWidth, distanceToFitHeight);

        float minDistance = myOffset.magnitude;
        if (resultDistance < minDistance)
            resultDistance = minDistance;

        myCamera.transform.localPosition = Vector3.SmoothDamp(myCamera.transform.localPosition, -myCamera.transform.forward * resultDistance, ref myZoomVelocity, myDampTime);
    }

    private Bounds FindBounds()
    {
        if (myPlayerTransforms.Count == 0)
        {
            return new Bounds();
        }

        Bounds bounds = new Bounds(myPlayerTransforms[0].position, Vector3.zero);
        for (int index = 1; index < myPlayerTransforms.Count; index++)
        {
            bounds.Encapsulate(myPlayerTransforms[index].position);
        }

        return bounds;
    }

    private void ReceiveMessage(Message aMessage)
    {
        switch (aMessage.Type)
        {
            case MessageType.PlayerDied:
                for (int index = 0; index < myPlayerTransforms.Count; index++)
                {
                    if (myPlayerTransforms[index].gameObject.GetInstanceID() == aMessage.Data.myInt)
                    {
                        myPlayerTransforms.RemoveAt(index);
                        break;
                    }
                }
                break;
            case MessageType.PlayerResucitated:
                myPlayerTransforms.Add(myTargetHandler.GetPlayer(aMessage.Data.myInt).transform);
                break;
        }
    }
}
