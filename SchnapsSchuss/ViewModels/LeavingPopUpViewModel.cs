using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

public class LeavingPopUpViewModel : BaseViewModel
{
    private readonly InvoiceDatabase _invoiceDb;
    private Invoice _invoice;
    private Invoice Invoice
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
    
    private string _labelText = string.Empty;
    public string LabelText
    {
        get => _labelText;
        set => SetProperty(ref _labelText, value);
    }

    private ObservableCollection<string> _articleNames;
    public ObservableCollection<string> ArticleNames
    {
        get => _articleNames;
        set => SetProperty(ref _articleNames, value);
    }

    private ObservableCollection<InvoiceItem> _invoiceItems = [];
    public ObservableCollection<InvoiceItem> InvoiceItems
    {
        get => _invoiceItems;
        set
        {
            if (!SetProperty(ref _invoiceItems, value)) return;
            OnPropertyChanged(nameof(InvoiceTotal));
            _invoiceItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(InvoiceTotal));
        }
    }


    public float InvoiceTotal => InvoiceItems?.Sum(i => i.TotalPrice) ?? 0f;

    public LeavingPopUpViewModel(Invoice invoice)
    {
        _invoiceDb = new InvoiceDatabase();
        CloseCommand = new Command(async () => await OnCloseClicked());
        PayCommand = new Command(async () => await OnPayClicked());

        _invoice = invoice;

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
        var invoiceItems = await new InvoiceItemDatabase().GetInvoiceItemsOfInvoiceAsync(Invoice);
        foreach(var item in invoiceItems)
        {
            item.Article = await new ArticleDatabase().GetOneAsync(item.ArticleId);
        }
        InvoiceItems = new ObservableCollection<InvoiceItem>(invoiceItems);
        Invoice.InvoiceItems = InvoiceItems.ToList();
        OnPropertyChanged(nameof(InvoiceItems));
    }

    private async Task OnPayClicked()
    {
        Invoice.isPaidFor = true;
        await _invoiceDb.SaveAsync(Invoice);
        await Shell.Current.CurrentPage.ClosePopupAsync(true);
    }

    private async Task OnCloseClicked()
    {
        await Shell.Current.CurrentPage.ClosePopupAsync(false);
    }
}
