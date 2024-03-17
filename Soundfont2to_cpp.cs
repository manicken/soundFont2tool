using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Soundfont2
{
    public class Soundfont2to_cpp
    {
        public class CodeFile
        {
            public string data = "";
            public string fileName = "";

            public CodeFile(string fileName)
            {
                this.fileName = fileName;
            }
            public override string ToString()
            {
                return fileName + Environment.NewLine + data;
            }
        }
        public class CodeFiles
        {
            public CodeFile cpp;
            public CodeFile hpp;

            public CodeFiles(string name)
            {
                cpp = new CodeFile(name + "_samples.cpp");
                hpp = new CodeFile(name + "_samples.h");
            }
            public override string ToString()
            {
                return hpp.ToString() + Environment.NewLine + cpp.ToString();
            }
        }
        public static CodeFiles getcpp(Soundfont2_reader sfFile, int instrumentIndex)
        {
            sfbk_rec sfbk = sfFile.fileData.sfbk;
            string INSTRUMENT_NAME = SanitizeForCppVariableName(sfbk.pdta.inst[instrumentIndex].achInstName);

            CodeFiles files = new CodeFiles(INSTRUMENT_NAME);

            int startIbagIndex = sfbk.pdta.inst[instrumentIndex].wInstBagNdx;
            int endIbagIndex = sfbk.pdta.inst[instrumentIndex+1].wInstBagNdx;

            FileStream fs = new FileStream(sfFile.FilePath, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs, Encoding.UTF8);
                

            SF2GeneratorAmount get_gen_parameter_value(int ibagIndex, SFGenerator genType)
            {
                int start = sfbk.pdta.ibag[ibagIndex].wGenNdx;
                int end = sfbk.pdta.ibag[ibagIndex + 1].wGenNdx;
                //Debug.rtxt.AppendLine("try to find @ sample:" + genType.ToString());
                for (int i=start;i<end;i++)
                {
                    if (sfbk.pdta.igen[i].sfGenOper == genType) return sfbk.pdta.igen[i].genAmount;
                }
                //Debug.rtxt.AppendLine("try to find @ global:" + genType.ToString());
                // try again with global bag
                start = sfbk.pdta.ibag[startIbagIndex].wGenNdx;
                end = sfbk.pdta.ibag[startIbagIndex + 1].wGenNdx;
                for (int i = start; i < end; i++)
                {
                    if (sfbk.pdta.igen[i].sfGenOper == genType) return sfbk.pdta.igen[i].genAmount;
                }
                return null;
            }
            shdr_rec getSampleDef(int SAMPLE_NUM)
            {
                SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.sampleID);
                if (val == null) return null;

                return sfbk.pdta.shdr[val.Amount];
            }
            string SanitizeForCppVariableName(string input)
            {
                input = input.Trim(' ').Replace(' ', '_');
                // Remove other characters not allowed in C++ variable names
                string sanitized = System.Text.RegularExpressions.Regex.Replace(input, @"[^a-zA-Z0-9_]", "");

                // Ensure the string doesn't start with a digit
                if (char.IsDigit(sanitized[0]))
                {
                    sanitized = "_" + sanitized;
                }
                return sanitized;
            }
            string get_sample_array_name(int SAMPLE_NUM, string SAMPLE_NAME)
            {
                return $"sample_{SAMPLE_NUM}_{INSTRUMENT_NAME}_{SAMPLE_NAME}";
            }
            bool get_sample_repeat(bool defaultValue, int SAMPLE_NUM)
            {
                SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.sampleModes);
                if (val == null) return defaultValue;
                return (val.sample_mode == SampleMode.kLoopContinuously);// || (val.sample_mode == SampleMode.kLoopEndsByKeyDepression);
            }
            int get_cooked_loop_end(shdr_rec shdr, int SAMPLE_NUM)
            {
                int result = (int)(shdr.dwEndloop - shdr.dwStart);
                SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.endloopAddrsOffset);
                if (val != null) result += val.Amount;
                val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.endloopAddrsCoarseOffset);
                if (val != null) result += val.Amount * 32768;
                return result;
            }
            int get_length(shdr_rec shdr, int SAMPLE_NUM)
            {
                int length = (int)(shdr.dwEnd - shdr.dwStart);
                int cooked_loop_end = get_cooked_loop_end(shdr, SAMPLE_NUM);
                if (get_sample_repeat(false, SAMPLE_NUM) && cooked_loop_end < length)
                {
                    Debug.rtxt.AppendLine("cooked_loop_end < length:" + cooked_loop_end.ToString() + " < " + length.ToString());
                    length = cooked_loop_end + 1;

                }
                return length;
            }

            string getSampleHeader(int SAMPLE_NUM)
            {
                shdr_rec shdr = getSampleDef(SAMPLE_NUM);
                if (shdr == null) { return $"// invalid sample @ num {SAMPLE_NUM}\n"; }

                string get_decibel_value_as_string(SFGenerator genType, double DEFAULT, double MIN, double MAX, string outFormat, double outMult=1)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    double val = (genval != null) ? genval.centibels : DEFAULT;
                    val = (val > MAX) ? MAX : ((val < MIN) ? MIN : val);
                    return (val*outMult).ToString(outFormat).Replace(',', '.');
                }
                string get_timecents_value_as_string(SFGenerator genType, double DEFAULT, double MIN, string outFormat)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    double val = (genval != null) ? genval.cents * 1000.0f : DEFAULT;
                    val = (val > MIN) ? val : MIN;
                    return val.ToString(outFormat).Replace(',', '.');
                }
                string get_hertz_as_string(SFGenerator genType, double DEFAULT, double MIN, double MAX, string outFormat)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    if (genval != null) Debug.rtxt.AppendLine(genval.ToString());
                    double val = (genval != null) ? genval.absolute_cents : DEFAULT;
                    val = (val > MAX) ? MAX : ((val < MIN) ? MIN : val);
                    return val.ToString(outFormat).Replace(',', '.');
                }
                int get_pitch_cents(SFGenerator genType, double DEFAULT, double MIN, double MAX)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    double val = (genval != null) ? genval.Amount : DEFAULT;
                    return Convert.ToInt32((val > MAX) ? MAX : ((val < MIN) ? MIN : val));
                }
                
                int get_cooked_loop_start()
                {
                    int result = (int)(shdr.dwStartloop - shdr.dwStart);
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.startloopAddrsOffset);
                    if (val != null) result += val.Amount;
                    val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.startloopAddrsCoarseOffset);
                    if (val != null) result += val.Amount*32768;
                    return result;
                }
                
                int get_sample_note()
                {
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.overridingRootKey);
                    if (val != null) return (int)val.UAmount;
                    return (shdr.byOriginalKey <= 127)?shdr.byOriginalKey:60;
                }
                int get_fine_tuning()
                {
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.fineTune);
                    if (val != null) return (int)val.Amount;
                    return 0;
                }
                
                int get_length_bits(int len)
                {
                    int length_bits = 0;
                    while (len != 0)
                    {
                        length_bits += 1;
                        len = len >> 1;
                    }
                    return length_bits;
                }
                string SAMPLE_NAME = SanitizeForCppVariableName(shdr.achSampleName);
                
                string SAMPLE_ARRAY_NAME = get_sample_array_name(SAMPLE_NUM, SAMPLE_NAME);
                
                string LOOP = get_sample_repeat(false, SAMPLE_NUM)?"true":"false"; // c++ boolean
                int SAMPLE_NOTE = get_sample_note();
                int CENTS_OFFSET = get_fine_tuning();
                int LENGTH = get_length(shdr, SAMPLE_NUM);
                int LENGTH_BITS = get_length_bits(LENGTH);
                string SAMPLE_RATE = shdr.dwSampleRate.ToString("F1").Replace(',', '.');
                int LOOP_START = get_cooked_loop_start();
                int LOOP_END = get_cooked_loop_end(shdr, SAMPLE_NUM);

                string DELAY_ENV = get_timecents_value_as_string(SFGenerator.delayVolEnv, 0, 0, "F2");
                string ATTACK_ENV = get_timecents_value_as_string(SFGenerator.attackVolEnv, 1, 1, "F2");
                string HOLD_ENV = get_timecents_value_as_string(SFGenerator.holdVolEnv, 0, 0, "F2");
                string DECAY_ENV = get_timecents_value_as_string(SFGenerator.decayVolEnv, 1, 1, "F2");
                string SUSTAIN_FRAC = get_decibel_value_as_string(SFGenerator.sustainVolEnv, 0, 0, 144, "F1", -1);
                string RELEASE_ENV = get_timecents_value_as_string(SFGenerator.releaseVolEnv, 1, 1, "F2");
                string VIB_DELAY_ENV = get_timecents_value_as_string(SFGenerator.delayVibLFO, 0, 0, "F2");
                string VIB_INC_ENV = get_hertz_as_string(SFGenerator.freqVibLFO, 8.176, 0.1, 100, "F1");
                string MOD_DELAY_ENV = get_timecents_value_as_string(SFGenerator.delayModLFO, 0, 0, "F2");
                string MOD_INC_ENV = get_hertz_as_string(SFGenerator.freqModLFO, 8.176, 0.1, 100, "F1");
                int VIB_PITCH_INIT = get_pitch_cents(SFGenerator.vibLfoToPitch, 0, -12000, 12000);
                int VIB_PITCH_SCND = get_pitch_cents(SFGenerator.vibLfoToPitch, 0, -12000, 12000)*-1;
                int MOD_PITCH_INIT = get_pitch_cents(SFGenerator.modLfoToPitch, 0, -12000, 12000);
                int MOD_PITCH_SCND = get_pitch_cents(SFGenerator.modLfoToPitch, 0, -12000, 12000)*-1;
                string INIT_ATTENUATION = get_decibel_value_as_string(SFGenerator.initialAttenuation, 0, 0, 144, "F2", -1);
                string MOD_AMP_INIT_GAIN = get_decibel_value_as_string(SFGenerator.modLfoToVolume, 0, -96, 96, "F0");
                string MOD_AMP_SCND_GAIN = get_decibel_value_as_string(SFGenerator.modLfoToVolume, 0, -96, 96, "F0", -1);

                List<string> lines = new List<string>();
                lines.Add("// SAMPLE VALUES");
                lines.Add($"/*sample pointer*/             (int16_t*){SAMPLE_ARRAY_NAME},");
                lines.Add($"/*loop*/                       {LOOP},");
                lines.Add($"/*LENGTH_BITS (INDEX_BITS)*/   {LENGTH_BITS},");
                lines.Add($"/*PER_HERTZ_PHASE_INCREMENT*/  (1 << (32 - {LENGTH_BITS})) * WAVETABLE_CENTS_SHIFT({CENTS_OFFSET}) * {SAMPLE_RATE} / WAVETABLE_NOTE_TO_FREQUENCY({SAMPLE_NOTE}) / AUDIO_SAMPLE_RATE_EXACT + 0.5,");
                lines.Add($"/*MAX_PHASE*/                  ((uint32_t){LENGTH} - 1) << (32 - {LENGTH_BITS}),");
                lines.Add($"/*LOOP_PHASE_END*/             ((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS}),");
                lines.Add($"/*LOOP_PHASE_LENGTH*/          (((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS})) - (((uint32_t){LOOP_START} - 1) << (32 - {LENGTH_BITS})),");
                lines.Add($"/*INITIAL_ATTENUATION_SCALAR*/ uint16_t(UINT16_MAX * WAVETABLE_DECIBEL_SHIFT({INIT_ATTENUATION})),");
                lines.Add("");
                lines.Add("// VOLUME ENVELOPE VALUES");
                lines.Add($"/*DELAY_COUNT*/   uint32_t({DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),");
                lines.Add($"/*ATTACK_COUNT*/  uint32_t({ATTACK_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),");
                lines.Add($"/*HOLD_COUNT*/    uint32_t({HOLD_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),");
                lines.Add($"/*DECAY_COUNT*/   uint32_t({DECAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),");
                lines.Add($"/*RELEASE_COUNT*/ uint32_t({RELEASE_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),");
                lines.Add($"/*SUSTAIN_MULT*/  int32_t((1.0 - WAVETABLE_DECIBEL_SHIFT({SUSTAIN_FRAC})) * AudioSynthWavetable::UNITY_GAIN),");
                lines.Add("");
                lines.Add("// VIRBRATO VALUES");
                lines.Add($"/*VIBRATO_DELAY*/                      uint32_t({VIB_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)),");
                lines.Add($"/*VIBRATO_INCREMENT*/                  uint32_t({VIB_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)),");
                lines.Add($"/*VIBRATO_PITCH_COEFFICIENT_INITIAL*/  (WAVETABLE_CENTS_SHIFT({VIB_PITCH_INIT}) - 1.0) * 4,");
                lines.Add($"/*VIBRATO_PITCH_COEFFICIENT_SECOND*/   (1.0 - WAVETABLE_CENTS_SHIFT({VIB_PITCH_SCND})) * 4,");
                lines.Add("");
                lines.Add("// MODULATION VALUES");
                lines.Add($"/*MODULATION_DELAY*/                     uint32_t({MOD_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)),");
                lines.Add($"/*MODULATION_INCREMENT*/                 uint32_t({MOD_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)),");
                lines.Add($"/*MODULATION_PITCH_COEFFICIENT_INITIAL*/ (WAVETABLE_CENTS_SHIFT({MOD_PITCH_INIT}) - 1.0) * 4,");
                lines.Add($"/*MODULATION_PITCH_COEFFICIENT_SECOND*/  (1.0 - WAVETABLE_CENTS_SHIFT({MOD_PITCH_SCND})) * 4,");
                lines.Add($"/*MODULATION_AMPLITUDE_INITIAL_GAIN*/    int32_t(UINT16_MAX * (WAVETABLE_DECIBEL_SHIFT({MOD_AMP_INIT_GAIN}) - 1.0)) * 4,");
                lines.Add($"/*MODULATION_AMPLITUDE_SECOND_GAIN*/     int32_t(UINT16_MAX * (1.0 - WAVETABLE_DECIBEL_SHIFT({MOD_AMP_SCND_GAIN}))) * 4,");
                
                
                for (int i = 0; i < lines.Count; i++)
                    lines[i] = "\t\t" + lines[i];
                return string.Join("\n", lines);
            }
            string getSampleData(int SAMPLE_NUM)
            {
                shdr_rec shdr = getSampleDef(SAMPLE_NUM);
                if (shdr == null) { return $"// invalid sample @ num {SAMPLE_NUM}\n"; }
                string sampleName = SanitizeForCppVariableName(shdr.achSampleName);
                string SAMPLE_ARRAY_NAME = get_sample_array_name(SAMPLE_NUM, sampleName);
                int length_16 = get_length(shdr, SAMPLE_NUM);
                int length_8 = length_16 * 2;
                int length_32 = (int)Math.Ceiling((double)length_16 / 2);
                int pad_length = (length_32 % 128 == 0) ? 0 : (128 - length_32 % 128);
                int ary_length = length_32 + pad_length;
                Debug.rtxt.AppendLine($"name={sampleName}, length_16_orginal={shdr.dwEnd-shdr.dwStart}, length_16_cook={length_16}, pad_length={pad_length}, ary_length={ary_length}");
                string ret = $"static const PROGMEM uint32_t {SAMPLE_ARRAY_NAME}[{ary_length}] = {{\n";
                int line_width = 0;
                fs.Seek(sfbk.sdta.smpl.position + shdr.dwStart*2, SeekOrigin.Begin);
                for (int i=0; i< length_32; i++)
                {
                    UInt32 data = br.ReadUInt32();
                    string strData = data.ToString("X4").PadLeft(8, '0');
                    ret += $"0x{strData},";
                    line_width++;
                    if (line_width == 8)
                    {
                        line_width = 0;
                        ret += "\n";
                    }
                }
                while (pad_length > 0)
                {
                    ret += "0x00000000,";
                    line_width++;
                    if (line_width == 8)
                    {
                        line_width = 0;
                        ret += "\n";
                    }
                    pad_length -= 4;
                }

                return ret + "};\n";
            }
            int sampleCount = endIbagIndex - startIbagIndex - 1;
            files.cpp.data += $"#include \"{files.hpp.fileName}\"\n\n";
            for (int i=0; i < sampleCount; i++)
            {
                files.cpp.data += "\n" + getSampleData(i) + "\n";
            }

            files.cpp.data += $"static const AudioSynthWavetable::sample_data {INSTRUMENT_NAME}_samples[{sampleCount}] =\n";
            files.cpp.data += "{\n";
            for (int i = 0; i < sampleCount; i++)
            {
                files.cpp.data += "\t{\n" + getSampleHeader(i) + "\n\t},\n";
            }
            files.cpp.data += "\n};\n";
            files.cpp.data += "\n";

            string[] RANGE_DATA_ITEMS = new string[sampleCount];
            for (int i = 0; i<sampleCount;i++)
            {
                SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + i, SFGenerator.keyRange);
                int rangeEnd = 0;
                if (val == null) rangeEnd = 127;
                rangeEnd = val.sorted_range[1];
                RANGE_DATA_ITEMS[i] = rangeEnd.ToString();
            }
            files.cpp.data += $"static const uint8_t {INSTRUMENT_NAME}_ranges[] = {{{string.Join(", ", RANGE_DATA_ITEMS)}}};\n";
            files.cpp.data += "\n";
            files.cpp.data += $"const AudioSynthWavetable::instrument_data {INSTRUMENT_NAME} = {{{sampleCount}, {INSTRUMENT_NAME}_ranges, {INSTRUMENT_NAME}_samples }};\n\n";
            br.Dispose();
            fs.Dispose();

            files.hpp.data = "#pragma once\n";
            files.hpp.data += "#include <Audio.h>\n";
            files.hpp.data += $"extern const AudioSynthWavetable::instrument_data {INSTRUMENT_NAME};\n";



            return files;
        }
    }
}
