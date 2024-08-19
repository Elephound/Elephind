using UnityEngine;
using Oculus.Interaction;

public class StorageContainerCustomPokeInteraction : MonoBehaviour
{

    [SerializeField] private StorageContainerMono storageContainerMono;


    void OnTriggerEnter(Collider collider)
    {
        if(!collider.GetComponent<HandTag>())
        return;

        if(!StorageContainerManager.Instance.IsInSetupPhase)
            return;

        UpdateContainer();
    }

     void OnTriggerExit(Collider collider)
    {

    }


    void UpdateContainer()
    {

        if (storageContainerMono.IsActivated)
        {
            storageContainerMono.SetContainerAsActive(false);
        }
        else 
        {
            storageContainerMono.SetContainerAsActive(true);
        }

    }

}
