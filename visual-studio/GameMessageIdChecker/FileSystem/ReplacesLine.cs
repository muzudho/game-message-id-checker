namespace GameMessageIdChecker
{
    using System;
    using System.IO;

    /// <summary>
    /// ファイルの行を置換するぜ☆（＾ｑ＾）
    /// </summary>
    public static class ReplacesLine
    {
        public static void Go(RenamesModel renames)
        {
            if (renames==null)
            {
                throw new ArgumentNullException(nameof(renames));
            }

            var dirty = false;
            var lines = File.ReadAllLines(renames.File);

            // 確認☆（＾～＾）
            if (lines[renames.Row - 1] == renames.OldName && lines[renames.Row - 1] != renames.NewName)
            {
                // 置換☆（＾～＾）
                lines[renames.Row - 1] = renames.NewName;
                dirty = true;
            }

            if (dirty)
            {
                File.WriteAllLines(renames.File, lines);
            }
        }
    }
}
