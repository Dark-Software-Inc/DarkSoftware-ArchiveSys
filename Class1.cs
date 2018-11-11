using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkSoft.OS.SecurityModel;
using System.IO;
using Ionic.Zip;
using Ionic;
namespace DarkSoftware
{
    public enum ReturnCode {
        Success = 0x01,
        Failed = 0x02, 
        NoSuchaDirectory = 0x03,
        NoAdminRights = 0x04,
        NoNameSpecified = 0x05,
        NoTypeSpecified = 0x06,
        WrongFType = 0x07,
        NoDirectorySpecified = 0x08,
        WrongPassword = 0x09,
        NoFilesToArchive = 0x0A
    } 
    public class DArchive
    {
        protected string arcN, adPwd, aPwd, sDir;
        protected ArchiveType aType;
        protected string[] fArc;
        public string[] filesInArc { get; set; }
        public string ArchivePassword { get; set; }
        public string AdditonalPassword { get; set; }
        public long ProgressSaving { get; set; }
        public long BytesToTransfer { get; set; }
        public string ArchiveDestination { get; set; }
        public string AlghorithmUsed { get; set; }
        public long ArchiveSize { get; set; }
        public string[] ArchivedFiles { get; set; }
        public string ArchiveName { get; set; }
        public ArchiveType Type { get; set; }
        public string SaveDir { get; set; }
        public ReturnCode ArchivingStat { get; set; }
        public enum ArchiveType
        {
            ZipArc,
            DarkSoftArc
        }
        public DArchive(ArchiveType arcType, string arcName, string[] filesInArchive, string saveDirect, string ArchivPassword = "", string AdditonalPasswordE = "")
        {
            Type = arcType;
            ArchiveName = arcName;
            ArchivePassword = ArchivPassword;
            AdditonalPassword = AdditonalPasswordE;
            SaveDir = saveDirect;
            filesInArc = filesInArchive;
        }
        public ReturnCode TryArcWNP() {
            //switch (Type) {
                //case ArchiveType.DarkSoftArc:
                    if (ArchiveName == null) return ReturnCode.NoNameSpecified;
                    if (ArchivePassword != null) return ReturnCode.WrongFType;
                    if (SaveDir == null) return ReturnCode.NoDirectorySpecified;
                    if (!Directory.Exists(SaveDir)) return ReturnCode.NoSuchaDirectory;
                    if (filesInArc.Count() <= 0) return ReturnCode.NoFilesToArchive;
                    if (AdditonalPassword != null) {
                        try
                        {
                            
                            ZipFile file = new ZipFile();
                            foreach (string f in filesInArc) {
                                file.AddFile(f);
                            }
                            file.Comment = "A archive system founded by Dark Software Inc.";
                            file.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                            file.Save((SaveDir + "\\" + ArchiveName.Trim() + ".zip").Trim());
                            SND_F_CREATE();
                            
                        }
                        catch (Exception ex) { throw new Exception(ex.Message); }
                    }
                    //break;
                //default:
                    //return ReturnCode.NoTypeSpecified;

            //}
            return ReturnCode.Success;
            
        }
        protected ReturnCode SND_F_CREATE() {
            string[] ARC_CONT = File.ReadAllLines((SaveDir + "\\" + ArchiveName.Trim() + ".zip").Trim());
            List<string> ENV_CONT = new List<string>();
            foreach (string s in ARC_CONT) {
                ENV_CONT.Add(new FileSecurity().encText(s, AdditonalPassword));
            }
            if (ENV_CONT.Count <= 0) { ArchivingStat = ReturnCode.Failed; return ReturnCode.Failed; }
            else {
                File.WriteAllLines((SaveDir + "\\" + ArchiveName.Trim() + ".dsoftarc").Trim(), ENV_CONT);
                File.Delete((SaveDir + "\\" + ArchiveName.Trim() + ".zip").Trim());
                //File.Move((SaveDir + "\\" + ArchiveName.Trim() + ".zip").Trim(), (SaveDir + "\\" + ArchiveName.Trim() + ".dsoftarc").Trim());
            }
            ArchiveDestination = (SaveDir + "\\" + ArchiveName.Trim() + ".dsoftarc").Trim();
            ArchivingStat = ReturnCode.Success;
            AlghorithmUsed = "AES-256 Bits";
            FileInfo fI = new FileInfo(ArchiveDestination);
            ArchiveSize = fI.Length;
            ArchivedFiles = filesInArc;
            return ReturnCode.Success;
        }
        protected void S_PROC_EVN(object sender, SaveProgressEventArgs e)
        {
            ProgressSaving = e.BytesTransferred;
            BytesToTransfer = e.TotalBytesToTransfer;
            if (e.BytesTransferred == e.TotalBytesToTransfer) {
                SND_F_CREATE();
            }
        }

    }
    public static class DoAct
    {
        public static void Arc(this DArchive arc)
        {
            switch (arc.Type)
            {
                case DArchive.ArchiveType.DarkSoftArc:
                    try
                    {


                        using (ZipFile file = new ZipFile(Directory.GetCurrentDirectory() + arc.ArchiveName))
                        {
                            foreach (string f in arc.filesInArc)
                            {
                                file.AddFile(f);
                            }
                            file.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                            file.Save();
                            string[] contOfArchive = File.ReadAllLines(Directory.GetCurrentDirectory() + arc.ArchiveName);
                            List<string> formattedCont = new List<string>();
                            foreach (string s in contOfArchive)
                            {
                                formattedCont.Add(new DarkSoft.OS.SecurityModel.FileSecurity().encText(s, arc.AdditonalPassword));
                            }
                            File.Move(Directory.GetCurrentDirectory() + arc.ArchiveName, Directory.GetCurrentDirectory() + arc.ArchiveName + ".dsoftarc");
                            File.WriteAllLines(Directory.GetCurrentDirectory() + arc.ArchiveName + ".dsoftarc", formattedCont);
                        }




                    }
                    catch (ZipException ex)
                    {
                        throw new ZipException(ex.Message);
                    }
                    break;
                    /*case Archive.ArchiveType.ZipArc:
                        if (!arc.ArchiveName.Contains(".zip"))
                        {
                            using (ZipFile file = new ZipFile(arc.ArchiveName))
                            {
                                foreach (string f in arc.filesInArc)
                                {
                                    file.AddFile(f);
                                }
                                file.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                                file.Save(arc.SaveDir + arc.ArchiveName + ".zip");
                            }
                        }
                        else {
                            using (ZipFile file = new ZipFile(arc.ArchiveName + ".zip"))
                            {
                                foreach (string f in arc.filesInArc)
                                {
                                    file.AddFile(f);
                                }
                                file.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                                file.Save(arc.SaveDir + arc.ArchiveName);
                            }
                        }
                        break;*/
            }
        }
    }
}
 
