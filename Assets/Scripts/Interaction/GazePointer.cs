using System.Collections.Generic;
using UnityEngine;


public class StorageContainerHit
{
    public StorageContainerMono StorageContainerMono;
    public float LifeTicks;

    public StorageContainerHit(StorageContainerMono storageContainerMono, float lifeTicks)
    {
        this.StorageContainerMono = storageContainerMono;
        this.LifeTicks = lifeTicks;
    }

    public bool UpdateLife(float lifeTickChange, float maxLifeTicks)
    {
        LifeTicks += lifeTickChange;
        if (LifeTicks > maxLifeTicks)
            LifeTicks = maxLifeTicks;

        if (LifeTicks <= 0)
            return false;

        return true;
    }
}


public class GazePointer : MonoBehaviour
{

    [SerializeField] float _rayDistance;

    [SerializeField] float StartingLifeTime;
    [SerializeField] float _maxLifeTickTime;

    [SerializeField] float _firstStageDistance;

    [SerializeField] float _secondStageDistance;

    List<StorageContainerHit> storageContainerHits = new List<StorageContainerHit>();
    List<StorageContainerHit> deleteList = new List<StorageContainerHit>();


    void ShootRay()
    {

        Debug.DrawRay(transform.position, transform.forward*_rayDistance, Color.green);
        Ray ray = new Ray(transform.position, transform.forward); // Create a ray from the camera based on the mouse position
        RaycastHit hit; // Variable to store information about the raycast hit

        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            if (hit.collider.tag == "StorageContainer")
                RefreshHit(hit.transform.GetComponent<StorageContainerMono>());

        }
        else
        {
        }

    }

    void RefreshHit(StorageContainerMono storageContainerMono)
    {
        Debug.Log("Hit");

        StorageContainerHit storageContainerHit = storageContainerHits.Find(item => item.StorageContainerMono == storageContainerMono);
        if (storageContainerHit == null)
            storageContainerHits.Add(new StorageContainerHit(storageContainerMono, _maxLifeTickTime));
        else
            storageContainerHit.LifeTicks = _maxLifeTickTime;


    }

    void Update()
    {
        if(StorageContainerManager.Instance.IsInSetupPhase)
        return;


        ShootRay();

        foreach (StorageContainerHit storageContainerHit in storageContainerHits)
        {
            if (!storageContainerHit.UpdateLife(-Time.deltaTime, _maxLifeTickTime))
                deleteList.Add(storageContainerHit);
            CalculateDistance(storageContainerHit);
        }

        foreach (StorageContainerHit storageContainerHit in deleteList)
        {
            storageContainerHits.Remove(storageContainerHit);
        }
        deleteList.Clear();

    }

    void CalculateDistance(StorageContainerHit storageContainerHit)
    {
        float distance = Vector3.Distance(this.transform.position, storageContainerHit.StorageContainerMono.transform.position);

        if (distance <= _firstStageDistance && distance <= _secondStageDistance)
        {
            Debug.LogWarning("First Stage UI");
            storageContainerHit.StorageContainerMono.SetFirstStageUIActive(true);
            storageContainerHit.StorageContainerMono.SetSecondStageUIActive(true);
        }
        else if (distance <= _firstStageDistance && distance > _secondStageDistance)
        {
            Debug.LogWarning("Second Stage UI");
            storageContainerHit.StorageContainerMono.SetFirstStageUIActive(true);
            storageContainerHit.StorageContainerMono.SetSecondStageUIActive(false);
        }
        else
        {
            Debug.LogWarning("UI Off");
            storageContainerHit.StorageContainerMono.SetFirstStageUIActive(false);
            storageContainerHit.StorageContainerMono.SetSecondStageUIActive(false);
        }
    }







    bool IsObjectNearScreenCenter(Camera cam, GameObject obj)
    {
        Vector3 viewportPoint = cam.WorldToViewportPoint(obj.transform.position);
        float threshold = 0.1f; // Adjust this threshold to control how close to the center is considered "centered"
        return Mathf.Abs(viewportPoint.x - 0.5f) < threshold && Mathf.Abs(viewportPoint.y - 0.5f) < threshold && viewportPoint.z > 0;
    }
}
