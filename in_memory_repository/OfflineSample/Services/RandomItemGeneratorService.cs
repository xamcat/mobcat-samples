using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OfflineSample.Services
{
    public class RandomItemGeneratorService
    {
        public void GenerateItems()
        {
            MessagingCenter.Send(this, "AddItem", Item);
        }
    }
}
