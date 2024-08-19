using UnityEngine;
using Oculus.Interaction;

public class StorageContainerCustomPokeInteraction : MonoBehaviour
{
    [SerializeField]
    private PokeInteractable _pokeInteractable;

    [SerializeField] private StorageContainerMono storageContainerMono;



    protected virtual void OnEnable()
    {
        _pokeInteractable.WhenStateChanged += UpdateContainer;
    }

    protected virtual void OnDisable()
    {
        _pokeInteractable.WhenStateChanged -= UpdateContainer;
    }

    void UpdateContainer(InteractableStateChangeArgs args)
    {
        Debug.LogWarning("poked");

        if (_pokeInteractable.State == InteractableState.Select)
        {
            storageContainerMono.SetContainerAsActive();
            storageContainerMono.SetContainerActiveVisual(true);
            Debug.LogWarning("Active");
        }
        else if (_pokeInteractable.State == InteractableState.Normal)
        {
            storageContainerMono.SetContainerAsActive();
            storageContainerMono.SetContainerActiveVisual(false);
            Debug.LogWarning("normal");
        }

    }

}
