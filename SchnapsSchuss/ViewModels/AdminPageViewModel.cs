using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

public class AdminPageViewModel : BaseViewModel
{
    public ICommand DrinkButtonCommand { get; } = new Command(OnDrinkButtonClicked);
    public ICommand FoodButtonCommand { get; } = new Command(OnFoodButtonClicked);
    public ICommand DisciplineButtonCommand { get; } = new Command(OnDisciplineButtonClicked);
    public ICommand MunitionButtonCommand { get; } = new Command(OnMunitionButtonClicked);
    public ICommand MemberButtonCommand { get; } = new Command(OnMemberButtonClicked);
    public ICommand OthersButtonCommand { get; } = new Command(OnOthersButtonClicked);

    private static void OnDrinkButtonClicked()
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

    private static void OnFoodButtonClicked()
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

    private static void OnDisciplineButtonClicked()
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

    private static void OnMunitionButtonClicked()
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

    private static void OnMemberButtonClicked()
    {
        

        var navigationParameter = new Dictionary<string, object>
        {
            { "title", "Mitglieder" },
            { "modelType", typeof(Member) },
            { "shownColumns", Member.Columns }
        };

        Shell.Current.GoToAsync(nameof(CrudPage), navigationParameter);
    }

    private static void OnOthersButtonClicked()
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