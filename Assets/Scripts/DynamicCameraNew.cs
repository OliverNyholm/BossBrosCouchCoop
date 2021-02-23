using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class DynamicCameraNew : MonoBehaviour
{
    public float myDampTime = 0.2f;
    public float myFindZoomLocationSpeed = 10.0f;
    public float myZoomLerpSpeed = 5.0f;
    public float myScreenEdgeBuffer = 4f;
    public List<Transform> myPlayerTransforms = new List<Transform>();
    public List<Vector3> myPlayersScreenSpace = new List<Vector3>(4);

    private Vector3 myOffset = new Vector3(0.0f, 28.0f, -20.0f);
    public float myMinimumDistance;
    public float myDistanceOffset = 1.0f;

    private Camera myCamera;
    private CameraShaker myCameraShaker;
    private Vector3 myMoveVelocity;

    private TargetHandler myTargetHandler;
    private Subscriber mySubscriber;

    private void Awake()
    {
        myCamera = GetComponentInChildren<Camera>();
        myCameraShaker = GetComponentInChildren<CameraShaker>();
        myCameraShaker.RestPositionOffset = myCamera.transform.localPosition;
        myCameraShaker.RestRotationOffset = myCamera.transform.rotation.eulerAngles;
    }

    private void Start()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();

        AddPlayers();

        myOffset = myCamera.transform.localPosition;
        myMinimumDistance = myOffset.magnitude;

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;
        //PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.PlayerResucitated);
    }

    private void OnDestroy()
    {
        //PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.PlayerResucitated);
    }

    public void AddPlayers()
    {
        List<GameObject> players = myTargetHandler.GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            myPlayerTransforms.Add(players[index].transform);
        }
    }

    private void LateUpdate()
    {
        CalculateCameraFactors(out Vector3 centerPoint);
        transform.position = Vector3.SmoothDamp(transform.position, centerPoint, ref myMoveVelocity, myDampTime);

        bool isOutsideViewport = false;
        float closestEdgePosition = 0.0f;
        for (int index = 0; index < myPlayersScreenSpace.Count; index++)
        {
            if (IsOutsideViewport(myPlayersScreenSpace[index], out float edgePosition))
            {
                isOutsideViewport = true;
                break;
            }

            if (edgePosition > closestEdgePosition)
                closestEdgePosition = edgePosition;
        }

        if (isOutsideViewport)
        {
            myOffset += -myCamera.transform.forward * myFindZoomLocationSpeed * Time.deltaTime;
        }
        else
        {
            float distanceToCamera = myOffset.magnitude;
            if (distanceToCamera > myMinimumDistance && closestEdgePosition < 0.3f)
                myOffset += myCamera.transform.forward * myFindZoomLocationSpeed * Time.deltaTime;
        }

        myCamera.transform.localPosition = Vector3.Lerp(myCamera.transform.localPosition, myOffset, myZoomLerpSpeed * Time.deltaTime);
        myCameraShaker.RestPositionOffset = myCamera.transform.localPosition;
    }

    private void CalculateCameraFactors(out Vector3 aCenterPoint)
    {
        myPlayersScreenSpace.Clear();

        aCenterPoint = Vector3.zero;
        for (int index = 0; index < myPlayerTransforms.Count; index++)
        {
            aCenterPoint += myPlayerTransforms[index].position;
            myPlayersScreenSpace.Add(myCamera.WorldToScreenPoint(myPlayerTransforms[index].position));
        }

        if (myPlayerTransforms.Count > 0)
            aCenterPoint /= myPlayerTransforms.Count;
        else
            aCenterPoint = transform.position;
    }

    private bool IsOutsideViewport(Vector3 aScreenSpace, out float aClosestEdgePosition)
    {
        Vector3 viewportPosition = myCamera.ScreenToViewportPoint(aScreenSpace);


        Vector3 centeredZeroPosition = viewportPosition - Vector3.one * 0.5f;

        aClosestEdgePosition = Mathf.Abs(centeredZeroPosition.x);
        if(aClosestEdgePosition < Mathf.Abs(centeredZeroPosition.y))
            aClosestEdgePosition = Mathf.Abs(centeredZeroPosition.y);

        return viewportPosition.x < 0.075f || viewportPosition.y < 0.075f || viewportPosition.x > 0.925f || viewportPosition.y > 0.925f;
    }

    private void ReceiveMessage(Message aMessage)
    {
        switch (aMessage.Type)
        {
            case MessageCategory.PlayerDied:
                for (int index = 0; index < myPlayerTransforms.Count; index++)
                {
                    if (myPlayerTransforms[index].gameObject.GetInstanceID() == aMessage.Data.myInt)
                    {
                        myPlayerTransforms.RemoveAt(index);
                        break;
                    }
                }
                break;
            case MessageCategory.PlayerResucitated:
                myPlayerTransforms.Add(myTargetHandler.GetPlayer(aMessage.Data.myInt).transform);
                break;
        }
    }
}
