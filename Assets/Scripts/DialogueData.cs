using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    [System.Serializable]
    public class DialogueLine
    {
        [System.Serializable]
        public class DialogueOption
        {
            public string text;
            public string diversion;
        }

        public string type;
        public string characterName;
        public string text;
        public bool pause = false;
        public string imageAssetId;
        public string diversion;
        public DialogueOption[] options = { };

        public int GetTextCharLength()
        {
            if (type != "text")
            {
                Debug.LogError("Trying to get text char length for a non-text dialogue line");
                return -1;
            }

            // Return the length of the text skipping the tags
            int counter = 0;
            bool skip = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    skip = true;
                }
                else if (text[i] == '>')
                {
                    skip = false;
                }
                else if (!skip)
                {
                    counter++;
                }
            }

            return counter;
        }

        public string GetTextUpUntil(int charCount)
        {
            if (type != "text")
            {
                Debug.LogError("Trying to get text up until a char count for a non-text dialogue line");
                return null;
            }

            // Return the text with tags up until the charCount
            string result = "";
            bool inTag = false;
            int counter = 0;

            int tagStackCount = 0;
            for (int i = 0; (tagStackCount != 0 || counter < charCount || inTag) && (i < text.Length); i++)
            {
                if (text[i] == '<')
                {
                    tagStackCount += 1;
                    inTag = true;
                    result += text[i];
                }
                else if (text[i] == '>')
                {
                    inTag = false;
                    result += text[i];
                }
                else if (text[i] == '/' && inTag)
                {
                    // This is a closing tag, so we need to remove the last tag from the stack
                    tagStackCount -= 2;
                    result += text[i];
                }
                else if (counter < charCount && !inTag)
                {
                    counter++;
                    result += text[i];
                } else if (inTag)
                {
                    result += text[i];
                }
            }

            return result;
        }

        public bool CheckFormatOk()
        {
            if (type != "text" && type != "options" && type != "diversion")
            {
                Debug.LogError("Invalid dialogue line type: " + type);
                return false;
            }

            if (type == "text" && string.IsNullOrEmpty(text))
            {
                Debug.LogError("Dialogue line type is text but no text is defined");
                return false;
            }

            if (type == "options" && (options == null || options.Length == 0))
            {
                Debug.LogError("Dialogue line type is options but no options are defined");
                return false;
            }

            if (type == "options" && options.Length > 3)
            {
                Debug.LogError("Dialogue line type is options but more than 3 options are defined");
                return false;
            }

            if (type == "diversion" && string.IsNullOrEmpty(diversion))
            {
                Debug.LogError("Dialogue line type is diversion but no diversion is defined");
                return false;
            }

            return true;
        }
    }

    public DialogueLine[] lines = { };
}
