using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DS_Environment.SearchParameters;
using System.Linq;
public class SearchManager : MonoBehaviour
{
    [SerializeField] ListSave.ListType listType;
    [SerializeField] TMPro.TMP_InputField nameInput;
    [SerializeField] TMPro.TMP_InputField descInput;
    [SerializeField] TMPro.TMP_Dropdown formatInput;
    [SerializeField] Sprite customFormatToggleSprite;
    [SerializeField] Sprite constantFormatToggleSprite;
    
    [SerializeField] UnityEngine.UI.Toggle formatToggle;//on is constant formats, off is Custom formats
    SearchParameterDropdownInputs dropdownInputs;
    UnityEngine.UI.Button requestButton;
    BuilderSearchZone searchZone;

    private string currentFormat => formatInput.options.ElementAt(formatInput.value).text;
    public const string REMOVE_PARAMETER_VALUE = "Any";
    private readonly string[] CONSTANT_FORMATS =
    {
        "TCG",
        "OCG",
        "GOAT",
        "OCG GOAT",
        "Edison",
        "Rush Duel",
        "Speed Duel",
        "Duel Links",
        "Common Charity",
    };

    void Awake()
    {
        requestButton = GetComponentInChildren<UnityEngine.UI.Button>();
        searchZone = GetComponentInChildren<BuilderSearchZone>();
        dropdownInputs = GetComponentInChildren<SearchParameterDropdownInputs>();
        requestButton.onClick.AddListener(() => Search());

        if (formatInput != null)
        {
            formatToggle = formatInput.gameObject.GetComponentInChildren<UnityEngine.UI.Toggle>();
            formatToggle.onValueChanged.AddListener((bool state) => UpdateDropdown(state));
            UpdateDropdown(formatToggle.isOn);

            formatInput.onValueChanged.AddListener((int i) =>
            {
                if (!formatToggle.isOn)
                {
                    ListSave.CardCollectionList List = ListSave.LoadList(currentFormat, listType);
                    Debug.Log(currentFormat);
                    if (List.listType == ListSave.ListType.fail)
                        List = new ListSave.CardCollectionList(listType);
                    searchZone.Load((List.mainDeck.ToList().Concat(List.sideDeck).Concat(List.extraDeck)).ToArray());
                }
            });
        }
    }

    //does not search  or limit extra/side or limited/banned zones
    private void Search()
    {
        SearchRequest request = new();
        request.Add(SRH_Option.fname, nameInput.text);
        request.Add(SRH_Option.desc, descInput.text);
        request.Add(dropdownInputs.GetSearchRequestParameters());

        CardInfo[] deckInfo;

        if (formatInput == null)
        {
            Debug.Log("Local search");
            deckInfo = searchZone.results;
            deckInfo = request.GetSearchedCollection(ref deckInfo);
            searchZone.Filter(deckInfo);
        }
        else
        {


            if (formatToggle.isOn && !currentFormat.Equals(REMOVE_PARAMETER_VALUE))
            {
                request.Add(SRH_Option.format, currentFormat);
            }

            if (formatToggle.isOn)
            {
                Debug.Log("Online Search");
                deckInfo = API_InfoHandler.GetSearchInfo(request);
            }
            else
            {
                Debug.Log("Local search");
                deckInfo = API_InfoHandler.GetCardInfo(ListSave.LoadList(currentFormat, ListSave.ListType.pool).mainDeck);
                deckInfo = request.GetSearchedCollection(ref deckInfo);
            }
            searchZone.Load(deckInfo);
        }


        
    }


    private void UpdateDropdown(bool state)
    {
        if (state)
        {
            FillDropdown(formatInput, CONSTANT_FORMATS.ToList());
            formatToggle.image.sprite = constantFormatToggleSprite;
        }
        else
        {
            FillDropdown(formatInput, ListSave.GetListNames(listType).ToList());
            formatToggle.image.sprite = customFormatToggleSprite;
        }
    }

    public static void FillDropdown(TMPro.TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        options.Insert(0, REMOVE_PARAMETER_VALUE);
        dropdown.AddOptions(options);
    }
}
