public class CustomTask
{
    public string Sentence { get; set; }  // The original sentence presented to the user
    public List<string> Options { get; set; }  // The list of punctuation or answer options, though not currently used in free typing
    public string CorrectAnswer { get; set; }  // The correct version of the sentence with correct punctuation
    public string Explanation { get; set; }  // The explanation for why this punctuation is correct

    public struct ErrorHighlight
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
