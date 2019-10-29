namespace GameMessageIdChecker
{
    /// <summary>
    /// ソートするのに使うぜ☆（＾～＾）
    /// </summary>
    public class SortsModel
    {
        public string File { get; private set; }
        public int Row { get; private set; }
        public string Name { get; private set; }

        public SortsModel(string file, int row, string name)
        {
            this.File = file;
            this.Row = row;
            this.Name = name;
        }
    }
}
