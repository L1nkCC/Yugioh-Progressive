using UnityEngine;
using DS_Environment;
using static DS_Environment.LinkArrow;
public class INS_Arrow : UnityEngine.UI.Image
{

    [SerializeField] public INS_ARW_Option arrowType;


    private static Sprite activeArrow;
    private static Sprite inactiveArrow;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        if(activeArrow==null)
            activeArrow = Resources.Load<Sprite>(ResourcePath.linkArrowActive);
        if(inactiveArrow == null)
            inactiveArrow = Resources.Load<Sprite>(ResourcePath.linkArrowInactive);

        enabled = false;
    }

    public void SetArrow(bool active)
    {
        sprite = active ? activeArrow : inactiveArrow;
    }
}
