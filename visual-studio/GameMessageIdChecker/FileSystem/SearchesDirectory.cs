namespace GameMessageIdChecker
{
    using System.IO;

    public static class SearchesDirectory
    {
        public delegate void FileEntryCallback(string fileEntry);

        public static void Go(string directoryFromExe, FileEntryCallback fileEntryCallback)
        {
            // ファイルを探せだぜ☆（＾～＾）
            var entries = Directory.GetFileSystemEntries(directoryFromExe);
            foreach (var fileEntry in entries)
            {
                if (File.Exists(fileEntry))
                {
                    // entry は、ファイルのフルパス☆（＾～＾）
                    fileEntryCallback(fileEntry);
                }
                else
                {
                    // これが　お馴染みの再帰関数だぜ☆（＾～＾）！
                    // ディレクトリーの中に　ごちゃごちゃ　ファイル入れてんなよ☆（＾～＾）
                    Go(fileEntry, fileEntryCallback);
                }
            }
        }
    }
}
