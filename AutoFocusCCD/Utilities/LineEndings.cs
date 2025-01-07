
namespace Multi_Camera_MINI_AOI_V3.Utilities
{
    public static class LineEnding
    {
        public enum LineEndingType
        {
            NONE,
            CR,
            LF,
            CRLF
        }

        public static string ToLineEnding(this LineEndingType lineEnding)
        {
            switch (lineEnding)
            {
                case LineEndingType.CR:
                    return "\r";
                case LineEndingType.LF:
                    return "\n";
                case LineEndingType.CRLF:
                    return "\r\n";
                default:
                    return string.Empty;
            }
        }
    }
}
