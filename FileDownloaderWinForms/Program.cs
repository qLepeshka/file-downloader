namespace FileDownloaderWinForms
{
    static class Program
    {
        /// <summary>
        /// ������� ����� ����� ��� ����������.
        /// </summary>
        [STAThread] // ����� ����� ��� Windows Forms
        static void Main()
        {
            Application.EnableVisualStyles(); // �������� ���������� ����� (������ �������� ��� � ����������� Windows)
            Application.SetCompatibleTextRenderingDefault(false); // ������������� ����� ���������� ������ (������ false ��� ������� ��������)
            Application.Run(new Form1()); // ������� � ��������� ��������� ����� ������� ����� (Form1)
        }
    }
}