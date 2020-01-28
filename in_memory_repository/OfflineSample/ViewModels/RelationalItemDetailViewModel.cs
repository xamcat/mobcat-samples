using Microsoft.MobCAT;
using Microsoft.MobCAT.MVVM;
using OfflineSample.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace OfflineSample.ViewModels
{
    public class RelationalItemDetailViewModel : BaseViewModel
    {
        private IOfflineSampleRepositoryContext OfflineSampleRepositoryContext => DependencyService.Get<IOfflineSampleRepositoryContext>();

        public SampleUserModel User { get; set; }
        public ObservableCollection<SampleOrderModel> Orders { get; set; }
        public Command LoadOrdersCommand { get; set; }
               
        public RelationalItemDetailViewModel(SampleUserModel user = null)
        {
            IsBusy = false;
            Title = $"Orders for {user.Id}";
            User = user;
            Orders = new ObservableCollection<SampleOrderModel>();
            LoadOrdersCommand = new Command(async () => await ExecuteLoadOrdersCommand());
        }

        async Task ExecuteLoadOrdersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Orders.Clear();
                var items = await OfflineSampleRepositoryContext.GetOrdersForUserIdAsync(User.Id);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Orders.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
