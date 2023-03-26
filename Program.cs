using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace VisaBot
{
    class Program
    {
        private static IWebDriver driver;
        private static TelegramBotClient bot;

        static void Main(string[] args)
        {
            // Configurar el bot de Telegram
            bot = new TelegramBotClient("TOKEN_DE_ACCESO_AQUI");
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();

            // Iniciar sesión en la página de ais.usvisa-info.com
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://ais.usvisa-info.com/es-AR/niv");
            driver.FindElement(By.Id("ctl00_plhMain_lnkLogin")).Click();
            driver.FindElement(By.Id("ctl00_plhMain_txtUsername")).SendKeys("TU_USUARIO_AQUI");
            driver.FindElement(By.Id("ctl00_plhMain_txtPassword")).SendKeys("TU_CONTRASEÑA_AQUI");
            driver.FindElement(By.Id("ctl00_plhMain_btnSubmit")).Click();
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            // Verificar si el usuario envió el comando "/verificar"
            if (e.Message.Text == "/verificar")
            {
                // Navegar a la página de programación de citas
                driver.Navigate().GoToUrl("https://ais.usvisa-info.com/es-AR/niv/schedule/19179443/visa/choose-visa");

                // Obtener la lista de meses disponibles
                List<string> mesesDisponibles = new List<string>();
                var elementosMeses = driver.FindElements(By.ClassName("calendar_month_nav_link"));
                foreach (var elemento in elementosMeses)
                {
                    mesesDisponibles.Add(elemento.Text);
                }

                // Verificar si hay citas disponibles en el mes deseado
                string mesDeseado = "Julio"; // Reemplazar con el mes deseado
                if (mesesDisponibles.Contains(mesDeseado))
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "¡Hay citas disponibles en " + mesDeseado + "!");
                }
                else
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "No hay citas disponibles en " + mesDeseado + ".");
                }
            }
        }
    }
}
