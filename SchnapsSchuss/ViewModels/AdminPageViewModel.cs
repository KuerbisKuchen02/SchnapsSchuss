using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

public class AdminPageViewModel : BaseViewModel
{
    public ICommand DrinkButtonCommand { get; }
    public ICommand FoodButtonCommand { get; }
    public ICommand DisciplineButtonCommand { get; }
    public ICommand MunitionButtonCommand { get; }
    public ICommand MemberButtonCommand { get; }
    public ICommand OthersButtonCommand { get; }

    public AdminPageViewModel()
    {
        DrinkButtonCommand = new Command(OnDrinkButtonClicked);
        FoodButtonCommand = new Command(OnFoodButtonClicked);
        DisciplineButtonCommand = new Command(OnDisciplineButtonClicked);
        MunitionButtonCommand = new Command(OnMunitionButtonClicked);
        MemberButtonCommand = new Command(OnMemberButtonClicked);
        OthersButtonCommand = new Command(OnOthersButtonClicked);
    }

    private void OnDrinkButtonClicked()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Getränke" },
            { "modelType", typeof(Article) },
            { "articleType", ArticleType.DRINK },
            { "shownColumns", Article.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }

    private void OnFoodButtonClicked()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Gerichte" },
            { "modelType", typeof(Article) },
            { "articleType", ArticleType.FOOD },
            { "shownColumns", Article.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }

    private void OnDisciplineButtonClicked()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Disziplinen" },
            { "modelType", typeof(Article) },
            { "articleType", ArticleType.DISCIPLINE},
            { "shownColumns", Article.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }

    private void OnMunitionButtonClicked()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Munitionen" },
            { "modelType", typeof(Article) },
            { "articleType", ArticleType.MUNITION },
            { "shownColumns", Article.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }

    private void OnMemberButtonClicked()
    {
        

        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Mitglieder" },
            { "modelType", typeof(Member) },
            { "shownColumns", Member.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }

    private void OnOthersButtonClicked()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Anderes" },
            { "modelType", typeof(Article) },
            { "articleType", ArticleType.OTHER },
            { "shownColumns", Article.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }
}