using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Soundfont2
{
    public class Soundfont2_reader
    {
        public Soundfont2.RIFF fileData;
        private string currFilePath;
        bool fileRead = false;

        public string lastError = "";

        public string FilePath
        {
            get { return currFilePath; }
        }
        public Soundfont2_reader()
        {
            fileData = new RIFF();
        }



        public int getInstrumenSampleCount(int instIndex)
        {
            if (fileRead == false) return -1;
            pdta_rec pdta = fileData.sfbk.pdta;
            int sampleCount = 0;
            int startIbagIndex = pdta.inst[instIndex].wInstBagNdx;
            int endIbagIndex = pdta.inst[instIndex+1].wInstBagNdx;
            for (int bi=startIbagIndex;bi<endIbagIndex;bi++)
            {
                int startIgenIndex = pdta.ibag[bi].wGenNdx;
                int endIgenIndex = pdta.ibag[bi+1].wGenNdx;
                for (int gi=startIgenIndex; gi<endIgenIndex; gi++)
                {
                    if (pdta.igen[gi].sfGenOper == SFGenerator.sampleID) sampleCount++;
                }
            }
            return sampleCount;
        }

        public bool readFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            fileData.size = fi.Length;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.UTF8))
                {
                    string riffTag = new String(br.ReadCharsSafe(4));
                    if (riffTag != "RIFF")
                    {
                        lastError = "this is not a RIFF fileformat";
                        return false;
                    }
                    fileData.sfbk.size = br.ReadUInt32();
                    string sfbkTag = new string(br.ReadCharsSafe(4));
                    if (sfbkTag != "sfbk")
                    {
                        lastError = "this is not sfbk fileformat";
                        return false;
                    }
                    string listType = "sfbk";
                    while (br.PeekChar() != -1)
                    {
                        // every block starts with a LIST tag
                        string listTag = new string(br.ReadCharsSafe(4));
                        if (listTag != "LIST")
                        {
                            lastError = "LIST item not found after " + listType;
                            return false;
                        }
                        UInt32 listSize = br.ReadUInt32();
                        listType = new string(br.ReadCharsSafe(4));
                        if (listType == "INFO")
                        {
                            fileData.sfbk.info.size = listSize;
                            
                            if (readInfoBlock(br/*, fs.Position + listSize - 4*/) == false) return false;

                            //return true; // early return debug test
                        }
                        else if (listType == "sdta")
                        {
                            fileData.sfbk.sdta.size = listSize;
                            if (read_sdta_block(br/*, fs.Position + listSize + 4*/) == false) return false;

                            //return true; // early return debug test
                        }
                        else if (listType == "pdta")
                        {
                            fileData.sfbk.pdta.size = listSize;
                            if (read_pdta_block(br/*, fs.Position + listSize + 4*/) == false) return false;
                        }
                    }
                    

                }
            }
            fileRead = true;
            currFilePath = filePath;
            return true;
        }

        private bool readInfoBlock(BinaryReader br)
        {
            INFO info = fileData.sfbk.info; // simplify the usage
            while (br.PeekChar() != -1)
            {
                string type = new string(br.ReadCharsSafe(4));
                //Debug.rtxt.AppendLine("type: " + type);
                if (type == "ifil") info.ifil = new sfVersionTag(br);
                else if (type == "isng") info.isng = br.ReadStringSafeUsingLeadingSize();
                else if (type == "INAM") info.INAM = br.ReadStringSafeUsingLeadingSize();
                else if (type == "irom") info.irom = br.ReadStringSafeUsingLeadingSize();
                else if (type == "iver") info.iver = new sfVersionTag(br);
                else if (type == "ICRD") info.ICRD = br.ReadStringSafeUsingLeadingSize();
                else if (type == "IENG") info.IENG = br.ReadStringSafeUsingLeadingSize();
                else if (type == "IPRD") info.IPRD = br.ReadStringSafeUsingLeadingSize();
                else if (type == "ICOP") info.ICOP = br.ReadStringSafeUsingLeadingSize();
                else if (type == "ICMT") info.ICMT = br.ReadStringSafeUsingLeadingSize();
                else if (type == "ISFT") info.ISFT = br.ReadStringSafeUsingLeadingSize();
                else if (type == "LIST") {
                    br.BaseStream.Position -= 4; // skip back
                    return true;
                }
            }
            return true;
        }
        /// <summary>
        /// reads data offset pointers and sizes of sample data, not the actual data
        /// as the data is read from file on demand
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private bool read_sdta_block(BinaryReader br)
        {
            sdta_rec sdta = fileData.sfbk.sdta; // simplify the usage
            while (br.PeekChar() != -1)
            {
                string type = new string(br.ReadCharsSafe(4));
                //Debug.rtxt.AppendLine("sdta: " + type);
                if (type == "smpl") {
                    sdta.smpl = new smpl_rec(br);
                    br.BaseStream.Position += sdta.smpl.size; // skip sample data
                } else if (type == "sm24") {
                    sdta.sm24 = new smpl_rec(br);
                    br.BaseStream.Position += sdta.sm24.size; // skip sample data
                } else if (type == "LIST") {
                    br.BaseStream.Position -= 4; // skip back
                    return true;
                } else
                {
                    Debug.rtxt.AppendLine("unknown sdta block type:" + type);
                }
            }
            return true;
        }

        private bool read_pdta_block(BinaryReader br/*, long endPos*/)
        {
            pdta_rec pdta = fileData.sfbk.pdta; // simplify the usage
            string type = "";
            
        
            while (br.PeekChar() != -1)
            {
                try { type = new string(br.ReadCharsSafe(4));}
                catch (Exception ex) { Debug.rtxt.AppendLine(ex); return false; }
                

                UInt32 size = br.ReadUInt32();
                //Debug.rtxt.AppendLine("pdta: " + type + ", size:" + size.ToString());
                if (type == "phdr")
                {
                    pdta.phdr = new phdr_rec[size/phdr_rec.Size];
                    for (int i=0;i<pdta.phdr.Length;i++)
                        pdta.phdr[i] = new phdr_rec(br);
                }
                else if (type == "pbag")
                {
                    pdta.pbag = new bag_rec[size/bag_rec.Size];
                    for (int i = 0; i < pdta.pbag.Length; i++)
                        pdta.pbag[i] = new bag_rec(br,"p");
                }
                else if (type == "pmod")
                {
                    pdta.pmod = new mod_rec[size/mod_rec.Size];
                    for (int i = 0; i < pdta.pmod.Length; i++)
                        pdta.pmod[i] = new mod_rec(br);
                }
                else if (type == "pgen")
                {
                    pdta.pgen = new gen_rec[size/gen_rec.Size];
                    for (int i = 0; i < pdta.pgen.Length; i++)
                        pdta.pgen[i] = new gen_rec(br);
                }
                else if (type == "inst")
                {
                    pdta.inst = new inst_rec[size/inst_rec.Size];
                    for (int i = 0; i < pdta.inst.Length; i++)
                        pdta.inst[i] = new inst_rec(br);
                }
                else if (type == "ibag")
                {
                    pdta.ibag = new bag_rec[size/bag_rec.Size];
                    for (int i = 0; i < pdta.ibag.Length; i++)
                        pdta.ibag[i] = new bag_rec(br,"i");
                }
                else if (type == "imod")
                {
                    pdta.imod = new mod_rec[size/mod_rec.Size];
                    for (int i = 0; i < pdta.imod.Length; i++)
                        pdta.imod[i] = new mod_rec(br);
                }
                else if (type == "igen")
                {
                    pdta.igen = new gen_rec[size/gen_rec.Size];
                    for (int i = 0; i < pdta.igen.Length; i++)
                        pdta.igen[i] = new gen_rec(br);
                }
                else if (type == "shdr")
                {
                    pdta.shdr = new shdr_rec[size/shdr_rec.Size];
                    for (int i = 0; i < pdta.shdr.Length; i++)
                        pdta.shdr[i] = new shdr_rec(br);
                }
                else if (type == "LIST") // failsafe
                {
                    br.BaseStream.Position -= 8; // skip back
                    return true;
                }
            }
            return true;
        }
    }
    public static class Extensions
    {
        public static char[] ReadCharsSafe(this BinaryReader thisBr, int maxCharCount, bool allow_LfCr = false)
        {
            try
            {
                byte[] bytes = thisBr.ReadBytes(maxCharCount);
                char[] chars = new char[bytes.Length];
                int indexOf = -1;
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == 0)
                    {
                        indexOf = i;
                        break;
                    }
                    else if ((bytes[i] <= 126 && bytes[i] >= 32) || allow_LfCr && (bytes[i] == '\n' || bytes[i] == '\r')) // sanitize toxic chars
                        chars[i] = (char)bytes[i];
                    else
                        chars[i] = ' ';
                }

                //int indexOf = chars.IndexOf('\0');
                if (indexOf != -1)
                {
                    char[] ret = new char[indexOf];
                    for (int i = 0; i < indexOf; i++)
                        ret[i] = chars[i];
                    return ret;
                }
                else
                    return chars;
            }
            catch (Exception ex) { Debug.rtxt.AppendLine("ReadCharsSafe error @ position:" + thisBr.BaseStream.Position + thisBr.ReadStringSafe(20)); return "err:".ToCharArray(); }

        }

        public static string ReadStringSafe(this BinaryReader thisBr, int maxCharCount, bool allow_LfCr = false)
        {
            return new string(thisBr.ReadCharsSafe(maxCharCount, allow_LfCr));
        }

        public static string ReadStringSafeUsingLeadingSize(this BinaryReader thisBr, bool allow_LfCr = true)
        {
            return thisBr.ReadStringSafe((int)Convert.ToUInt32(thisBr.ReadUInt32()), allow_LfCr);
        }
    }
}
