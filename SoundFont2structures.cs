using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BYTE = System.Byte;
using CHAR = System.SByte;
using WORD = System.UInt16;
using DWORD = System.UInt32;
using SHORT = System.Int16;

namespace Soundfont2
{
    public class RIFF
    {
        public long size; // is the whole file size
        public sfbk_rec sfbk;
        public RIFF()
        {
            size = 0;
            sfbk = new sfbk_rec();
        }
    }
    public class sfbk_rec
    {
        public UInt32 size; // comes from parent RIFF
        public INFO info;
        public sdta_rec sdta;
        public pdta_rec pdta;
        public sfbk_rec()
        {
            size = 0;
            info = new INFO();
            sdta = new sdta_rec();
            pdta = new pdta_rec();
        }
    }

    /// <summary> </summary>
    public class sfVersionTag
    {
        public UInt16 major;
        public UInt16 minor;
        public sfVersionTag(BinaryReader br)
        {
            br.BaseStream.Position += 4;
            major = br.ReadUInt16();
            minor = br.ReadUInt16();
        }
        public sfVersionTag(UInt32 val)
        {
            major = (UInt16)(val & 0xFFFF);
            minor = (UInt16)((val & 0xFFFF0000) >> 16);
        }
        /// <summary>
        /// Returns the version as major.minor with zero padding at minor
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return major.ToString() + "." + minor.ToString().PadLeft(2, '0');
        }
        public static implicit operator sfVersionTag(UInt32 val)
        {
          return new sfVersionTag(val);
        }
    }
    public class INFO
    {
        public UInt32 size; // comes from the parent LIST
        // mandatory data fields

        /// <summary>The SoundFont specification version</summary>
        public sfVersionTag ifil;
        /// <summary>The sound engine for which the SoundFont was optimized</summary>
        public string isng;
        /// <summary>The name of the SoundFont</summary>
        public string INAM;

        // optional data fields

        /// <summary>A sound data ROM to which any ROM samples refer</summary>
        public string irom;
        /// <summary>A sound data ROM revision to which any ROM samples refer</summary>
        public sfVersionTag iver;
        /// <summary>The creation date of the SoundFont, conventionally in the 'Month Day, Year' format</summary>
        public string ICRD;
        /// <summary>The author or authors of the SoundFont</summary>
        public string IENG;
        /// <summary>The product for which the SoundFont is intended</summary>
        public string IPRD;
        /// <summary>Copyright assertion string associated with the SoundFont</summary>
        public string ICOP;
        /// <summary>Any comments associated with the SoundFont</summary>
        public string ICMT;
        /// <summary>The tool used to create or edit the SoundFont</summary>
        public string ISFT;

        public INFO()
        {
            size = 0;
            isng = "";
            INAM = "";
            irom = "";
            ICRD = "";
            IENG = "";
            IPRD = "";
            ICOP = "";
            ICMT = "";
            ISFT = "";
        }
        public override string ToString()
        {
            string ret = Environment.NewLine;
            ret += "*** Info *** ( size: "+ size.ToString() + " )" + Environment.NewLine;
            if (ifil != null) // failsafe
                ret += "Soundfont version: " + ifil.ToString() + Environment.NewLine;
            ret += "Name: " + INAM + Environment.NewLine;
            ret += "SoundEngine: " + isng + Environment.NewLine;
            ret += "ROM: " + irom + Environment.NewLine;
            if (iver != null)
                ret += "ROM ver: " + iver.ToString() + Environment.NewLine;
            ret += "Date: " + ICRD + Environment.NewLine;
            ret += "Credits: " + IENG + Environment.NewLine;
            ret += "Product: " + IPRD + Environment.NewLine;
            ret += "Copyright: " + ICOP + Environment.NewLine;
            ret += "Comment: " + ICMT.Replace("\n", "\r\n") + Environment.NewLine;
            ret += "Tools: " + ISFT + Environment.NewLine;
            return ret;
        }
    }

    public class smpl_rec
    {
        /// <summary>smpl data offset as from the beginning of the file</summary>
        public long position = 0;
        /// <summary>smpl data size</summary>
        public UInt32 size = 0;

        public smpl_rec(BinaryReader br)
        {
            size = br.ReadUInt32();
            position = br.BaseStream.Position;
        }

        public override string ToString()
        {
            return $"position: {position}, size: {size}";
        }
    }

    /// <summary>sample data offset pointers (data is loaded from file on demand)</summary>
    public class sdta_rec
    {
        public UInt32 size; // comes from parent LIST
        public smpl_rec smpl;
        public smpl_rec sm24;
        

        public sdta_rec()
        {
            size = 0;
        }

        public override string ToString()
        {
            string ret = Environment.NewLine;
            ret += "*** Sample Data *** ( size: " + size.ToString() + " )" + Environment.NewLine;
            if (smpl != null) ret += smpl.ToString() + Environment.NewLine;
            if (sm24 != null) ret += sm24.ToString() + Environment.NewLine;
            return ret;
        }
    }

    /// <summary>preset data which contains the instrument and preset definitions </summary>
    public class pdta_rec
    {
        public UInt32 size; // comes from parent LIST
        /// <summary>The Preset Headers</summary>
        public phdr_rec[] phdr;
        /// <summary>The Preset Index list</summary>
        public bag_rec[] pbag;
        /// <summary>The Preset Modulator list</summary>
        public mod_rec[] pmod;
        /// <summary>The Preset Generator list</summary>
        public gen_rec[] pgen;
        /// <summary>The Instrument Names and Indices</summary>
        public inst_rec[] inst;
        /// <summary>The Instrument Index list</summary>
        public bag_rec[] ibag;
        /// <summary>The Instrument Modulator list</summary>
        public mod_rec[] imod;
        /// <summary>The Instrument Generator list</summary>
        public gen_rec[] igen;
        /// <summary>The Sample Headers</summary>
        public shdr_rec[] shdr;

        public pdta_rec()
        {
            phdr = new phdr_rec[0];
            pbag = new bag_rec[0];
            pmod = new mod_rec[0];
            pgen = new gen_rec[0];
            inst = new inst_rec[0];
            ibag = new bag_rec[0];
            imod = new mod_rec[0];
            igen = new gen_rec[0];
            shdr = new shdr_rec[0];
        }
        public override string ToString()
        {
            string ret = Environment.NewLine;
            ret += "*** Preset Data *** ( size: " + size.ToString() + " )" + Environment.NewLine;
            ret += "phdr count: " + phdr.Length + Environment.NewLine;
            //ret += phdr.GetAllToStrings();
            ret += "pbag count: " + pbag.Length + Environment.NewLine;
            //ret += pbag.GetAllToStrings();
            ret += "pmod count: " + pmod.Length + Environment.NewLine;
            //ret += pmod.GetAllToStrings();
            ret += "pgen count: " + pgen.Length + Environment.NewLine;
            //ret += pgen.GetAllToStrings();
            ret += "inst count: " + inst.Length + Environment.NewLine;
            ret += inst.GetAllToStrings();
            ret += "ibag count: " + ibag.Length + Environment.NewLine;
            //ret += ibag.GetAllToStrings();
            ret += "imod count: " + imod.Length + Environment.NewLine;
            //ret += imod.GetAllToStrings();
            ret += "igen count: " + igen.Length + Environment.NewLine;
            //ret += igen.GetAllToStrings();
            ret += "shdr count: " + shdr.Length + Environment.NewLine;
            //ret += shdr.GetAllToStrings();
            return ret;
        }
    }

    /// <summary>preset data headers size is 38 bytes</summary>
    public class phdr_rec // 38 bytes each item
    {
        public static int Size = 38;
        public string achPresetName; // max lenght of 20
        public WORD wPreset;
        public WORD wBank;
        public WORD wPresetBagNdx;
        public DWORD dwLibrary;
        public DWORD dwGenre;
        public DWORD dwMorphology;

        public phdr_rec(BinaryReader br)
        {
            achPresetName = br.ReadString(20);
            wPreset = br.ReadUInt16();
            wBank = br.ReadUInt16();
            wPresetBagNdx = br.ReadUInt16();
            dwLibrary = br.ReadUInt32();
            dwGenre = br.ReadUInt32();
            dwMorphology = br.ReadUInt32();
        }
        public override string ToString()
        {
            string r = "";
            r += $"{achPresetName.PadRight(20)}, bank: {wBank}, preset: {wPreset}, presetBag: {wPresetBagNdx}, library: {dwLibrary}, genre: {dwGenre}, morphology: {dwMorphology}";
            return r;
        }
    }
    /// <summary>
    /// size is 2+2 = 4 bytes
    /// </summary>
    public class bag_rec
    {
        public static int Size = 4;
        public WORD wGenNdx;
        public WORD wModNdx;
        public string type = "";

        public bag_rec(BinaryReader br, string type)
        {
            wGenNdx = br.ReadUInt16();
            wModNdx = br.ReadUInt16();
            this.type = type;
        }
        public override string ToString()
        {
            string r = "";
            r += $"{type}Gen: {wGenNdx}, {type}Mod: {wModNdx}";
            return r;
        }
    }
    /// <summary> size is 2+2+2+2+1 = 9 bytes + 1 padding</summary>
    public class mod_rec
    {
        public static int Size = 10;
        public SFModulator sfModSrcOper;
        public SFGenerator sfModDestOper;
        public SHORT modAmount;
        public SFModulator sfModAmtSrcOper;
        public SFTransform sfModTransOper;

        public mod_rec(BinaryReader br)
        {
            sfModSrcOper = new SFModulator(br);
            sfModDestOper = (SFGenerator)br.ReadUInt16(); // SFGenerator.startAddrsOffset;
            modAmount = br.ReadInt16();
            sfModAmtSrcOper = new SFModulator(br);
            sfModTransOper = (SFTransform)br.ReadByte();// .absoluteValue;
            br.ReadByte(); // padding dummy read
        }
        public override string ToString()
        {
            string r = "";
            r += $"Mod Src Oper: {sfModSrcOper}, Mod Dest Oper: {sfModDestOper}, mod amount: {modAmount}, Mod Amt Src Oper: {sfModAmtSrcOper}, Mod Trans Oper: {sfModTransOper}";
            return r;
        }
    }
    /// <summary>
    /// size is 2+2 = 4 bytes
    /// </summary>
    public class gen_rec
    {
        public static int Size = 4;
        public SFGenerator sfGenOper;
        public SF2GeneratorAmount genAmount;
        
        public gen_rec(BinaryReader br)
        {
            sfGenOper = (SFGenerator)br.ReadUInt16();
            genAmount = new SF2GeneratorAmount(br.ReadUInt16());
        }
        public override string ToString()
        {
            string r = "";
            r += $"{((ushort)sfGenOper).ToString().PadLeft(2)} {sfGenOper.ToString().PadRight(26)} = ({genAmount.UAmount.ToString().PadLeft(5)})  {genAmount.ToString(sfGenOper)}";
            return r;
        }
    }
    /// <summary>
    /// size is 20+2 = 22 bytes
    /// </summary>
    public class inst_rec
    {
        public static int Size = 22;
        public string achInstName;
        public WORD wInstBagNdx;

        public inst_rec(BinaryReader br)
        {
            achInstName = br.ReadString(20);
            wInstBagNdx = br.ReadUInt16();
        }
        public override string ToString()
        {
            string r = "";
            r += $"{achInstName.PadRight(20)}, ibag: {wInstBagNdx}";
            return r;
        }
    }

    /// <summary>
    /// size is 20+4*5+1+1+2+2 = 46
    /// </summary>
    public class shdr_rec
    {
        public static int Size = 46;
        public string achSampleName;
        public DWORD dwStart;
        public DWORD dwEnd;
        public DWORD dwStartloop;
        public DWORD dwEndloop;
        public DWORD dwSampleRate;
        public BYTE byOriginalKey;
        public CHAR chCorrection;
        public WORD wSampleLink;
        public SFSampleLink sfSampleType;

        public shdr_rec(BinaryReader br)
        {
            achSampleName = br.ReadString(20);
            dwStart = br.ReadUInt32();
            dwEnd = br.ReadUInt32(); ;
            dwStartloop = br.ReadUInt32();
            dwEndloop = br.ReadUInt32();
            dwSampleRate = br.ReadUInt32();
            byOriginalKey = br.ReadByte();
            chCorrection = br.ReadSByte();
            wSampleLink = br.ReadUInt16();
            sfSampleType = (SFSampleLink)br.ReadUInt16();
        }

        public override string ToString()
        {
            string r = "";
            /*System.Reflection.FieldInfo[] fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Debug.rtxt.AppendLine(fields.Length.ToString());
            foreach (System.Reflection.FieldInfo field in fields)
                r += $"{field.Name} = {field.GetValue(this)}, ";*/
            r += $"{achSampleName.PadRight(20)}, Rate: {dwSampleRate}";
            r += $", Start: {dwStart}, End: {dwEnd}, StartLoop: {dwStartloop}, EndLoop: {dwEndloop}";
            r += $", RootKey: {byOriginalKey}, Corr: {chCorrection}";
            r += $", Link: {wSampleLink}, Type: {sfSampleType}";
            
            return r;
        }
    }

    /// <summary>
    /// size is 2 bytes
    /// </summary>
    public class SFModulator // this is a little overcomplicated as C# cannot handle bitfields
    {
        public UInt16 rawdata = 0;
        /// <summary>A 6 bit value specifying the continuity of the controller</summary>
        public byte Type
        {
            get { return (byte)((rawdata >> 10) & 0x1F); }
            set { rawdata = (UInt16)((rawdata & 0x03FF) | ((value & 0x1F) << 10)); }
        }
        /// <summary>Polarity</summary>
        public byte P
        {
            get { return (byte)((rawdata >> 9) & 0x01); }
            set { rawdata = (UInt16)((rawdata & 0xFDFF) | ((value & 0x01) << 9)); }
        }
        /// <summary>Direction</summary>
        public byte D
        {
            get { return (byte)((rawdata >> 8) & 0x01); }
            set { rawdata = (ushort)((rawdata & 0xFEFF) | ((value & 0x01) << 8)); }
        }
        /// <summary>MIDI Continuous Controller Flag</summary>
        public byte CC
        {
            get { return (byte)((rawdata >> 7) & 0x01); }
            set { rawdata = (ushort)((rawdata & 0xFF7F) | ((value & 0x01) << 7)); }
        }
        /// <summary>A 7 bit value specifying the controller source</summary>
        public byte Index
        {
            get { return (byte)(rawdata & 0x7F); }
            set { rawdata = (ushort)((rawdata & 0xFF80) | (value & 0x7F)); }
        }

        public SFModulator(BinaryReader br)
        {
            rawdata = br.ReadUInt16();
        }

        public override string ToString()
        {
            return $"Type = {Type}, P = {P}, D = {D}, CC = {CC}, Index = {Index}";
        }
    }

    /// <summary>SF2 spec v2.1 page 19 - Two bytes that can handle either two 8-bit values or a single 16-bit value</summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public class SF2GeneratorAmount
    {
        [System.Runtime.InteropServices.FieldOffset(0)] public byte LowByte;
        [System.Runtime.InteropServices.FieldOffset(1)] public byte HighByte;
        [System.Runtime.InteropServices.FieldOffset(0)] public short Amount;
        [System.Runtime.InteropServices.FieldOffset(0)] public ushort UAmount;

        public override string ToString()
        {
            return $"BL:{LowByte.ToString().PadLeft(3)}, BH:{HighByte.ToString().PadLeft(3)}, Sh:{Amount.ToString().PadLeft(6)}, U:{UAmount.ToString().PadLeft(5)}";
        }

        public SF2GeneratorAmount(BinaryReader br)
        {
            Amount = br.ReadInt16();
        }

        public SF2GeneratorAmount(ushort value)
        {
            UAmount = value;
        }
        /// <summary>Math.Pow(2, (double)Amount / (double)1200)</summary>
        public double cents
        {
            get { return Math.Pow(2, (double)Amount / (double)1200); }
        }
        /// <summary>(double)Amount / (double)100</summary>
        public double bells
        {
            get { return (double)Amount / (double)100; }
        }

        /// <summary>(double)Amount / (double)10</summary>
        public double centibels
        {
            get { return (double)Amount / (double)10; }
        }

        /// <summary>32768 * Amount</summary>
        public int coarse_offset
        {
            get { return 32768 * Amount; }
        }
        /// <summary>8.176f * Math.Pow(2, Amount / 1200)</summary>
        public double absolute_cents
        {
            get { return 8.176f * Math.Pow(2, (double)Amount / (double)1200); }
        }

        public SampleMode sample_mode
        {
            get { return ((SampleMode)UAmount); }
        }

        public byte[] sorted_range
        {
            get
            {
                if (LowByte < HighByte) return new byte[] { LowByte, HighByte };
                else return new byte[] { HighByte, LowByte };
            }
        }
        public string ToString(SFGenerator type)
        {
            if (type == SFGenerator.startAddrsOffset)
                return Amount.ToString();
            else if (type == SFGenerator.endAddrsOffset)
                return Amount.ToString();
            else if (type == SFGenerator.startloopAddrsOffset)
                return Amount.ToString();
            else if (type == SFGenerator.endloopAddrsOffset)
                return Amount.ToString();
            else if (type == SFGenerator.startAddrsCoarseOffset)
                return Amount.ToString();
            else if (type == SFGenerator.modLfoToPitch)
                return Amount.ToString();
            else if (type == SFGenerator.vibLfoToPitch)
                return Amount.ToString();
            else if (type == SFGenerator.modEnvToPitch)
                return Amount.ToString();
            else if (type == SFGenerator.initialFilterFc)
                return Amount.ToString();
            else if (type == SFGenerator.initialFilterQ)
                return Amount.ToString();
            else if (type == SFGenerator.modLfoToFilterFc)
                return Amount.ToString();
            else if (type == SFGenerator.modEnvToFilterFc)
                return Amount.ToString();
            else if (type == SFGenerator.endAddrsCoarseOffset)
                return Amount.ToString();
            else if (type == SFGenerator.modLfoToVolume)
                return centibels.ToString().Replace(',','.');
            else if (type == SFGenerator.chorusEffectsSend)
                return centibels.ToString().Replace(',', '.');
            else if (type == SFGenerator.reverbEffectsSend)
                return centibels.ToString().Replace(',', '.');
            else if (type == SFGenerator.pan)
                return centibels.ToString().Replace(',', '.');
            else if (type == SFGenerator.delayModLFO)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.freqModLFO)
                return absolute_cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.delayVibLFO)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.freqVibLFO)
                return absolute_cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.delayModEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.attackModEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.holdModEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.decayModEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.sustainModEnv)
                return centibels.ToString().Replace(',', '.');
            else if (type == SFGenerator.releaseModEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.keynumToModEnvHold)
                return Amount.ToString();
            else if (type == SFGenerator.keynumToModEnvDecay)
                return Amount.ToString();
            else if (type == SFGenerator.delayVolEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.attackVolEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.holdVolEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.decayVolEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.sustainVolEnv)
                return centibels.ToString().Replace(',', '.');
            else if (type == SFGenerator.releaseVolEnv)
                return cents.ToString().Replace(',', '.');
            else if (type == SFGenerator.keynumToVolEnvHold)
                return Amount.ToString();
            else if (type == SFGenerator.keynumToVolEnvDecay)
                return Amount.ToString();
            else if (type == SFGenerator.instrument)
                return Amount.ToString();
            else if (type == SFGenerator.keyRange)
                return sorted_range.ToString(false);// $"L:{LowByte}, H:{HighByte}";
            else if (type == SFGenerator.velRange)
                return sorted_range.ToString(false);//$"H:{HighByte}, L:{LowByte}";
            else if (type == SFGenerator.startloopAddrsCoarseOffset)
                return Amount.ToString();
            else if (type == SFGenerator.keynum)
                return UAmount.ToString();
            else if (type == SFGenerator.velocity)
                return UAmount.ToString();
            else if (type == SFGenerator.initialAttenuation)
                return centibels.ToString().Replace(',', '.');
            else if (type == SFGenerator.endloopAddrsCoarseOffset)
                return Amount.ToString();
            else if (type == SFGenerator.coarseTune)
                return Amount.ToString();
            else if (type == SFGenerator.fineTune)
                return Amount.ToString();
            else if (type == SFGenerator.sampleID)
                return UAmount.ToString();
            else if (type == SFGenerator.sampleModes)
                return sample_mode.ToString();
            else if (type == SFGenerator.scaleTuning)
                return UAmount.ToString();
            else if (type == SFGenerator.exclusiveClass)
                return UAmount.ToString();
            else if (type == SFGenerator.overridingRootKey)
                return UAmount.ToString();
            else if (type == SFGenerator.endOper)
                return Amount.ToString();

            return this.ToString();
        }
    }

    
}
