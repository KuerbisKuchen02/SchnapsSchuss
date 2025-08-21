namespace SchnapsSchuss.ViewModels;

public class CrudViewModel<T> : BaseViewModel
{
    public List<T> Items { get; set; }

    private string _title;

    public string Title
    {
        get =>  _title;
        set => SetProperty(ref  this._title, value);
    }
    
    
}