using System.Collections.Generic;
using System.Linq;
using B83.Win32;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using Utilities;

public class FileDropZone : MonoBehaviour, Instance.IInstance<FileDropZone>
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI fileName;
    [SerializeField] public TMP_InputField filePath;
    const string INVALID_PATH = "Invalid Path";
    Regex regex = new Regex("");


    private void Awake()
    {
        this.SetInstance();
        icon.enabled = false;
        filePath.onValueChanged.AddListener(UpdateGUI);
    }

    public void SetValidExtensions(string[] exts)
    {
        Debug.Assert(exts.Length > 0);

        string pattern = "\\" + exts[0];
        for(int i = 1; i < exts.Length; i++)
        {
            pattern += "$|\\" + exts[i];
        }
        pattern += "$";

        regex = new Regex(pattern);
    }

    private static bool IsInDropZone(RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, DS_Environment.PlaySceneGameObjects.MainCamera.GetComponent<Camera>());
    }

    void OnEnable()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }

    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    public bool ValidFile()
    {
        if (!System.IO.File.Exists(filePath.text))
            return false;
        if (!regex.IsMatch(filePath.text))
            return false;
        return true;
    }

    static void UpdateGUI(string path)
    {
        bool active = Instance.IInstance<FileDropZone>.Instance.ValidFile();
        Instance.IInstance<FileDropZone>.Instance.icon.enabled = active;
        Instance.IInstance<FileDropZone>.Instance.fileName.text = active ? System.IO.Path.GetFileNameWithoutExtension(path) : INVALID_PATH;
    }


    static void OnFiles(List<string> aFiles, POINT aPos)
    {
        Debug.Log("Dropped " + aFiles.Count + " files at: " + aPos + "\n" + aFiles.Aggregate((a, b) => a + "\n" + b));
        if (IsInDropZone(Instance.IInstance<FileDropZone>.Instance.GetComponent<RectTransform>()))
        {
            string path = aFiles[0];
            Instance.IInstance<FileDropZone>.Instance.filePath.text = path;
            UpdateGUI(path);
        }
    }
}
