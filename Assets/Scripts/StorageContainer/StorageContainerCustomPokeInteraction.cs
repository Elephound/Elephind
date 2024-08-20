using UnityEngine;
using Oculus.Interaction;

public class StorageContainerCustomPokeInteraction : MonoBehaviour
{

    [SerializeField] private StorageContainerMono storageContainerMono;

    bool isTouched;

    void OnTriggerEnter(Collider collider)
    {
        if(!collider.GetComponent<HandTag>())
        return;

        if(!StorageContainerManager.Instance.IsInSetupPhase)
            return;

        if(isTouched)
            return;

        isTouched=true;
        UpdateContainer();
    }

     void OnTriggerExit(Collider collider)
    {
       if(!collider.GetComponent<HandTag>())
        return;

        isTouched = false;

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
