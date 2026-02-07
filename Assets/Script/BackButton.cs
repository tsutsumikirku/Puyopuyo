using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] KeyCode backKey = KeyCode.Escape;
    [SerializeField] private SceneChanger1 sceneChanger;
    [SerializeField] private string backScene;
    [SerializeField] private Image image;
    float backKeyPushTime = 0.5f;
    private bool isPointerDown = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(backKey) || isPointerDown)
        {
            backKeyPushTime -= Time.deltaTime;
            if(backKeyPushTime <= 0f)
            {
                _ = sceneChanger.ChangeSceneAsync(backScene);
            }
            image.fillAmount = 1f - (backKeyPushTime / 0.5f);
        }
        else
        {
            backKeyPushTime += Time.deltaTime;
            if(backKeyPushTime >= 0.5f)
            {
                backKeyPushTime = 0.5f;
            }
            image.fillAmount = 1f - (backKeyPushTime / 0.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }
}
