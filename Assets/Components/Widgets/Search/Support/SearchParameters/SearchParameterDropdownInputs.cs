using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DS_Environment;
using Utilities;
using System.Linq;
public class SearchParameterDropdownInputs : MonoBehaviour
{
    TMP_Dropdown MainCardTypeDropdown;
    List<(SearchParameters.SRH_Option, TMP_Dropdown)> parameterDropdown = new();

    private static readonly List<string> cardMainTypeOptions = new List<string>()
        {
            "Monster",
            "Spell",
            "Trap",
        };

    private void Awake()
    {
        CreateMainCardTypeDropdown();
        MainCardTypeDropdown.onValueChanged.AddListener((int optionIndex) => Activate(optionIndex));
    }

    private void Activate(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0:
                Clear();
                break;
            case 1:
                CreateMonsterRaceDropdown();
                CreateMonsterTypeDropdown();
                break;
            case 2:
                CreateSpellRaceDropdown();
                break;
            case 3:
                CreateTrapRaceDropdown();
                break;
            default:
                break;
        }
    }
    private void Clear()
    {
        foreach((SearchParameters.SRH_Option option, TMP_Dropdown dpd) in parameterDropdown)
        {
            Destroy(dpd.gameObject);
        }
        parameterDropdown.RemoveRange(0, parameterDropdown.Count);
    }

    private void CreateMainCardTypeDropdown()
    {
        if (MainCardTypeDropdown == null)
            MainCardTypeDropdown = CreateDropdown(cardMainTypeOptions);
    }

    private void CreateMonsterTypeDropdown()
    {
        CreateDropdownInList(CardType.monsterTypes.ToList(), SearchParameters.SRH_Option.type);
    }

    private void CreateMonsterRaceDropdown()
    {
        CreateDropdownInList(CardRace.monsterRaces.ToList(), SearchParameters.SRH_Option.race);
    }
    private void CreateSpellRaceDropdown()
    {
        CreateDropdownInList(CardRace.spellRaces.ToList(), SearchParameters.SRH_Option.race);
    }
    private void CreateTrapRaceDropdown()
    {
        CreateDropdownInList(CardRace.trapRaces.ToList(), SearchParameters.SRH_Option.race);
    }


    private TMP_Dropdown CreateDropdown(List<string> options)
    {
        TMP_Dropdown newDropdown = Instantiate(Resources.Load<GameObject>(ResourcePath.dropdown), this.transform).GetComponent<TMP_Dropdown>();
        SearchManager.FillDropdown(newDropdown, options);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        return newDropdown;
    }

    private TMP_Dropdown CreateDropdownInList(List<string> options, SearchParameters.SRH_Option parameterOption)
    {
        TMP_Dropdown newDropdown = CreateDropdown(options);
        parameterDropdown.Add((parameterOption, newDropdown));
        return newDropdown;
    }

    public SearchRequest GetSearchRequestParameters()
    {
        SearchRequest request = new();
        foreach ((SearchParameters.SRH_Option option, TMP_Dropdown dropdown) in parameterDropdown)
        {
            string value = dropdown.options.ElementAt(dropdown.value).text;
            if (!SearchManager.REMOVE_PARAMETER_VALUE.Equals(value))
            {
                request.Add(option, value);
            }
        }
        return request;
    }

}
