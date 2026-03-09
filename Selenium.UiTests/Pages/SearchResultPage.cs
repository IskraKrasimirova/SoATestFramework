using Common.Utilities;
using OpenQA.Selenium;
using Selenium.UiTests.DatabaseOperations.Entities;
using Selenium.UiTests.Utilities.Extensions;

namespace Selenium.UiTests.Pages
{
    public class SearchResultPage : BasePage
    {
        private readonly By ResultsTableLocator = By.XPath("//table[contains(@class, 'table')]");
        private IWebElement ResultsHeader => _driver.FindElement(By.XPath("//h3[contains(text(),'Search Results')]"));
        private IWebElement ResultsTable => _driver.FindElement(ResultsTableLocator);
        private IWebElement NewSearchButton => _driver.FindElement(By.XPath("//a[@href='search.php' and contains(@class, 'btn')]"));
        private IWebElement MessageElement => _driver.FindElement(By.XPath("//div[contains(@class, 'alert-info')]"));
        private IWebElement? FindUserRowByEmail(string email) =>
            _driver.FindElements(By.XPath($"//td[contains(text(), '{email}')]/parent::tr"))
           .FirstOrDefault();

        private ICollection<IWebElement> FindRowsBySkill(string skillName) =>
            _driver.FindElements(By.XPath($"//td[text()='{skillName}']/parent::tr"));

        public SearchResultPage(IWebDriver driver) : base(driver)
        {
        }

        // Retry is added because of CI pipeline flakiness
        public int GetCountOfRowsInResultTable()
        {
            int count = 0;

            Retry.Until(() =>
            {
                var tableRows = ResultsTable.FindElements(By.XPath(".//tbody/tr"));
                count = tableRows.Count;
            },
            exceptionsToCatch: [new StaleElementReferenceException()]);

            return count;
        }

        public List<UserSkillDto> GetAllRowsOfResultsTable()
        {
            var rows = ResultsTable.FindElements(By.XPath(".//tbody/tr"));
            var result = new List<UserSkillDto>();

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));

                result.Add(new UserSkillDto
                {
                    FirstName = cells[0].Text.Trim(),
                    Surname = cells[1].Text.Trim(),
                    Email = cells[2].Text.Trim(),
                    Country = cells[3].Text.Trim(),
                    City = cells[4].Text.Trim(),
                    Skill = cells[5].Text.Trim(),
                    SkillCategory = cells[6].Text.Trim()
                });
            }

            return result;
        }

        public void VerifyIsAtSearchResultPage()
        {
            _driver.WaitUntilUrlContains("/search_result");

            Retry.Until(() =>
            {
                if (!ResultsHeader.Displayed)
                    throw new RetryException("Search Results page not loaded yet.");
            });

            Assert.That(NewSearchButton.Displayed, "Button for New Search is not visible.");
        }

        public void VerifyUserExists(string email)
        {
            IWebElement? userRow = FindUserRowByEmail(email);
            Assert.That(userRow, Is.Not.Null, $"User with email {email} was not found.");
        }

        public void VerifyUserDoesNotExist(string email) 
        { 
            var userRow = FindUserRowByEmail(email); 
            Assert.That(userRow, Is.Null, $"User with email {email} should NOT appear in the results."); 
        }

        public void VerifyAllRowsHaveSkill(string skillName)
        {
            var skillCells = GetColumnCells("Skill");

            Assert.That(skillCells, Is.Not.Empty, $"No rows found for skill '{skillName}'.");

            foreach (var cell in skillCells) 
            { 
                Assert.That(cell.Text.Trim(), Is.EqualTo(skillName), $"Expected skill '{skillName}', but found '{cell.Text.Trim()}'."); 
            }
        }

        public void VerifyAllRowsHaveCountry(string countryName)
        {
            var countryCells = GetColumnCells("Country");

            Assert.That(countryCells, Is.Not.Empty, $"No rows found for country '{countryName}'.");

            foreach (var cell in countryCells)
            {
                Assert.That(cell.Text.Trim(), Is.EqualTo(countryName), $"Expected country '{countryName}', but found '{cell.Text.Trim()}'.");
            }
        }

        public void VerifyRowsContainOnlyCountries(List<string> expectedCountries)
        {
            var countryCells = GetColumnCells("Country");

            Assert.That(countryCells, Is.Not.Empty, "No rows found in the Country column.");

            var actualCountries = countryCells
                .Select(c => c.Text.Trim())
                .ToList();

            var unexpectedCountries = actualCountries.Except(expectedCountries).ToList();

            Assert.That(unexpectedCountries, Is.Empty, $"Unexpected countries found: {string.Join(", ", unexpectedCountries)}");

            var missingCountries = expectedCountries.Except(actualCountries).ToList();

            Assert.That(missingCountries, Is.Empty, $"Expected cities not found: {string.Join(", ", missingCountries)}");
        }

        public void VerifyRowsContainOnlyCities(List<string> expectedCities)
        {
            var cityCells = GetColumnCells("City"); 

            Assert.That(cityCells, Is.Not.Empty, "No rows found in the City column."); 

            var actualCities = cityCells
                .Select(c => c.Text.Trim())
                .ToList();

            var unexpectedCities = actualCities.Except(expectedCities).ToList(); 
            
            Assert.That(unexpectedCities, Is.Empty, $"Unexpected cities found: {string.Join(", ", unexpectedCities)}");

            var missingCities = expectedCities.Except(actualCities).ToList(); 
            
            Assert.That(missingCities, Is.Empty, $"Expected cities not found: {string.Join(", ", missingCities)}");
        }

        public void VerifyNoUsersFound()
        {
            VerifyResultsTableIsNotVisible();
        }

        public void VerifyResultsTableIsVisible()
        {
            Assert.That(ResultsTable.Displayed, Is.True, "Results table should be visible.");
        }

        public void VerifyInfoMessage(string expectedMessage)
        {
            var actualMessage = MessageElement.Text.Trim();
            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }

        // Retry is added because of CI pipeline flakiness
        private int GetColumnIndex(string columnName)
        {
            int foundIndex = -1;

            Retry.Until(() =>
            {
                foundIndex = -1;

                var headers = _driver.FindElements(By.XPath("//table//thead//th"));

                for (int i = 0; i < headers.Count; i++)
                {
                    if (headers[i].Text.Trim().Equals(columnName, StringComparison.OrdinalIgnoreCase))
                        foundIndex = i;
                }

                if (foundIndex == -1)
                    throw new Exception($"Column '{columnName}' not found.");

            },
            exceptionsToCatch: [new StaleElementReferenceException(), new UnknownErrorException("unknown error")],
            waitInMilliseconds: 700);

            return foundIndex;
        }

        private IReadOnlyCollection<IWebElement> GetColumnCells(string columnName)
        {
            int columnIndex = GetColumnIndex(columnName);
            var cellsByColomn = _driver.FindElements(By.XPath($"//table//tbody//tr/td[{columnIndex}]"));

            return cellsByColomn;
        }

        // For negative scenarios
        private void VerifyResultsTableIsNotVisible()
        {
            var resultTables = _driver.FindElements(ResultsTableLocator);
            Assert.That(resultTables, Has.Count.EqualTo(0), "Results table should not be visible when no results by given criteria are found.");
        }
    }
}
