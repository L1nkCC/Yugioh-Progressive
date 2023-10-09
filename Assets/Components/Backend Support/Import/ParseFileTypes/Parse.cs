using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ListSave;
using Sylvan.Data.Csv;
using System.IO;

namespace ImportDeckUtilities
{

    [System.Serializable]
    public class ParsingException : System.Exception
    {
        public ParsingException() { }
        public ParsingException(string message) : base(message) { }
        public ParsingException(string message, System.Exception inner) : base(message, inner) { }
        protected ParsingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [System.Serializable]
    public class FileExtensionException : System.Exception
    {
        public FileExtensionException() { }
        public FileExtensionException(string message) : base(message) { }
        public FileExtensionException(string message, System.Exception inner) : base(message, inner) { }
        protected FileExtensionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public static class File
    {
        public static string ReadFile(this string path)
        {
            string content = "";
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    content = reader.ReadToEnd();
                };
            }
            catch (IOException e)
            {
                Debug.LogError("ReadFile Failed. path = " + path + "\n"+e);
            }
            return content;
        }


    }


    //trim is used to assure no white space no matter the LF encoding on the operating system
    public static class CSV
    {
        public const string EXTENSION = ".csv";



        const string CARD_ID_COL_NAME = "cardid";
        const string CARD_QUANTITY_COL_NAME = "cardq";
        public static CardCollectionList ParseCSV(this string path, ListType isPool, string outputName)
        {
            List<int> cardIds = new List<int>();

            using CsvDataReader reader = CsvDataReader.Create(path);
            int cardIdIndex = reader.GetOrdinal(CARD_ID_COL_NAME);
            int cardQuantityIndex = reader.GetOrdinal(CARD_QUANTITY_COL_NAME);

            while (reader.Read())
            {
                int quantity = reader.GetInt32(cardQuantityIndex);
                for(int i = 0; i < quantity; i++)
                    cardIds.Add(reader.GetInt32(cardIdIndex));
            }
            return new CardCollectionList(outputName, isPool, cardIds.ToArray());

        }

    }
    public static class YDK
    {
        public const string EXTENSION = ".ydk";


        const string MAIN_DELIMITER = "#main\n";
        const string EXTRA_DELIMITER = "#extra\n";
        const string SIDE_DELIMITER = "!side\n";
        public static CardCollectionList ParseYDK(this string path, ListType isPool, string outputName)
        {
            string ydkContent = "";
            using (StreamReader reader = new StreamReader(path))
            {
                ydkContent = reader.ReadToEnd();
            }

            int startIndex = ydkContent.IndexOf(MAIN_DELIMITER) + MAIN_DELIMITER.Length;
            string[] mainDeck = ydkContent.Substring(startIndex, ydkContent.IndexOf(EXTRA_DELIMITER)- startIndex).Trim().Split("\n", System.StringSplitOptions.RemoveEmptyEntries);

            startIndex = ydkContent.IndexOf(EXTRA_DELIMITER) + EXTRA_DELIMITER.Length;
            string[] extraDeck = ydkContent.Substring(startIndex, ydkContent.IndexOf(SIDE_DELIMITER)-startIndex).Trim().Split("\n", System.StringSplitOptions.RemoveEmptyEntries);


            startIndex = ydkContent.IndexOf(SIDE_DELIMITER) + SIDE_DELIMITER.Length;
            string[] sideDeck = ydkContent.Substring(startIndex).Trim().Split("\n",System.StringSplitOptions.RemoveEmptyEntries);

            string[][] stringParsedList = new string[][] { mainDeck, extraDeck, sideDeck };

            List<int>[] list = new List<int>[] { new List<int>(), new List<int>(), new List<int>() };
            for (int i = 0; i < stringParsedList.Length; i++)
            {
                for (int j = 0; j < stringParsedList[i].Length; j++)
                {
                    if (!int.TryParse(stringParsedList[i][j].Trim(), out int id))
                    {
                        throw new ParsingException("Not all elements are YGOpro fromated card ids!");
                    }
                    list[i].Add(id);
                }
            }

            return new CardCollectionList(outputName, isPool, list[0].ToArray(), list[2].ToArray(), list[1].ToArray());//order for CardCollectionList is different from import format

        }
    }
}