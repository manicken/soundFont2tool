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
        public string debugInfo = "";

        public Soundfont2_reader()
        {
            fileData = new RIFF();
        }

        public bool readFile(string filePath)
        {
            debugInfo = "";
            FileInfo fi = new FileInfo(filePath);
            fileData.size = fi.Length;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
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
                            
                            if (readInfoBlock(br, fs.Position + listSize + 4) == false) return false;

                            return true; // early return debug test
                        }
                        else if (listType == "sdta")
                        {
                            fileData.sfbk.sdta.size = listSize;
                            read_sdta_block(br, fs.Position + listSize + 4);
                        }
                        else if (listType == "pdta")
                        {
                            fileData.sfbk.sdta.size = listSize;
                            read_pdta_block(br, fs.Position + listSize + 4);
                        }
                    }
                    

                }
            }
            return true;
        }

        private bool readInfoBlock(BinaryReader br, long endPos)
        {
            INFO info = fileData.sfbk.info;
            debugInfo += "end pos: " + endPos.ToString() + "\r\n";
            while (br.BaseStream.Position < endPos)
            {
                string type = new string(br.ReadChars(4));
                debugInfo += "type: " + type + "\r\n";
                if (type == "ifil")
                {
                    br.BaseStream.Position += 4; // skip size
                    //info.ifil = br.ReadUInt32();
                    
                    info.ifil.major = br.ReadUInt16();
                    info.ifil.minor = br.ReadUInt16();
                }
                else if (type == "isng")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.isng = br.ReadZSTR(Encoding.UTF8);
                    
                    
                    //info.isng = new string(br.ReadChars(size));
                }
                else if (type == "INAM")
                {
                    //br.BaseStream.Position += 4; // skip size
                    int size = Convert.ToInt32(br.ReadUInt32());
                    debugInfo += "INAM size: " + size.ToString() + "\r\n";
                    //info.INAM = br.ReadZSTR(Encoding.UTF8);
                    info.INAM = new string(br.ReadChars(size));
                }
                else if (type == "irom")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.irom = br.ReadZSTR(Encoding.UTF8);
                    //info.irom = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
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
                    br.BaseStream.Position += 4; // skip size
                    info.ICRD = br.ReadZSTR(Encoding.UTF8);
                    //info.ICRD = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
                }
                else if (type == "IENG")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.IENG = br.ReadZSTR(Encoding.UTF8);
                    //info.IENG = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
                }
                else if (type == "IPRD")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.IPRD = br.ReadZSTR(Encoding.UTF8);
                    //info.IPRD = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
                }
                else if (type == "ICOP")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.ICOP = br.ReadZSTR(Encoding.UTF8);
                    //info.ICOP = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
                }
                else if (type == "ICMT")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.ICMT = br.ReadZSTR(Encoding.UTF8);
                    //info.ICMT = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
                }
                else if (type == "ISFT")
                {
                    br.BaseStream.Position += 4; // skip size
                    info.ISFT = br.ReadZSTR(Encoding.UTF8);
                    //info.ISFT = new string(br.ReadChars(Convert.ToInt32(br.ReadUInt32())));
                }
                else
                    debugInfo += "type not found:" + type + "\r\n";
            }
            return true;
        }
        /// <summary>
        /// reads data offset pointers and sizes of sample data, not the actual data
        /// as the data is read from file on demand
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private bool read_sdta_block(BinaryReader br, long endPos)
        {
            while (br.BaseStream.Position < endPos)
            {
                string type = new string(br.ReadChars(4));
                if (type == "smpl")
                {

                }
                else if (type == "sm24")
                {

                }
            }
            return true;
        }

        private bool read_pdta_block(BinaryReader br, long endPos)
        {
            while (br.BaseStream.Position < endPos)
            {

            }
            return true;
        }
    }
    public static class Extensions
    {
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
    }
}
