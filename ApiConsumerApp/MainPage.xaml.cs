using ApiConsumerApp.Authentication;
using ApiConsumerApp.ViewModels;
using System.Text;
using System.Text.Json;

namespace ApiConsumerApp;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewmodel;

    public MainPage()
	{
		InitializeComponent();
		BindingContext = viewmodel = new MainViewModel();

	}

	



}

