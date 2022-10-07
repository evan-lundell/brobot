namespace Brobot.Commands.Models
{
    public class DictionaryResponse
    {
        public string Word { get; set; }
        public Meaning[] Meanings { get; set; }
    }
}