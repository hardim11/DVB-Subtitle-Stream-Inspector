﻿namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class EndOfDisplaySetSegment
    {

        public EndOfDisplaySetSegment(byte[] buffer, int index)
        {
        }

        public string ToHtml()
        {
            string res = "<p>End of Display Set Segment has no attributes</p>";
            return res;
        }
    }
}
