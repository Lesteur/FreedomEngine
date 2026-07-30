using System;
using System.IO;

using MyGame.Scripts;

namespace MyGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using var game = new Core();
                game.Run();
            }
            catch (Exception ex)
            {
                // Écrit l'erreur complète dans le dossier de l'exécutable
                File.WriteAllText("AOT_CrashLog.txt", ex.ToString());
                throw;
            }
        }
    }
}