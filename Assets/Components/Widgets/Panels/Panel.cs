using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using DS_Environment;
public class Panel : MonoBehaviour, Instance.IInstance<Panel>
{
    protected RectTransform body;

    public TextMeshProUGUI header;
    public TextMeshProUGUI prompt;
    public Button cancelButton;
    public Button confirmButton;

    protected virtual void Awake()
    {
        this.SetInstance();
        body = GetComponentsInChildren<RectTransform>()[1];
    }

    public static Panel CreatePanel(string header, string prompt, System.Action OnConfirm)
    {
        GameObject panelObject = Instantiate(Resources.Load<GameObject>(ResourcePath.panel), PlaySceneGameObjects.Canvas.transform);
        Panel panel = panelObject.GetComponent<Panel>();
        panel.cancelButton.onClick.AddListener(() =>
        {
            panel.Delete();
        });
        panel.confirmButton.onClick.AddListener(() =>
        {
            OnConfirm();
            panel.Delete();
        });

        panel.header.text = header;
        panel.prompt.text = prompt;
        return panel;
    }
    public static Panel CreatePanel(string header, string prompt, System.Action OnConfirm, System.Action OnExit)
    {
        Panel panel = CreatePanel(header, prompt, OnConfirm);
        panel.cancelButton.onClick.AddListener(() => { OnExit(); });
        return panel;
    }
    //may delete twice
    public void Delete()
    {
        this.DeleteInstance();
    }
}
