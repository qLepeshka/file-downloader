namespace FileDownloaderWinForms
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread] // Очень важно для Windows Forms
        static void Main()
        {
            Application.EnableVisualStyles(); // Включает визуальные стили (кнопки выглядят как в современной Windows)
            Application.SetCompatibleTextRenderingDefault(false); // Устанавливает режим рендеринга текста (обычно false для лучшего качества)
            Application.Run(new Form1()); // Создает и запускает экземпляр вашей главной формы (Form1)
        }
    }
}