using System.Collections.Generic;

namespace lithuanian_language_learning_tool.Models
{
    public class PunctuationTask : CustomTask
    {
        public List<Highlight> Highlights { get; set; }
        public PunctuationTask()
        {
            Highlights = new List<Highlight>();
        }

        public void InitializeHighlights()
        {
            Highlights.Clear();
            for (int i = 0; i < Sentence.Length; i++)
            {
                if (Sentence[i] == ' ')
                {
                    Highlights.Add(new Highlight(i));
                }
            }
        }

        public struct Highlight
        {
            public int SpaceIndex { get; set; }  // The index of the space between words
            public bool IsSelected { get; set; } // Whether this space is currently highlighted
            public bool HasPunctuation { get; set; } // Indicates if a punctuation mark has been inserted

            public Highlight(int index)
            {
                SpaceIndex = index;
                IsSelected = false;
                HasPunctuation = false;
            }
        }

    }
}
