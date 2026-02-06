using UnityEngine;
using YourMaidTools;

public class TitleButton : MonoBehaviour
{
    [SerializeField] YMAnimationBase[] selectAnimations;
    public void Select()
    {
        foreach (var anim in selectAnimations)
        {
            anim.PlayAnimation(false);
        }
    }
    public void Unselect()
    {
        foreach (var anim in selectAnimations)
        {
            anim.ReverseAnimation(false);
        }
    }
}
