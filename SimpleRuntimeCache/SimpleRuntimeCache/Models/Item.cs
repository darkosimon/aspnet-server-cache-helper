using SimpleRuntimeCache.Models.BaseModels;

namespace SimpleRuntimeCache.Models
{
    public class Item : BaseRepoModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}