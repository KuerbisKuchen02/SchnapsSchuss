using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private ObservableCollection<string> _articleNames;
        public ObservableCollection<string> ArticleNames
        {
            get => _articleNames ??= new ObservableCollection<string>(
                InvoiceItems.Select(i => i.Article?.Name ?? "Unbekannt").Distinct());
            set => SetProperty(ref _articleNames, value);
        }

        private ObservableCollection<InvoiceItem> _invoiceItems;
        public ObservableCollection<InvoiceItem> InvoiceItems
        {
            get => _invoiceItems;
            set
            {
                if (SetProperty(ref _invoiceItems, value))
                {
                    OnPropertyChanged(nameof(InvoiceTotal));
                    if (_invoiceItems != null)
                        _invoiceItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(InvoiceTotal));
                }
            }
        }


        public float InvoiceTotal => InvoiceItems?.Sum(i => i.TotalPrice) ?? 0f;

        public LeavingPopUpViewModel(Invoice Invoice)
        {
            _invoiceDb = new InvoiceDatabase();
            CloseCommand = new Command(async () => await OnCloseClicked());
            PayCommand = new Command(async () => await OnPayClicked());

            this.Invoice = Invoice;

            InitPerson();
            InitInvoiceItems();
            Debug.WriteLine($"LeavingPopUpViewModel initialized with Invoice ID: {Invoice.Id}");
        }


        private async void InitPerson()
        {
            Person = await new PersonDatabase().GetOneAsync(Invoice.PersonId);
            LabelText = $"{Person.FirstName} {Person.LastName} auschecken";
        }

        private async void InitInvoiceItems()
        {
            List<InvoiceItem> invoiceItems = await new InvoiceItemDatabase().GetInvoiceItemsOfInvoiceAsync(Invoice);
            foreach(InvoiceItem item in invoiceItems)
            {
                item.Article = await new ArticleDatabase().GetOneAsync(item.ArticleId);
            }
            InvoiceItems = new ObservableCollection<InvoiceItem>(invoiceItems);
            Invoice.InvoiceItems = InvoiceItems.ToList();
            OnPropertyChanged(nameof(InvoiceItems));
        }

        private async Task OnPayClicked()
        {
            if (Invoice != null)
            {
                Invoice.isPaidFor = true;
                await _invoiceDb.SaveAsync(Invoice);
            }

            if (Shell.Current.CurrentPage is not null)
            {
                await Shell.Current.CurrentPage.ClosePopupAsync();
            }
            
            GunOwnershipPopUpViewModel gunOwnershipPopUpViewModel = new GunOwnershipPopUpViewModel(Person);

            await Shell.Current.ShowPopupAsync(new GunOwnershipPopUp(gunOwnershipPopUpViewModel), new PopupOptions());
        }

        private async Task OnCloseClicked()
        {
            if (Shell.Current.CurrentPage is not null)
            {
                await Shell.Current.CurrentPage.ClosePopupAsync();

            }
        }

    }
}
