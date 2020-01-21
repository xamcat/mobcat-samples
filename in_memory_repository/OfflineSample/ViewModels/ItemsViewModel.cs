using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

using OfflineSample.Views;
using OfflineSample.Data;
using OfflineSample.Services;

namespace OfflineSample.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<SampleModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<SampleModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, SampleModel>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as SampleModel;
                Items.Add(newItem);
                await DataStore.InsertItemAsync(newItem);
            }); 
            
            MessagingCenter.Subscribe<RandomItemGeneratorService, SampleModel>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as SampleModel;
                Items.Add(newItem);
                await DataStore.InsertItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetAsync();
                foreach (var item in items)
                {
                    Items.Add(item);
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