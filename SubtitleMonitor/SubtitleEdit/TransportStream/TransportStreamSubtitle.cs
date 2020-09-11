using System.Drawing;
using Nikse.SubtitleEdit.Core.Interfaces;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class TransportStreamSubtitle : IBinaryParagraph
    {
        public ulong StartMilliseconds { get; set; }

        public ulong EndMilliseconds { get; set; }

        public DvbSubPes Pes { get; set; }
        public int? ActiveImageIndex { get; set; }
        public bool IsDvbSub => Pes != null;
        public TransportStreamSubtitle()
        {
        }

        /// <summary>
        /// Gets full image if 'ActiveImageIndex' not set, otherwise only gets image by index
        /// </summary>
        /// <returns></returns>
        public Bitmap GetBitmap()
        {
            if (ActiveImageIndex.HasValue && ActiveImageIndex >= 0 && ActiveImageIndex < Pes.ObjectDataList.Count)
            {
                return (Bitmap)Pes.GetImage(Pes.ObjectDataList[ActiveImageIndex.Value]).Clone();
            }

            return Pes.GetImageFull();
        }

        public bool IsForced
        {
            get
            {
                return false;
            }
        }

        public Position GetPosition()
        {
            return new Position(0, 0);
        }


        public int NumberOfImages
        {
            get
            {
                if (Pes != null)
                {
                    return Pes.ObjectDataList.Count;
                }
                return 0;
            }
        }

    }
}
