using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using Core.ViewModels;
using Microsoft.Identity.Client;
using Microsoft.Intune.Mam.Client.App;
using System;

namespace Intune.MAM.NET7.Droid.UI;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : MAMActivity
{
    MainViewModel viewModel;
    Button loginButton;
    Button enrollButton;
    TextView welcomeTextView;
    ContentLoadingProgressBar loadingIndicator;
    LinearLayout buttonsContainer;
    RecyclerView logsRecyclerView;
    LogsAdapter logsAdapter;
    TextView userTextView;
    TextView loginAndEnrollmentStateTextView;

    public override void OnMAMCreate(Bundle? savedInstanceState)
    {
        base.OnMAMCreate(savedInstanceState);
        SetContentView(Resource.Layout.main_activity);
        viewModel = new MainViewModel(Core.Application.AuthenticationService, Core.Application.MobileApplicationManagementService, Core.Application.Logger)
        {
            OnUIStateChanged = SetData
        };

        loginButton = FindViewById<Button>(Resource.Id.loginButton);
        loginButton.Click += LoginButton_Click;

        enrollButton = FindViewById<Button>(Resource.Id.enrollButton);
        enrollButton.Click += EnrollButton_Click;

        welcomeTextView = FindViewById<TextView>(Resource.Id.welcomeTextView);
        welcomeTextView.Text = Core.Reosurces.Strings.Welcome;

        userTextView = FindViewById<TextView>(Resource.Id.usernameTextView);

        loadingIndicator = FindViewById<ContentLoadingProgressBar>(Resource.Id.loadingIndicator);
        buttonsContainer = FindViewById<LinearLayout>(Resource.Id.buttonsContainer);

        loadingIndicator = FindViewById<ContentLoadingProgressBar>(Resource.Id.loadingIndicator);
        buttonsContainer = FindViewById<LinearLayout>(Resource.Id.buttonsContainer);

        loginAndEnrollmentStateTextView = FindViewById<TextView>(Resource.Id.loginAndEnrollmentStateTextView);

        logsRecyclerView = FindViewById<RecyclerView>(Resource.Id.logsRecyclerView);
        logsRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

        logsAdapter = new LogsAdapter(this, viewModel.Logs);
        logsRecyclerView.SetAdapter(logsAdapter);
        logsAdapter.NotifyDataSetChanged();
        viewModel.Logs.CollectionChanged += Logs_CollectionChanged;
    }

    void Logs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                Handler handler = new Handler(this.MainLooper);
                handler.Post(() =>
                {
                    logsAdapter.NotifyItemInserted(e.NewStartingIndex);
                    logsRecyclerView.SmoothScrollToPosition(0);
                });
                break;
        }
    }

    void LoginButton_Click(object? sender, EventArgs e)
    {
        _ = viewModel.Login(this)
            .ContinueWith(t => {
                SetData();
            });
    }

    void EnrollButton_Click(object? sender, System.EventArgs e)
    {
        _ = viewModel.Enroll()
            .ContinueWith(t => {
                SetData();
            });
    }

    public override void OnMAMResume()
    {
        base.OnMAMResume();
        viewModel.OnResume();
        SetData();
    }

    public override void OnMAMActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
    {
        base.OnMAMActivityResult(requestCode, resultCode, data);
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
    void SetData()
    {
        Handler handler = new Handler(this.MainLooper);
        handler.Post(() => {
            loginButton.Text = viewModel.LoginButtonText;
            loginButton.Enabled = !viewModel.Loading;
            enrollButton.Text = viewModel.EnrollButtonText;
            enrollButton.Enabled = viewModel.EnrollButtonEnabled && !viewModel.Loading;
            userTextView.Text = viewModel.Username;
            buttonsContainer.Visibility = !viewModel.Loading ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;
            loadingIndicator.Visibility = viewModel.Loading ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;
            loginAndEnrollmentStateTextView.Text = viewModel.LoginAndEnrollmentStateText;
        });
    }
}
