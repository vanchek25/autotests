using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace _1_test;

public class Tests
{
    public IWebDriver driver;
    public WebDriverWait wait;

    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3); // установка неявного ожидания
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3)); // явное ожидание

    }
    [TearDown]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }

    private void Authorize()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/"); // переход на страницу

        var login = driver.FindElement(By.Id("Username")); // поиск поля логина
        login.SendKeys("vanchek25@gmail.com"); // ввод логина

        var password = driver.FindElement(By.Id("Password")); // поиск поля пароля
        password.SendKeys("Avto20042513!"); // ввод пароля

        var enter = driver.FindElement(By.Name("button")); // поиск кнопки "Войти"
        enter.Click(); // клик по кнопке

        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Title']"))); // явное ожидание появления заголовка

    }

    [Test]
    public void AuthorizationTest() // тест авторизации
    {
        Authorize();

        Assert.That(driver.Title, Does.Contain("Новости"), "На главной странице мы не смогли найти заголовок Новости"); //проверка, что открыта нужная страница
    }

    [Test]
    public void NavigationMenuElementTest() // тест перехода в меню "Сообщества"
    {
        Authorize();

        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news")); // ожидание открытия страницы новостей

        var SidebarMenuButton = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']")); // кнопка открытия меню
        SidebarMenuButton.Click(); // клик по кнопке

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SidePage__root']"))); // ожидание появления боковой панели

        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed); // фильтрация отображаемого

        community.Click(); // клик по "Сообщества"

        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/communities")); // ожидание перехода на страницу
        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='Title']")); // поиск заголовка

        Assert.That(titlePageElement.Text, Does.Contain("Сообщества"), "При переходе на вкладку Сообщества мы не смогли найти заголовок Сообщества"); // проверка заголовка
    }
    [Test]
    public void SearchTest() // тест поисковой строки
    {
        Authorize();

        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news")); // ожидание перехода на главную

        var search = driver.FindElement(By.CssSelector("[data-tid='SearchBar']")); // поиск строки поиска
        search.Click(); // клик по строке поиска

        var searchInput = driver.FindElement(By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']")); // поиск поля ввода
        searchInput.SendKeys("чукин иван сергеевич"); // ввод запроса

        Assert.That(searchInput.GetAttribute("value"), Does.Contain("чукин иван сергеевич"), "Поле поиска должно содержать введенное значение");
    }
    [Test]
    public void VersionsChangeListTest() // тест журнала изменений версий
    {
        Authorize();
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));

        driver.Manage().Window.Maximize();

        var search = driver.FindElement(By.CssSelector("[data-tid='Version']"));
        search.Click();

        var PageElement = driver.FindElement(By.ClassName("react-ui-1cu2sp8")); // я помню, что говорили, что лучше не использовать классы, но тут нет уникальных локаторов...
        Assert.That(PageElement.Text, Does.Contain("Журнал изменений"), "При переходе на вкладку Сообщества мы не смогли найти заголовок Сообщества");
    }

    [Test]
    public void CommentTest() 
    {
        Authorize();

        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/comments");
        var search = driver.FindElement(By.CssSelector("[data-tid='AddComment']"));
        search.Click();

        var searchInput = driver.FindElement(By.CssSelector("[placeholder='Комментировать...']"));
        searchInput.SendKeys("автотесты топпп");

        var button = driver.FindElement(By.CssSelector("[data-tid='SendComment']"));
        button.Click();
        var comment = driver.FindElement(By.CssSelector("[data-tid='TextComment']")); // есть подозрение, что неправильный тестайди выбрал, там есть еще CommentItem, нужна обратная связь, чтобы разобраться

        Assert.That(comment.Text, Does.Contain("автотесты топпп"), "Твой комментарий не обнаружен");
        
    }
}