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

    [SerializeField] Camera mainCamera;



    void ShootRay()
    {

        Ray ray = new Ray(transform.position, transform.forward); // Create a ray from the camera based on the mouse position
        RaycastHit hit; // Variable to store information about the raycast hit

        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            if (hit.collider.tag == "StorageContainer")
                RefreshHit(hit.transform.GetComponentInParent<StorageContainerMono>());

        }
        else
        {
        }

    }

    void RefreshHit(StorageContainerMono storageContainerMono)
    {
        StorageContainerHit storageContainerHit = storageContainerHits.Find(item => item.StorageContainerMono == storageContainerMono);
        if (storageContainerHit == null)
        {
            storageContainerHits.Add(new StorageContainerHit(storageContainerMono, _maxLifeTickTime));
        }
        else
        {
            storageContainerHit.LifeTicks = _maxLifeTickTime;
        }


    }

    void Update()
    {

        foreach (StorageContainerHit storageContainerHit in storageContainerHits)
        {
            storageContainerHit.StorageContainerMono.SetUIActive(IsInView(storageContainerHit.StorageContainerMono.gameObject));
            CalculateDistance(storageContainerHit);
        }


      //  ShootRay();
    }

    void CalculateDistance(StorageContainerHit storageContainerHit)
    {
        float distance = Vector3.Distance(this.transform.position, storageContainerHit.StorageContainerMono.transform.position);

        if (distance <= _firstStageDistance && distance <= _secondStageDistance)
        {
            storageContainerHit.StorageContainerMono.SetFirstStageUIActive(true);
            storageContainerHit.StorageContainerMono.SetSecondStageUIActive(true);
        }
        else if (distance <= _firstStageDistance && distance > _secondStageDistance)
        {
            storageContainerHit.StorageContainerMono.SetFirstStageUIActive(true);
            storageContainerHit.StorageContainerMono.SetSecondStageUIActive(false);
        }
        else
        {
            storageContainerHit.StorageContainerMono.SetFirstStageUIActive(false);
            storageContainerHit.StorageContainerMono.SetSecondStageUIActive(false);
        }
    }




    bool IsInView(GameObject target)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(target.transform.position);

        bool isInView = viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
                        viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
                        viewportPosition.z > 0;

        return isInView;
    }
}
