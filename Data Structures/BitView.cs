using System;

namespace Protsyk.Collections.Btree
{
    public struct BitView
    {
        private static readonly int maxCount = sizeof(ulong) * 8;
        private ulong data;

        public BitView(ulong bits)
        {
            this.data = bits;
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= maxCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return (data & (1ul << index)) != 0;
            }
            set
            {
                if (index < 0 || index >= maxCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (value)
                {
                    data |= (1ul << index);
                }
                else
                {
                    data &= ~(1ul << index);
                }
            }
        }
    }

    public class BitViewVector
    {
        private readonly byte[][] buckets;
        private readonly int bucketSize;
        private readonly int bucketSizeBits;
        private readonly int maxCount;

        public BitViewVector()
            : this(4096, int.MaxValue) { }

        public BitViewVector(int bucketSize, int maxCount)
        {
            if (bucketSize < 1 || maxCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bucketSize));
            }

            int bucketsCount = (int) (maxCount / (8 * sizeof(byte) * bucketSize));
            if (maxCount % (8 * bucketSize) != 0)
            {
                ++bucketsCount;
            }

            this.maxCount = maxCount;
            this.bucketSize = bucketSize;
            this.bucketSizeBits = (bucketSize << 3);
            this.buckets = new byte[bucketsCount][];
        }


        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= maxCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                int bucketIndex, indexByte, bit;
                GetAdress(index, out bucketIndex, out indexByte, out bit);

                byte data = buckets[bucketIndex][indexByte];
                return (data & (1 << bit)) != 0;
            }
            set
            {
                if (index < 0 || index >= maxCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                int bucketIndex, indexByte, bit;
                GetAdress(index, out bucketIndex, out indexByte, out bit);

                var bucket = buckets[bucketIndex];
                if (bucket == null)
                {
                    bucket = new byte[bucketSize];
                    buckets[bucketIndex] = bucket;
                }

                if (value)
                {
                    bucket[indexByte] |= (byte) (1 << bit);
                }
                else
                {
                    bucket[indexByte] &= (byte) (~(1 << bit));
                }
            }
        }

        private void GetAdress(int index, out int bucketIndex, out int indexByte, out int bit)
        {
            bucketIndex = index / bucketSizeBits;
            int indexInBucket = index % bucketSizeBits;
            indexByte = indexInBucket >> 3;
            bit = indexInBucket % 8;
        }
    }
}