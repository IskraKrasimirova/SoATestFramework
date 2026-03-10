using Common.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.UiTests.Models;
using Selenium.UiTests.Models.UserModels;
using Selenium.UiTests.Utilities.Extensions;

namespace Selenium.UiTests.Pages
{
    public class RegisterPage : BasePage
    {
        private IWebElement RegistrationHeader => _driver.FindElement(By.XPath("//h3[text()='Register']"));
        private IWebElement TitleDropdown => _driver.FindElement(By.XPath("//select[@id='title']"));
        private IWebElement FirstNameInput => _driver.FindElement(By.XPath("//input[@id='first_name']"));
        private IWebElement SurnameInput => _driver.FindElement(By.XPath("//input[@id='sir_name']"));
        private IWebElement EmailInput => _driver.FindElement(By.XPath("//input[@type='email']"));
        private IWebElement PasswordInput => _driver.FindElement(By.XPath("//input[@type='password']"));
        private IWebElement CountryInput => _driver.FindElement(By.XPath("//input[@id='country']"));
        private IWebElement CityInput => _driver.FindElement(By.XPath("//input[@id='city']"));
        private IWebElement AgreementCheckbox => _driver.FindElement(By.XPath("//input[@type='checkbox' and @id='tos']"));
        private IWebElement SubmitButton => _driver.FindElement(By.XPath("//button[@type='submit' and @name='signup']"));
        public IWebElement AlertElement => _driver.FindElement(By.XPath("//form//div[contains(@class,'alert-warning')]"));

        public RegisterPage(IWebDriver driver) : base(driver)
        {
        }

        public void RegisterNewUser(RegisterModel model)
        {
            var select = new SelectElement(TitleDropdown);
            select.SelectByText(model.Title);

            FirstNameInput.EnterText(model.FirstName);
            SurnameInput.EnterText(model.Surname);
            EmailInput.EnterText(model.Email);
            PasswordInput.EnterText(model.Password);
            CountryInput.EnterText(model.Country);
            CityInput.EnterText(model.City);

            if (model.AgreeToTerms && !AgreementCheckbox.Selected)
            {
                AgreementCheckbox.Click();
            }

            _driver.ScrollToElementAndClick(SubmitButton);
        }

        public void RegisterNewUser(UserModel model)
        {
            var select = new SelectElement(TitleDropdown);
            select.SelectByText(model.Title);

            _driver.ScrollToElementAndSendText(FirstNameInput, model.FirstName);
            _driver.ScrollToElementAndSendText(SurnameInput, model.Surname);
            _driver.ScrollToElementAndSendText(EmailInput, model.Email);
            _driver.ScrollToElementAndSendText(PasswordInput, model.Password);
            _driver.ScrollToElementAndSendText(CountryInput, model.Country);
            _driver.ScrollToElementAndSendText(CityInput, model.City);

            if (!AgreementCheckbox.Selected)
            {
                _driver.ScrollToElementAndClick(AgreementCheckbox);
            }

            _driver.ScrollToElementAndClick(SubmitButton);
        }

        public string GetFirstNameValidationMessage() => GetValidationMessage(FirstNameInput);

        public string GetSurnameValidationMessage() => GetValidationMessage(SurnameInput);

        public string GetEmailValidationMessage() => GetValidationMessage(EmailInput);

        public string GetPasswordValidationMessage() => GetValidationMessage(PasswordInput);

        public string GetCountryValidationMessage() => GetValidationMessage(CountryInput);

        public string GetCityValidationMessage() => GetValidationMessage(CityInput);

        public string GetAgreementValidationMessage() => GetValidationMessage(AgreementCheckbox);

        public string GetGlobalAlertMessage() => AlertElement.Text.Trim();

        public bool IsPasswordInputEmpty()
        {
            return string.IsNullOrWhiteSpace(PasswordInput.GetAttribute("value"));
        }

        private string GetValidationMessage(IWebElement element)
        {
            var messageElement = element.FindElement(By.XPath("./following-sibling::div[@class='invalid-feedback']"));

            return messageElement.Text;
        }

        // Validations
        public void VerifyIsAtRegisterPage()
        {
            _driver.WaitUntilUrlContains("/register");

            Assert.Multiple(() =>
            {
                Assert.That(RegistrationHeader.Displayed, Is.True, "Registration header is not visible.");
                Assert.That(FirstNameInput.Displayed, Is.True, "First Name input is not visible.");
                Assert.That(SurnameInput.Displayed, Is.True, "Surname input is not visible.");
                Assert.That(EmailInput.Displayed, Is.True, "Email input is not visible.");
                Assert.That(PasswordInput.Displayed, Is.True, "Password input is not visible.");
                Assert.That(CountryInput.Displayed, Is.True, "Country input is not visible.");
                Assert.That(CityInput.Displayed, Is.True, "City input is not visible.");
                Assert.That(AgreementCheckbox.Displayed, Is.True, "Agreement checkbox is not visible.");
                Assert.That(SubmitButton.Displayed, Is.True, "Submit button is not visible.");
            });
        }

        // Retry is added because of CI pipeline flakiness, as the password input may not be cleared immediately after a failed registration attempt
        public void VerifyPasswordInputIsEmpty()
        {
            string? text = null;

            Retry.Until(() =>
            {
                text = PasswordInput.GetAttribute("value");

                if (!string.IsNullOrEmpty(text))
                    throw new RetryException("Password input is not empty yet.");
            },
            exceptionsToCatch: [new StaleElementReferenceException(), new UnknownErrorException("unknown error")], 
            waitInMilliseconds: 1000);

            Assert.That(text, Is.EqualTo(string.Empty), "Password input should be cleared after failed registration.");
        }

        public void VerifyGlobalAlertMessage(string expectedMessage)
        {
            var actualMessage = GetGlobalAlertMessage();
            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The alert message is incorrect.");
        }
    }
}