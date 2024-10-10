using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        public string text;
        public bool pause;
        public string imageAssetId;

        public int GetTextCharLength()
        {
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
    }

    public DialogueLine[] lines;
}
