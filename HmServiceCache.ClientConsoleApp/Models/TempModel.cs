using MessagePack;

namespace HmServiceCache.ClientConsoleApp.Models
{
    [MessagePackObject]
    public class TempModel
    {
        [Key(0)]
        public int A { get; set; }

        [Key(1)]
        public string B { get; set; }

        public override string ToString()
        {
            return "{" + $"A = {A} B = {B}" + "}";
        }
    }
}
