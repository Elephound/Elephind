using UnityEngine;

public class SpatialAnchorRelay : MonoBehaviour
{
    public void RelayCreationEvent(OVRSpatialAnchor oVRSpatialAnchor, OVRSpatialAnchor.OperationResult operationResult)
    {
        oVRSpatialAnchor.transform.GetComponent<ActivateStorageObject>().ActivateNow();
    }
}
