using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DS_Environment;
using System.Text.RegularExpressions;
using DG.Tweening;
public class TextInputPanel : Panel
{
    public TMP_InputField textInputField;
    private const float time = .1f;
    private const float strength = 40;
    private const int vibrato = 14;
    private const int loops = 4;


    protected override void Awake()
    {
        base.Awake();
    }

    public static TextInputPanel CreatePanel(string header, string prompt, System.Action<string> OnConfirm, List<string> unacceptableInputs)
    {
        return CreatePanel(header, prompt, OnConfirm,unacceptableInputs, StandardInputValidatator);
    }
    public static TextInputPanel CreatePanel(string header, string prompt, System.Action<string> OnConfirm, List<string> unacceptableInputs, System.Func<string, bool> textValidator)
    {
        GameObject panelObject = Instantiate(Resources.Load<GameObject>(ResourcePath.textInputPanel), PlaySceneGameObjects.Canvas.transform);
        TextInputPanel panel = panelObject.GetComponent<TextInputPanel>();
        panel.cancelButton.onClick.AddListener(() =>
        {
            panel.Delete();
        });
        panel.confirmButton.onClick.AddListener(() =>
        {
            string input = panel.textInputField.text;
            Utilities.String.RemoveZeroWidthSpace(ref input);
            if (textValidator(input) && !unacceptableInputs.Contains(input))
            {
                OnConfirm(input);
                panel.Delete();
            }
            else
            {
                panel.Shake();
            }
        });

        panel.header.text = header;
        panel.prompt.text = prompt;
        return panel;
    }

    //https://www.techiedelight.com/check-string-consists-alphanumeric-characters-csharp/#:~:text=Using%20Regular%20Expression,%2C%20use%20%2B%20instead%20of%20*%20.
    //What a dummy to forget about regular expressions
    private static readonly Regex alphaNumeric = new Regex("^[a-zA-Z0-9 _+=]+$");

    protected static readonly System.Func<string, bool> StandardInputValidatator = (string input) =>
    {
        if (input.Length > 50)
            return false;
        return alphaNumeric.IsMatch(input);
    };

    protected void Shake()
    {
        Sequence shake = DOTween.Sequence();
        for (int i = 0; i < loops; i++)
            shake.Append(body.DOShakeAnchorPos(time, strength, vibrato));
        shake.Play();
        
    }
}
