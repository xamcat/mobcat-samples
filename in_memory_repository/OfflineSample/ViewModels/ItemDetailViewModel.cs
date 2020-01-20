using System;
using OfflineSample.Data;

namespace OfflineSample.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public SampleModel Item { get; set; }
        public ItemDetailViewModel(SampleModel item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
