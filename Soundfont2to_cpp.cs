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
            string INSTRUMENT_NAME = sfbk.pdta.inst[instrumentIndex].achInstName;
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
                double get_decibel_value(SFGenerator genType, double DEFAULT, double MIN, double MAX)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    double val = (genval != null) ? genval.centibels : DEFAULT;
                    return (val > MAX) ? MAX : ((val < MIN) ? MIN : val);
                }
                double get_timecents_value(SFGenerator genType, double DEFAULT, double MIN)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    double val = (genval != null) ? genval.cents * 1000.0f : DEFAULT;
                    return (val > MIN) ? val : MIN;
                }
                double get_hertz(SFGenerator genType, double DEFAULT, double MIN, double MAX)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    if (genval != null) Debug.rtxt.AppendLine(genval.ToString());
                    double val = (genval != null) ? genval.absolute_cents : DEFAULT;
                    return (val > MAX) ? MAX : ((val < MIN) ? MIN : val);
                }
                double get_pitch_cents(SFGenerator genType, double DEFAULT, double MIN, double MAX)
                {
                    SF2GeneratorAmount genval = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, genType);
                    double val = (genval != null) ? genval.Amount : DEFAULT;
                    return (val > MAX) ? MAX : ((val < MIN) ? MIN : val);
                }
                bool get_sample_repeat(bool defaultValue)
                {
                    SF2GeneratorAmount val = get_gen_parameter_value(startIbagIndex + 1 + SAMPLE_NUM, SFGenerator.sampleModes);
                    if (val == null) return defaultValue;
                    return (val.sample_mode == SampleMode.kLoopContinuously) || (val.sample_mode == SampleMode.kLoopEndsByKeyDepression);
                }
                shdr_rec shdr = getSampleDef();
                string SAMPLE_NAME = shdr.achSampleName;
                
                string SAMPLE_ARRAY_NAME = $"sample_{SAMPLE_NUM}_{INSTRUMENT_NAME}_{SAMPLE_NAME}";
                string LOOP = (get_sample_repeat(false) == true) ? "true" : "false";
                int LENGTH_BITS = 16;
                int CENTS_OFFSET = 0;
                string SAMPLE_RATE = shdr.dwSampleRate.ToString("F1").Replace(',', '.');
                int SAMPLE_NOTE = shdr.byOriginalKey;
                int LENGTH = (int)(shdr.dwEnd - shdr.dwStart);
                int LOOP_START = (int)shdr.dwStartloop;
                int LOOP_END = (int)shdr.dwEndloop;
                
                string DELAY_ENV = get_timecents_value(SFGenerator.delayVolEnv, 0, 0).ToString("F2").Replace(',', '.');
                string ATTACK_ENV = get_timecents_value(SFGenerator.attackVolEnv, 1, 1).ToString("F2").Replace(',', '.');
                string HOLD_ENV = get_timecents_value(SFGenerator.holdVolEnv, 0, 0).ToString("F2").Replace(',', '.');
                string DECAY_ENV = get_timecents_value(SFGenerator.decayVolEnv, 1, 1).ToString("F2").Replace(',', '.');
                string SUSTAIN_FRAC = (get_decibel_value(SFGenerator.sustainVolEnv, 0, 0, 144)*-1).ToString("F1").Replace(',', '.');
                string RELEASE_ENV = get_timecents_value(SFGenerator.releaseVolEnv, 1, 1).ToString("F2").Replace(',', '.');
                string VIB_DELAY_ENV = get_timecents_value(SFGenerator.delayVibLFO, 0, 0).ToString("F2").Replace(',', '.');
                string VIB_INC_ENV = get_hertz(SFGenerator.freqVibLFO, 8.176, 0.1, 100).ToString("F1").Replace(',', '.');
                string MOD_DELAY_ENV = get_timecents_value(SFGenerator.delayModLFO, 0, 0).ToString("F2").Replace(',', '.');
                string MOD_INC_ENV = get_hertz(SFGenerator.freqModLFO, 8.176, 0.1, 100).ToString("F1").Replace(',', '.');
                int VIB_PITCH_INIT = Convert.ToInt32(get_pitch_cents(SFGenerator.vibLfoToPitch, 0, -12000, 12000));
                int VIB_PITCH_SCND = Convert.ToInt32(get_pitch_cents(SFGenerator.vibLfoToPitch, 0, -12000, 12000))*-1;
                int MOD_PITCH_INIT = Convert.ToInt32(get_pitch_cents(SFGenerator.modLfoToPitch, 0, -12000, 12000));
                int MOD_PITCH_SCND = Convert.ToInt32(get_pitch_cents(SFGenerator.modLfoToPitch, 0, -12000, 12000))*-1;
                string INIT_ATTENUATION = (get_decibel_value(SFGenerator.initialAttenuation, 0, 0, 144)*-1).ToString("F2").Replace(',', '.');
                int MOD_AMP_INIT_GAIN = Convert.ToInt32(get_decibel_value(SFGenerator.modLfoToVolume, 0, -96, 96));
                int MOD_AMP_SCND_GAIN = Convert.ToInt32(get_decibel_value(SFGenerator.modLfoToVolume, 0, -96, 96))*-1;

                string out_fmt_str =
                     "{\n" +
                    $"  (int16_t*){SAMPLE_ARRAY_NAME}, // sample\n" +
                    $"  {LOOP}, // LOOP\n" +
                    $"  {LENGTH_BITS}, // LENGTH_BITS\n" +
                    $"  (1 << (32 - {LENGTH_BITS})) * WAVETABLE_CENTS_SHIFT({CENTS_OFFSET}) * {SAMPLE_RATE} / WAVETABLE_NOTE_TO_FREQUENCY({SAMPLE_NOTE}) / AUDIO_SAMPLE_RATE_EXACT + 0.5, // PER_HERTZ_PHASE_INCREMENT\n" +
                    $"  ((uint32_t){LENGTH} - 1) << (32 - {LENGTH_BITS}), // MAX_PHASE\n" +
                    $"  ((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS}), // LOOP_PHASE_END\n" +
                    $"  (((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS})) - (((uint32_t){LOOP_START} - 1) << (32 - {LENGTH_BITS})), // LOOP_PHASE_LENGTH\n" +
                    $"  uint16_t(UINT16_MAX * WAVETABLE_DECIBEL_SHIFT({INIT_ATTENUATION})), // INITIAL_ATTENUATION_SCALAR\n" +
                    $"  uint32_t({DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // DELAY_COUNT\n" +
                    $"  uint32_t({ATTACK_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // ATTACK_COUNT\n" +
                    $"  uint32_t({HOLD_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // HOLD_COUNT\n" +
                    $"  uint32_t({DECAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // DECAY_COUNT\n" +
                    $"  uint32_t({RELEASE_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // RELEASE_COUNT\n" +
                    $"  int32_t((1.0 - WAVETABLE_DECIBEL_SHIFT({SUSTAIN_FRAC})) * AudioSynthWavetable::UNITY_GAIN), // SUSTAIN_MULT\n" +
                    $"  uint32_t({VIB_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)), // VIBRATO_DELAY\n" +
                    $"  uint32_t({VIB_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)), // VIBRATO_INCREMENT\n" +
                    $"  (WAVETABLE_CENTS_SHIFT({VIB_PITCH_INIT}) - 1.0) * 4, // VIBRATO_PITCH_COEFFICIENT_INITIAL\n" +
                    $"  (1.0 - WAVETABLE_CENTS_SHIFT({VIB_PITCH_SCND})) * 4, // VIBRATO_COEFFICIENT_SECONDARY\n" +
                    $"  uint32_t({MOD_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)), // MODULATION_DELAY\n" +
                    $"  uint32_t({MOD_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)), // MODULATION_INCREMENT\n" +
                    $"  (WAVETABLE_CENTS_SHIFT({MOD_PITCH_INIT}) - 1.0) * 4, // MODULATION_PITCH_COEFFICIENT_INITIAL\n" +
                    $"  (1.0 - WAVETABLE_CENTS_SHIFT({MOD_PITCH_SCND})) * 4, // MODULATION_PITCH_COEFFICIENT_SECOND\n" +
                    $"  int32_t(UINT16_MAX * (WAVETABLE_DECIBEL_SHIFT({MOD_AMP_INIT_GAIN}) - 1.0)) * 4, // MODULATION_AMPLITUDE_INITIAL_GAIN\n" +
                    $"  int32_t(UINT16_MAX * (1.0 - WAVETABLE_DECIBEL_SHIFT({MOD_AMP_SCND_GAIN}))) * 4, // MODULATION_AMPLITUDE_FINAL_GAIN\n" +
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
