using MessagePack;

namespace HmServiceCache.ClientConsoleApp.Models
{
    [MessagePackObject]
    public class LargeItemModel
    {
        [Key(0)]
        public long[] LargeLongArray { get; set; }
        
        [Key(1)]
        public string LargeString { get; set; }


        public string GetSize() 
        {
            return ((LargeLongArray.Length * 8 + LargeString.Length * 2) / 1024) + "kb";
        } 
    }
}
