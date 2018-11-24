using System;
using System.Collections.Generic;
using System.Linq;

namespace BitDepthChecker
{
    public class Checker
    {
        /// <summary>
        /// 帧长度
        /// </summary>
        private readonly int frameLength = 0;

        /// <summary>
        /// 波形数据
        /// </summary>
        private readonly byte[] waveData;

        /// <summary>
        /// 原始位深度
        /// </summary>
        public int OriginalBitDepth { get; }

        /// <summary>
        /// 可能存在的位深度
        /// </summary>
        public enum BitDepth
        {
            /// <summary>
            /// 8位
            /// </summary>
            Is8 = 8,

            /// <summary>
            /// 12位
            /// </summary>
            Is12 = 12,

            /// <summary>
            /// 16位
            /// </summary>
            Is16 = 16,

            /// <summary>
            /// 20位
            /// </summary>
            Is20 = 20,

            /// <summary>
            /// 24位
            /// </summary>
            Is24 = 24,

            /// <summary>
            /// 32位
            /// </summary>
            Is32 = 32,

            /// <summary>
            /// 64位
            /// </summary>
            Is64 = 64,

            /// <summary>
            /// 其他位深度
            /// </summary>
            Other = -2
        }

        /// <summary>
        /// 建立一个检查器
        /// </summary>
        /// <param name="frameLength">帧大小</param>
        /// <param name="waveData">波形数据</param>
        /// <param name="originalBitDepth">原位深</param>
        public Checker(int frameLength, byte[] waveData, int originalBitDepth)
        {
            this.frameLength = frameLength;
            this.waveData = waveData ?? throw new ArgumentNullException(nameof(waveData));
            OriginalBitDepth = originalBitDepth;
        }

        /// <summary>
        /// just for Test
        /// </summary>
        public Checker()
        {
        }

        /// <summary>
        /// 真实位深度完全检测
        /// </summary>
        /// <returns>真实位深度</returns>
        public BitDepth FullyCheckBitDepth()
        {
            #region

            if (OriginalBitDepth > 32)
            {
                return BitDepth.Other;//超过32位深度无法检测
            }
            if (frameLength == 1)
            {
                return BitDepth.Is8;
            }

            #endregion

            Dictionary<BitDepth, bool> flags = new Dictionary<BitDepth, bool>()
            {
                { BitDepth.Is8, true },
                { BitDepth.Is12, true },
                { BitDepth.Is16, true },
                { BitDepth.Is20, true },
                { BitDepth.Is24, true },
                { BitDepth.Is32, true }
            };
            for (int i = 0; i < flags.Count; i++)
            {
                var item = flags.ElementAt(i);
                if ((int)item.Key > OriginalBitDepth)
                {
                    flags[item.Key] = false;
                }
            }
            for (int i = 0; i < waveData.Length; i += frameLength)
            {
                byte[] frame = new byte[frameLength];
                for (int j = 0; j < frameLength; j++)
                {
                    frame[j] = waveData[i + j];
                }
                if (flags[BitDepth.Is8])
                {
                    flags[BitDepth.Is8] = Test8Bit(frame);
                }
                else if (flags[BitDepth.Is12])
                {
                    flags[BitDepth.Is12] = Test12Bit(frame);
                }
                else if (flags[BitDepth.Is16])
                {
                    flags[BitDepth.Is16] = Test16Bit(frame);
                }
                else if (flags[BitDepth.Is20])
                {
                    flags[BitDepth.Is20] = Test20Bit(frame);
                }
                else if (flags[BitDepth.Is24])
                {
                    flags[BitDepth.Is24] = flags[BitDepth.Is24] = Test24Bit(frame);
                }
                else if (flags[BitDepth.Is32])
                {
                    flags[BitDepth.Is32] = Test32Bit(frame);
                }
                if (flags.Where(pair => pair.Value == true).Count() == 1)
                {
                    break;
                }
            }

            return flags.First(pair => pair.Value == true).Key;
        }

        /// <summary>
        /// 检测一个采样是否为8位深度
        /// </summary>
        /// <param name="frame">采样</param>
        /// <returns>是/否</returns>
        public bool Test8Bit(byte[] frame)
        {
            if (frame.Length == 1)
            {
                return true;
            }
            int zeroData = frame.Where(data => data == 0).Count();
            if (zeroData == frame.Length)
            {
                return true;
            }
            else if (zeroData == frame.Length - 1 && frame[frame.Length - 1] != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测一个采样是否为12位深度
        /// </summary>
        /// <param name="frame">采样</param>
        /// <returns>是/否</returns>
        public bool Test12Bit(byte[] frame)
        {
            if (Test8Bit(frame))
            {
                return true;
            }
            else if (frame[frame.Length - 2] > 15 && frame[frame.Length - 2] < 240 && frame.Length == 2)
            {
                return true;
            }
            var frameList = frame.ToList();
            frameList.RemoveAt(frame.Length - 1);
            frameList.RemoveAt(frame.Length - 2);
            if (frame[frame.Length - 2] > 15 && frame[frame.Length - 2] < 240 && frameList.All(b => b == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测一个采样是否为16位深度
        /// </summary>
        /// <param name="frame">采样</param>
        /// <returns>是/否</returns>
        public bool Test16Bit(byte[] frame)
        {
            if (Test8Bit(frame))
            {
                return true;
            }
            else if (frame.Length == 2)
            {
                return true;
            }
            var frameList = frame.ToList();
            frameList.RemoveAt(frame.Length - 1);
            frameList.RemoveAt(frame.Length - 2);
            if (frameList.All(b => b == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测一个采样是否为20位深度
        /// </summary>
        /// <param name="frame">采样</param>
        /// <returns>是/否</returns>
        public bool Test20Bit(byte[] frame)
        {
            if (Test16Bit(frame))
            {
                return true;
            }
            else if (frame[frame.Length - 3] > 15 && frame[frame.Length - 3] < 240 && frame.Length == 3)
            {
                return true;
            }
            var frameList = frame.ToList();
            frameList.RemoveAt(frame.Length - 1);
            frameList.RemoveAt(frame.Length - 2);
            frameList.RemoveAt(frame.Length - 3);
            if (frame[frame.Length - 3] > 15 && frame[frame.Length - 3] < 240 && frameList.All(b => b == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测一个采样是否为24位深度
        /// </summary>
        /// <param name="frame">采样</param>
        /// <returns>是/否</returns>
        public bool Test24Bit(byte[] frame)
        {
            if (Test16Bit(frame))
            {
                return true;
            }
            else if (frame.Length == 3)
            {
                return true;
            }
            var frameList = frame.ToList();
            frameList.RemoveAt(frame.Length - 1);
            frameList.RemoveAt(frame.Length - 2);
            frameList.RemoveAt(frame.Length - 3);
            if (frameList.All(b => b == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测一个采样是否为32位深度
        /// </summary>
        /// <param name="frame">采样</param>
        /// <returns>是/否</returns>
        public bool Test32Bit(byte[] frame)
        {
            if (Test24Bit(frame))
            {
                return true;
            }
            else if (frame.Length == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}