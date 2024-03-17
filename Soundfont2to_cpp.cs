using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Soundfont2
{
    public class Soundfont2to_cpp
    {
        public class CodeFiles
        {
            public string cpp = "";
            public string hpp = "";
            public override string ToString()
            {
                return hpp + Environment.NewLine + cpp;
            }
        }
        public static CodeFiles getcpp(sfbk_rec sfbk, int instrumentIndex)
        {
            CodeFiles files = new CodeFiles();
            string INSTRUMENT_NAME = sfbk.pdta.inst[instrumentIndex].achInstName.Trim(' ').Replace(' ', '_');
            int startIbagIndex = sfbk.pdta.inst[instrumentIndex].wInstBagNdx;
            int endIbagIndex = sfbk.pdta.inst[instrumentIndex+1].wInstBagNdx;

            SF2GeneratorAmount get_gen_parameter_value(int ibagIndex, SFGenerator genType)
            {
                int start = sfbk.pdta.ibag[ibagIndex].wGenNdx;
                int end = sfbk.pdta.ibag[ibagIndex + 1].wGenNdx;
                Debug.rtxt.AppendLine("try to find @ sample:" + genType.ToString());
                for (int i=start;i<end;i++)
                {
                    if (sfbk.pdta.igen[i].sfGenOper == genType) return sfbk.pdta.igen[i].genAmount;
                }
                Debug.rtxt.AppendLine("try to find @ global:" + genType.ToString());
                // try again with global bag
                start = sfbk.pdta.ibag[startIbagIndex].wGenNdx;
                end = sfbk.pdta.ibag[startIbagIndex + 1].wGenNdx;
                for (int i = start; i < end; i++)
                {
                    if (sfbk.pdta.igen[i].sfGenOper == genType) return sfbk.pdta.igen[i].genAmount;
                }
                return null;
            }
            
            
            string getSampleHeader(int SAMPLE_NUM)
            {
                shdr_rec getSampleDef()
                {
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.sampleID);
                    if (val == null) return null;

                    return sfbk.pdta.shdr[val.Amount];
                }
                shdr_rec shdr = getSampleDef();
                if (shdr == null) { return ""; }

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
                bool get_sample_repeat(bool defaultValue)
                {
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.sampleModes);
                    if (val == null) return defaultValue;
                    return (val.sample_mode == SampleMode.kLoopContinuously);// || (val.sample_mode == SampleMode.kLoopEndsByKeyDepression);
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
                int get_cooked_loop_end()
                {
                    int result = (int)(shdr.dwEndloop - shdr.dwStart);
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.endloopAddrsOffset);
                    if (val != null) result += val.Amount;
                    val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.endloopAddrsCoarseOffset);
                    if (val != null) result += val.Amount * 32768;
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
                int get_length()
                {
                    int length = (int)(shdr.dwEnd - shdr.dwStart);
                    int cooked_loop_end = get_cooked_loop_end();
                    if (get_sample_repeat(false) && cooked_loop_end < length)
                        length = cooked_loop_end + 1;
                    return length;
                }
                int get_length_bits(int len)
                {
                    int length_bits = 0;
                    while (len != 0)
                    {
                        length_bits += 1;
                        len = len >> 1;
                    }
                    return len;
                }
                string SAMPLE_NAME = shdr.achSampleName.Trim(' ').Replace(' ', '_');
                
                string SAMPLE_ARRAY_NAME = $"sample_{SAMPLE_NUM}_{INSTRUMENT_NAME}_{SAMPLE_NAME}";
                
                string LOOP = get_sample_repeat(false)?"true":"false"; // c++ boolean
                int SAMPLE_NOTE = get_sample_note();
                int CENTS_OFFSET = get_fine_tuning();
                int LENGTH = get_length();
                int LENGTH_BITS = get_length_bits(LENGTH);
                string SAMPLE_RATE = shdr.dwSampleRate.ToString("F1").Replace(',', '.');
                int LOOP_START = get_cooked_loop_start();
                int LOOP_END = get_cooked_loop_end();

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

                string out_fmt_str =
                     "{\n" +
                     "// SAMPLE VALUES\n"+
                    $"/*sample pointer*/             (int16_t*){SAMPLE_ARRAY_NAME},\n" +
                    $"/*loop*/                       {LOOP},\n" +
                    $"/*LENGTH_BITS (INDEX_BITS)*/   {LENGTH_BITS},\n" +
                    $"/*PER_HERTZ_PHASE_INCREMENT*/  (1 << (32 - {LENGTH_BITS})) * WAVETABLE_CENTS_SHIFT({CENTS_OFFSET}) * {SAMPLE_RATE} / WAVETABLE_NOTE_TO_FREQUENCY({SAMPLE_NOTE}) / AUDIO_SAMPLE_RATE_EXACT + 0.5,\n" +
                    $"/*MAX_PHASE*/                  ((uint32_t){LENGTH} - 1) << (32 - {LENGTH_BITS}),\n" +
                    $"/*LOOP_PHASE_END*/             ((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS}),\n" +
                    $"/*LOOP_PHASE_LENGTH*/          (((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS})) - (((uint32_t){LOOP_START} - 1) << (32 - {LENGTH_BITS})),\n" +
                    $"/*INITIAL_ATTENUATION_SCALAR*/ uint16_t(UINT16_MAX * WAVETABLE_DECIBEL_SHIFT({INIT_ATTENUATION})),\n" +
                    "\n" +
                    "// VOLUME ENVELOPE VALUES\n"+
                    $"/*DELAY_COUNT*/   uint32_t({DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),\n" +
                    $"/*ATTACK_COUNT*/  uint32_t({ATTACK_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),\n" +
                    $"/*HOLD_COUNT*/    uint32_t({HOLD_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),T\n" +
                    $"/*DECAY_COUNT*/   uint32_t({DECAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),\n" +
                    $"/*RELEASE_COUNT*/ uint32_t({RELEASE_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5),\n" +
                    $"/*SUSTAIN_MULT*/  int32_t((1.0 - WAVETABLE_DECIBEL_SHIFT({SUSTAIN_FRAC})) * AudioSynthWavetable::UNITY_GAIN),\n" +
                    "\n" +
                    "// VIRBRATO VALUES\n" +
                    $"/*VIBRATO_DELAY*/                      uint32_t({VIB_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)),\n" +
                    $"/*VIBRATO_INCREMENT*/                  uint32_t({VIB_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)),\n" +
                    $"/*VIBRATO_PITCH_COEFFICIENT_INITIAL*/  (WAVETABLE_CENTS_SHIFT({VIB_PITCH_INIT}) - 1.0) * 4,\n" +
                    $"/*VIBRATO_PITCH_COEFFICIENT_SECOND*/   (1.0 - WAVETABLE_CENTS_SHIFT({VIB_PITCH_SCND})) * 4,\n" +
                    "\n" +
                    "// MODULATION VALUES\n" +
                    $"/*MODULATION_DELAY*/                     uint32_t({MOD_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)),\n" +
                    $"/*MODULATION_INCREMENT*/                 uint32_t({MOD_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)),\n" +
                    $"/*MODULATION_PITCH_COEFFICIENT_INITIAL*/ (WAVETABLE_CENTS_SHIFT({MOD_PITCH_INIT}) - 1.0) * 4,\n" +
                    $"/*MODULATION_PITCH_COEFFICIENT_SECOND*/  (1.0 - WAVETABLE_CENTS_SHIFT({MOD_PITCH_SCND})) * 4,\n" +
                    $"/*MODULATION_AMPLITUDE_INITIAL_GAIN*/    int32_t(UINT16_MAX * (WAVETABLE_DECIBEL_SHIFT({MOD_AMP_INIT_GAIN}) - 1.0)) * 4,\n" +
                    $"/*MODULATION_AMPLITUDE_SECOND_GAIN*/     int32_t(UINT16_MAX * (1.0 - WAVETABLE_DECIBEL_SHIFT({MOD_AMP_SCND_GAIN}))) * 4,\n" +
                    "},\n";

                return out_fmt_str;
            }
            int sampleCount = endIbagIndex - startIbagIndex - 1;
            for (int i = 0;i< sampleCount; i++)
                files.cpp += getSampleHeader(i);
            return files;
        }
    }
}
