using SubtitleMonitor;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class ObjectDataSegment
    {
        public int ObjectId { get; set; }
        public int ObjectVersionNumber { get; set; }

        /// <summary>
        /// 0x00 coding of pixels, 0x01 coded as a string of characters
        /// </summary>
        public int ObjectCodingMethod { get; set; }
        public string ObjectCodingMethodString 
        { 
            get
            {
                switch (ObjectCodingMethod)
                {
                    case 0:
                        return "coding of pixels";
                    case 1:
                        return "coded as a string of characters";
                    case 2:
                        return "progressive coding of pixels";
                    default:
                        return "unkonwn or reserved";
                }
            } 
        }

        public bool NonModifyingColorFlag { get; set; }

        public int TopFieldDataBlockLength { get; set; }
        public int BottomFieldDataBlockLength { get; set; }

        public int NumberOfCodes { get; set; }

        public List<string> FirstDataTypes = new List<string>();
        public Bitmap Image { get; set; }
        private FastBitmap _fastImage;

        public static int PixelDecoding2Bit => 0x10;
        public static int PixelDecoding4Bit => 0x11;
        public static int PixelDecoding8Bit => 0x12;
        public static int MapTable2To4Bit => 0x20;
        public static int MapTable2To8Bit => 0x21;
        public static int MapTable4To8Bit => 0x22;
        public static int EndOfObjectLineCode => 0xf0;

        public int BufferIndex { get; private set; }

        public ObjectDataSegment(byte[] buffer, int index)
        {
            ObjectId = Helper.GetEndianWord(buffer, index);
            ObjectVersionNumber = buffer[index + 2] >> 4;
            ObjectCodingMethod = (buffer[index + 2] & 0b00001100) >> 2;
            NonModifyingColorFlag = (buffer[index + 2] & 0b00000010) > 0;

            switch (ObjectCodingMethod)
            {
                case 0: // interlaced bitmap
                    TopFieldDataBlockLength = Helper.GetEndianWord(buffer, index + 3);
                    BottomFieldDataBlockLength = Helper.GetEndianWord(buffer, index + 5);


                    // get top field data
                    //      loop pixel-data_sub-block() 

                    // get bottom field data
                    //      loop pixel-data_sub-block() 


                    break;
                case 1: // teletext
                    NumberOfCodes = buffer[index + 3];


                    // loop of character codes
                    //      loop character_code 
                    break;
                case 2: //; progressive bitmap
                        // get progressive_pixel_block()


                default:
                    break;
            }

            
            
            BufferIndex = index;
        }

        public void DecodeImage(byte[] buffer, int index, ClutDefinitionSegment cds)
        {
            if (ObjectCodingMethod == 0)
            {
                var twoToFourBitColorLookup = new List<int> { 0, 1, 2, 3 };
                var twoToEightBitColorLookup = new List<int> { 0, 1, 2, 3 };
                var fourToEightBitColorLookup = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

                int pixelCode = 0;
                int runLength = 0;
                int dataType = buffer[index + 7];
                int length = TopFieldDataBlockLength;

                if (length + index + 7 > buffer.Length) // check if buffer is large enough
                {
                    Image = new Bitmap(1, 1);
                    return;
                }

                index += 8;
                int start = index;
                int x = 0;
                int y = 0;

                // Pre-decoding to determine image size
                int width = 0;
                while (index < start + TopFieldDataBlockLength)
                {
                    index = CalculateSize(buffer, index, ref dataType, start, ref x, ref y, length, ref runLength, ref width);
                }
                if (width > 2000)
                {
                    width = 2000;
                }

                if (y > 500)
                {
                    y = 500;
                }

                Image = new Bitmap(width, y + 1);
                _fastImage = new FastBitmap(Image);
                _fastImage.LockImage();

                x = 0;
                y = 0;
                index = start;
                while (index < start + TopFieldDataBlockLength)
                {
                    index = ProcessDataType(buffer, index, cds, ref dataType, start, twoToFourBitColorLookup, fourToEightBitColorLookup, twoToEightBitColorLookup, ref x, ref y, length, ref pixelCode, ref runLength);
                }

                x = 0;
                y = 1;
                if (BottomFieldDataBlockLength == 0)
                {
                    index = start;
                }
                else
                {
                    length = BottomFieldDataBlockLength;
                    index = start + TopFieldDataBlockLength;
                    start = index;
                }
                dataType = buffer[index - 1];
                while (index < start + BottomFieldDataBlockLength - 1)
                {
                    index = ProcessDataType(buffer, index, cds, ref dataType, start, twoToFourBitColorLookup, fourToEightBitColorLookup, twoToEightBitColorLookup, ref x, ref y, length, ref pixelCode, ref runLength);
                }
                _fastImage.UnlockImage();
            }
            else if (ObjectCodingMethod == 1)
            {
                Image = new Bitmap(1, 1);
                NumberOfCodes = buffer[index + 3];
            }
        }

        private int ProcessDataType(byte[] buffer, int index, ClutDefinitionSegment cds, ref int dataType, int start,
                                    List<int> twoToFourBitColorLookup, List<int> fourToEightBitColorLookup, List<int> twoToEightBitColorLookup,
                                    ref int x, ref int y, int length, ref int pixelCode, ref int runLength)
        {
            if (dataType == PixelDecoding2Bit)
            {
                int bitIndex = 0;
                while (index < start + length - 1 && TwoBitPixelDecoding(buffer, ref index, ref bitIndex, out pixelCode, out runLength))
                {
                    DrawPixels(cds, twoToFourBitColorLookup[pixelCode], runLength, ref x, ref y);
                }
            }
            else if (dataType == PixelDecoding4Bit)
            {
                bool startHalf = false;
                while (index < start + length - 1 && FourBitPixelDecoding(buffer, ref index, ref startHalf, out pixelCode, out runLength))
                {
                    DrawPixels(cds, fourToEightBitColorLookup[pixelCode], runLength, ref x, ref y);
                }
            }
            else if (dataType == PixelDecoding8Bit)
            {
                while (index < start + length - 1 && EightBitPixelDecoding(buffer, ref index, out pixelCode, out runLength))
                {
                    DrawPixels(cds, pixelCode, runLength, ref x, ref y);
                }
            }
            else if (dataType == MapTable2To4Bit)
            {
                //4 entry numbers of 4-bits each; entry number 0 first, entry number 3 last
                twoToFourBitColorLookup[0] = buffer[index] >> 4;
                twoToFourBitColorLookup[1] = buffer[index] & 0b00001111;
                index++;
                twoToFourBitColorLookup[2] = buffer[index] >> 4;
                twoToFourBitColorLookup[3] = buffer[index] & 0b00001111;
                index++;
            }
            else if (dataType == MapTable2To8Bit)
            {
                //4 entry numbers of 8-bits each; entry number 0 first, entry number 3 last
                twoToEightBitColorLookup[0] = buffer[index];
                index++;
                twoToEightBitColorLookup[1] = buffer[index];
                index++;
                twoToEightBitColorLookup[2] = buffer[index];
                index++;
                twoToEightBitColorLookup[3] = buffer[index];
                index++;
            }
            else if (dataType == MapTable4To8Bit)
            {
                // 16 entry numbers of 8-bits each
                for (int k = 0; k < 16; k++)
                {
                    fourToEightBitColorLookup[k] = buffer[index];
                    index++;
                }
            }
            else if (dataType == EndOfObjectLineCode)
            {
                x = 0;
                y += 2; // interlaced - skip one line
            }

            if (index < start + length)
            {
                dataType = buffer[index];
                index++;
            }

            return index;
        }

        private static int CalculateSize(byte[] buffer, int index, ref int dataType, int start, ref int x, ref int y, int length, ref int runLength, ref int width)
        {
            if (dataType == PixelDecoding2Bit)
            {
                int bitIndex = 0;
                while (index < start + length - 1 && TwoBitPixelDecoding(buffer, ref index, ref bitIndex, out _, out runLength))
                {
                    x += runLength;
                }
            }
            else if (dataType == PixelDecoding4Bit)
            {
                bool startHalf = false;
                while (index < start + length - 1 && FourBitPixelDecoding(buffer, ref index, ref startHalf, out _, out runLength))
                {
                    x += runLength;
                }
            }
            else if (dataType == PixelDecoding8Bit)
            {
                while (index < start + length - 1 && EightBitPixelDecoding(buffer, ref index, out _, out runLength))
                {
                    x += runLength;
                }
            }
            else if (dataType == MapTable2To4Bit)
            {
                index += 2;
            }
            else if (dataType == MapTable2To8Bit)
            {
                index += 4;
            }
            else if (dataType == MapTable4To8Bit)
            {
                index += 16;
            }
            else if (dataType == EndOfObjectLineCode)
            {
                x = 0;
                y += 2; // interlaced - skip one line
            }

            if (index < start + length)
            {
                dataType = buffer[index];
                index++;
            }
            if (x > width)
            {
                width = x;
            }

            return index;
        }

        private void DrawPixels(ClutDefinitionSegment cds, int pixelCode, int runLength, ref int x, ref int y)
        {
            var c = Color.Red;
            if (cds != null)
            {
                foreach (var item in cds.Entries)
                {
                    if (item.ClutEntryId == pixelCode)
                    {
                        c = item.GetColor();
                        break;
                    }
                }
            }

            for (int k = 0; k < runLength; k++)
            {
                if (y < _fastImage.Height && x < _fastImage.Width)
                {
                    _fastImage.SetPixel(x, y, c);
                }

                x++;
            }
        }

        private static int Next8Bits(byte[] buffer, ref int index)
        {
            int result = buffer[index];
            index++;
            return result;
        }

        private static bool EightBitPixelDecoding(byte[] buffer, ref int index, out int pixelCode, out int runLength)
        {
            pixelCode = 0;
            runLength = 1;
            int firstByte = Next8Bits(buffer, ref index);
            if (firstByte != 0)
            {
                runLength = 1;
                pixelCode = firstByte;
            }
            else
            {
                int nextByte = Next8Bits(buffer, ref index);
                if (nextByte >> 7 == 0)
                {
                    if (nextByte != 0)
                    {
                        runLength = nextByte & 0b01111111; // 1-127
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    runLength = nextByte & 0b01111111; // 3-127
                    pixelCode = Next8Bits(buffer, ref index);
                }
            }
            return true;
        }

        private static int Next4Bits(byte[] buffer, ref int index, ref bool startHalf)
        {
            int result;
            if (startHalf)
            {
                startHalf = false;
                result = buffer[index] & 0b00001111;
                index++;
            }
            else
            {
                startHalf = true;
                result = buffer[index] >> 4;
            }
            return result;
        }

        private static bool FourBitPixelDecoding(byte[] buffer, ref int index, ref bool startHalf, out int pixelCode, out int runLength)
        {
            pixelCode = 0;
            runLength = 1;
            int first = Next4Bits(buffer, ref index, ref startHalf);
            if (first != 0)
            {
                pixelCode = first;
                runLength = 1;
            }
            else
            {
                int next1 = Next4Bits(buffer, ref index, ref startHalf);
                if ((next1 & 0b00001000) == 0)
                {
                    if (next1 != 0)
                    {
                        runLength = (next1 & 0b00000111) + 2; // 3-9
                    }
                    else
                    {
                        if (startHalf)
                        {
                            startHalf = false;
                            index++;
                        }
                        return false;
                    }
                }
                else if (next1 == 0b00001100)
                {
                    runLength = 1;
                    pixelCode = 0;
                }
                else if (next1 == 0b00001101)
                {
                    runLength = 2;
                    pixelCode = 0;
                }
                else
                {
                    int next2 = Next4Bits(buffer, ref index, ref startHalf);
                    if ((next1 & 0b00000100) == 0)
                    {
                        runLength = (next1 & 0b00000011) + 4; // 4-7
                        pixelCode = next2;
                    }
                    else
                    {
                        int next3 = Next4Bits(buffer, ref index, ref startHalf);
                        if ((next1 & 0b00000001) == 0)
                        {
                            runLength = next2 + 9; // 9-24
                            pixelCode = next3;
                        }
                        else if (next1 == 0b00001111)
                        {
                            runLength = ((next2 << 4) + next3) + 25; // 25-280
                            pixelCode = Next4Bits(buffer, ref index, ref startHalf);
                        }
                    }
                }
            }
            return true;
        }

        private static int Next2Bits(byte[] buffer, ref int index, ref int bitIndex)
        {
            int result;
            if (bitIndex == 0)
            {
                bitIndex++;
                result = (buffer[index] & 0b11000000) >> 6;
            }
            else if (bitIndex == 1)
            {
                bitIndex++;
                result = (buffer[index] & 0b00110000) >> 4;
            }
            else if (bitIndex == 2)
            {
                bitIndex++;
                result = (buffer[index] & 0b00001100) >> 2;
            }
            else // 3 - last bit pair
            {
                bitIndex = 0;
                result = buffer[index] & 0b00000011;
                index++;
            }
            return result;
        }

        private static bool TwoBitPixelDecoding(byte[] buffer, ref int index, ref int bitIndex, out int pixelCode, out int runLength)
        {
            runLength = 1;
            pixelCode = 0;
            int first = Next2Bits(buffer, ref index, ref bitIndex);
            if (first != 0)
            {
                runLength = 1;
                pixelCode = first;
            }
            else
            {
                int next = Next2Bits(buffer, ref index, ref bitIndex);
                if (next == 1)
                {
                    runLength = 1;
                    pixelCode = 0;
                }
                else if (next > 1)
                {
                    runLength = ((next & 0b00000001) << 2) + Next2Bits(buffer, ref index, ref bitIndex) + 3; // 3-10
                    pixelCode = Next2Bits(buffer, ref index, ref bitIndex);
                }
                else
                {
                    int next2 = Next2Bits(buffer, ref index, ref bitIndex);
                    if (next2 == 0b00000001)
                    {
                        runLength = 2;
                        pixelCode = 0;
                    }
                    else if (next2 == 0b00000010)
                    {
                        runLength = (Next2Bits(buffer, ref index, ref bitIndex) << 2) +  // 12-27
                                     Next2Bits(buffer, ref index, ref bitIndex) + 12;
                        pixelCode = Next2Bits(buffer, ref index, ref bitIndex);
                    }
                    else if (next2 == 0b00000011)
                    {
                        runLength = (Next2Bits(buffer, ref index, ref bitIndex) << 6) + // 29 - 284
                                    (Next2Bits(buffer, ref index, ref bitIndex) << 4) +
                                    (Next2Bits(buffer, ref index, ref bitIndex) << 2) +
                                        Next2Bits(buffer, ref index, ref bitIndex) + 29;

                        pixelCode = Next2Bits(buffer, ref index, ref bitIndex);
                    }
                    else
                    {
                        if (bitIndex != 0)
                        {
                            index++;
                        }

                        return false; // end of 2-bit/pixel code string
                    }
                }
            }
            return true;
        }

        public string ToHtml()
        {
            string res = "<table>\r\n";

            res += "<tr><td>ObjectId:</td<td>" + ObjectId.ToString() + "</td></tr>\r\n";
            res += "<tr><td>ObjectVersionNumber:</td<td>" + ObjectVersionNumber.ToString() + "</td></tr>\r\n";
            res += "<tr><td>ObjectCodingMethod:</td<td>" + ObjectCodingMethod.ToString() + "</td></tr>\r\n";
            switch (ObjectCodingMethod)
            {
                case 0:
                    res += "<tr><td>ObjectCodingMethod:</td<td>Coding of Pixels (interlaced)</td></tr>\r\n";
                    break;
                case 1:
                    res += "<tr><td>ObjectCodingMethod:</td<td>Coded as string of characters</td></tr>\r\n";
                    break;
                case 2:
                    res += "<tr><td>ObjectCodingMethod:</td<td>progressive coding of Pixels</td></tr>\r\n";
                    break;
                case 3:
                    res += "<tr><td>ObjectCodingMethod:</td<td>reserved</td></tr>\r\n";
                    break;
                default:
                    break;
            }
            res += "<tr><td>NonModifyingColorFlag:</td<td>" + (NonModifyingColorFlag ? "non modifying colour" : "modifying colour") + "</td></tr>\r\n";

            res += "<tr><td>TopFieldDataBlockLength:</td<td>" + TopFieldDataBlockLength.ToString() + "</td></tr>\r\n";
            res += "<tr><td>BottomFieldDataBlockLength:</td<td>" + BottomFieldDataBlockLength.ToString() + "</td></tr>\r\n";
            res += "<tr><td>NumberOfCodes:</td<td>" + NumberOfCodes.ToString() + "</td></tr>\r\n";

            //res += "<tr><td>RegionClutId:</td<td>" + RegionClutId.ToString() + "</td></tr>\r\n";
            //res += "<tr><td>Region8BitPixelCode:</td<td>" + Region8BitPixelCode.ToString() + "</td></tr>\r\n";
            //res += "<tr><td>Region4BitPixelCode:</td<td>" + Region4BitPixelCode.ToString() + "</td></tr>\r\n";
            //res += "<tr><td>Region2BitPixelCode:</td<td>" + Region2BitPixelCode.ToString() + "</td></tr>\r\n";

            //if (this.Objects.Count < 1)
            //{
            //    res += "<tr><td colspan=\"2\"><b>No Objects</b></td></tr>\r\n";
            //}
            //else
            //{
            //    res += "<tr><td colspan=\"2\"><b>Objects:</b></td></tr>\r\n";

            //    foreach (RegionCompositionSegmentObject item in this.Objects)
            //    {
            //        res += "<tr><td>&nbsp;</td><td style=\"border: 1px solid black;\"><table>";
            //        res += "<tr><td>ObjectId</td><td>" + item.ObjectId.ToString() + "</td></tr>\r\n";
            //        res += "<tr><td>ObjectType</td><td>" + item.ObjectType.ToString() + "</td></tr>\r\n";
            //        res += "<tr><td>ObjectProviderFlag</td><td>" + item.ObjectProviderFlag.ToString() + "</td></tr>\r\n";
            //        res += "<tr><td>ObjectHorizontalPosition</td><td>" + item.ObjectHorizontalPosition.ToString() + "</td></tr>\r\n";
            //        res += "<tr><td>ObjectVerticalPosition</td><td>" + item.ObjectVerticalPosition.ToString() + "</td></tr>\r\n";
            //        res += "<tr><td>ObjectForegroundPixelCode</td><td>" + item.ObjectForegroundPixelCode.ToString() + "</td></tr>\r\n";
            //        res += "<tr><td>ObjectBackgroundPixelCode</td><td>" + item.ObjectBackgroundPixelCode.ToString() + "</td></tr>\r\n";
            //        res += "</table></td></tr>";
            //    }
            //}
            res += "</table>\r\n";

            return res;
        }


        public void PopulateListViewDetails(ListView Lv)
        {
            Lv.Items.Clear();
            ListViewGroup grpGeneral = Lv.Groups.Add("General", "General");

            Utils.AddListViewEntry(
                Lv,
                "object_id",
                this.ObjectId.ToString(),
                "Uniquely identifies within the page the object for which data is contained in this object_data_segment field.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "object_version_number",
                this.ObjectVersionNumber.ToString(),
                "Indicates the version of this segment data. When any of the contents of this segment change, this version number is incremented(modulo 16).",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "object_coding_method",
                this.ObjectCodingMethod.ToString() + " (" + this.ObjectCodingMethodString + ")",
                "Specifies the method used to code the object",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "non_modifying_colour_flag",
                this.NonModifyingColorFlag.ToString(),
                "If set to '1' this indicates that the CLUT entry value '1' is a non modifying colour. When the non modifying colour is assigned to an object pixel, then the pixel of the underlying region background or object shall not be modified.This can be used to create \"transparent holes\" in objects.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "top_field_data_block_length",
                this.TopFieldDataBlockLength.ToString(),
                "Specifies the number of bytes contained in the pixel-data_sub-blocks for the top field (when coded as pixels)",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "bottom_field_data_block_length",
                this.BottomFieldDataBlockLength.ToString(),
                "Specifies the number of bytes contained in the data_sub-block for the bottom field (when coded as pixels)",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "number_of_codes",
                this.NumberOfCodes.ToString(),
                "Specifies the number of character codes in the string (when coded as characters)",
                grpGeneral
            );
            

        }
    }
}
