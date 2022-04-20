using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    PointerEventData pointerData;

    private void Update()
    {
        transform.Translate(0,0,speed * Time.deltaTime,Space.World);
    }

    public void TouchDown(BaseEventData data)
    {
        pointerData = data as PointerEventData;
        MeshGeneration.Instance.Generate();
    }

    public void TouchUp()
    {
        MeshGeneration.Instance.StopScraping();
    }
}
