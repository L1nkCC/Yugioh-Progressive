using UnityEngine;
using System;

namespace TMPro
{
    [Serializable]
    [CreateAssetMenu(fileName = "SearchInputValidator.asset", menuName = "TextMeshPro/Input Validators", order = 100)]
    public class SearchInputValidator : TMP_InputValidator
    {
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (Char.IsLetterOrDigit(ch) || ch == ' ' || ch == '-' || ch == '\'' || ch == '.')
            {
                text = text.Insert(pos, ch.ToString());
                pos += 1;
                return ch;
            }
            return (char)0;
        }
    }
}
