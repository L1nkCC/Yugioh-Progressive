using System.IO;
using UnityEngine;
using DS_Environment;

public class SFX_Manager : MonoBehaviour
{
    [SerializeField] AudioClip[] BTN_SFX = new AudioClip[Utilities.Enum.Length(typeof(BTN_Option))];
    [SerializeField] AudioClip[] PHA_SFX = new AudioClip[Utilities.Enum.Length(typeof(PHA_Handler.PHA_Option))];
    [SerializeField] AudioClip UI_BTN_CLICK_SFX;
    [SerializeField] AudioClip UI_BTN_HOVER_SFX;
    [SerializeField] AudioClip FlipCoinSFX;
    [SerializeField] AudioClip RollDieSFX;
    [SerializeField] AudioClip GainLifeSFX;
    [SerializeField] AudioClip LoseLifeSFX;

    public bool muted = false;


    public enum SFX_Option
    {
        master = 0,
        duelAction,
        duelTool,
        phaseChange,
        lifeChange,
        UIEffect,
        backgroundMusic,
    }

    public float[] volumeSettings = new float[System.Enum.GetValues(typeof(SFX_Option)).Length];


    AudioSource source;
    private void Awake()
    {
        LoadVolumeSettings();

        source = GetComponent<AudioSource>();
        foreach (BTN_Option option in System.Enum.GetValues(typeof(BTN_Option)))
        {
            BTN_Handler.BTN_Assign(option, (Card card) => {PlaySFX(BTN_SFX[(int)option], SFX_Option.duelAction); });
        }
        foreach(PHA_Handler.PHA_Option option in System.Enum.GetValues(typeof(PHA_Handler.PHA_Option)))
        {
            PHA_Handler.PHA_Assign(option, () => { PlaySFX(PHA_SFX[(int)option], SFX_Option.phaseChange); });
        }
        
    }

    private void PlaySFX(AudioClip clip, SFX_Option option)
    {
        if (source == null) { source = GetComponent<AudioSource>(); }

        if (muted) { return; }

        if (option == SFX_Option.master)
        {
            Debug.LogWarning("SFX_Option passed to playSFX was master. This will cause a sound to be unassigned");
            source.PlayOneShot(clip, volumeSettings[(int)SFX_Option.master]);
        }
        else
        {
            source.PlayOneShot(clip, volumeSettings[(int)SFX_Option.master] * volumeSettings[(int)option]);
        }
    }
    private float GetVolume(SFX_Option option)
    {
        return volumeSettings[(int)option];
    }

    public void PlayUI_BTN_CLICK()
    {
        PlaySFX(UI_BTN_CLICK_SFX, SFX_Option.UIEffect);
    }
    public void PlayUI_BTN_HOVER()
    {
        PlaySFX(UI_BTN_HOVER_SFX, SFX_Option.UIEffect);
    }

    public void PlayFlipCoinSFX()
    {
        PlaySFX(FlipCoinSFX, SFX_Option.duelTool);
    }
    public void PlayRollDieSFX()
    {
        PlaySFX(RollDieSFX, SFX_Option.duelTool);
    }

    public void PlayGainLifeSFX()
    {
        PlaySFX(GainLifeSFX, SFX_Option.lifeChange);
    }
    public void PlayLoseLifeSFX()
    {
        PlaySFX(LoseLifeSFX, SFX_Option.lifeChange);
    }




    //Save Sound Settings
    //Save Data to drive
    private static string ROOT_PATH => Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SFX_Settings";
    private const string FILE_NAME = "SFX_Settings.json";

    [System.Serializable]
    struct SFX_save
    {
        public float[] volumeSettings;
        public bool muted;

        public SFX_save(float[] _volumeSettings, bool _muted)
        {
            volumeSettings = _volumeSettings;
            muted = _muted;
        }
    }

    public static bool SaveVolumeSettings()
    {
        SFX_Manager sfx = PersistantScripts.SFX_Manager;
        if (sfx.volumeSettings.Length != System.Enum.GetValues(typeof(SFX_Option)).Length)
        {
            throw new System.Exception("VolumeSettings Length to save is not the correct length");
        }

        SFX_save saveData = new(sfx.volumeSettings, sfx.muted);

        return SaveVolumeSettings(saveData);
    }

    private static bool SaveVolumeSettings(SFX_save saveData, int attempt = 100)
    {
        if (attempt < 0)
        {
            return false;
        }

        string json = JsonUtility.ToJson(saveData);
        Utilities.IO.AssureDirectory(ROOT_PATH);
        try
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(ROOT_PATH, FILE_NAME)))
            {
                writer.Write(json);
                writer.Close();
            }
            return true;
        }
        catch (IOException)
        {
            return SaveVolumeSettings(saveData, attempt - 1);
        }
    }

    public static bool LoadVolumeSettings()
    {
        float[] baseVolumeSettings = new float[System.Enum.GetValues(typeof(SFX_Manager.SFX_Option)).Length];
        for (int i = 0; i < baseVolumeSettings.Length; i++) baseVolumeSettings[i] = 1f;
        SFX_save saveData = new(baseVolumeSettings, false);

        SFX_Manager sfx = PersistantScripts.SFX_Manager;
        Utilities.IO.AssureDirectory(ROOT_PATH);
        try
        {
            using (StreamReader reader = new StreamReader(Path.Combine(ROOT_PATH, FILE_NAME)))
            {
                string json = reader.ReadToEnd();
                saveData = JsonUtility.FromJson<SFX_save>(json);
            }
            sfx.volumeSettings = saveData.volumeSettings;
            sfx.muted = saveData.muted;
            return true;
        }
        catch (IOException)
        {
            sfx.volumeSettings = saveData.volumeSettings;
            sfx.muted = saveData.muted;
            return false;
        }
    }

}
