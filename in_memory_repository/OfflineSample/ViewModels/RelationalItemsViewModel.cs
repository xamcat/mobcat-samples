using Microsoft.MobCAT;
using Microsoft.MobCAT.MVVM;
using OfflineSample.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace OfflineSample.ViewModels
{
    public class RelationalItemsViewModel : BaseViewModel
    {
        private IOfflineSampleRepositoryContext OfflineSampleRepositoryContext => DependencyService.Get<IOfflineSampleRepositoryContext>();

        public ObservableCollection<SampleUserModel> Users { get; set; }
        public Command LoadUsersCommand { get; set; }

        private bool _areUsersFiltered = false;

        private const string _unfilterText = "Show all Users";
        private const int _filterMinimum = 3;
        private string _filterText = $"Show Users with {_filterMinimum} or more orders";


        private bool _isFilterEnabled;
        public bool IsFilterEnabled
        {
            get => _isFilterEnabled;
            set => SetProperty(ref _isFilterEnabled, value);
        }

        private string _filterButtonText;
        public string FilterButtonText
        {
            get => _filterButtonText;
            set => SetProperty(ref _filterButtonText, value);
        }

        public RelationalItemsViewModel()
        {
            Title = "Users";
            Users = new ObservableCollection<SampleUserModel>();
            LoadUsersCommand = new Command(async () => await ExecuteLoadUsersCommand());
            FilterButtonText = _filterText;
        }

        private AsyncCommand _filterCommand;
        public AsyncCommand FilterCommand => _filterCommand ?? (_filterCommand = new AsyncCommand(async () =>
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Users.Clear();
                IEnumerable<SampleUserModel> users = default;

                if (_areUsersFiltered)
                {
                    //show all users
                    users = await OfflineSampleRepositoryContext.GetUsersAsync();
                }
                else
                {
                    //filter users
                    users = await OfflineSampleRepositoryContext.GetUsersWithMinimumOrderCountAsync(_filterMinimum);
                }
                foreach (var user in users)
                {
                    Users.Add(user);
                }
                _areUsersFiltered = !_areUsersFiltered;
                FilterButtonText = _areUsersFiltered ? _unfilterText : _filterText;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }));

        async Task ExecuteLoadUsersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Users.Clear();
                var users = await OfflineSampleRepositoryContext.GetUsersAsync();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
                if (users.Count() > 0)
                {
                    IsFilterEnabled = true;
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
