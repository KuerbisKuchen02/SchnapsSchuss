using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels
{
    public class LeavingPopUpViewModel : BaseViewModel
    {
        private InvoiceDatabase _invoiceDb;
        private Invoice _invoice;
        public Invoice Invoice
        {
            get => _invoice;
            set => SetProperty(ref _invoice, value);
        }

        private Person _person;
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand PayCommand { get; }
        private string _labelText;
        public string LabelText
        {
            get => _labelText;
            set => SetProperty(ref _labelText, value);
        }

        private ObservableCollection<InvoiceItem> _invoiceItems;
        public ObservableCollection<InvoiceItem> InvoiceItems
        {
            get => _invoiceItems ??= new ObservableCollection<InvoiceItem>(_invoice?.InvoiceItems ?? Enumerable.Empty<InvoiceItem>());
            set => SetProperty(ref _invoiceItems, value);
        }

        public float InvoiceTotal => InvoiceItems.Sum(i => i.TotalPrice);

        public LeavingPopUpViewModel(Invoice Invoice)
        {
            _invoiceDb = new InvoiceDatabase();
            CloseCommand = new Command(async () => await OnClicked());
            PayCommand = new Command(async () => await OnPayClicked());

            this.Invoice = Invoice;

            InitPerson();
            InitInvoiceItems();
            OnPropertyChanged(nameof(InvoiceTotal));
        }


        private async void InitPerson()
        {
            Person = await new PersonDatabase().GetOneAsync(Invoice.PersonId);
            LabelText = $"{Person.FirstName} {Person.LastName} auschecken";
        }

        private async void InitInvoiceItems()
        {
            List<InvoiceItem> invoiceItems = await new InvoiceItemDatabase().GetInvoiceItemsOfInvoiceAsync(Invoice);
            InvoiceItems = new ObservableCollection<InvoiceItem>(invoiceItems);
        }

        private async Task OnPayClicked()
        {
            if (_invoice != null)
            {
                await _invoiceDb.DeleteAsync(_invoice);
            }

            if (Shell.Current.CurrentPage is not null)
            {
                await Shell.Current.CurrentPage.ClosePopupAsync();
            }
            
            GunOwnershipPopUpViewModel gunOwnershipPopUpViewModel = new GunOwnershipPopUpViewModel(Invoice.Person);

            await Shell.Current.ShowPopupAsync(new GunOwnershipPopUp(gunOwnershipPopUpViewModel), new PopupOptions());
        }

        private async Task OnClicked()
        {
            if (Shell.Current.CurrentPage is not null)
            {
                await Shell.Current.CurrentPage.ClosePopupAsync();

            }
        }

    }
}
