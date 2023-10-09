using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using DS_Environment;
public class FileInputPanel : TextInputPanel
{
    [SerializeField] FileDropZone fileDrop;

    public static FileInputPanel CreatePanel(string header, string prompt, System.Action<string, string> OnConfirm, List<string> unacceptableInputs, string[] acceptableFileExtensions)
    {
        return CreatePanel(header, prompt, OnConfirm, unacceptableInputs, acceptableFileExtensions, StandardInputValidatator);
    }
    public static FileInputPanel CreatePanel(string header, string prompt, System.Action<string,string> OnConfirm, List<string> unacceptableInputs, string[] acceptableFileExtensions, System.Func<string, bool> textValidator)
    {
        GameObject panelObject = Instantiate(Resources.Load<GameObject>(ResourcePath.fileInputPanel), PlaySceneGameObjects.Canvas.transform);
        FileInputPanel panel = panelObject.GetComponent<FileInputPanel>();
        panel.fileDrop.SetValidExtensions(acceptableFileExtensions);
        panel.cancelButton.onClick.AddListener(() =>
        {
            panel.Delete();
        });
        panel.confirmButton.onClick.AddListener(() =>
        {
            string downloadName = panel.textInputField.text;
            Utilities.String.RemoveZeroWidthSpace(ref downloadName);
            if (textValidator(downloadName) && !unacceptableInputs.Contains(downloadName) && panel.fileDrop.ValidFile())
            {
                OnConfirm(downloadName,panel.fileDrop.filePath.text);
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
}
