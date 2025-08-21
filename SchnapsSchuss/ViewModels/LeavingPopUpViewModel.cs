using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System;
using System.Collections.Generic;
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
        public ICommand CloseCommand { get; }
        public ICommand PayCommand { get; }
        private Person _person;
        private string _labelText;
        public string LabelText
        {
            get => _labelText;
            set => SetProperty(ref _labelText, value);
        }

        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        public LeavingPopUpViewModel(Person Person)
        {
            _invoiceDb = new InvoiceDatabase();
            CloseCommand = new Command(async () => await OnClicked());
            PayCommand = new Command(async () => await OnPayClicked());

            // TODO: Remove hardcoded Person:
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
            List<Invoice> openInvoices = await _invoiceDb.GetInvoicesAsync();
            openInvoices = openInvoices.Where(i => i.isPaidFor == false && i.PersonId == Person.Id).ToList();

            // If there is an open Invoice, show it. If not, create a new Invoice.
            if (openInvoices.Count() == 1) _invoice = openInvoices[0];
            else
            {
                _invoice = null;
            }
                ShowInvoice();
        }

        private async void ShowInvoice()
        {
            if (_invoice == null)
            {
                // TODO think of how to display empty invoice   
            }
            else
            {
                // TODO Set the content view to the invoice details
                
            }
        }

        private async Task OnPayClicked()
        {
            if (_invoice != null)
            {
                await _invoiceDb.DeleteInvoiceAsync(_invoice);
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
