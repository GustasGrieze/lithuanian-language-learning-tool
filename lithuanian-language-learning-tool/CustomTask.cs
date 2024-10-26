public class CustomTask
{
    public string Sentence { get; set; }  // The original sentence presented to the user
    public string UserText { get; set; }  // User's current version of the sentence
    public List<string> Options { get; set; }  // The list of punctuation or answer options
    public string CorrectAnswer { get; set; }  // The correct version of the sentence with correct punctuation
    public string Explanation { get; set; }  // The explanation for why this punctuation is correct

    public class Highlight
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

    public List<Highlight> Highlights { get; set; }

    public CustomTask()
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
}
