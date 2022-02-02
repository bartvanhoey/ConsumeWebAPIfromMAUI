using ApiConsumerApp.Authentication;
using ApiConsumerApp.Models;
using IdentityModel.Client;
using MvvmHelpers.Commands;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace ApiConsumerApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string BaseUrl = "https://f7db-2a02-810d-98c0-576c-647e-cd22-5b-e9a3.eu.ngrok.io";
        private ICommand _registerUserCommand, _loginUserCommand, _getWeatherForecastCommand;
        private HttpClient _httpClient;
        private string _registerUserName, _registerPassword, _registerEmail, _loginUserName, _loginPassword, _weatherForecastMessage, _registerUserMessage, _loginUserMessage;
        private AuthenticationResponse _authenticationResponse;
        
        public ICommand RegisterUserCommand => _registerUserCommand ??=new AsyncCommand(RegisterUserAsync);
        public ICommand LoginUserCommand => _loginUserCommand ??=new AsyncCommand(LoginUserAsync);
        public ICommand GetWeatherForecastCommand => _getWeatherForecastCommand ??=new AsyncCommand(GetWeatherForecastAsync);

        public MainViewModel()
        {
            _httpClient = GetHttpClient();
        }

        private async Task RegisterUserAsync()
        {
            var registerUrl = $"{BaseUrl}/api/authentication/register";

            var registerModel = new RegisterModel
            {
                Username = RegisterUserName,
                Password =RegisterPassword,
                Email = RegisterEmail
            };

            var json = JsonSerializer.Serialize(registerModel, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(registerUrl, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                RegisterUserMessage = "User successfully registered!";
            }
            else
            {
                RegisterUserMessage = $"User registration went wrong - Status code: {response.StatusCode}";
            }

            RegisterUserName = null;
            RegisterPassword = null;
            RegisterEmail = null;
        }


        private async Task LoginUserAsync()
        {
            var loginUrl = $"{BaseUrl}/api/authentication/login";

            var loginModel = new LoginModel
            {
                Username = LoginUserName,
                Password = LoginPassword,
            };

            var json = JsonSerializer.Serialize(loginModel, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(loginUrl, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                _authenticationResponse = JsonSerializer.Deserialize<AuthenticationResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                LoginUserMessage = $"User successfully logged in!";
            }
            else
            {
                LoginUserMessage = $"User registration went wrong - Status code: {response.StatusCode}";
            }
            LoginUserName = null;
            LoginPassword = null;
        }

        private async Task GetWeatherForecastAsync()
        {
            var weatherForecastUrl = $"{BaseUrl}/api/weatherforecast";

            if (!string.IsNullOrWhiteSpace(_authenticationResponse?.Token))
            {
                _httpClient.SetBearerToken(_authenticationResponse.Token);
            }

            var response = await _httpClient.GetAsync(weatherForecastUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var forecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                WeatherForecastMessage =  $"{forecasts.FirstOrDefault().TemperatureC} - {forecasts.FirstOrDefault().Summary}";
            }
            else
            {
                WeatherForecastMessage = response.StatusCode.ToString();
            }
        }



        private HttpClient GetHttpClient() => new(GetHttpClientHandler());

        private HttpClientHandler GetHttpClientHandler()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // EXCEPTION : Javax.Net.Ssl.SSLHandshakeException: 'java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.'
            // SOLUTION :
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            return httpClientHandler;
        }

        public string WeatherForecastMessage
        {
            get => _weatherForecastMessage;
            set => SetProperty(ref _weatherForecastMessage, value);
        }

        public string RegisterUserMessage
        {
            get => _registerUserMessage;
            set => SetProperty(ref _registerUserMessage, value);
        }

        public string LoginUserMessage
        {
            get => _loginUserMessage;
            set => SetProperty(ref _loginUserMessage, value);
        }

        public string RegisterUserName
        {
            get => _registerUserName;
            set => SetProperty(ref _registerUserName, value);
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set => SetProperty(ref _registerPassword, value);
        }

        public string RegisterEmail
        {
            get => _registerEmail;
            set => SetProperty(ref _registerEmail, value);
        }

        public string LoginUserName
        {
            get => _loginUserName;
            set => SetProperty(ref _loginUserName, value);
        }

        public string LoginPassword
        {
            get => _loginPassword;
            set => SetProperty(ref _loginPassword, value);
        }

    }
}
