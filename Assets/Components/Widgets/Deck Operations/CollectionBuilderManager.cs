using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using ImportDeckUtilities;
using static ListSave;
using System.Linq;


public class CollectionBuilderManager : MonoBehaviour, Instance.IInstance<CollectionBuilderManager>
{

    [SerializeField] Zone deckZone;
    [SerializeField] Zone sideZone;
    [SerializeField] Zone extraZone;
    [SerializeField] ListType listType;
    [SerializeField] TMPro.TMP_Dropdown DeckSelecter;
    [SerializeField] Button SetDefaultButton;
    [SerializeField] Button CreateButton;
    [SerializeField] Button ImportButton;
    [SerializeField] Button SaveButton;
    [SerializeField] Button RenameButton;
    [SerializeField] Button CopyButton;
    [SerializeField] Button DeleteButton;

    private string currentDeckName => DeckSelecter.options.ElementAt(DeckSelecter.value).text;
    const string COPY_SUFFIX = "(COPY)";

    private readonly string[] ACCEPTABLE_IMPORT_EXT = new string[] { CSV.EXTENSION, YDK.EXTENSION };

    private void Awake()
    {
        this.SetInstance();

        if(DeckSelecter != null) DeckSelecter.onValueChanged.AddListener((int i) => { LoadDeck(); });

        if(SetDefaultButton != null) SetDefaultButton.onClick.AddListener(() => { SetDefaultDeck(); });
        if(CreateButton != null) CreateButton.onClick.AddListener(() => { CreateDeck(); });
        if(ImportButton != null) ImportButton.onClick.AddListener(() => { ImportDeck(); });
        if(SaveButton != null) SaveButton.onClick.AddListener(() => { SaveDeck(); });
        if(RenameButton != null) RenameButton.onClick.AddListener(() => { RenameDeck(); });
        if(CopyButton != null) CopyButton.onClick.AddListener(() => { CopyDeck(); });
        if(DeleteButton != null) DeleteButton.onClick.AddListener(() => { DeleteDeck(); });
    }

    private void Start()
    {
        SetDeckSelection(ListSave.GetDefaultList(listType).name);
    }
    private void SetDeckSelection(string ListName)
    {
        DeckSelecter.ClearOptions();
        List<string> names = ListSave.GetListNames(listType).ToList();
        DeckSelecter.AddOptions(names);
        DeckSelecter.value = names.IndexOf(ListName);
        LoadDeck();
    }

    private System.Action<string> rename => new System.Action<string>((string newName) =>
    {
        CardCollectionList newRenamedList = new CardCollectionList(LoadList(currentDeckName, listType))
        {
            name = newName
        };
        SaveList(new CardCollectionList(newRenamedList));
        DeleteList(currentDeckName, listType);
        SetDeckSelection(newName);
    });
    private System.Action<string> create => (string name) =>
    {
        ListSave.SaveList(new ListSave.CardCollectionList(name, listType));
        SetDeckSelection(name);
    };
    private System.Action<string,string> import => (string name, string path) =>
    {
        string extension = System.IO.Path.GetExtension(path);
        Debug.LogError("IMPORT");
        if (!ACCEPTABLE_IMPORT_EXT.Contains(extension))
        {
            Debug.LogError("IMPORT Unacceptable extention");
            throw new FileExtensionException();
        }

        CardCollectionList list;
        list.listType = ListType.fail;
        switch (extension)
        {
            case CSV.EXTENSION:
                Debug.LogError("IMPORT parseCSV");
                list = CSV.ParseCSV(path,listType,name);
                break;
            case YDK.EXTENSION:
                Debug.LogError("IMPORT parseYDK");
                list = YDK.ParseYDK(path, listType, name);
                break;
            default:
                Debug.LogError("IMPORT EXT failure");
                throw new FileExtensionException();
        }
        if(list.listType != ListType.fail)
        {
            ListSave.SaveList(list);
            SetDeckSelection(list.name);
        }
        
    };
    private System.Action copy => () =>
    {
        ListSave.CardCollectionList copiedList = ListSave.LoadList(currentDeckName, listType);
        copiedList.name += COPY_SUFFIX;
        ListSave.SaveList(copiedList);
        SetDeckSelection(copiedList.name);
    };
    private System.Action delete => () =>
    {
        ListSave.DeleteList(currentDeckName, listType);
        SetDeckSelection(GetDefaultList(listType).name);
    };

    private void RenameDeck()
    {
        TextInputPanel panel = TextInputPanel.CreatePanel("RENAME DECK", "Please give a unique name for " + currentDeckName, rename, GetListNames(listType).ToList());
    }
    private void CreateDeck()
    {
        TextInputPanel panel = TextInputPanel.CreatePanel("CREATE DECK", "Please give a unique name for the new Deck", create, GetListNames(listType).ToList());
    }
    private void ImportDeck()
    {
        FileInputPanel panel = FileInputPanel.CreatePanel("IMPORT DECK", "Please Drag your file inside", import, GetListNames(listType).ToList(), ACCEPTABLE_IMPORT_EXT);
    }
    private void CopyDeck()
    {
        Panel panel = Panel.CreatePanel("COPY DECK", "Copy " + currentDeckName + "?", copy);
    }
    private void DeleteDeck()
    {
        Panel panel = Panel.CreatePanel("COPY DECK", "Delete " +currentDeckName +"?", delete);
    }


    private void SetDefaultDeck()
    {
        Debug.Log("SetDefaultDeck");
        ListSave.SetDefaultList(currentDeckName, listType);
        foreach(TMPro.TMP_Dropdown.OptionData optionData in DeckSelecter.options)
        {
            optionData.image = null;
        }

        DeckSelecter.options.ElementAt(DeckSelecter.value).image = (SetDefaultButton.GetComponent<Image>().sprite);
    }

    private void LoadDeck()
    {
        ListSave.CardCollectionList List = ListSave.LoadList(currentDeckName, listType);
        if (List.listType == ListType.fail)
            List = new ListSave.CardCollectionList(listType);
        if(deckZone != null) deckZone.Load(List.mainDeck);
        if(sideZone != null) sideZone.Load(List.sideDeck);
        if(extraZone != null) extraZone.Load(List.extraDeck);
    }

    private void SaveDeck()
    {
        Debug.Log("SaveDeck");
        Card[] MainDeckCards = deckZone.cards.ToArray();
        Card[] SideDeckCards = sideZone.cards.ToArray();
        Card[] ExtraDeckCards = extraZone.cards.ToArray();

        CardCollectionList List = new(currentDeckName,listType, CardCollectionList.GetIds(MainDeckCards), CardCollectionList.GetIds(SideDeckCards), CardCollectionList.GetIds(ExtraDeckCards));
        Debug.Log(List);

        ListSave.SaveList(List);
    }









}
