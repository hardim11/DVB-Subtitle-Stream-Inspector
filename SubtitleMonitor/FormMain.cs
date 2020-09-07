using Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleMonitor
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void DoIt(string Filepath, bool ShowAll = false)
        {
            TransportStreamParser tsParser = new TransportStreamParser();
            tsParser.Parse(Filepath, (pos, total) => UpdateProgress(pos, total, "Parsing Transport Stream file. Please wait..."));

            foreach (var item in tsParser.SubtitlesLookup)
            {
                string res = "<html><body>";
                res += GetHtmlPid(item);
                res += "</body></html>";
                string outFile = @"C:\projects\c#\SubtitleMonitor\SubtitleMonitor\bin\Debug\" + Path.GetFileNameWithoutExtension(Filepath) + "_(PID" + item.Key.ToString() + ").html";
                File.WriteAllText(outFile, res);
            }
            PopulateTreeView(tsParser);

            Console.Beep(900, 20);
        }

        private void PopulateTreeView(TransportStreamParser TsParser)
        {
            this.treeView1.Nodes.Clear();
            foreach (var aPid in TsParser.SubtitlesLookup)
            {
                TreeNode pidNode = this.treeView1.Nodes.Add(aPid.Key.ToString());
                //do the PES
                foreach (var aDvbPes in aPid.Value)
                {
                    TreeNode pesNode = pidNode.Nodes.Add(aDvbPes.PresentationTimestamp.ToString() + " (" +  aDvbPes.SubtitleSegments.Count.ToString() + " segments)");


                    foreach (var aSubtitleSegment in aDvbPes.SubtitleSegments)
                    {
                        TreeNode segmentNode = pesNode.Nodes.Add(aSubtitleSegment.SegmentTypeDescription);
                        segmentNode.Tag = aSubtitleSegment;

                        //switch (aSubtitleSegment.SegmentType)
                        //{
                        //    case SubtitleSegment.PageCompositionSegment:
                        //        res += aSubtitleSegment.PageComposition.ToHtml();
                        //        break;
                        //    case SubtitleSegment.RegionCompositionSegment:
                        //        res += aSubtitleSegment.RegionComposition.ToHtml();
                        //        break;
                        //    case SubtitleSegment.ClutDefinitionSegment:
                        //        res += aSubtitleSegment.ClutDefinition.ToHtml();
                        //        break;
                        //    case SubtitleSegment.ObjectDataSegment:
                        //        res += aSubtitleSegment.ObjectData.ToHtml();
                        //        break;
                        //    case SubtitleSegment.DisplayDefinitionSegment:
                        //        res += aSubtitleSegment.DisplayDefinition.ToHtml();
                        //        break;
                        //    case SubtitleSegment.EndOfDisplaySetSegment:
                        //        res += aSubtitleSegment.EndOfDisplaySet.ToHtml();
                        //        break;
                        //    default:
                        //        Console.WriteLine("Unknown Segment Type");
                        //        break;
                        //}



                    }


                }
            }
        }

        private string GetHtmlPid(KeyValuePair<int, List<DvbSubPes>> SubtitlePid)
        {
            string res = "<table border=\"1\">\r\n";

            res += "<tr><th>PID: " + SubtitlePid.Key.ToString() + "</th><td>&nbsp;</td></tr>\r\n";


            res += "<tr><td>&nbsp;</td><td colsp=\"1\"><table border=\"1\">";
            foreach (var aDvbPes in SubtitlePid.Value)
            {
                res += "<tr><th>PTS: " + aDvbPes.PresentationTimestamp.ToString() + "</th><td><table border=\"1\" width=\"100%\">\r\n";

                foreach (var aSubtitleSegment in aDvbPes.SubtitleSegments)
                {
                    res += "<tr><td>" + aSubtitleSegment.SegmentTypeDescription + "</td><td>Page ID:" + aSubtitleSegment.PageId.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>&nbsp;</td><td>";
                    switch (aSubtitleSegment.SegmentType)
                    {
                        case SubtitleSegment.PageCompositionSegment:
                            res += aSubtitleSegment.PageComposition.ToHtml();
                            break;
                        case SubtitleSegment.RegionCompositionSegment:
                            res += aSubtitleSegment.RegionComposition.ToHtml();
                            break;
                        case SubtitleSegment.ClutDefinitionSegment:
                            res += aSubtitleSegment.ClutDefinition.ToHtml();
                            break;
                        case SubtitleSegment.ObjectDataSegment:
                            res += aSubtitleSegment.ObjectData.ToHtml();
                            break;
                        case SubtitleSegment.DisplayDefinitionSegment:
                            res += aSubtitleSegment.DisplayDefinition.ToHtml();
                            break;
                        case SubtitleSegment.EndOfDisplaySetSegment:
                            res += aSubtitleSegment.EndOfDisplaySet.ToHtml();
                            break;
                        default:
                            Console.WriteLine("Unknown Segment Type");
                            break;
                    }


                    res += "</td></tr>\r\n";
                }
                res += "</table></td>";
            }
            res += "</table></td></tr>\r\n";
            
            res += "</table>\r\n";

            return res;
        }

        private void UpdateProgress(long position, long total, string statusMessage)
        {
            var percent = (int)Math.Round(position * 100.0 / total);
            Console.WriteLine(statusMessage + " progess "  + percent.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = @"C:\Video\HBO-Hungary-20200903.ts";
            DoIt(fileName, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = @"C:\Video\HBO-Encoders\232_250_1_8.ts";
            DoIt(fileName, true);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.treeView1.SelectedNode == null) { return; }

            if (this.treeView1.SelectedNode.Tag != null)
            {
                string tagType = this.treeView1.SelectedNode.Tag.GetType().ToString();
                Console.WriteLine(tagType);



                switch (tagType)
                {
                    case SubtitleSegment.PageCompositionSegment:
                        res += aSubtitleSegment.PageComposition.ToHtml();
                        break;
                    case SubtitleSegment.RegionCompositionSegment:
                        res += aSubtitleSegment.RegionComposition.ToHtml();
                        break;
                    case SubtitleSegment.ClutDefinitionSegment:
                        res += aSubtitleSegment.ClutDefinition.ToHtml();
                        break;
                    case SubtitleSegment.ObjectDataSegment:
                        res += aSubtitleSegment.ObjectData.ToHtml();
                        break;
                    case SubtitleSegment.DisplayDefinitionSegment:
                        res += aSubtitleSegment.DisplayDefinition.ToHtml();
                        break;
                    case SubtitleSegment.EndOfDisplaySetSegment:
                        res += aSubtitleSegment.EndOfDisplaySet.ToHtml();
                        break;
                    default:
                        Console.WriteLine("Unknown Segment Type");
                        break;
                }

            }
            else
            {
                Console.WriteLine("tag null");
            }
        }
    }
}
