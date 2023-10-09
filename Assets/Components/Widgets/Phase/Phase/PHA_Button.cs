using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PHA_Handler;
using Utilities;

[System.Serializable]
public class PHA_Button : Button, Instance.IEnumeratedInstance<PHA_Button, PHA_Option>
{
    [SerializeField] public PHA_Option type = PHA_Option.draw;
    public PHA_Option EnumeratedInstanceType => type;

    protected override void Awake()
    {
        this.SetInstance();
        onClick.AddListener(() => { PHA_Click(type); });

        if(type == DEFAULT_OPTION)
        {
            PHA_Current = type;
            Select();
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        //don't want any actions when deselected to maintain color
    }

    //not sure which is better here OnSelect or OnPointerClick
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (PHA_Current != PHA_Option.unassigned)
            Instance.GetInstanceOf<PHA_Button,PHA_Option>(PHA_Current).OnButtonDeselect(eventData);
        PHA_Current = type;
    }
    //both required to extend button
    public virtual void OnButtonDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }
}
