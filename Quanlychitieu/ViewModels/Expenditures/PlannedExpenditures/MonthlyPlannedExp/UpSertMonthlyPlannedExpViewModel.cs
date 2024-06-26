﻿using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Platforms.Android.NavigationsMethods;
using Quanlychitieu.PopUpPages;

namespace Quanlychitieu.ViewModels.Expenditures.PlannedExpenditures.MonthlyPlannedExp;

[QueryProperty(nameof(PageTitle), nameof(PageTitle))]
[QueryProperty(nameof(Mode), "Mode")]
[QueryProperty(nameof(CreateNewMonthlyPlanned), "CreateNewMP")]
[QueryProperty(nameof(SingleExpenditureDetails), "SingleExpenditureDetails")]
[QueryProperty(nameof(SingleMonthlyPlanned), "SingleMonthlyPlanned")]
[QueryProperty(nameof(ActiveUser), "ActiveUser")]
[QueryProperty(nameof(IsAdd), "IsAdd")]
public partial class UpSertMonthlyPlannedExpViewModel : ObservableObject
{
    private readonly IPlannedExpendituresRepository monthlyPlannedExpService;
    private IUsersRepository userService;

    readonly MonthlyPlannedExpNavs NavFunctions = new();

    public UpSertMonthlyPlannedExpViewModel(IPlannedExpendituresRepository monthlyPlannedExpRepo, IUsersRepository userRepo)
    {
        monthlyPlannedExpService = monthlyPlannedExpRepo;
        userService = userRepo;
    }

    [ObservableProperty]
    PlannedExpendituresModel _SingleMonthlyPlanned;

    [ObservableProperty]
    private ExpendituresModel _SingleExpenditureDetails = new();

    [ObservableProperty]
    string mode;

    [ObservableProperty]
    bool isAdd;

    [ObservableProperty]
    bool createNewMonthlyPlanned;

    [ObservableProperty]
    public string pageTitle;

    [ObservableProperty]
    private UsersModel activeUser;

    [ObservableProperty]
    bool hasComment;

    [ObservableProperty]
    private bool addAnotherExp;

    int expCounter;

    double InitialSingleMonthlyPlannedExp;
    double InitialExpenditureAmount;

    [RelayCommand]
    public void PageLoaded()
    {
        SingleExpenditureDetails.AmountSpent = SingleExpenditureDetails.AmountSpent == 0 ? 1 : SingleExpenditureDetails.AmountSpent;
        expCounter = 1;
        InitialSingleMonthlyPlannedExp = SingleMonthlyPlanned.TotalAmount;
        InitialExpenditureAmount = SingleExpenditureDetails.AmountSpent;
        if (SingleExpenditureDetails.Comment is not null)
        {
            HasComment = !SingleExpenditureDetails.Comment.Equals("None");
        }
    }

    [RelayCommand]
    public async Task UpSertMonthlyPlanned()
    {
        if (SingleExpenditureDetails.Comment is null)
        {
            SingleExpenditureDetails.Comment = "None";
        }

        CancellationTokenSource cancellationTokenSource = new();

        const ToastDuration duration = ToastDuration.Short;
        const double fontSize = 14;
        var navParam = new Dictionary<string, object>
                {
                    {"SingleMonthlyPlanDetails", SingleMonthlyPlanned },
                    {"PageTitle", new string($"Edit {SingleMonthlyPlanned.Title}") },
                    {"ActiveUser", ActiveUser }
                };

        if (SingleMonthlyPlanned.Id is null)
        {
            if (await AddMonthlyPlannedExp(duration, fontSize, cancellationTokenSource))
            {
                if (AddAnotherExp)
                {
                    expCounter++;
                    PageTitle = $"Add Flow Out N° {expCounter}";
                    SingleExpenditureDetails = new() { DateSpent = DateTime.Now };
                    AddAnotherExp = false;
                }
                else
                {
                    await NavFunctions.ReturnToDetailsMonthlyPlanned(navParam);
                }
            }
            else
            {
                await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Failed to Save Flow Out"));
            }
        }
        else
        {
            if (SingleExpenditureDetails.Id is null)
            {
                if (await AddExpToExistingMonthlyPlanned(duration, fontSize, cancellationTokenSource))
                {
                    if (AddAnotherExp)
                    {
                        expCounter++;
                        PageTitle = $"Add Flow Out N° {expCounter}";
                        SingleExpenditureDetails = new() { DateSpent = DateTime.Now };
                        AddAnotherExp = false;
                    }
                    else
                    {
                        await NavFunctions.ReturnToDetailsMonthlyPlanned(navParam);
                    }
                }
                else
                {
                    await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Failed to Save Flow Out"));
                }
            }
            else
            {
                if (await EditExpInExistingMonthlyPlanned(duration, fontSize, cancellationTokenSource))
                {
                    await NavFunctions.ReturnOnce(navParam);
                }
                else
                {
                    await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Failed to Save Flow Out"));
                }
            }
        }
    }

    async Task<bool> AddMonthlyPlannedExp(ToastDuration duration, double fontsize, CancellationTokenSource tokenSource)
    {
        SingleMonthlyPlanned.Id = Guid.NewGuid().ToString();

        SingleExpenditureDetails.Id = Guid.NewGuid().ToString();
        SingleMonthlyPlanned.Expenditures.Add(SingleExpenditureDetails);

        SingleMonthlyPlanned.TotalAmount += SingleExpenditureDetails.AmountSpent;
        SingleMonthlyPlanned.NumberOfExpenditures++;

        bool dialogResult = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Add New Monthly Planned Flow Out?"));
        if (dialogResult)
        {
            if (!await monthlyPlannedExpService.AddPlannedExp(SingleMonthlyPlanned))
                return false;

            const string ToastNotifMessage = "Monthly Flow Out Added";
            var toast = Toast.Make(ToastNotifMessage, duration, fontsize);
            await toast.Show(tokenSource.Token);
        }

        return true;
    }

    async Task<bool> AddExpToExistingMonthlyPlanned(ToastDuration duration, double fontsize, CancellationTokenSource tokenSource)
    {
        SingleExpenditureDetails.Id = Guid.NewGuid().ToString();

        SingleMonthlyPlanned.Expenditures.Add(SingleExpenditureDetails);

        SingleMonthlyPlanned.TotalAmount += SingleExpenditureDetails.AmountSpent;
        SingleMonthlyPlanned.NumberOfExpenditures += 1;

        bool dialogResult = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Add New Flow?"));
        if (dialogResult)
        {
            if (!await monthlyPlannedExpService.UpdatePlannedExp(SingleMonthlyPlanned))
                return false;
        }
        const string ToastNotifMessage = "Flow Out Added";
        var toast = Toast.Make(ToastNotifMessage, duration, fontsize);
        await toast.Show(tokenSource.Token);
        return true;
    }

    async Task<bool> EditExpInExistingMonthlyPlanned(ToastDuration duration, double fontsize, CancellationTokenSource tokenSource)
    {
        int ExpIndex = SingleMonthlyPlanned.Expenditures.FindIndex(exp => exp.Id == SingleExpenditureDetails.Id);
        if (ExpIndex != -1)
        {
            SingleMonthlyPlanned.UpdatedDateTime = DateTime.UtcNow;
            SingleMonthlyPlanned.UpdateOnSync = true;
            SingleMonthlyPlanned.Expenditures[ExpIndex] = SingleExpenditureDetails;
        }
        var difference = InitialExpenditureAmount - SingleExpenditureDetails.AmountSpent;
        SingleMonthlyPlanned.TotalAmount = InitialSingleMonthlyPlannedExp - difference;

        bool dialogResult = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Save Edit?"));
        if (dialogResult)
        {
            if (!await monthlyPlannedExpService.UpdatePlannedExp(SingleMonthlyPlanned))
                return false;
        }
        const string ToastNotifMessage = "Flow Out Edited";
        var toast = Toast.Make(ToastNotifMessage, duration, fontsize);
        await toast.Show(tokenSource.Token);

        return true;
    }
}
