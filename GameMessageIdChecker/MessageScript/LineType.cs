namespace GameMessageIdChecker.MessageScript
{
    public enum LineType
    {
        /// <summary>
        /// enumの先頭はNoneが定番☆（＾～＾）
        /// 空行でもなく、行がないという意味だぜ☆（＾～＾）つまり使わない☆（＾～＾）
        /// </summary>
        None,

        /// <summary>
        /// 空行☆（＾～＾）改行、またはスペースしかない行☆（＾～＾）
        /// </summary>
        Empty,

        /// <summary>
        /// コメント☆（＾～＾）#で始まる行☆（＾～＾）
        /// </summary>
        Comment,

        /// <summary>
        /// 本文☆（＾～＾）
        /// </summary>
        Body,

        /// <summary>
        /// ID☆（＾～＾）$で始まる行☆（＾～＾）
        /// </summary>
        Id,

        /// <summary>
        /// 命令文☆（＾～＾）&で始まる行☆（＾～＾）
        /// </summary>
        Instruction,
    }
}
