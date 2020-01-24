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
        private IOfflineSampleRepository<SampleUserModel> SampleUserStore => DependencyService.Get<IOfflineSampleRepository<SampleUserModel>>();

        private IOfflineSampleRepository<SampleOrderModel> SampleOrderStore => DependencyService.Get<IOfflineSampleRepository<SampleOrderModel>>();

        public SampleUserModel User { get; set; }
        public ObservableCollection<SampleOrderModel> Orders { get; set; }
        public Command LoadOrdersCommand { get; set; }

        public RelationalItemDetailViewModel(SampleUserModel user = null)
        {
            Title = $"{user?.Name} orders";
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
                var items = await SampleOrderStore.ExecuteTableQueryAsync(a => a.UserId == User.Id);
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
