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

        public string lastError = "";

        public Soundfont2_reader()
        {
            fileData = new RIFF();
        }

        public bool readFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            fileData.size = fi.Length;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.UTF8))
                {
                    string riffTag = new String(br.ReadChars(4));
                    if (riffTag != "RIFF")
                    {
                        lastError = "this is not a RIFF fileformat";
                        return false;
                    }
                    fileData.sfbk.size = br.ReadUInt32();
                    string sfbkTag = new string(br.ReadChars(4));
                    if (sfbkTag != "sfbk")
                    {
                        lastError = "this is not sfbk fileformat";
                        return false;
                    }
                    string listType = "sfbk";
                    while (br.PeekChar() != -1)
                    {
                        // every block starts with a LIST tag
                        string listTag = new string(br.ReadChars(4));
                        if (listTag != "LIST")
                        {
                            lastError = "LIST item not found after " + listType;
                            return false;
                        }
                        UInt32 listSize = br.ReadUInt32();
                        listType = new string(br.ReadChars(4));
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
            return true;
        }

        private bool readInfoBlock(BinaryReader br/*, long endPos*/)
        {
            INFO info = fileData.sfbk.info; // simplify the usage

            while (br.PeekChar() != -1)//br.BaseStream.Position < endPos)
            {
                string type = new string(br.ReadChars(4));
                //Debug.rtxt.AppendLine("type: " + type);
                if (type == "ifil")
                {
                    br.BaseStream.Position += 4; // skip size
                    
                    //info.ifil = br.ReadUInt32();
                    info.ifil.major = br.ReadUInt16();
                    info.ifil.minor = br.ReadUInt16();
                }
                else if (type == "isng")
                {
                    //Debug.rtxt.AppendLine("isng:");
                    info.isng = br.ReadStringUsingLeadingSize();
                    //info.isng = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "INAM")
                {
                    //Debug.rtxt.AppendLine("INAM:");
                    info.INAM = br.ReadStringUsingLeadingSize();
                    //info.INAM = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "irom")
                {
                    //Debug.rtxt.AppendLine("irom:");
                    info.irom = br.ReadStringUsingLeadingSize();
                    //info.irom = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "iver")
                {
                    br.BaseStream.Position += 4; // skip size
                    
                    //info.iver = br.ReadUInt32(); // read both minor and major
                    info.iver.major = br.ReadUInt16();
                    info.iver.minor = br.ReadUInt16();
                }
                else if (type == "ICRD")
                {
                    //Debug.rtxt.AppendLine("ICRD:");
                    info.ICRD = br.ReadStringUsingLeadingSize();
                    //info.ICRD = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "IENG")
                {
                    //Debug.rtxt.AppendLine("IENG:");
                    info.IENG = br.ReadStringUsingLeadingSize();
                    //info.IENG = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "IPRD")
                {
                    //Debug.rtxt.AppendLine("IPRD:");
                    info.IPRD = br.ReadStringUsingLeadingSize();
                    //info.IPRD = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "ICOP")
                {
                    //Debug.rtxt.AppendLine("ICOP:");
                    info.ICOP = br.ReadStringUsingLeadingSize();
                    //info.ICOP = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "ICMT")
                {
                    //Debug.rtxt.AppendLine("ICMT:");
                    info.ICMT = br.ReadStringUsingLeadingSize();
                    //info.ICMT = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "ISFT")
                {
                    //Debug.rtxt.AppendLine("ISFT:");
                    info.ISFT = br.ReadStringUsingLeadingSize();
                    //info.ISFT = br.ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes();
                }
                else if (type == "LIST")
                {
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
        private bool read_sdta_block(BinaryReader br/*, long endPos*/)
        {
            sdta_rec sdta = fileData.sfbk.sdta; // simplify the usage
            while (br.PeekChar() != -1)
            {
                string type = new string(br.ReadChars(4));
                //Debug.rtxt.AppendLine("sdta: " + type);
                if (type == "smpl")
                {
                    sdta.smplSize = br.ReadUInt32();
                    sdta.smpl = br.BaseStream.Position;
                    br.BaseStream.Position += sdta.smplSize; // skip sample data
                }
                else if (type == "sm24")
                {
                    sdta.sm24Size = br.ReadUInt32();
                    sdta.sm24 = br.BaseStream.Position;
                    br.BaseStream.Position += sdta.sm24Size; // skip sample data
                }
                else if (type == "LIST")
                {
                    br.BaseStream.Position -= 4; // skip back
                    return true;
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
                try { type = new string(br.ReadChars(4)); }
                catch (Exception ex) { return false; }
                UInt32 size = br.ReadUInt32();
                //Debug.rtxt.AppendLine("pdta: " + type + ", size:" + size.ToString());
                if (type == "phdr")
                {
                    pdta.phdr = new phdr_rec[size/phdr_rec.Size];
                    for (int i=0;i<pdta.phdr.Length;i++)
                    {
                        pdta.phdr[i] = new phdr_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "pbag")
                {
                    pdta.pbag = new bag_rec[size/bag_rec.Size];
                    for (int i = 0; i < pdta.pbag.Length; i++)
                    {
                        pdta.pbag[i] = new bag_rec(br,"p");
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "pmod")
                {
                    pdta.pmod = new mod_rec[size/mod_rec.Size];
                    for (int i = 0; i < pdta.pmod.Length; i++)
                    {
                        pdta.pmod[i] = new mod_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "pgen")
                {
                    pdta.pgen = new gen_rec[size/gen_rec.Size];
                    for (int i = 0; i < pdta.pgen.Length; i++)
                    {
                        pdta.pgen[i] = new gen_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "inst")
                {
                    pdta.inst = new inst_rec[size/inst_rec.Size];
                    for (int i = 0; i < pdta.inst.Length; i++)
                    {
                        pdta.inst[i] = new inst_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "ibag")
                {
                    pdta.ibag = new bag_rec[size/bag_rec.Size];
                    for (int i = 0; i < pdta.ibag.Length; i++)
                    {
                        pdta.ibag[i] = new bag_rec(br,"i");
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "imod")
                {
                    pdta.imod = new mod_rec[size/mod_rec.Size];
                    for (int i = 0; i < pdta.imod.Length; i++)
                    {
                        pdta.imod[i] = new mod_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "igen")
                {
                    pdta.igen = new gen_rec[size/gen_rec.Size];
                    for (int i = 0; i < pdta.igen.Length; i++)
                    {
                        pdta.igen[i] = new gen_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "shdr")
                {
                    pdta.shdr = new shdr_rec[size/shdr_rec.Size];
                    //Debug.rtxt.AppendLine($"shdr count: { pdta.shdr.Length}");
                    for (int i = 0; i < pdta.shdr.Length; i++)
                    {
                        pdta.shdr[i] = new shdr_rec(br);
                    }
                    //br.BaseStream.Position += size; // in development skip
                }
                else if (type == "LIST") // failsafe
                {
                    br.BaseStream.Position -= 8; // skip back
                    return true;
                }
                /*try
                {
                    int testNextRead = br.BaseStream.ReadByte();
                    br.BaseStream.Position -= 1;
                    if (testNextRead == -1) break;
                }
                catch (Exception ex) { Debug.rtxt.AppendLine(ex.ToString() + $"br.Lenght: {br.BaseStream.Length}, br.Position: {br.BaseStream.Position}"); return true; }*/
            }
            return true;
        }
    }
    public static class Extensions
    {
        public static int IndexOf(this char[] thisCharArray, char val)
        {
            for (int i = 0; i < thisCharArray.Length; i++)
                if (thisCharArray[i] == val) return i;
            return -1;
        }
        public static string ReadStringUsingLeadingSize(this BinaryReader thisBr)
        {
            int size = Convert.ToInt32(thisBr.ReadUInt32());
            char[] charArray = thisBr.ReadChars(size);
            int indexOfZero = charArray.IndexOf('\0');
            string str;
            if (indexOfZero != -1)
                str = new string(charArray, 0, indexOfZero);
            else
                str = new string(charArray);
            return str;
        }

        public static string ReadZSTR_SkippingLeadingSizeAndAdditionalZeroes(this BinaryReader thisBr)
        {
            thisBr.BaseStream.Position += 4;
            string ret = thisBr.ReadZSTR(Encoding.UTF8);
            thisBr.SkipAdditionalZeroes();
            return ret;
        }
        public static string ReadZSTR(this BinaryReader thisBr, Encoding encoding)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte b;
                while((b = thisBr.ReadByte()) != 0)
                {
                    ms.WriteByte(b);
                }
                return encoding.GetString(ms.ToArray());
            }
        }
        public static void SkipAdditionalZeroes(this BinaryReader thisBr)
        {
            while (thisBr.PeekChar() == 0)
            {
                thisBr.ReadChar();
            }
        }
    }
}
