using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Linq;
//duel scene environment
namespace DS_Environment
{ 
    public static class SceneManagement
    {
        public static bool inPlayScene()
        {
            return SceneManager.GetActiveScene().buildIndex == (int)BuildIndex.soloPlay || SceneManager.GetActiveScene().buildIndex == (int)BuildIndex.multiPlay;
        }
        public static bool inSoloPlayScene()
        {
            return SceneManager.GetActiveScene().buildIndex == (int)BuildIndex.soloPlay;
        }
        public static bool inMultiPlayScene()
        {
            return SceneManager.GetActiveScene().buildIndex == (int)BuildIndex.multiPlay;
        }
        public enum BuildIndex
        {
            quit = -2,
            previous = -1,
            boot = 0,
            mainMenu,
            soloPlay,
            multiPlay,
            deckConstructor,
            collectionConstructor,
            soundOptions,
            loading,
            findLobby,
        }
    }
    public enum DuelistDesignation
    {
        Player = 0,
        Opponent = 1,
    }
    public static class ZoneType
    {
        public const int MAIN_ZONE_COUNT = 5;
        public const int PEND_ZONE_COUNT = 2;
        public const int PLAYER_ZONE_COUNT = 17;
        public enum ZON_Option
        {
            handZone = 0,
            mainDeckZone = 1,
            extraDeckZone = 2,
            graveyardZone = 3,
            banishZone = 4,
            fieldSpellZone = 5,
            linkZone = 6,
            mainMonsterZone = 7,
            mainBackrowZone = 12,
            pendulumZone = 15,
            unassigned = 17,
            b_mainDeckZone = 18,
            b_sideDeckZone = 19,
            b_extraDeckZone = 20,
            b_searchZone = 21,
            b_poolZone = 22,
        }

        public static string ZON_Name(ZON_Option option)
        {
            return option switch
            {
                ZON_Option.banishZone => "banish",
                ZON_Option.extraDeckZone => "extraDeck",
                ZON_Option.fieldSpellZone => "fieldSpell",
                ZON_Option.graveyardZone => "graveyard",
                ZON_Option.handZone => "hand",
                ZON_Option.linkZone => "link",
                ZON_Option.mainBackrowZone => "backrow",
                ZON_Option.mainDeckZone => "deck",
                ZON_Option.mainMonsterZone => "monster",
                ZON_Option.pendulumZone => "pendulum",
                ZON_Option.b_mainDeckZone => "builder main",
                ZON_Option.b_sideDeckZone => "builder side",
                ZON_Option.b_extraDeckZone => "builder extra",
                ZON_Option.b_searchZone => "builder search",
                _ => "unassigned",
            };
        }

        private static readonly HashSet<ZON_Option> onFieldTypes = new()
        {
            ZON_Option.mainMonsterZone,
            ZON_Option.mainBackrowZone,
            ZON_Option.linkZone,
            ZON_Option.fieldSpellZone
        };
        public static bool IsOnFieldType(this ZON_Option option)
        {
            return onFieldTypes.Contains(option);
        }
        

    }

    public static class SearchParameters
    {
        public enum SRH_Option
        {
            fname,
            desc,
            type,
            atk,
            def,
            level,
            race,
            attribute,
            link,
            linkMarkers,
            scale,
            cardset,
            archetype,
            banlist,
            format,
            misc,
            staple,
            has_effect,
            startdate,
            enddate,
            dateregion,
        }

        public static string SRH_Name(SRH_Option option)
        {
            return option switch
            {
                SRH_Option.fname => "fname",
                SRH_Option.type => "type",
                SRH_Option.atk => "atk",
                SRH_Option.def => "def",
                SRH_Option.level => "level",
                SRH_Option.race => "race",
                SRH_Option.attribute => "attribute",
                SRH_Option.link => "link",
                SRH_Option.linkMarkers => "linkmarker",
                SRH_Option.scale => "scale",
                SRH_Option.cardset => "cardset",
                SRH_Option.archetype => "archetype",
                SRH_Option.banlist => "banlist",
                SRH_Option.format => "format",
                SRH_Option.misc => "misc",
                SRH_Option.staple => "staple",
                SRH_Option.has_effect => "has_effect",
                SRH_Option.startdate => "startdate",
                SRH_Option.enddate => "enddate",
                SRH_Option.dateregion => "dateregion",
                _ => "unassigned",
            };
        }
    }

    public enum BTN_Option
    {
        unassigned,
        declare,
        normalSummon,
        flipSummon,
        specialSummonAttack,
        specialSummonDefense,
        set,
        flip,
        activate,
        toGraveyard,
        toBanish,
        toBanishFacedown,
        toExtraDeck,
        toTopOfDeck,
        mill,
        shuffleDeck,
        toSpellTrap,
        move,
        toHand,
        draw,
        inspect,
        reveal,
        toLink,
        changeBattlePosition,
        attack,
        view,
        toFieldZone,
        toBottomOfDeck,
    }

    public enum CMD_Option
    {
        unassigned = 0,
        mill,
        draw,
        toBanish,
        toBanishFacedown,
        reveal,
    }

    public static class CardRace
    {
        public static readonly HashSet<string> allRaces = new()
        {
            //monster races
            "Aqua",
            "Beast",
            "Beast-Warrior",
            "Creator-God",
            "Cyberse",
            "Dinosaur",
            "Divine-Beast",
            "Dragon",
            "Fairy",
            "Fiend",
            "Fish",
            "Insect",
            "Machine",
            "Plant",
            "Psychic",
            "Pyro",
            "Reptile",
            "Rock",
            "Sea Serpent",
            "Spellcaster",
            "Thunder",
            "Warrior",
            "Winged Beast",
            "Wyrm",
            "Zombie",
            //spellTrap
            "Normal",
            "Field",
            "Equip",
            "Continuous",
            "Quick - Play",
            "Ritual",
            "Counter",
        };

        public static readonly HashSet<string> monsterRaces = new()
        {
            "Aqua",
            "Beast",
            "Beast-Warrior",
            "Creator-God",
            "Cyberse",
            "Dinosaur",
            "Divine-Beast",
            "Dragon",
            "Fairy",
            "Fiend",
            "Fish",
            "Insect",
            "Machine",
            "Plant",
            "Psychic",
            "Pyro",
            "Reptile",
            "Rock",
            "Sea Serpent",
            "Spellcaster",
            "Thunder",
            "Warrior",
            "Winged Beast",
            "Wyrm",
            "Zombie",
        };
        public static readonly HashSet<string> spellRaces = new()
        {
            "Normal",
            "Field",
            "Equip",
            "Continuous",
            "Quick - Play",
            "Ritual",
        };
        public static readonly HashSet<string> trapRaces = new()
        {
            "Normal",
            "Continuous",
            "Counter",
        };

    }

    public static class CardType
    {
        #region Classifying Types
        private static readonly HashSet<string> allTypes = new()
        {
            "Effect Monster",
            "Flip Effect Monster",
            "Flip Tuner Effect Monster",
            "Gemini Monster",
            "Normal Monster",
            "Normal Tuner Monster",
            "Pendulum Effect Monster",
            "Pendulum Effect Ritual Monster",
            "Pendulum Flip Effect Monster",
            "Pendulum Normal Monster",
            "Pendulum Tuner Effect Monster",
            "Ritual Effect Monster",
            "Ritual Monster",
            "Spell Card",
            "Spirit Monster",
            "Toon Monster",
            "Trap Card",
            "Tuner Monster",
            "Union Effect Monster",
            //ExtraDeck
            "Fusion Monster",
            "Link Monster",
            "Pendulum Effect Fusion Monster",
            "Synchro Monster",
            "Synchro Pendulum Effect Monster",
            "Synchro Tuner Monster",
            "XYZ Monster",
            "XYZ Pendulum Effect Monster",
            //Others
            "Skill Card",
            "Token",
        };

        public static readonly HashSet<string> monsterTypes = new()
        {
            "Effect Monster",
            "Flip Effect Monster",
            "Flip Tuner Effect Monster",
            "Gemini Monster",
            "Normal Monster",
            "Normal Tuner Monster",
            "Pendulum Effect Monster",
            "Pendulum Effect Ritual Monster",
            "Pendulum Flip Effect Monster",
            "Pendulum Normal Monster",
            "Pendulum Tuner Effect Monster",
            "Ritual Effect Monster",
            "Ritual Monster",
            "Spirit Monster",
            "Toon Monster",
            "Tuner Monster",
            "Union Effect Monster",
            "Fusion Monster",
            "Link Monster",
            "Pendulum Effect Fusion Monster",
            "Synchro Monster",
            "Synchro Pendulum Effect Monster",
            "Synchro Tuner Monster",
            "XYZ Monster",
            "XYZ Pendulum Effect Monster",
        };
        private static readonly HashSet<string> mainDeckMonsterTypes = new()
        {
            "Effect Monster",
            "Flip Effect Monster",
            "Flip Tuner Effect Monster",
            "Gemini Monster",
            "Normal Monster",
            "Normal Tuner Monster",
            "Pendulum Effect Monster",
            "Pendulum Effect Ritual Monster",
            "Pendulum Flip Effect Monster",
            "Pendulum Normal Monster",
            "Pendulum Tuner Effect Monster",
            "Ritual Effect Monster",
            "Ritual Monster",
            "Spirit Monster",
            "Toon Monster",
            "Tuner Monster",
            "Union Effect Monster",
        };
        private static readonly HashSet<string> extraDeckMonsterTypes = new()
        {
            "Fusion Monster",
            "Link Monster",
            "Pendulum Effect Fusion Monster",
            "Synchro Monster",
            "Synchro Pendulum Effect Monster",
            "Synchro Tuner Monster",
            "XYZ Monster",
            "XYZ Pendulum Effect Monster",
        };
        private static readonly HashSet<string> pendulumTypes = new()
        {
            "Pendulum Effect Monster",
            "Pendulum Effect Ritual Monster",
            "Pendulum Flip Effect Monster",
            "Pendulum Normal Monster",
            "Pendulum Tuner Effect Monster",
            "Synchro Pendulum Effect Monster",
            "XYZ Pendulum Effect Monster",
        };
        private static readonly HashSet<string> XYZTypes = new()
        {
            "XYZ Monster",
            "XYZ Pendulum Effect Monster",
        };
        private static readonly HashSet<string> linkMonsterTypes = new()
        {
            "Link Monster",
        };
        private static readonly HashSet<string> spellTrapTypes = new()
        {
            "Spell Card",
            "Trap Card",
        };


        public static bool isMonster(this CardInfo info)
        {
            return monsterTypes.Contains(info.type);
        }
        public static List<string> allMonsterTypesWithoutSuffix()
        {
            List<string> monsterTypesList = new List<string>();
            foreach(string type in allTypes)
            {
                if (type.Contains(" Monster"))
                {
                    monsterTypesList.Add(type.Replace(" Monster", ""));
                }
            }
            return monsterTypesList;
        }
        public static bool isMainDeckMonster(this CardInfo info)
        {
            return mainDeckMonsterTypes.Contains(info.type);
        }
        public static bool isExtraDeckMonster(this CardInfo info)
        {
            return extraDeckMonsterTypes.Contains(info.type);
        }
        public static bool isPendulum(this CardInfo info)
        {
            return pendulumTypes.Contains(info.type);
        }
        public static bool isXYZ(this CardInfo info)
        {
            return XYZTypes.Contains(info.type);
        }
        public static bool isLinkMonster(this CardInfo info)
        {
            return linkMonsterTypes.Contains(info.type);
        }
        public static bool isSpellTrap(this CardInfo info)
        {
            return spellTrapTypes.Contains(info.type);
        }
        public static bool isFieldSpell(this CardInfo info)
        {
            return (info.race.Equals("Field"));
        }
        #endregion

    }
    public static class CardFrame 
    { 
        public static string FRAME_PENDULUM_SUFFIX = "_pendulum";
        
        public static readonly Color normalMonsterFrame = new Color(.76f, .60f, .31f);
        public static readonly Color effectMonsterFrame = new Color(.72f,.42f,.24f);
        public static readonly Color synchroMonsterFrame = new Color(.94f,.92f,.92f);
        public static readonly Color ritualMonsterFrame = new Color(.25f,.42f,.71f);
        public static readonly Color fusionMonsterFrame = new Color(.49f,.2f,.6f);
        public static readonly Color xyzMonsterFrame = new Color(.11f,.12f,.13f);
        public static readonly Color linkMonsterFrame = new Color(.08f,.38f,.6f);
        public static readonly Color spellCardFrame = new Color(.04f,.55f,.51f);
        public static readonly Color trapCardFrame = new Color(.95f,.14f,.43f);
        public static readonly Color defaultFrame = new Color(.5f, .5f, .5f);

        public static Color FrameColor(this CardInfo info)
        {
            string baseFrame = GetBaseFrame(info);

            return baseFrame switch
            {
                "normal" => normalMonsterFrame,
                "effect" => effectMonsterFrame,
                "ritual" => ritualMonsterFrame,
                "fusion" => fusionMonsterFrame,
                "synchro"=> synchroMonsterFrame,
                "xyz"    => xyzMonsterFrame,
                "link"   => linkMonsterFrame,
                "spell"  => spellCardFrame,
                "trap"   => trapCardFrame,

                _ => defaultFrame
            };
        }

        public static bool isPendulumFrame(this CardInfo info)
        {
            return info.frameType.Contains(FRAME_PENDULUM_SUFFIX);
        }

        private static string GetBaseFrame(this CardInfo info)
        {
            string baseFrame = "";
            if (info.isPendulumFrame())
            {
                int index = info.frameType.IndexOf(FRAME_PENDULUM_SUFFIX);
                baseFrame = info.frameType.Substring(0, index);
            }
            else
            {
                baseFrame = info.frameType;
            }
            return baseFrame;
        }
    }

    public static class LinkArrow
    {
        public enum INS_ARW_Option
        {
            bottomLeft,
            bottom,
            bottomRight,
            right,
            topRight,
            top,
            topLeft,
            left,
        }

        public static INS_ARW_Option GetArrowType(this string arrowName)
        {
            return arrowName switch
            {
                "Bottom-Left" => INS_ARW_Option.bottomLeft,
                "Bottom" => INS_ARW_Option.bottom,
                "Bottom-Right" => INS_ARW_Option.bottomRight,
                "Right" => INS_ARW_Option.right,
                "Top-Right" => INS_ARW_Option.topRight,
                "Top" => INS_ARW_Option.top,
                "Top-Left" => INS_ARW_Option.topLeft,
                "Left" => INS_ARW_Option.left,
                _ => INS_ARW_Option.bottomLeft,
            };
        }

        public static string INS_ARW_Name(this INS_ARW_Option option)
        {
            return option switch
            {
                INS_ARW_Option.bottomLeft => "Bottom-Left",
                INS_ARW_Option.bottom => "Bottom",
                INS_ARW_Option.bottomRight => "Bottom-Right",
                INS_ARW_Option.right => "Right",
                INS_ARW_Option.topRight => "Top-Right",
                INS_ARW_Option.top => "Top",
                INS_ARW_Option.topLeft => "Top-Left",
                INS_ARW_Option.left => "Left",
                _ => "unassigned",
            };
        }
    }

    public static class NetworkStatus
    {
        public static bool isConnectedToPlayer()
        {
            return Player.connection != null;
        }
    }

    public static class PlaySceneGameObjects {
        public static GameObject CardHolder => GameObject.Find("CardHolder");
        public static GameObject MainCamera => GameObject.FindGameObjectWithTag("MainCamera");
        public static GameObject Canvas => GameObject.Find("Canvas");
        public static GameObject Board => GameObject.Find("Board");
        public static GameObject Player => GameObject.Find("Player");
        public static GameObject Inspect => GameObject.Find("Inspect");
        public static GameObject ViewZone => GameObject.Find("ViewMenuHolder").transform.GetChild(0).gameObject;//object is constently setInactive and this acts as a reliable reference
        public static GameObject LifePoints => GameObject.Find("LifePoints");
        public static GameObject Client => NetworkStatus.isConnectedToPlayer() ? Unity.Netcode.NetworkManager.Singleton.LocalClient.PlayerObject.gameObject : Player;
        public static GameObject Lobby => GameObject.Find("LobbyManager");
    }
    public static class LoadingSceneGameObjects
    {
        public static GameObject LoadingBar => GameObject.FindObjectOfType<LoadingBar>().gameObject;
    }

    public static class PersistantScripts
    {
        public static SFX_Manager SFX_Manager => GameObject.FindObjectOfType<SFX_Manager>();
        public static SCN_Manager SCN_Manager => GameObject.FindObjectOfType<SCN_Manager>();
    }

    public static class ResourcePath
    {
        public static string card = "Card/Card";
        public static string cardBack = "Card/SimpleYugiohCardBack";

        public static string levelStar = "Inspect_Elements/LevelStar";
        public static string rankStar = "Inspect_Elements/RankStar";
        public static string linkArrowActive = "Inspect_Elements/LinkBottomLeftArrowActive";
        public static string linkArrowInactive = "Inspect_Elements/LinkBottomLeftArrowInactive";



        public static string dropdown = "UI_Elements/dropdown";
        public static string BTN_button = "UI_Elements/BTN_Button";
        public static string SFX_Slider = "UI_Elements/Dependents/SFX_Slider";

        public static string panel = "Panels/Panel";
        public static string textInputPanel = "Panels/TextInputPanel";
        public static string fileInputPanel = "Panels/FileInputPanel";

        public static class CardAttribute
        {
            public static Sprite GetAttributeImage(string attribute)
            {
                return Resources.Load<Sprite>("Inspect_Elements/Attributes/" + attribute.ToUpper());
            }
        }
    }
}
namespace Utilities
{
    public static class IO
    {
        public static void AssureDirectory(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        } 
    }
    /// <summary>
    /// christophfranke123
    /// https://discussions.unity.com/t/solved-how-to-serialize-dictionary-with-unity-serialization-system/71474
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, UnityEngine.ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
    public static class Random
    {
        private static RandomNumberGenerator rg = RandomNumberGenerator.Create();
        public static int NextInt(int lowerInclusive, int higherExclusive) 
        { 
            byte[] rno = new byte[5];
            rg.GetBytes(rno);    
            int randomvalue = System.BitConverter.ToInt32(rno, 0);
            randomvalue = randomvalue >= 0 ? randomvalue : -randomvalue;
            return (randomvalue % (higherExclusive - lowerInclusive)) + lowerInclusive;
        }
    }
    public static class String
    {
        public static void RemoveZeroWidthSpace(ref string s)
        {
            s = RemoveZeroWidthSpace(s);
        }
        public static string RemoveZeroWidthSpace(string s)
        {
            System.Text.StringBuilder newText = new System.Text.StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '\u200b')
                {
                    newText.Append(s[i]);
                }
            }
            return newText.ToString();
        }
    }

    public static class Enum
    {
        public static int Length(this System.Type enumType)
        {
            return System.Enum.GetNames(enumType).Length;
        }
    }

    public static class Instance
    {
        /// <summary>
        /// Interface that requires that the class has a Instance
        /// which represents the only Instance Allowed in the scene
        /// <summary>
        /// 
        /// Warning:
        /// this is not safe. Can be used Maliciously if used as Type
        /// ex: class EvilClass : IInstance/OtherClass\{ public static Instance  = ...}
        /// Luckly its not to important in this case as it requires
        /// 
        /// https://stackoverflow.com/questions/8202384/how-to-reference-the-implementing-class-from-an-interface
        /// 
        /// <typeparam name="T"></typeparam>
        public interface IInstance<T> where T : MonoBehaviour
        {
            public static T Instance { get; private set; }

            public bool SetInstance()
            {
                if (Instance == null)
                {
                    Instance = (T)this;
                    return true;
                }
                else
                {
                    Debug.Log(GetType() + " has Instance " + Instance.name + " is already in the scene. Destroy: " + ((T)this).name);
                    Object.Destroy(((T)this).gameObject);
                    return false;
                }
            }

            public void DeleteInstance()
            {
                if(Instance != null)
                {
                    Object.Destroy(Instance.gameObject);
                }
                Instance = null;
            }
        }

        public static bool SetInstance<T>(this IInstance<T> possibleInstance) where T : MonoBehaviour
        {
            return possibleInstance.SetInstance();
        }
        public static void DeleteInstance<T>(this IInstance<T> currentInstance) where T : MonoBehaviour
        {
            currentInstance.DeleteInstance();
        }

        public interface IEnumeratedInstance<T,E>
            where T : MonoBehaviour
            where E : System.Enum
        {
            public E EnumeratedInstanceType { get; }
            public static T[] Instances { get; private set; }
            /// <summary>
            /// Should typically be used at in Awake() to garentee limited instances
            /// </summary>
            /// <returns></returns>
            public bool SetInstance()
            {
                if (Instances == null)
                {
                    Instances = new T[typeof(E).Length()];
                }

                if (typeof(E).GetEnumUnderlyingType() != typeof(int))
                {
                    Debug.Log("IEnumeratedInstance was passed enum with a non-int base value");
                    return false;
                }

                if(Instances[(int)(object)EnumeratedInstanceType] == null)
                {
                    Instances[(int)(object)EnumeratedInstanceType] = (T)this;
                    return true;
                }
                else
                {
                    Debug.Log(GetType() + " has Instance " + Instances[(int)(object)EnumeratedInstanceType].name + " is already in the scene. Destroy: " + ((T)this).name);
                    Object.Destroy(((T)this).gameObject);
                    return false;
                }
            }
        }
        public static bool SetInstance<T,E>(this IEnumeratedInstance<T, E> possibleInstance) where T : MonoBehaviour where E : System.Enum
        {
            return possibleInstance.SetInstance();
        }
        public static T GetInstanceOf<T,E>(E enumeratedInstanceType) where T: MonoBehaviour where E: System.Enum
        {
            return IEnumeratedInstance<T, E>.Instances[(int)(object)enumeratedInstanceType];
        }
    }
}
