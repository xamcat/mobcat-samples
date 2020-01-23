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
    public class RelationalItemsViewModel : BaseViewModel
    {
        private IOfflineSampleRepository<SampleUserModel> SampleUserStore => DependencyService.Get<IOfflineSampleRepository<SampleUserModel>>();

        private IOfflineSampleRepository<SampleOrderModel> SampleOrderStore => DependencyService.Get<IOfflineSampleRepository<SampleOrderModel>>();

        public ObservableCollection<SampleUserModel> Users { get; set; }
        public Command LoadUsersCommand { get; set; }

        public RelationalItemsViewModel()
        {
            Title = "Users";
            Users = new ObservableCollection<SampleUserModel>();
            LoadUsersCommand = new Command(async () => await ExecuteLoadUsersCommand());           
        }

        async Task ExecuteLoadUsersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Users.Clear();
                var users = await SampleUserStore.GetAsync();
                foreach (var user in users)
                {
                    Users.Add(user);
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
