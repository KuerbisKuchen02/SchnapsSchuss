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
        

        public ICommand CloseCommand { get; }
        public ICommand PayCommand { get; }
        private string _labelText;
        public string LabelText
        {
            get => _labelText;
            set => SetProperty(ref _labelText, value);
        }

        private Person _person;
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        private ObservableCollection<InvoiceItem> _invoiceItems;
        public ObservableCollection<InvoiceItem> InvoiceItems
        {
            get => _invoiceItems ??= new ObservableCollection<InvoiceItem>(_invoice?.InvoiceItems ?? Enumerable.Empty<InvoiceItem>());
            set => SetProperty(ref _invoiceItems, value);
        }

        public float InvoiceTotal => InvoiceItems.Sum(i => i.TotalPrice);

        public LeavingPopUpViewModel(Person Person)
        {
            _invoiceDb = new InvoiceDatabase();
            CloseCommand = new Command(async () => await OnClicked());
            PayCommand = new Command(async () => await OnPayClicked());

            this._person = Person;
            LoadInvoiceAsync();
            LabelText = $"{Person.FirstName} {Person.LastName} auschecken";
            
        }

        private async Task LoadInvoiceAsync()
        {
            if (Person == null)
            {
                Console.WriteLine("Person is null, cannot load invoice.");
                return;
            }

            // Get all open Invoices for the current Person.
            Invoice openInvoice = await _invoiceDb.GetOpenInvoiceForPerson(Person.Id);

            // If there is an open Invoice, show it. If not, show the next popup.
            if (openInvoice != null)
            {
                InvoiceItems = new ObservableCollection<InvoiceItem>(_invoice.InvoiceItems);
                OnPropertyChanged(nameof(InvoiceTotal));
            }
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
            
            GunOwnershipPopUpViewModel gunOwnershipPopUpViewModel = new GunOwnershipPopUpViewModel(Person);

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
